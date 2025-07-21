using UnityEngine;

namespace MyPet.AI
{
    public class Cat : Animal
    {
        /// <summary>
        /// 고양이를 관리하는 클래스, Animal을 상속받는다
        /// </summary>
        #region Unity Event Method
        protected override void Start()
        {
            base.Start();       // animal start(), 상태 머신 생성, 대기, 앉기, 마시기 상태가 등록되어 있다

            // 추가되는 고양이 상태를 등록
            // stateMachine.RegisterState(new CatWalkState());
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