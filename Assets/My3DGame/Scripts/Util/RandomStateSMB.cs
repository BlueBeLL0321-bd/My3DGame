using UnityEngine;

namespace My3DGame.Util
{
    /// <summary>
    /// 애니메이션 상태에 부착되어 타이머를 작동 시 지정한 상태들 중 랜덤하게 하나를 선택해서 이동한다
    /// </summary>
    public class RandomStateSMB : StateMachineBehaviour
    {
        #region Variables
        public int numberOfState = 3;       // 랜덤하게 선택할 상태의 총 개수
        // 타이머
        public float minNormTime = 0f;
        public float maxNormTime = 5f;
        protected float m_RandomNormTime;   // 타이머 시간(0 ~ 5 사이의 랜덤값 구한다)

        readonly int m_HashRandomIdle = Animator.StringToHash("RandomIdle");
        #endregion

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // 타이머 랜덤값 구하기
            m_RandomNormTime = Random.Range(minNormTime, maxNormTime);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // m_HashRandomIdle 초기화
            if(animator.IsInTransition(0) 
                && animator.GetCurrentAnimatorStateInfo(0).fullPathHash == stateInfo.fullPathHash)
            {
                animator.SetInteger(m_HashRandomIdle, -1);
            }

            // 타이머 시간 체크
            if(stateInfo.normalizedTime > m_RandomNormTime && !animator.IsInTransition(0))
            {
                animator.SetInteger(m_HashRandomIdle, Random.Range(0, numberOfState));
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }

}
