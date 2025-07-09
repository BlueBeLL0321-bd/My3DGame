using UnityEngine;

namespace MySample
{
    // 카드 정보를 담는 스크립터블 오브젝트
    [CreateAssetMenu(fileName = "New Card", menuName = "Scriptable Objects/Card")]
    public class Card : ScriptableObject
    {
        new public string name;             // 카드 이름
        public string description;          // 카드 설명

        public int manaCost;                // 마나 비용
        public int attack;                  // 공격력
        public int health;                  // 체력

        public Sprite artImage;             // 카드 아트 이미지
    }
}

