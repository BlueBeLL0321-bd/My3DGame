using My3DGame.Util;
using UnityEngine;

namespace My3DGame.AI
{
    /// <summary>
    /// 적(Enemy)을 관리하는 클래스, 적 클래스들의 부모 클래스
    /// 속성 : 상태 머신, 공격 범위
    /// 기능 : 공격 가능 여부 체크, 상태 변경, 타깃을 바라본다
    /// </summary>
    public class Enemy : MonoBehaviour, IMessageReceiver
    {
        #region Variables
        // 참조
        protected DetectionModule m_DetectionModule;
        protected Damageable m_Damageable;

        // 상태를 관리하는 상태 머신
        protected StateMachine stateMachine;

        // 공격 가능 범위
        [SerializeField]
        private float attackRange = 2.0f;

        // 공격 대미지 값
        [SerializeField]
        private float attackDamage = 20f;

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
                    return (distance <= AttackRange);
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
            m_Damageable = this.GetComponent<Damageable>();
        }

        protected virtual void OnEnable()
        {
            // 대미지 메시지 리시버 추가
            m_Damageable.onDamageMessageReceivers.Add(this);
            m_Damageable.IsInvulnerable = true;
        }

        protected virtual void OnDisable()
        {
            // 대미지 메시지 리시버 제거
            m_Damageable.onDamageMessageReceivers.Remove(this);
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

        // 대미지 처리
        public void OnReceiveMessage(MessageType type, object sender, object msg)
        {
            switch(type)
            {
                case MessageType.DAMAGED:
                    {
                        Damageable.DamageMessage damageData = (Damageable.DamageMessage)msg;
                        Damaged(damageData);
                    }
                    break;
                case MessageType.DEAD:
                    {
                        Damageable.DamageMessage damageData = (Damageable.DamageMessage)msg;
                        Die(damageData);
                    }
                    break;
            }
        }

        private void Damaged(Damageable.DamageMessage damageMessage)
        {
            // TODO
        }

        private void Die(Damageable.DamageMessage damageMessage)
        {
            // TODO
            stateMachine.ChangeState(new DeathState());

            // 오브젝트 킬
            Destroy(gameObject, 2f);
        }

        // 공격 애니메이션 프레임에서 자동으로 대미지 준다
        public void CheckDamage()
        {
            if (Target == null)
                return;

            Damageable damageable = Target.GetComponent<Damageable>();

            if(damageable)
            {
                // 대미지 데이터 구성
                Damageable.DamageMessage data;

                data.amount = attackDamage;
                data.damager = this;
                data.direction = Vector3.zero;
                data.damageSource = Vector3.zero;
                data.throwing = false;
                data.stopCamera = false;

                damageable.TakeDamage(data);
            }
        }
        #endregion
    }
}

