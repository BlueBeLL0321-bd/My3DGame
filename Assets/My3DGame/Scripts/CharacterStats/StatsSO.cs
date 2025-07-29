using My3DGame.Common;
using System;
using UnityEngine;

namespace My3DGame
{
    /// <summary>
    /// 캐릭터 스탯 데이터를 가지고 있는 스크립터블 오브젝트
    /// </summary>
    [CreateAssetMenu(fileName = "new Stats", menuName = "Stats System/Character Stats")]
    public class StatsSO : ScriptableObject
    {
        #region Variables
        public Attribute[] attributes;          // 캐릭터 속성 배열

        // 스탯 변경 시 등록된 함수를 호출하는 이벤트 함수
        public Action<StatsSO> OnChangedStats;

        // 초기화 실행 여부 체크
        private bool isInitialized = false;
        #endregion

        #region Unity Event Method
        private void OnEnable()
        {
            InitializeAttributes();
        }
        #endregion

        #region Custom Method
        // 속성 초기화 - 최초 1회만 실행
        private void InitializeAttributes()
        {
            // 초기화 실행 여부 체크
            if (isInitialized)
                return;

            isInitialized = true;
            Debug.Log("캐릭터 Attributes 초기화");

            foreach (var attribute in attributes)
            {
                attribute.value = new ModifiableInt(OnModifiedValue);
            }

            // 속성
            SetBaseValue(CharacterAttribute.Agility, 100);
            SetBaseValue(CharacterAttribute.Intellect, 100);
            SetBaseValue(CharacterAttribute.Stamina, 100);
            SetBaseValue(CharacterAttribute.Strength, 100);
            SetBaseValue(CharacterAttribute.Health, 100);
            SetBaseValue(CharacterAttribute.Mana, 100);
        }

        // 속성의 BaseValue 값 초기화
        private void SetBaseValue(CharacterAttribute type, int value)
        {
            foreach (var attribute in attributes)
            {
                if(attribute.type == type)
                {
                    attribute.value.BaseValue = value;
                }
            }
        }

        // 속성의 BaseValue 값 가져오기
        public int GetBaseValue(CharacterAttribute type)
        {
            foreach (var attribute in attributes)
            {
                if (attribute.type == type)
                {
                    return attribute.value.BaseValue;
                }
            }

            // 지정된 타입이 없으면
            return -1;
        }

        // 속성 값 변경 시 호출되는 함수
        private void OnModifiedValue(ModifiableInt value)
        {
            OnChangedStats?.Invoke(this);
        }
        #endregion
    }
}

