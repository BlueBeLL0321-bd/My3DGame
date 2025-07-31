using System;
using System.Collections.Generic;

namespace My3DGame
{
    /// <summary>
    /// 퀘스트 데이터 리스트
    /// </summary>
    [Serializable]
    public class Quests
    {
        public List<Quest> quests { get; set; }
    }
}

