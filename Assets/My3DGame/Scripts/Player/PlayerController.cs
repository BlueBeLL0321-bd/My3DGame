using UnityEngine;

namespace My3DGame
{
    /// <summary>
    /// 플레이어 액션을 관리하는 클래스
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        #region Variables
        // 참조
        protected PlayerInput m_Input;
        protected CharacterController m_CharCtrl;
        protected Animator m_Animator;

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

        // idle
        public float idleTimeout = 5f;              // 5초 타임 아웃
        protected float m_IdleTimer;                // 카운트 다운

        // 상수 정의
        const float k_GroundAcceleration = 20f;     // 이동 시 가속도 값
        const float k_GroundDeceleration = 25f;     // 이동 시 감속도 값
        const float k_GroundedRayDistance = 1f;     // 바닥으로부터 레이를 쏘는 높이
        const float k_AirbornedTurnSpeedProportion = 5.4f;
        const float k_InverseOneEighty = 1f / 100f;

        // 애니메이션 Parameters Hash
        readonly int m_HashForwardSpeed = Animator.StringToHash("ForwardSpeed");
        readonly int m_HashVerticalSpeed = Animator.StringToHash("VerticalSpeed");
        readonly int m_HashAngleDeltaRad = Animator.StringToHash("AngleDeltaRad");
        readonly int m_HashInputDetected = Animator.StringToHash("InputDetected");
        readonly int m_HashGrounded = Animator.StringToHash("Grounded");
        readonly int m_HashTimeOutToIdle = Animator.StringToHash("TimeOutToIdle");

        // 애니메이션 상태 Hash
        readonly int m_HashLocomotion = Animator.StringToHash("Locomotion");

        // 애니메이션 상태 Tag Hash
        readonly int m_HashBlockInput = Animator.StringToHash("BlockInput");
        #endregion

        #region Property
        // 이동 입력 체크
        protected bool IsMoveInput
        {
            get
            {
                return Mathf.Approximately(m_Input.Movement.sqrMagnitude, 0f);
            }
        }
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 참조
            m_Input = GetComponent<PlayerInput>();
            m_CharCtrl = GetComponent<CharacterController>();
            m_Animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            CacheAnimatorState();       // 애니메이션 상태값 읽어 오기
            UpdateInputBlocking();      // 애니메이션 상태에 따른 인풋 처리

            // 이동
            CalculateForwardMovement();

            // 방향 전환
            SetTargetRotation();
            if(IsOrientationUpdate())
            {
                UpdateOrientation();
            }

            // 대기 상태 처리
            TimeoutToIdle();
        }

        private void OnAnimatorMove()
        {
            // 이동 속도
            Vector3 movement;

            if(m_IsGrounded)
            {
                // 이동 속도 구하기
                RaycastHit hit;
                Ray ray = new Ray(transform.position + Vector3.up * k_GroundedRayDistance * 0.5f, 
                    -Vector3.up);
                if(Physics.Raycast(ray, out hit, k_GroundedRayDistance, Physics.AllLayers,
                    QueryTriggerInteraction.Ignore))
                {
                    movement = Vector3.ProjectOnPlane(m_Animator.deltaPosition, hit.normal);
                }
                else
                {
                    movement = m_Animator.deltaPosition;
                }
            }
            else               // 공중에 떠 있는 상태
            {
                // 이동 스피드로 속도값 구하기
                movement = m_ForwardSpeed * transform.forward * Time.deltaTime;
            }


            // 구한 이동 속도를 캐릭터 컨트롤러에 적용
            m_CharCtrl.Move(movement);
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

        // 애니메이션 상태에 따른 인풋 처리
        private void UpdateInputBlocking()
        {
            bool inputBlocked = m_CurrentStateInfo.tagHash == m_HashBlockInput && m_IsAnimatorTransitioning;
            inputBlocked |= m_NextStateInfo.tagHash == m_HashBlockInput;
            m_Input.playerControllInputBlocked = inputBlocked;
        }

        // 앞으로 이동
        private void CalculateForwardMovement()
        {
            Vector2 moveInput = m_Input.Movement;
            if(moveInput.sqrMagnitude > 1f)
            {
                moveInput.Normalize();
            }

            // 입력에 따른 이동 스피드 구하기
            m_DesiredForwardSpeed = moveInput.magnitude * maxForwardSpeed;

            // 가속도 값 구하기
            float acceleration = IsMoveInput ? k_GroundAcceleration : k_GroundDeceleration; ;

            // 실제 앞으로 이동하는 스피드 구하기
            m_ForwardSpeed = Mathf.MoveTowards(m_ForwardSpeed, m_DesiredForwardSpeed, 
                Time.deltaTime * acceleration);

            // 애니메이터 파라미터 설정
            m_Animator.SetFloat(m_HashForwardSpeed, m_ForwardSpeed);
        }

        // 입력에 따른 방향 전환값 구하기
        private void SetTargetRotation()
        {
            Vector2 moveInput = m_Input.Movement;
            // 입력에 따른 방향
            Vector3 localMovementDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            // 입력 방향을 바라보는 앞 방향 구하기
            Vector3 forward = Quaternion.Euler(localMovementDirection.x, localMovementDirection.y,
                localMovementDirection.z) * Vector3.forward;
            forward.y = 0f;
            forward.Normalize();

            // 
            Quaternion targetRotation;
            if(Mathf.Approximately(Vector3.Dot(localMovementDirection, Vector3.forward), -1.0f))
            {
                targetRotation = Quaternion.LookRotation(-forward);
            }
            else
            {
                Quaternion inputOffset = Quaternion.FromToRotation(Vector3.forward, localMovementDirection);
                targetRotation = Quaternion.LookRotation(inputOffset * forward);
            }

            // 
            Vector3 resultingForward = targetRotation * Vector3.forward;

            // 현재 앞 방향의 각
            float angleCurrent = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;

            // 타깃 방향의 각
            float targetAngle = Mathf.Atan2(resultingForward.x, resultingForward.z) * Mathf.Rad2Deg;

            // 현재 앞 방향의 각과 목표 방향의 각의 차이 구하기
            m_AngleDiff = Mathf.DeltaAngle(angleCurrent, targetAngle);

            m_TargetRotation = targetRotation;
        }

        // 회전값 적용 애니메이션 상태 체크
        private bool IsOrientationUpdate()
        {
            // locomotion(이동) 상태 여부 체크
            bool updateOrientationForLocomotion = !m_IsAnimatorTransitioning && m_CurrentStateInfo.shortNameHash == m_HashLocomotion
                || m_NextStateInfo.shortNameHash == m_HashLocomotion;

            
            return updateOrientationForLocomotion;
        }

        // 회전값 적용
        private void UpdateOrientation()
        {
            // 애니메이션 파라미터 적용
            m_Animator.SetFloat(m_HashAngleDeltaRad, m_AngleDiff);

            Vector3 localInput = new Vector3(m_Input.Movement.x, 0f, m_Input.Movement.y);

            // 이동 속도에 따른 회전 속도값 구하기
            float groundTurnSpeed = Mathf.Lerp(maxForwardSpeed, minTurnSpeed, 
                m_ForwardSpeed / m_DesiredForwardSpeed);
            float actualTurnSpeed = m_IsGrounded ? groundTurnSpeed
                : Vector3.Angle(transform.forward, localInput) * k_InverseOneEighty
                * k_AirbornedTurnSpeedProportion * groundTurnSpeed;

            m_TargetRotation = Quaternion.RotateTowards(transform.rotation, m_TargetRotation,
                Time.deltaTime * actualTurnSpeed);

            transform.rotation = m_TargetRotation;
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

