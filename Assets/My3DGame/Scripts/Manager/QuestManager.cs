using UnityEngine;
using My3DGame;
using My3DGame.Util;
using System.Collections.Generic;
using System;

namespace My3DGame.Manager
{
    /// <summary>
    /// 게임에서 진행하는 퀘스트를 관리하는 매니저 클래스
    /// </summary>
    public class QuestManager : Singleton<QuestManager>
    {
        #region Variables
        public List<QuestObject> playerQuests;          // 플레이어가 현재 진행 중인 퀘스트 리스트

        public QuestObject currentQuest;                // 텍스트 UI에 전달되는 퀘스트

        public Action<QuestObject> onAcceptQuest;       // 퀘스트 수락 시 호출되는 이벤트 함수
        public Action<QuestObject> onGiveUpQuest;       // 퀘스트 포기 시 호출되는 이벤트 함수
        public Action<QuestObject> onCompletedQuest;    // 퀘스트 완료 시 호출되는 이벤트 함수
        #endregion

        #region Unity Event Method
        protected void Start()
        {
            playerQuests = new List<QuestObject>();

            // TODO :
            // 파일(서버)에 저장된 데이터를 가져와서 현재 진행 중인 퀘스트 세팅
        }
        #endregion

        #region Custom Method
        // 현재 퀘스트 세팅
        public void SetCurrentQuest(QuestObject quest)
        {
            currentQuest = quest;
        }

        // 플레이어 퀘스트 리스트에 현재 선택된 퀘스트를 추가
        public void AddPlayerQuests()
        {
            // 선택된 퀘스트 체크
            if (currentQuest == null)
                return;

            onAcceptQuest?.Invoke(currentQuest);

            // 추가할 퀘스트 오브젝트 만들기
            Quest quest = DataManager.GetQuestData().quests.quests[currentQuest.number];
            QuestObject newQuest = new QuestObject(quest);
            newQuest.questState = Common.QuestState.Accept;

            playerQuests.Add(newQuest);
        }

        // 플레이어 퀘스트 리스트에 현재 선택된 리스트를 제거
        public void GiveUpPlayerQuest()
        {
            // 선택된 퀘스트 체크
            if (currentQuest == null)
                return;

            onGiveUpQuest?.Invoke(currentQuest);

            playerQuests.Remove(currentQuest);
        }

        // 퀘스트 완료 시 보상 받는다
        public void RewardQuest()
        {
            // 선택된 퀘스트 체크
            if (currentQuest == null)
                return;

            Quest quest = DataManager.GetQuestData().quests.quests[currentQuest.number];
            Debug.Log($"{quest.rewardGold}골드를 보상 받았습니다");
            Debug.Log($"{quest.rewardExp}경험치를 보상 받았습니다");
            if(quest.rewardItem >= 0)
            {
                Debug.Log($"{quest.rewardItem}번 아이템을 보상 받았습니다");
            }
            // 리스트에서 제거
            playerQuests.Remove(currentQuest);
        }
        #endregion
    }
}
