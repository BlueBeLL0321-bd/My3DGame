using UnityEngine;
using Unity.Cinemachine;
using My3DGame.Util;

namespace My3DGame
{
    /// <summary>
    /// 플레이어 액션을 관리하는 클래스(대기, 이동, 점프)
    /// </summary>
    public class PlayerController : MonoBehaviour, IMessageReceiver
    {
        #region Variables
        // 참조
        protected PlayerInput m_Input;
        protected CharacterController m_CharCtrl;
        protected Animator m_Animator;

        protected CameraSettings m_CameraSettings;
        protected CinemachineOrbitalFollow m_OrbitalFollow;

        protected Damageable m_Damageable;

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

        // 공격
        public MeleeWeapon meleeWeapon;
        private bool m_InAttack;                    // 공격 중이냐

        // 상수 정의
        const float k_GroundAcceleration = 20f;             // 이동 시 가속도 값
        const float k_GroundDeceleration = 25f;             // 이동 시 감속도 값
        const float k_GroundedRayDistance = 1f;             // 바닥으로부터 레이를 쏘는 높이
        const float k_AirbornedTurnSpeedProportion = 5.4f;
        const float k_InverseOneEighty = 1f / 180f;
        const float k_StickingGravityProportion = 0.3f;     // 바닥에 있을 때 중력 적용 계수
        const float k_JumpAbortSpeed = 10f;                 // 점프 키를 떼면 아래로 내려가는 속도를 가속

        // 애니메이션 Parameters Hash
        readonly int m_HashForwardSpeed = Animator.StringToHash("ForwardSpeed");
        readonly int m_HashAirborneVerticalSpeed = Animator.StringToHash("AirborneVerticalSpeed");
        readonly int m_HashAngleDeltaRad = Animator.StringToHash("AngleDeltaRad");
        readonly int m_HashInputDetected = Animator.StringToHash("InputDetected");
        readonly int m_HashGrounded = Animator.StringToHash("Grounded");
        readonly int m_HashTimeOutToIdle = Animator.StringToHash("TimeoutToIdle");

        readonly int m_HashMeleeAttack = Animator.StringToHash("MeleeAttack");
        readonly int m_HashStateTime = Animator.StringToHash("StateTime");

        readonly int m_HashHurtFromX = Animator.StringToHash("HurtFromX");
        readonly int m_HashHurtFromY = Animator.StringToHash("HurtFromY");
        readonly int m_HashHurt = Animator.StringToHash("Hurt");
        readonly int m_HashDeath = Animator.StringToHash("Death");

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
                return !Mathf.Approximately(m_Input.Movement.sqrMagnitude, 0f);
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

            m_CameraSettings = FindFirstObjectByType<CameraSettings>();
            m_OrbitalFollow = m_CameraSettings.freeLookCamera.GetComponent<CinemachineOrbitalFollow>();

            m_Damageable = GetComponent<Damageable>();
        }

        private void OnEnable()
        {
            // 대미지 메시지 리시버 리스트에 추가
            m_Damageable.onDamageMessageReceivers.Add(this);
            m_Damageable.IsInvulnerable = true;
        }

        private void OnDisable()
        {
            // 대미지 메시지 리시버 리스트에 제거
            m_Damageable.onDamageMessageReceivers.Remove(this);
        }

        private void Start()
        {
            // 초기화
            m_InAttack = false;
            meleeWeapon.SetOwner(this.gameObject);
        }

        private void FixedUpdate()
        {
            // 애니메이션
            CacheAnimatorState();       // 애니메이션 상태값 읽어 오기
            UpdateInputBlocking();      // 애니메이션 상태에 따른 인풋 처리

            // 공격
            m_Animator.ResetTrigger(m_HashMeleeAttack);
            m_Animator.SetFloat(m_HashStateTime,
                Mathf.Repeat(m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f));
            if(m_Input.Attack)
            {
                m_Animator.SetTrigger(m_HashMeleeAttack);
            }

            // 이동
            CalculateForwardMovement();
            CalculateVerticalMovement();

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

            // 애니메이션의 회전값을 캐릭터 컨트롤러에 적용
            m_CharCtrl.transform.rotation *= m_Animator.deltaRotation;

            // 위아래 이동 속도 적용
            movement += m_VerticalSpeed * Vector3.up * Time.deltaTime;

            // 구한 이동 속도를 캐릭터 컨트롤러에 적용
            m_CharCtrl.Move(movement);

            // 그라운드 체크
            m_IsGrounded = m_CharCtrl.isGrounded;

            // 애니메이션 적용
            if(m_IsGrounded == false)
                m_Animator.SetFloat(m_HashAirborneVerticalSpeed, m_VerticalSpeed);

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
            m_NextStateInfo = m_Animator.GetNextAnimatorStateInfo(0);
            m_IsAnimatorTransitioning = m_Animator.IsInTransition(0);
        }

        // 애니메이션 상태(tag string)에 따른 인풋 체크
        private void UpdateInputBlocking()
        {
            bool inputBlocked = m_CurrentStateInfo.tagHash == m_HashBlockInput && !m_IsAnimatorTransitioning;
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
            float acceleration = IsMoveInput ? k_GroundAcceleration : k_GroundDeceleration;

            // 실제 앞으로 이동하는 스피드 구하기
            m_ForwardSpeed = Mathf.MoveTowards(m_ForwardSpeed, m_DesiredForwardSpeed, 
                Time.deltaTime * acceleration);

            // 애니메이터 파라미터 설정
            m_Animator.SetFloat(m_HashForwardSpeed, m_ForwardSpeed);
        }

        // 위로 이동
        private void CalculateVerticalMovement()
        {
            // 점프 대기 체크
            if(!m_Input.Jump && m_IsGrounded)
            {
                m_ReadyToJump = true;
            }

            if(m_IsGrounded)    // 지면 상태
            {
                // 중력값의 0.3만큼 적용
                m_VerticalSpeed = -gravity * k_StickingGravityProportion;

                if(m_Input.Jump && m_ReadyToJump)
                {
                    m_VerticalSpeed = jumpSpeed;
                    m_IsGrounded = false;
                    m_ReadyToJump = false;
                }
            }
            else                // 공중 상태
            {
                // 점프 키 체크
                if(m_Input.Jump && m_VerticalSpeed > 0f)
                {
                    // 점프 키를 떼면 가속시켜 준다
                    m_VerticalSpeed -= k_JumpAbortSpeed * Time.deltaTime;
                }

                // m_VerticalSpeed 값 체크
                if(Mathf.Approximately(m_VerticalSpeed, 0f))
                {
                    m_VerticalSpeed = 0f;
                }

                // 중력 적용
                m_VerticalSpeed -= gravity * Time.deltaTime;
            }
        }

        // 입력에 따른 방향 전환값 구하기
        private void SetTargetRotation()
        {
            Vector2 moveInput = m_Input.Movement;
            // 입력에 따른 방향
            Vector3 localMovementDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            // 카메라가 바라보는 앞 방향 구하기
            Vector3 forward = Quaternion.Euler(0f, m_OrbitalFollow.HorizontalAxis.Value,
                0f) * Vector3.forward;

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
            // locomotion(이동), Airborne, Landing 상태 여부 체크
            bool updateOrientationForLocomotion = !m_IsAnimatorTransitioning && m_CurrentStateInfo.shortNameHash == m_HashLocomotion
                || m_NextStateInfo.shortNameHash == m_HashLocomotion;
            bool updateOrientationForAirborne = !m_IsAnimatorTransitioning && m_CurrentStateInfo.shortNameHash == m_HashAirborne
                || m_NextStateInfo.shortNameHash == m_HashAirborne;
            bool updateOrientationForLanding = !m_IsAnimatorTransitioning && m_CurrentStateInfo.shortNameHash == m_HashLanding
                || m_NextStateInfo.shortNameHash == m_HashLanding;

            return updateOrientationForLocomotion || updateOrientationForAirborne || updateOrientationForLanding || !m_InAttack;
        }

        // 회전값 적용
        private void UpdateOrientation()
        {
            // 애니메이션 파라미터 적용
            m_Animator.SetFloat(m_HashAngleDeltaRad, m_AngleDiff * Mathf.Deg2Rad);

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
            bool inputDetected = IsMoveInput || m_Input.Jump || m_Input.Attack;

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

        // 공격
        public void MeleeAttackStart()
        {
            m_InAttack = true;
            meleeWeapon.BeginAttack(false);
        }

        public void MeleeAttackEnd()
        {
            m_InAttack = false;
            meleeWeapon.EndAttack();
        }

        // 대미지 처리
        public void OnReceiveMessage(MessageType type, object sender, object msg)
        {
            switch (type)
            {
                case MessageType.DAMAGED:
                    {
                        Damageable.DamageMessage damageData = (Damageable.DamageMessage)msg;
                        Debug.Log($"MessageType.DAMAGED : {damageData.amount}");
                        Damaged(damageData);
                    }
                    break;
                case MessageType.DEAD:
                    {
                        Damageable.DamageMessage damageData = (Damageable.DamageMessage)msg;
                        Debug.Log($"MessageType.DEAD : {damageData.amount}");
                        Die(damageData);
                    }
                    break;
                case MessageType.RESPAWN:
                    break;
            }
        }

        private void Damaged(Damageable.DamageMessage damageMessage)
        {
            // TODO
            // 애니메이션
            m_Animator.SetTrigger(m_HashHurt);

            // 대미지 방향 구하기
            Vector3 forward = damageMessage.damageSource - transform.position;
            forward.y = 0f;

            Vector3 localHurt = transform.InverseTransformDirection(forward);
            m_Animator.SetFloat(m_HashHurtFromX, localHurt.x);
            m_Animator.SetFloat(m_HashHurtFromY, localHurt.z);

            // Vfx, Sfx
        }

        private void Die(Damageable.DamageMessage damageMessage)
        {
            // TODO
            m_Animator.SetTrigger(m_HashDeath);
            m_Damageable.IsInvulnerable = true;
        }
        #endregion
    }
}

