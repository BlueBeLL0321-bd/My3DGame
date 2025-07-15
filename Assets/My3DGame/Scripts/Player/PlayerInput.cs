using UnityEngine;
using UnityEngine.InputSystem;

namespace My3DGame
{
    /// <summary>
    /// 플레이어와 관련된 인풋을 관리하는 클래스
    /// </summary>
    public class PlayerInput : MonoBehaviour
    {
        #region Variables
        protected InputSystem_Actions inputActions;

        // input Action
        protected InputAction moveAction;
        protected InputAction jumpAction;

        // 인풋 제어 처리
        [HideInInspector]
        public bool playerControllInputBlocked;     // 플레이어 상태에 따라 인풋 블록 처리
        protected bool m_ExternalInputBlocked;      // 외부 요인에 따라 인풋 블록 처리

        // 이동 wasd 인풋값
        protected Vector2 m_Movement;
        // 점프
        protected bool m_Jump;
        #endregion

        #region Property
        // 이동
        public Vector2 Movement
        {
            get
            {
                if (playerControllInputBlocked || m_ExternalInputBlocked)
                    return Vector2.zero;

                return m_Movement;
            }
            private set
            {
                m_Movement = value;
            }
        }

        // 점프
        public bool Jump
        {
            get
            {
               return m_Jump && !playerControllInputBlocked && !m_ExternalInputBlocked;
            }
            private set
            {
                m_Jump = value;
            }
        }
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 참조
            inputActions = new InputSystem_Actions();
            moveAction = inputActions.Player.Move;
            jumpAction = inputActions.Player.Jump;
        }

        private void OnEnable()
        {
            // Action Map 활성화
            inputActions.Player.Enable();

            // 이벤트 발생 시 호출되는 함수 등록
            jumpAction.started += Jump_Started;
            jumpAction.canceled += Jump_Canceled;

            /*// 액션 인풋 처리 샘플 - 이벤트 발생 시 호출되는 함수 등록
            moveAction.performed += Move_Performed;
            moveAction.started += Move_Started;
            moveAction.canceled += Move_Canceled;*/
        }

        private void OnDisable()
        {
            // Action Map 비활성화
            inputActions.Player.Disable();

            // 이벤트 발생 시 호출되는 함수 해제
            jumpAction.started -= Jump_Started;
            jumpAction.canceled -= Jump_Canceled;

            /*// 액션 인풋 처리 샘플 - 이벤트 발생 시 호출되는 함수 해제
            moveAction.performed -= Move_Performed;
            moveAction.started -= Move_Started;
            moveAction.canceled -= Move_Canceled;*/
        }

        private void Update()
        {
            // 이동 입력값 처리
            Movement = moveAction.ReadValue<Vector2>();
        }
        #endregion

        #region Custom Method
        // 외부 요인에 따라 인풋 제어 블록 처리
        public void ReleasedControl()
        {
            m_ExternalInputBlocked = true;
        }

        public void GainControl()
        {
            m_ExternalInputBlocked = false;
        }

        // 인풋 제어권 소유 여부
        public bool HaveControl()
        {
            return !m_ExternalInputBlocked;
        }

        private void Jump_Started(InputAction.CallbackContext context)
        {
            Jump = true;
        }

        private void Jump_Canceled(InputAction.CallbackContext context)
        {
            Jump = false;
        }

        /*// 액션 인풋 처리 샘플
        private void Move_Performed(InputAction.CallbackContext context)
        {
            Debug.Log("Move_Performed");
        }

        private void Move_Started(InputAction.CallbackContext context)
        {
            Debug.Log("Move_Started");
        }

        private void Move_Canceled(InputAction.CallbackContext context)
        {
            Debug.Log("Move_Canceled");
        }*/
        #endregion
    }
}

