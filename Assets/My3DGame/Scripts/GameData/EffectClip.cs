using UnityEngine;
using My3DGame.Common;
using My3DGame.Manager;

namespace My3DGame.GameData
{
    /// <summary>
    /// 이펙트 속성 관리하는 클래스
    /// 속성 : 이펙트 아이디, 이름, 타입, 프리팹 에셋 이름, 프리팹 에셋 경로
    /// 기능 : 프리팹 에셋 로딩, 에셋 해제, 이펙트 인스턴스
    /// </summary>
    public class EffectClip
    {
        #region Variables
        private GameObject effectPrefab = null;     // 프리팹 에셋 경로 있는 프리팹 오브젝트
        #endregion

        #region Property
        public int Id { get; set; }
        public string Name { get; set; }
        public EffectType Type { get; set; }
        public string EffectPath { get; set; }      // 이펙트 프리팹 오브젝트 저장 경로
        public string EffectName { get; set; }      // 이펙트 프리팹 오브젝트 이름
        #endregion

        // 생성자
        public EffectClip() { }

        #region Custom Method
        // 프리팹 에셋 경로에 있는 프리팹 에셋 가져오기
        public void PreLoad()
        {
            // 에셋 경로
            var effectFullPath = EffectPath + EffectName;
            // 경로가 있고 아직 effectPrefab을 로딩하지 않았으면
            if(effectFullPath != string.Empty && effectPrefab == null)
            {
                effectPrefab = ResourcesManager.Load(effectFullPath) as GameObject;
            }
        }

        // 프리팹 에셋 해제
        public void ReleaseEffect()
        {
            if(effectPrefab != null)
            {
                effectPrefab = null;
            }
        }

        // 가져온 프리팹으로 이펙트 인스턴스
        public GameObject Instantiate(Vector3 position, Quaternion rotation)
        {
            // Prefab 체크해서 null이면 리소스 로드
            if(this.effectPrefab == null)
            {
                PreLoad();
            }

            if(this.effectPrefab == null)
            {
                return null;
            }

            GameObject effectGo = GameObject.Instantiate(effectPrefab, position, rotation);
            return effectGo;
        }
        #endregion
    }
}

