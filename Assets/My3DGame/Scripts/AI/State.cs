using UnityEngine;

namespace My3DGame.AI
{
    /// <summary>
    /// 적 상태를 관리하는 클래스, 모든 상태의 부모 추상 클래스
    /// 속성 : 현재 상태가 등록되어 있는 상태 머신, 상태 머신의 소유주
    /// 기능 : 상태를 세팅, 상태 초기화, 상태 들어가기, 상태 업데이트, 상태 나가기
    /// </summary>
    public abstract class State
    {
        #region Variables
        protected Enemy enemy;                          // 상태 머신의 소유주
        protected StateMachine stateMachine;            // 현재 상태가 등록되어 있는 상태 머신
        #endregion

        // 생성자
        public State() { }

        // 상태를 세팅 : enemy, stateMachine
        public void SetState(Enemy _enemy, StateMachine _stateMachine)
        {
            this.enemy = _enemy;
            this.stateMachine = _stateMachine;

            // 상태 초기화
            OnInitialize();
        }

        public virtual void OnInitialize() { }                      // 상태 초기화, 1회 호출
        public virtual void OnEnter() { }                           // 상태 들어가기, 1회 호출
        public abstract void OnUpdate(float deltaTime);             // 상태 업데이트, 추상 메서드, 강제로 구현, 매 프레임 호출
        public virtual void OnExit() { }                            // 상태 나가기, 1회 호출
    }
}