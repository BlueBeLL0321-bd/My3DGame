using System;
using My3DGame.Common;

namespace My3DGame
{
    /// <summary>
    /// NPC 데이터 모델 클래스
    /// </summary>
    [Serializable]
    public class Npc
    {
        public NpcType type;    // NPC 타입
        public int number;      // NPC 인덱스
        public string name;     // NPC 이름
    }
}

