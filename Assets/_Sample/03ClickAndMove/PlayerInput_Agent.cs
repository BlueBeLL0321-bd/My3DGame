using UnityEngine;
using UnityEngine.InputSystem;

namespace MySample
{
    /// <summary>
    /// 플레이어와 관련된 인풋을 관리하는 클래스
    /// </summary>
    public class PlayerInput_Agent : MonoBehaviour
    {
        #region Variables
        protected InputSystem_Actions inputActions;

        // input Action
        protected InputAction moveClickAction;

        // 인풋 제어 처리
        [HideInInspector]
        public bool playerControllInputBlocked;     // 플레이어 상태에 따라 인풋 블록 처리
        protected bool m_ExternalInputBlocked;      // 외부 요인에 따라 인풋 블록 처리

        // 이동 마우스 인풋값
        protected bool m_MouseClick;
        protected Vector3 m_MousePosition;
        #endregion

        #region Property
        // 마우스 위치
        public Vector2 MousePosition
        {
            get
            {
                if (playerControllInputBlocked || m_ExternalInputBlocked)
                    return Vector2.zero;

                return m_MousePosition;
            }
            private set
            {
                m_MousePosition = value;
            }
        }

        // 마우스 클릭
        public bool MouseClick
        {
            get
            {
                return m_MouseClick && !playerControllInputBlocked && !m_ExternalInputBlocked;
            }
            set
            {
                m_MouseClick = value;
            }
        }
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 참조
            inputActions = new InputSystem_Actions();
            moveClickAction = inputActions.PlayerAgent.MoveClick;
        }

        private void OnEnable()
        {
            // Action Map 활성화
            inputActions.PlayerAgent.Enable();

            // 이벤트 발생 시 호출되는 함수 등록
            moveClickAction.started += MoveClick_Started;
        }

        private void OnDisable()
        {
            // Action Map 비활성화
            inputActions.PlayerAgent.Disable();

            // 이벤트 발생 시 호출되는 함수 해제
            moveClickAction.started += MoveClick_Started;
        }

        private void Update()
        {
            // 마우스 위치값 가져오기
            MousePosition = Input.mousePosition;
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

        private void MoveClick_Started(InputAction.CallbackContext context)
        {
            Debug.Log("MoveClick_Started");
            MouseClick = true;
        }
        #endregion
    }
}

