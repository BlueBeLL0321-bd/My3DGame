using UnityEngine;
using UnityEngine.UI;
using TMPro;
using My3DGame.Manager;
using My3DGame.Common;
using System;

namespace My3DGame
{
    /// <summary>
    /// 퀘스트 UI를 관리하는 클래스
    /// </summary>
    public class QuestUI : MonoBehaviour
    {
        #region Variables
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descriptionText;

        public TextMeshProUGUI goalText;
        public TextMeshProUGUI rewardGoldText;
        public TextMeshProUGUI rewardExpText;
        public TextMeshProUGUI rewardItemText;
        public Image itemIcon;

        public GameObject acceptButton;
        public GameObject giveUpButton;
        public GameObject okButton;

        private QuestManager questManager;

        public Action OnCloseQuestUI;
        #endregion

        #region Unity Event Method
        #endregion

        #region Custom Method
        // 현재 선택된 퀘스트 정보 가져와서 UI 세팅
        private void SetQuestUI(QuestObject questObject)
        {
            // 퀘스트 데이터 가져오기
            Quest quest = DataManager.GetQuestData().quests.quests[questObject.number];

            nameText.text = quest.name;
            descriptionText.text = questObject.questGoal.IsReached ? "Quest Completed" : quest.description;

            goalText.text = questObject.questGoal.currentAmount.ToString() + " / " + quest.goalAmount.ToString();
            rewardGoldText.text = quest.rewardGold.ToString();
            rewardExpText.text = quest.rewardExp.ToString();

            if(quest.rewardItem >= 0)
            {
                rewardItemText.text
                    = UIManager.Instance.itemDataBase.itemObjects[quest.rewardItem].name;
                itemIcon.sprite
                    = UIManager.Instance.itemDataBase.itemObjects[quest.rewardItem].icon;
                itemIcon.enabled = true;
            }
            else
            {
                rewardItemText.text = "";
                itemIcon.sprite = null;
                itemIcon.enabled = false;
            }

            // 버튼 세팅
            ResetButtons();
            switch(questObject.questState)
            {
                case QuestState.Ready:
                    acceptButton.SetActive(true);
                    break;
                case QuestState.Accept:
                    giveUpButton.SetActive(true);
                    break;
                case QuestState.Complete:
                    okButton.SetActive(true);
                    break;
            }
        }

        private void ResetButtons()
        {
            acceptButton.SetActive(false);
            giveUpButton.SetActive(false);
            okButton.SetActive(false);
        }

        // 대화 종료 시 퀘스트 UI 오픈
        public bool OpenQuestUI()
        {
            if(questManager == null)
            {
                questManager = QuestManager.Instance;
            }

            if (questManager.currentQuest == null)
                return false;

            SetQuestUI(questManager.currentQuest);

            return true;
        }

        // 플레이어 퀘스트 정보를 확인하는 UI
        public bool OpenPlayerQuestUI()
        {
            // 플레이어 퀘스트 리스트 체크
            if(questManager.playerQuests.Count <= 0)
            {
                return false;
            }

            questManager.currentQuest = questManager.playerQuests[0];
            SetQuestUI(questManager.currentQuest);

            return true;
        }

        // 퀘스트 UI 닫기, 퀘스트 확인 버튼
        public void CloseQuestUI()
        {
            OnCloseQuestUI?.Invoke();
            OnCloseQuestUI = null;
        }

        // 퀘스트 수락 버튼
        public void AcceptQuest()
        {
            // 플레이어 퀘스트 리스트에 현재 선택된 퀘스트를 추가
            questManager.AddPlayerQuests();

            // 퀘스트 창 닫기
            CloseQuestUI();
        }

        // 퀘스트 포기 버튼
        public void GiveUpQuest()
        {
            // 플레이어 퀘스트 리스트에 현재 선택된 퀘스트를 추가
            questManager.GiveUpPlayerQuest();

            // 퀘스트 창 닫기
            CloseQuestUI();
        }
        #endregion
    }
}