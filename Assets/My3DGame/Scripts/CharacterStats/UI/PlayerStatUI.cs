using UnityEngine;
using TMPro;
using My3DGame.InventorySystem;
using My3DGame.Common;

namespace My3DGame
{
    /// <summary>
    /// 플레이어 스탯 UI를 관리하는 클래스
    /// </summary>
    public class PlayerStatUI : MonoBehaviour
    {
        #region Variables
        public StatsSO statsObject;
        public TextMeshProUGUI[] attributesText;

        public InventorySO equipmentInventory;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // equipmentInventory의 슬롯의 이벤트 함수에 등록
            if(statsObject != null && equipmentInventory != null)
            {
                foreach (var slot in equipmentInventory.Slots)
                {
                    slot.OnPreUpdate += OnUnequipItem;
                    slot.OnPostUpdate += OnEquipItem;
                }
            }
        }

        private void OnEnable()
        {
            UpdateAttributesText();
            statsObject.OnChangedStats += OnChangedStats;
        }

        private void OnDisable()
        {
            statsObject.OnChangedStats -= OnChangedStats;
        }
        #endregion

        #region Custom Method
        // UI Text 적용
        private void UpdateAttributesText()
        {
            attributesText[0].text = statsObject.GetModifiedValue(Common.CharacterAttribute.Agility).ToString();
            attributesText[1].text = statsObject.GetModifiedValue(Common.CharacterAttribute.Intellect).ToString();
            attributesText[2].text = statsObject.GetModifiedValue(Common.CharacterAttribute.Stamina).ToString();
            attributesText[3].text = statsObject.GetModifiedValue(Common.CharacterAttribute.Strength).ToString();
        }

        // 아이템 장착 시 stats에 아이템 buff 값 추가
        private void OnEquipItem(ItemSlot itemSlot)
        {
            // 슬롯 체크
            if (itemSlot.ItemObject == null)
                return;

            // 장착 인벤토리 여부 체크
            if(itemSlot.parent.type == InventoryType.Equipment)
            {
                foreach (var buff in itemSlot.item.buffs)
                {
                    foreach (var attribute in statsObject.attributes)
                    {
                        if (attribute.type == buff.stat)
                        {
                            attribute.value.AddModifier(buff);
                        }
                    }
                }
            }
        }

        // 아이템 탈착 시 stats에 아이템 buff 값 제거
        private void OnUnequipItem(ItemSlot itemSlot)
        {
            // 슬롯 체크
            if (itemSlot.ItemObject == null)
                return;

            // 탈착 인벤토리 여부 체크
            if (itemSlot.parent.type == InventoryType.Equipment)
            {
                foreach (var buff in itemSlot.item.buffs)
                {
                    foreach (var attribute in statsObject.attributes)
                    {
                        if (attribute.type == buff.stat)
                        {
                            attribute.value.RemoveModifier(buff);
                        }
                    }
                }
            }
        }

        private void OnChangedStats(StatsSO statsObject)
        {
            UpdateAttributesText();
        }
        #endregion
    }
}

