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

        private static DialogData dialogData;
        private static QuestData questData;
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
            // 사운드 데이터 가져오기
            if (soundData == null)
            {
                soundData = ScriptableObject.CreateInstance<SoundData>();
                soundData.LoadData();
            }
            // 대화창 데이터 가져오기
            if (dialogData == null)
            {
                dialogData = ScriptableObject.CreateInstance<DialogData>();
                dialogData.LoadData();
            }
            // 퀘스트 데이터 가져오기
            if (questData == null)
            {
                questData = ScriptableObject.CreateInstance<QuestData>();
                questData.LoadData();
            }
        }
        #endregion

        #region Custom Method
        // 이펙트 데이터 가져오기
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

        // 사운드 데이터 가져오기
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

        // 대화창 데이터 가져오기
        public static DialogData GetDialogData()
        {
            if(dialogData == null)
            {
                dialogData = ScriptableObject.CreateInstance<DialogData>();
                dialogData.LoadData();
            }

            return dialogData;
        }

        // 퀘스트 데이터 가져오기
        public static QuestData GetQuestData()
        {
            if (dialogData == null)
            {
                questData = ScriptableObject.CreateInstance<QuestData>();
                questData.LoadData();
            }

            return questData;
        }
        #endregion
    }
}

