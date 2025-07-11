using UnityEngine;
using My3DGame.GameData;

namespace My3DGame.Manager
{
    /// <summary>
    /// 게임에서 툴에서 생산된 데이터들을 관리하는 클래스
    /// </summary>
    public class DataManager : MonoBehaviour
    {
        #region Variables
        private static EffectData effectData;
        private static SoundData soundData;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            // 이펙트 데이터 가져오기
            if(effectData == null)
            {
                effectData = ScriptableObject.CreateInstance<EffectData>();
                effectData.LoadData();
            }
            if (soundData == null)
            {
                soundData = ScriptableObject.CreateInstance<SoundData>();
                soundData.LoadData();
            }
        }
        #endregion

        #region Custom Method
        public static EffectData GetEffectData()
        {
            // 이펙트 데이터 체크
            if(effectData == null)
            {
                effectData = ScriptableObject.CreateInstance<EffectData>();
                effectData.LoadData();
            }

            return effectData;
        }

        public static SoundData GetSoundData()
        {
            // 데이터 체크
            if (soundData == null)
            {
                soundData = ScriptableObject.CreateInstance<SoundData>();
                soundData.LoadData();
            }

            return soundData;
        }
        #endregion
    }
}

