using UnityEngine;
using System.Collections;

namespace My3DGame
{
    /// <summary>
    /// MeleeWeapon으로 공격 시 나오는 이펙트 효과
    /// </summary>
    public class TrailEffect : MonoBehaviour
    {
        #region Variables
        public Light staffLight;
        private Animation m_Animation;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 참조
            m_Animation = this.GetComponent<Animation>();

            // 이펙트 비활성
            this.gameObject.SetActive(false);
        }
        #endregion

        #region Custom Method
        // 이펙트 효과 실행
        public void Activate()
        {
            this.gameObject.SetActive(true);
            staffLight.enabled = true;

            if (m_Animation)
                m_Animation.Play();

            // 애니메이션 플레이 후 효과 끄기
            StartCoroutine(DisableAtEndOfAnimation());
        }

        IEnumerator DisableAtEndOfAnimation()
        {
            yield return new WaitForSeconds(m_Animation.clip.length);

            this.gameObject.SetActive(true);
            staffLight.enabled = false;
        }
        #endregion
    }
}

