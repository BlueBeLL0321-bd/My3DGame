using System.Collections.Generic;
using UnityEngine;
using My3DGame.Util;

namespace My3DGame
{
    /// <summary>
    /// MeleeWeapon 공격 충돌 체크를 위한 포인트 데이터
    /// </summary>
    [System.Serializable]
    public class AttackPoint
    {
        public float radius;
        public Vector3 offset;
        public Transform attackRoot;

#if UNITY_EDITOR
        [HideInInspector]
        public List<Vector3> previousPositions = new List<Vector3>();   // 어택 포인트의 이전 위치
#endif
    }

    /// <summary>
    /// 근접 무기(검, 스태프)를 관리하는 클래스
    /// </summary>
    public class MeleeWeapon : MonoBehaviour
    {
        #region Variables
        public float attackDamage = 20f;            // 공격력

        public AttackPoint[] attackPoints = new AttackPoint[0];

        public ParticleSystem hitParticlePrefab;    // 타격 이펙트 프리팹
        public LayerMask targetLayers;

        protected GameObject m_Owner;               // 무기 소유주

        protected Vector3[] m_PreviousPos = null;
        protected Vector3 m_Direction;

        protected bool m_IsThrowingHit = false;     // 원격 공격 여부
        protected bool m_InAttack = false;          // 유효 공격 여부

        // 충돌 체크
        protected static RaycastHit[] s_RaycastHitCache = new RaycastHit[32];
        protected static Collider[] s_ColliderCache = new Collider[32];

        // 효과 연출
        const int PARTICLE_COUNT = 10;
        protected ParticleSystem[] m_ParticlesPool = new ParticleSystem[PARTICLE_COUNT];
        protected int m_CurrentParticle = 0;

        public TrailEffect[] trailEffects;

        public RandomAudioPlayer hitSound;
        public RandomAudioPlayer swingSound;
        #endregion

        #region Property
        public bool IsThrowingHit
        {
            get { return m_IsThrowingHit; }
            set { m_IsThrowingHit = value; }
        }
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 파티클 오브젝트 풀에 저장
            if(hitParticlePrefab != null)
            {
                for (int i = 0; i < PARTICLE_COUNT; i++)
                {
                    m_ParticlesPool[i] = Instantiate(hitParticlePrefab);
                    m_ParticlesPool[i].Stop();
                }
            }
        }

        private void FixedUpdate()
        {
            if(m_InAttack)
            {
                for (int i = 0; i < attackPoints.Length; i++)
                {
                    AttackPoint pts = attackPoints[i];
                    Vector3 worldPos = pts.attackRoot.position +
                        pts.attackRoot.TransformVector(pts.offset);
                    // 검이 휘둘리는 방향
                    Vector3 attackVector = worldPos - m_PreviousPos[i];

                    if(attackVector.magnitude < 0.001f)
                    {
                        attackVector = Vector3.forward * 0.001f;
                    }

                    Ray r = new Ray(worldPos, attackVector.normalized);

                    int contacts = Physics.SphereCastNonAlloc(r, pts.radius, s_RaycastHitCache,
                        attackVector.magnitude, ~0, QueryTriggerInteraction.Ignore);

                    for (int k = 0; k < contacts; k++)
                    {
                        Collider col = s_RaycastHitCache[k].collider;
                        if(col)
                        {
                            // 대미지 주기
                            CheckDamage(col, pts);
                        }
                    }

                    // 이전 위치 저장
                    m_PreviousPos[i] = worldPos;
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            for(int i = 0; i < attackPoints.Length;i++)
            {
                AttackPoint pts = attackPoints[i];

                if(pts.attackRoot != null)
                {
                    Vector3 worldpos = pts.attackRoot.TransformVector(pts.offset);
                    Gizmos.color = new Color(1f, 1f, 1f, 0.4f);
                    Gizmos.DrawSphere(pts.attackRoot.position + worldpos, pts.radius);

                    if(pts.previousPositions.Count > 1)
                    {
                        UnityEditor.Handles.DrawAAPolyLine(10, pts.previousPositions.ToArray());
                    }
                }
            }
        }
#endif
        #endregion

        #region Custom Method
        // 대미지 주기
        private bool CheckDamage(Collider other, AttackPoint pts)
        {
            // Damageable 체크
            Damageable damageable = other.GetComponent<Damageable>();
            if(damageable == null)
            {
                return false;
            }

            // Self 대미지 체크
            if(damageable.gameObject == m_Owner)
            {
                return true;
            }

            // 레이어 마스크 체크
            if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
                return false;

            // 대미지 효과(Vfx, Sfx)
            if(hitParticlePrefab != null)
            {
                Vector3 worldPos = pts.attackRoot.position +
                    pts.attackRoot.TransformVector(pts.offset);
                m_ParticlesPool[m_CurrentParticle].transform.position = worldPos;
                m_ParticlesPool[m_CurrentParticle].time = 0f;
                m_ParticlesPool[m_CurrentParticle].Play();
                m_CurrentParticle = (m_CurrentParticle + 1) % PARTICLE_COUNT;
            }
            if(hitSound)
            {
                hitSound.PlayRandomClip();
            }

            // 대미지 계산
            Damageable.DamageMessage data;

            data.amount = attackDamage;
            data.damager = this;
            data.direction = m_Direction.normalized;
            data.damageSource = m_Owner.transform.position;
            data.stopCamera = false;
            data.throwing = m_IsThrowingHit;

            damageable.TakeDamage(data);

            return true;
        }

        // 무기 소유주 세팅
        public void SetOwner(GameObject owner)
        {
            this.m_Owner = owner;
        }

        // 유효 공격 시작
        public void BeginAttack(bool throwingAttack)
        {
            IsThrowingHit = throwingAttack;
            m_InAttack = true;

            m_PreviousPos = new Vector3[attackPoints.Length];

            for (int i = 0; i < attackPoints.Length; i++)
            {
                Vector3 worldPos = attackPoints[i].attackRoot.position +
                    attackPoints[i].attackRoot.TransformVector(attackPoints[i].offset);
                m_PreviousPos[i] = worldPos;

#if UNITY_EDITOR
                attackPoints[i].previousPositions.Clear();
                attackPoints[i].previousPositions.Add(m_PreviousPos[i]);
#endif
            }

            // 스윙 Sfx
            if(swingSound)
            {
                swingSound.PlayRandomClip();
            }
        }

        // 공격 끝
        public void EndAttack()
        {
            m_InAttack = false;

#if UNITY_EDITOR
            for (int i = 0; i < attackPoints.Length; i++)
            {
                attackPoints[i].previousPositions.Clear();
            }
#endif
        }
        #endregion
    }
}

