using UnityEngine;
using My3DGame.Common;

namespace My3DGame.ItemSystem
{
    /// <summary>
    /// 아이템 능력치를 관리하는 클래스
    /// 속성 : 아이템 속성, 능력치, 능력치의 최솟값, 능력치의 최댓값
    /// 기능 : 능력치 더하기, 능력치 생성하기
    /// </summary>
    [System.Serializable]
    public class ItemBuff : IModifier
    {
        #region Variables
        public CharacterAttribute stat;         // 아이템 능력치의 속성
        public int value;

        [SerializeField] private int min;
        [SerializeField] private int max;
        #endregion

        #region Property
        public int Min => min;
        public int Max => max;
        #endregion

        // 생성자
        #region Constructor
        public ItemBuff(int _min, int _max)
        {
            this.min = _min;
            this.max = _max;
            GenerateValue();
        }
        #endregion

        #region Custom Method
        // value값 생성
        private void GenerateValue()
        {
            value = Random.Range(min, max);
        }

        // ItemBuff가 가진 value를 매개 변수로 받은 값에 더해서 다시 결과 보내 준다
        public void AddValue(ref int baseValue)
        {
            baseValue += value;
        }
        #endregion
    }
}