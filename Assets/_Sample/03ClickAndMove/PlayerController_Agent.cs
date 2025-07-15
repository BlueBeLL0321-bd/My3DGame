using UnityEngine;
using UnityEngine.AI;

namespace MySample
{
    /// <summary>
    /// 플레이어(Agent) 액션을 관리하는 클래스(대기, 이동)
    /// </summary>
    public class PlayerController_Agent : MonoBehaviour
    {
        #region Variables
        // 참조
        protected PlayerInput_Agent m_Input;
        protected CharacterController m_CharCtrl;
        protected Animator m_Animator;
        protected NavMeshAgent m_Agent;
        protected Camera m_Camera;

        // 애니메이션 상태와 관련 변수
        protected AnimatorStateInfo m_CurrentStateInfo;
        protected AnimatorStateInfo m_NextStateInfo;
        protected bool m_IsAnimatorTransitioning;
        protected AnimatorStateInfo m_PreviousCurrentStateInfo;
        protected AnimatorStateInfo m_PreviousNextStateInfo;
        protected bool m_PreviousIsAnimatorTransitioning;

        // 이동
        public float maxForwardSpeed = 8f;
        public float minTurnSpeed = 400f;           // 회전 최솟값
        public float maxTurnSpeed = 1200f;          // 회전 최댓값

        protected bool m_IsGrounded = true;
        protected float m_DesiredForwardSpeed;
        protected float m_ForwardSpeed;
        protected float m_VerticalSpeed;

        // 회전
        protected Quaternion m_TargetRotation;      // 인풋에 따른 목표 회전값
        protected float m_AngleDiff;                // 현재 앞 방향의 각과 목표 방향의 각의 차이

        // 대기
        public float idleTimeout = 5f;              // 5초 타임 아웃
        [SerializeField]
        protected float m_IdleTimer;                // 카운트 다운

        // 점프
        public float gravity = 20f;                 // 중력값
        public float jumpSpeed = 10f;               // 점프키를 눌렀을 때 적용되는 스피드 값
        protected bool m_ReadyToJump = false;       // 점프 대기 중 체크

        // 마우스 클릭
        public LayerMask groundLayerMask;

        // 상수 정의
        const float k_GroundAcceleration = 20f;             // 이동 시 가속도 값
        const float k_GroundDeceleration = 25f;             // 이동 시 감속도 값
        const float k_GroundedRayDistance = 1f;             // 바닥으로부터 레이를 쏘는 높이
        const float k_AirbornedTurnSpeedProportion = 5.4f;
        const float k_InverseOneEighty = 1f / 100f;
        const float k_StickingGravityProportion = 0.3f;     // 바닥에 있을 때 중력 적용 계수
        const float k_JumpAbortSpeed = 10f;                 // 점프 키를 떼면 아래로 내려가는 속도를 가속

        // 애니메이션 Parameters Hash
        readonly int m_HashForwardSpeed = Animator.StringToHash("ForwardSpeed");
        readonly int m_HashAirborneVerticalSpeed = Animator.StringToHash("AirborneVerticalSpeed");
        readonly int m_HashAngleDeltaRad = Animator.StringToHash("AngleDeltaRad");
        readonly int m_HashInputDetected = Animator.StringToHash("InputDetected");
        readonly int m_HashGrounded = Animator.StringToHash("Grounded");
        readonly int m_HashTimeOutToIdle = Animator.StringToHash("TimeOutToIdle");

        // 애니메이션 상태 Hash
        readonly int m_HashLocomotion = Animator.StringToHash("Locomotion");
        readonly int m_HashAirborne = Animator.StringToHash("Airborne");
        readonly int m_HashLanding = Animator.StringToHash("Landing");

        // 애니메이션 상태 Tag Hash
        readonly int m_HashBlockInput = Animator.StringToHash("BlockInput");
        #endregion

        #region Property
        // 이동 입력 체크
        protected bool IsMoveInput
        {
            get
            {
                return !Mathf.Approximately(m_Agent.velocity.magnitude, 0f);
            }
        }
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 참조
            m_Input = GetComponent<PlayerInput_Agent>();
            m_CharCtrl = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();
            m_Agent = GetComponent<NavMeshAgent>();

            m_Camera = Camera.main;
        }

        private void Start()
        {
            // 초기화
            m_Agent.updatePosition = false;         // 길 찾기 AI에 의한 위치 이동 갱신 여부
            m_Agent.updateRotation = true;          // 길 찾기 AI에 의한 위치 회전 갱신 여부
        }

        private void FixedUpdate()
        {
            CacheAnimatorState();       // 애니메이션 상태값 읽어 오기
            UpdateInputBlocking();      // 애니메이션 상태(tag string)에 따른 인풋 처리

            // 이동
            CalculateForwardMovement();
            
            // 대기 상태 처리
            TimeoutToIdle();
        }

        private void OnAnimatorMove()
        {
            // 애니메이션 이동에 따른 위치 보정
            Vector3 position = m_Agent.nextPosition;
            m_Animator.rootPosition = position;
            transform.position = position;

            // 구한 이동 속도를 캐릭터 컨트롤러에 적용
            if(m_Agent.remainingDistance > m_Agent.stoppingDistance)
            {
                m_CharCtrl.Move(m_Agent.velocity * Time.deltaTime);
            }
            else
            {
                m_CharCtrl.Move(Vector3.zero);
            }

            // 그라운드 체크
            m_IsGrounded = m_CharCtrl.isGrounded;

            // 애니메이션 적용
            m_Animator.SetBool(m_HashGrounded, m_IsGrounded);
        }
        #endregion

        #region Custom Method
        // 애니메이션 상태값 읽어 오기
        private void CacheAnimatorState()
        {
            // 현재 상태를 구하기 전에 이전 상태에 저장해 놓는다
            m_PreviousCurrentStateInfo = m_CurrentStateInfo;
            m_PreviousNextStateInfo = m_NextStateInfo;
            m_PreviousIsAnimatorTransitioning = m_IsAnimatorTransitioning;

            // 현재 상태 저장
            m_CurrentStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
            m_NextStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
            m_IsAnimatorTransitioning = m_Animator.IsInTransition(0);
        }

        // 애니메이션 상태(tag string)에 따른 인풋 처리
        private void UpdateInputBlocking()
        {
            bool inputBlocked = m_CurrentStateInfo.tagHash == m_HashBlockInput && m_IsAnimatorTransitioning;
            inputBlocked |= m_NextStateInfo.tagHash == m_HashBlockInput;
            m_Input.playerControllInputBlocked = inputBlocked;
        }

        // 앞으로 이동
        private void CalculateForwardMovement()
        {
            if(m_Input.MouseClick)
            {
                // 마우스 클릭 좌표 구하기
                Ray ray = m_Camera.ScreenPointToRay(m_Input.MousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, 100f, groundLayerMask))
                {
                    m_Agent.SetDestination(hit.point);

                    // 클릭한 지점에 이펙트 효과
                }


                //
                m_Input.MouseClick = false;
            }

            // 애니메이터 파라미터 설정
            m_Animator.SetFloat(m_HashForwardSpeed, m_Agent.velocity.magnitude);
        }
        
        // 대기 상태 처리
        private void TimeoutToIdle()
        {
            // 입력값 체크 - 이동, 
            bool inputDetected = IsMoveInput;

            if(m_IsGrounded && !inputDetected)
            {
                m_IdleTimer += Time.deltaTime;
                if(m_IdleTimer >= idleTimeout)
                {
                    // 타이머
                    m_Animator.SetTrigger(m_HashTimeOutToIdle);

                    // 초기화
                    m_IdleTimer = 0f;
                }
            }
            else
            {
                // 초기화
                m_IdleTimer = 0f;
                m_Animator.ResetTrigger(m_HashTimeOutToIdle);
            }

            // 애니메이션 파라미터 설정
            m_Animator.SetBool(m_HashInputDetected, inputDetected);
        }
        #endregion
    }
}