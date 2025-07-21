using UnityEngine;

namespace MyPet.AI
{
    /// <summary>
    /// 강아지를 관리하는 클래스, Animal을 상속받는다
    /// </summary>
    public class Dog : Animal
    {
        #region Unity Event Method
        protected override void Start()
        {
            base.Start();       // animal start(), 상태 머신 생성, 대기, 앉기, 마시기 상태가 등록되어 있다

            // 추가되는 강아지 상태를 등록
            stateMachine.RegisterState(new DogWalkState());
        }
        #endregion

        #region Custom Method
        // 대기하라
        public void Idle()
        {
            ChangeState(new IdleState());
        }

        // 앉아
        public void Sit()
        {
            ChangeState(new SitState());
        }

        // 마셔라
        public void Drink()
        {
            ChangeState(new DrinkState());
        }
        #endregion
    }
}

