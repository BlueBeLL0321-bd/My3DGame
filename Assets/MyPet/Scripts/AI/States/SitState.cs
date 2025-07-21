using UnityEngine;

namespace MyPet.AI
{
    /// <summary>
    /// 동물이 앉아 있는 상태 구현
    /// </summary>
    /// 
    public class SitState : State
    {
        #region Variables
        private Animator animator;

        // 애니메이션 파라미터
        readonly int m_HashIsSit = Animator.StringToHash("IsSit");
        #endregion

        public override void OnInitialize()
        {
            // 참조
            animator = animal.GetComponent<Animator>();
        }

        public override void OnEnter()
        {
            // 애니메이션 변경
            animator.SetBool(m_HashIsSit, true);
        }

        public override void OnUpdate(float deltaTime)
        {

        }

        public override void OnExit()
        {
            // 애니메이션 변경
            animator.SetBool(m_HashIsSit, false);
        }
    }
}

