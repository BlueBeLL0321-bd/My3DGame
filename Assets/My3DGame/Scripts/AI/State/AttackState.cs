using UnityEngine;
using UnityEngine.AI;

namespace My3DGame.AI
{
    public class AttackState : State
    {
        #region Variables
        private Animator m_Animator;

        private float attackDelay = 3f;     // 다음에 다시 공격 들어올 때까지 걸린 시간

        // 애니메이션 파라미터
        readonly int m_HashForwardSpeed = Animator.StringToHash("ForwardSpeed");
        readonly int m_HashAttack = Animator.StringToHash("Attack");
        #endregion

        // 상태 초기화, 초기값 설정
        public override void OnInitialize()
        {
            // 참조
            m_Animator = enemy.GetComponent<Animator>();
        }

        // 공격 상태 시작하기
        public override void OnEnter()
        {
            m_Animator.SetFloat(m_HashForwardSpeed, 0f);
            m_Animator.SetTrigger(m_HashAttack);
        }

        public override void OnUpdate(float deltaTime)
        {
            
        }

        public override void OnExit()
        {
            // 다음에 다시 공격 들어올 때까지 지연 시간 결정
            enemy.AttackDelay = attackDelay;
        }
    }
}