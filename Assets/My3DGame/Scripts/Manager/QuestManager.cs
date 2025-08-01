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

            // 저장된 데이터를 가져와서 현재 진행 중인 퀘스트 세팅
        }
        #endregion
    }
}
