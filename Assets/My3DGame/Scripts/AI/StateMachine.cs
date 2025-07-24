using UnityEngine;
using System.Collections.Generic;

namespace My3DGame.AI
{
    /// <summary>
    /// 상태들을 관리하는 클래스, 봉인 클래스
    /// 속성 : 상태들을 저장하는 변수(Dictionary), 상태 머신 소유주, 현재 상태, 이전 상태, 상태 플레이 누적 시간
    /// 기능 : 상태 머신에 상태를 등록, 현재 상태 업데이트, 상태 변경, 
    /// </summary>
    public sealed class StateMachine
    {
        #region Variables
        private Enemy enemy;        // 이 상태 머신을 가지고 있는 Enemy, 상태 머신 소유주

        // 상태들을 저장하는 변수
        private Dictionary<System.Type, State> states = new Dictionary<System.Type, State>();

        // 현재 상태를 저장하는 변수
        private State m_CurrentState;           // 현재 상태
        private State m_PreviousState;          // 이전 상태

        private float m_ElapseTime = 0f;        // 현재 상태가 진행된 누적 시간 카운팅
        #endregion

        #region Property
        public State CurrentState => m_CurrentState;
        public State PreviousState => m_PreviousState;
        public float ElapseTime => m_ElapseTime;
        #endregion

        // 생성자, 매개 변수 Enemy(소유주), State(초기 상태)
        public StateMachine(Enemy _enemy, State initialState)
        {
            // 소유주 저장
            this.enemy = _enemy;

            // 초기값 설정, 초기 상태 등록
            RegisterState(initialState);

            // 초기화
            m_CurrentState = initialState;
            m_CurrentState.OnEnter();
            m_ElapseTime = 0f;
        }

        // 상태 머신에 상태를 등록
        public void RegisterState(State state)
        {
            // 등록하는 상태 세팅
            state.SetState(enemy, this);

            // 상태 등록 - 상태의 타입을 얻어 key값으로 저장
            states[state.GetType()] = state;
        }

        // 상태 머신 돌리기 - 현재 상태 업데이트, 매 프레임마다 호출
        public void Update(float deltaTime)
        {
            m_ElapseTime += deltaTime;
            m_CurrentState.OnUpdate(deltaTime);
        }

        // 상태 변경 : 변경 후 현재 상태 반환
        public State ChangeState(State newState)
        {
            // 새로운 상태로부터 key값 얻어 오기
            var newType = newState.GetType();

            // 현재 상태 체크 - 
            if(newType == m_CurrentState?.GetType())
            {
                // 변경할 상태가 현재 상태로 변경되지 않는다
                return m_CurrentState;
            }

            // 상태 바꾸기
            // 1. 이전 현재 상태에서 빠져 나오기
            if(m_CurrentState != null)
            {
                m_CurrentState.OnExit();
            }

            // 2. 현재 상태를 새로운 상태로 저장
            m_PreviousState = m_CurrentState;
            m_CurrentState = states[newType];

            // 3. 새로운 현재 상태에 들어가기
            m_CurrentState.OnEnter();
            m_ElapseTime = 0f;

            return m_CurrentState;
        }
    }
}