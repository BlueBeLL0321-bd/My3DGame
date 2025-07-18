using UnityEngine;
using UnityEngine.AI;

namespace My3DGame.AI
{
    /// <summary>
    /// 적(Enemy)을 관리하는 클래스, 적 클래스들의 부모 클래스
    /// 속성 : 상태 머신, 공격 범위
    /// 기능 : 공격 가능 여부 체크, 상태 변경, 타깃을 바라본다
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        #region Variables
        // 참조
        private NavMeshAgent m_Agent;
        private DetectionModule m_DetectionModule;

        // 상태를 관리하는 상태 머신
        protected StateMachine stateMachine;

        // 공격 가능 범위
        [SerializeField]
        private float attackRange = 2.0f;

        // 공격 지연 시간
        [SerializeField]
        private float attackDelay = 2.0f;

        // 회전 속도
        private float rotateSpeed = 5f;
        #endregion

        #region Property
        public Transform Target => m_DetectionModule.Target;

        // 공격 가능 범위
        public float AttackRange => attackRange;

        // 공격 지연 시간
        public float AttackDelay { get; set; }

        // 공격 가능 여부
        public bool IsAttackable
        {
            get
            {
                if(Target)
                {
                    float distance = Vector3.Distance(transform.position, Target.position);
                    return (distance <= attackRange);
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region Unity Event Method
        protected virtual void Awake()
        {
            // 참조
            m_DetectionModule = this.GetComponent<DetectionModule>();
        }

        protected virtual void Start()
        {
            // 상태 머신 생성 및 상태 등록
            stateMachine = new StateMachine(this, new IdleState());
            stateMachine.RegisterState(new WalkState());
            stateMachine.RegisterState(new AttackState());
            stateMachine.RegisterState(new DeathState());

            // 상속받은 후 추가로 새로운 상태 등록 가능

            // 초기화
            AttackDelay = 2f;
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

        // 타깃을 바라본다
        public void FaceToTarget()
        {
            if (Target == null)
                return;

            // 방향을 구하고 그 방향으로 회전
            Vector3 direction = (Target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 
                Time.deltaTime * rotateSpeed);
        }
        #endregion
    }
}

