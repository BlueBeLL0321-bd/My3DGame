using UnityEngine;

namespace My3DGame.AI
{
    /// <summary>
    /// 대기 상태를 관리하는 클래스, State 상속
    /// </summary>
    public class IdleState : State
    {
        #region Variables
        private Animator m_Animator;

        // 애니메이션 파라미터
        readonly int m_HashForwardSpeed = Animator.StringToHash("ForwardSpeed");
        #endregion

        // 상태 초기화, 초기값 설정
        public override void OnInitialize()
        {
            // 참조
            m_Animator = enemy.GetComponent<Animator>();
        }

        // 대기 상태 시작하기
        public override void OnEnter()
        {
            // 애니메이터 상태 변경
            m_Animator.SetFloat(m_HashForwardSpeed, 0f);
        }

        public override void OnUpdate(float deltaTime)
        {
            // 디텍션으로 타깃을 찾아 상태 변경
        }

        public override void OnExit()
        {
            
        }
    }
}

