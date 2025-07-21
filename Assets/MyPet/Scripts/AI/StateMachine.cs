using System.Collections.Generic;
using UnityEngine;

namespace MyPet.AI
{
    /// <summary>
    /// 동물 상태들을 관리하는 클래스, 봉인 클래스
    /// 속성 : 상태들을 저장하는 변수(Dictionary), 상태 머신 소유주, 현재 상태, 이전 상태, 상태 플레이 누적 시간
    /// 기능 : 상태 머신에 상태를 등록, 현재 상태 업데이트, 상태 변경, 
    /// </summary>
    public class StateMachine
    {
        #region Variables
        private Animal animal;                  // 이 상태 머신을 가지고 있는 Enemy, 상태 머신 소유주, 전용

        // 상태들을 저장하는 변수
        private Dictionary<System.Type, State> states = new Dictionary<System.Type, State>();

        // 현재 상태를 저장하는 변수
        private State m_CurrentState;           // 현재 상태
        private State m_PreviousState;          // 이전 상태

        private float m_ElapseTime = 0f;        // 현재 상태가 진행된 누적 시간 카운팅
        #endregion

        // 생성자, 매개 변수 Animal(소유주), State(초기 상태)
        public StateMachine(Animal _animal, State initialState)
        {
            // 소유주 저장
            this.animal = _animal;

            // 초기값 설정, 초기 상태 등록
            RegisterState(initialState);

            // 초기화
            m_CurrentState = initialState;
            m_CurrentState.OnEnter();
            m_ElapseTime = 0f;

            Debug.Log($"{m_CurrentState} 상태로 처음 시작");
        }

        // 상태를 머신에 등록
        public void RegisterState(State state)
        {
            // 상태 세팅
            state.SetState(this.animal, this);
            // 상태 저장
            states[state.GetType()] = state;
        }

        // 현재 상태 업데이트
        public void Update(float deltaTime)
        {
            m_ElapseTime += deltaTime;
            m_CurrentState.OnUpdate(deltaTime);
        }

        // 상태 변경
        public State ChangeState(State newState)
        {
            // 상태로부터 key값 받아 온다
            var newType = newState.GetType();

            // 현재 상태 체크
            if(newType == m_CurrentState.GetType())
            {
                // 동일한 상태로 변경 시도하면 상태 변경하지 않는다
                return m_CurrentState;
            }

            // 현재 상태 나가기
            if(m_CurrentState != null)
            {
                m_CurrentState.OnExit();
            }

            // 현재 상태로 새로운 상태 변경
            m_PreviousState = m_CurrentState;
            m_CurrentState = states[newType];

            // 새로운 상태 들어가기
            m_CurrentState.OnEnter();
            m_ElapseTime = 0f;              // 상태 누적 시간 초기화

            return m_CurrentState;
        }
    }
}

