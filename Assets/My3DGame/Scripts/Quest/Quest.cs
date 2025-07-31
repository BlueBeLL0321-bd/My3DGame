using System;
using My3DGame.Common;

namespace My3DGame
{
    /// <summary>
    /// 퀘스트 데이터 모델 클래스
    /// </summary>
    [Serializable]
    public class Quest
    {
        public int number { get; set; }              // 퀘스트 인덱스
        public int npcNumber { get; set; }           // 퀘스트를 의뢰하는 NPC 인덱스
        public string name { get; set; }             // 퀘스트 이름
        public string description { get; set; }      // 퀘스트 설명
        public int dialogIndex { get; set; }         // 퀘스트를 의뢰, 진행, 완료하는 대화 내용
        public int level { get; set; }               // 퀘스트 레벨 제한
        public int questType { get; set; }           // 퀘스트 타입
        public int goalIndex { get; set; }           // 퀘스트 목표 인덱스,
        public int goalAmount { get; set; }          // 퀘스트 달성 목표 수량
        public int rewardGold { get; set; }          // 퀘스트 보상 골드
        public int rewardExp { get; set; }           // 퀘스트 보상 경험치
        public int rewardItem { get; set; }          // 퀘스트 보상 아이템 아이디
    }
}
