using My3DGame.Common;

namespace My3DGame
{
    /// <summary>
    /// 캐릭터의 속성과 값을 가진 클래스
    /// </summary>
    [System.Serializable]
    public class Attribute
    {
        public CharacterAttribute type;     // 캐릭터의 속성
        public ModifiableInt value;         // 캐릭터의 속성 값
    }
}

