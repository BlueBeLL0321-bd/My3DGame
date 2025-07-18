using UnityEngine;
using UnityEngine.AI;

namespace My3DGame.AI
{
    public class WalkState : State
    {
        #region Variables
        private Animator m_Animator;
        private NavMeshAgent m_Agent;

        // 애니메이션 파라미터
        readonly int m_HashForwardSpeed = Animator.StringToHash("ForwardSpeed");
        #endregion

        // 상태 초기화, 초기값 설정
        public override void OnInitialize()
        {
            // 참조
            m_Animator = enemy.GetComponent<Animator>();
            m_Agent = enemy.GetComponent<NavMeshAgent>();
        }

        // 대기 상태 시작하기
        public override void OnEnter()
        {
            if(enemy.Target)
            {
                m_Agent.stoppingDistance = 1.5f;
                m_Agent.SetDestination(enemy.Target.position);
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            // 디텍션으로 타깃을 찾아 상태 변경
            if(enemy.Target)
            {
                if(enemy.IsAttackable)
                {
                    stateMachine.ChangeState(new AttackState());
                }
                else
                {
                    m_Agent.SetDestination(enemy.Target.position);
                }
            }

            // 애니메이션 변경
            if(enemy.IsAttackable == false)
            {
                m_Animator.SetFloat(m_HashForwardSpeed, m_Agent.velocity.magnitude);
            }
        }

        public override void OnExit()
        {
            // m_Agent 길 찾기 초기화
            m_Agent.ResetPath();
        }
    }
}

