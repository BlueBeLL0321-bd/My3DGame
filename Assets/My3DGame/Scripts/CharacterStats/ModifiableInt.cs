using UnityEngine;
using System;
using System.Collections.Generic;

namespace My3DGame
{
    /// <summary>
    /// 캐릭터의 속성 값을 관리하는 클래스
    /// </summary>
    [Serializable]
    public class ModifiableInt
    {
        #region Variables
        private int baseValue;      // stat 기본값
        [SerializeField]
        private int modifiedValue;  // 아이템 스탯이 적용된 최종 계산값

        // modifiedValue 값 변경(계산) 시 등록된 함수 실행하는 이벤트 함수
        private event Action<ModifiableInt> OnModifiedValue;

        // modifiedValue 값을 계산하기 위한 스탯 값들을 저장하는 리스트
        public List<IModifier> modifiers = new List<IModifier>();
        #endregion

        #region Property
        public int BaseValue
        {
            get { return baseValue; }
            set
            {
                baseValue = value;
                // 스탯 계산 업데이트
                UpdateModifiedValue();
            }
        }

        public int ModifiedValue
        {
            get { return modifiedValue; }
            set { modifiedValue = value; }
        }
        #endregion

        // 생성자 - 값 변경 시 호출되는 함수를 매개 변수로 받아 이벤트 함수에 등록
        #region Constructor
        public ModifiableInt(Action<ModifiableInt> method = null)
        {
            // 기본값으로 초기화
            ModifiedValue = baseValue;

            // 이벤트 함수 등록
            RegisterModEvent(method);
        }
        #endregion

        #region Custom Method
        public void RegisterModEvent(Action<ModifiableInt> method)
        {
            // method 체크
            if (method == null)
                return;

            OnModifiedValue += method;
        }

        // 매개 변수로 받아 이벤트 함수에서 제거
        public void RemoveModEvent(Action<ModifiableInt> method)
        {
            // method 체크
            if (method == null)
                return;

            OnModifiedValue -= method;
        }

        // modifiedValue 값을 계산하기
        private void UpdateModifiedValue()
        {
            // 스탯 누적 변수
            int valueToAdd = 0;

            foreach (var modifier in modifiers)
            {
                modifier.AddValue(ref valueToAdd);
            }
            // 최종값
            ModifiedValue = baseValue + valueToAdd;

            // modifiedValue 값 변경(계산) 시 등록된 함수 호출
            OnModifiedValue?.Invoke(this);
        }

        // 속성 값 추가
        public void AddModifier(IModifier modifier)
        {
            // 리스트에 추가
            modifiers.Add(modifier);
            // 최종값 계산
            UpdateModifiedValue();
        }

        // 속성 값 제거
        public void RemoveModifier(IModifier modifier)
        {
            // 리스트에서 제거
            modifiers.Remove(modifier);
            // 최종값 계산
            UpdateModifiedValue();
        }
        #endregion
    }
}

