using UnityEngine;
using My3DGame.Util;
using My3DGame.GameData;

namespace My3DGame.Manager
{
    /// <summary>
    /// 이펙트 데이터를 가져와서 이펙트를 구현
    /// </summary>
    public class EffectManager : Singleton<EffectManager>
    {
        #region Variables
        // 생성한 이펙트 게임 오브젝트의 부모 오브젝트
        private Transform effectRoot = null;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            // 생성한 이펙트 게임 오브젝트의 부모 오브젝트 생성
            if(effectRoot == null)
            {
                effectRoot = new GameObject("EffectRoot").transform;
                effectRoot.SetParent(this.transform);
            }
        }
        #endregion

        #region Custom Method
        // 이펙트 클립 데이터를 가져와서 생성하여 이펙트 효과 플레이
        public GameObject EffectOneShot(int index, Vector3 position)
        {
            // 이펙트 클립 데이터를 가져오기
            EffectClip clip = DataManager.GetEffectData().GetEffectClip(index);
            GameObject effectGo = clip.Instantiate(position, Quaternion.identity);
            effectGo.SetActive(true);
            return effectGo;
        }
        #endregion
    }
}

