using UnityEngine;

namespace My3DGame.AI
{
    /// <summary>
    /// 적(Enemy)을 관리하는 클래스, 적 클래스들의 부모 클래스
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        #region Variables
        // 상태를 관리하는 상태 머신
        protected StateMachine stateMachine;
        #endregion

        #region Unity Event Method
        protected virtual void Start()
        {
            // 상태 머신 생성 및 상태 등록
            stateMachine = new StateMachine(this, new IdleState());
            stateMachine.RegisterState(new WalkState());
            stateMachine.RegisterState(new AttackState());
            stateMachine.RegisterState(new DeathState());

            // 상속받은 후 추가로 새로운 상태 등록 가능
        }

        protected virtual void Update()
        {
            // 치팅
            if(Input.GetKeyDown(KeyCode.I))
            {
                ChangeState(new IdleState());
            }
            if(Input.GetKeyDown(KeyCode.O))
            {
                ChangeState(new WalkState());
            }

            // 상태 머신의 업데이트가 현재 상태의 업데이트를 실행
            stateMachine.Update(Time.deltaTime);
        }
        #endregion

        #region Custom Method
        // 상태 변경
        public State ChangeState(State newState)
        {
            return stateMachine.ChangeState(newState);
        }
        #endregion
    }
}

