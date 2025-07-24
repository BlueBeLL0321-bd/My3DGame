using UnityEngine;

namespace My3DGame
{
    /// <summary>
    /// 값을 연산하는 기능 정의
    /// </summary>
    public interface IModifier
    {
        // 매개 변수로 추가해야 되는 변수를 넘겨 주고 결과를 받아 온다
        public void AddValue(ref int baseValue);
    }
}

