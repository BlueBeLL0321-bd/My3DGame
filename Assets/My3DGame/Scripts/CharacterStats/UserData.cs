using System;

namespace My3DGame
{
    /// <summary>
    /// 유저 게임 데이터 정의
    /// </summary>
    [Serializable]
    public class UserData
    {
        public int level;
        public int exp;
        public int gold;
        public int health;      // Current Health
        public int mana;        // Current Mana
    }
}

