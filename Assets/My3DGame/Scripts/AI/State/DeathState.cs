using UnityEngine;
using UnityEngine.AI;

namespace My3DGame.AI
{
    public class DeathState : State
    {
        #region Variables
        private Animator m_Animator;
        
        // 애니메이션 파라미터
        readonly int m_HashDeath = Animator.StringToHash("Death");
        #endregion

        // 상태 초기화, 초기값 설정
        public override void OnInitialize()
        {
            // 참조
            m_Animator = enemy.GetComponent<Animator>();
        }

        // 죽음 상태
        public override void OnEnter()
        {
            m_Animator.SetTrigger(m_HashDeath);
        }

        public override void OnUpdate(float deltaTime)
        {
            
        }
    }
}

