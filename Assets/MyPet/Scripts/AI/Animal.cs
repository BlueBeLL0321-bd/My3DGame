using UnityEngine;
using UnityEngine.AI;

namespace MyPet.AI
{
    /// <summary>
    /// 동물들을 관리하는 클래스, 동물 클래스들의 부모 클래스
    /// 속성 : 상태 머신
    /// 기능 : 상태 변경, 
    /// </summary>
    public class Animal : MonoBehaviour
    {
        #region Variables
        // 상태를 관리하는 상태 머신
        protected StateMachine stateMachine;
        #endregion

        #region Unity Event Method
        protected virtual void Start()
        {
            // 상태 머신 생성과 상태 등록 및 동물들의 공통 상태 등록
            stateMachine = new StateMachine(this, new IdleState());
            stateMachine.RegisterState(new SitState());
            stateMachine.RegisterState(new DrinkState());
        }

        protected virtual void Update()
        {
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