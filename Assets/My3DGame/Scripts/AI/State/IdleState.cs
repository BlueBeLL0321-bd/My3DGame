using UnityEngine;

namespace My3DGame.AI
{
    /// <summary>
    /// 대기 상태를 관리하는 클래스, State 상속
    /// 디텍션하다 타깃이 잡히면 추격(걷기) 상태, Attack 범위에 들어오면 공격 상태로 변경
    /// 공격 가능 시 공격 딜레이 시간 체크 후 공격한다
    /// </summary>
    public class IdleState : State
    {
        #region Variables
        private Animator m_Animator;

        // 패트롤
        private bool m_IsPatrol = false;
        private float m_MinTime = 0f;
        private float m_MaxTime = 3f;
        private float m_IdleTime = 0f;

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

            // 패트롤 체크
            if(enemy is EnemyPatrol)
            {
                m_IsPatrol = true;
                m_IdleTime = Random.Range(m_MinTime, m_MaxTime);
            }    
        }

        public override void OnUpdate(float deltaTime)
        {
            // 디텍션하다 타깃이 잡히면 추격(걷기) 상태
            // Attack 범위에 들어오면 공격 상태로 변경
            if(enemy.Target)
            {
                // 공격 가능 여부 체크
                if(enemy.IsAttackable)
                {
                    if(stateMachine.ElapseTime >= enemy.AttackDelay)
                    {
                        stateMachine.ChangeState(new AttackState());
                    }
                }
                else
                {
                    stateMachine.ChangeState(new WalkState());
                }
            }
            else if(m_IsPatrol)
            {
                if(stateMachine.ElapseTime >= m_IdleTime)
                {
                    stateMachine.ChangeState(new PatrolState());
                }
            }
        }

        // 상태 나가기
        public override void OnExit()
        {
            
        }
    }
}

