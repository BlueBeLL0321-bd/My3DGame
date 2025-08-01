using UnityEngine;
using System.Collections.Generic;
using My3DGame.Manager;

namespace My3DGame
{
    /// <summary>
    /// 퀘스트를 주는 NPC
    /// </summary>
    public class PickupQuestGiver : PickupNpc
    {
        #region Variables
        public List<QuestObject> quests;            // 해당 NPC가 줄 수 있는 Quest 목록
        #endregion

        #region Unity Event Method
        protected override void Start()
        {
            base.Start();

            // 해당 NPC가 줄 수 있는 Quest 목록을 가져오기
        }
        #endregion

        #region Custom Method
        public List<QuestObject> GetNpcQuests(int npcNumber)
        {
            List<QuestObject> questObjects = new List<QuestObject>();

            foreach (var quest in DataManager.GetQuestData().quests.quests)
            {
                // 퀘스트 클리어 여부 체크

                if(quest.npcNumber == npcNumber)
                {
                    QuestObject questObject = new QuestObject(quest);
                    questObjects.Add(questObject);
                }
            }

            return questObjects;
        }

        protected override void DoAction()
        {
            // 퀘스트 체크
            if(quests.Count == 0)
            {
                // 해당 NPC의 퀘스트를 모두 클리어
                return;
            }

            // quest[0] : 지금 NPC가 진행할 Quest
            // quest[0].number
            int index = DataManager.GetQuestData().quests.quests[quests[0].number].dialogIndex;
        }
        #endregion
    }
}

