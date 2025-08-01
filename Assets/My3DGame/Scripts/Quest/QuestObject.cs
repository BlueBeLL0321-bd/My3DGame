using System;
using My3DGame.Common;

namespace My3DGame
{
    /// <summary>
    /// 퀘스트 목표 데이터 클래스
    /// </summary>
    [Serializable]
    public class QuestGoal
    {
        public QuestType questType;     // 퀘스트 타입, kill, collect
        public int goalIndex;           // Enemy Id, Item Id
        public int goalAmount;          // 퀘스트 목표 수량
        public int currentAmount;       // 퀘스트 현재 달성량

        // 목표 달성 여부
        public bool IsReached => currentAmount >= goalAmount;
    }

    /// <summary>
    /// 게임에서 진행하는 퀘스트 데이터
    /// </summary>
    [Serializable]
    public class QuestObject
    {
        #region Variables
        public int number;                  // 퀘스트 번호
        public QuestState questState;       // 퀘스트 상태
        public QuestGoal questGoal;         // 퀘스트 목표
        #endregion

        #region Constructor
        // 생성자 - 매개 변수로 퀘스트 데이터를 받아 게임에서 진행할 퀘스트 구성
        public QuestObject(Quest quest)
        {
            number = quest.number;
            questState = QuestState.Ready;

            questGoal = new QuestGoal();                    // 목표 세팅
            questGoal.questType = quest.questType;
            questGoal.goalIndex = quest.goalIndex;
            questGoal.goalAmount = quest.goalAmount;
            questGoal.currentAmount = 0;
        }
        #endregion

        #region Custom Method
        // 퀘스트 미션 달성 체크 - kill
        public void EnemyKill(int enemyId)
        {
            if(questGoal.questType == QuestType.Kill)
            {
                // if(questGoal.goalIndex == enemyId)
                {
                    questGoal.currentAmount++;
                }
            }
        }

        // 퀘스트 미션 달성 체크 - Collect
        public void ItemCollect(int itemId)
        {
            if (questGoal.questType == QuestType.Collect)
            {
                // if(questGoal.goalIndex == itemId)
                {
                    questGoal.currentAmount++;
                }
            }
        }

        // 퀘스트 미션 달성 체크 - 호위, 대화, 배달
        #endregion
    }
}

