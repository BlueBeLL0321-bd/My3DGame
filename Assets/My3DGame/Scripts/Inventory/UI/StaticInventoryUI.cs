using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using My3DGame.Manager;

namespace My3DGame.InventorySystem
{
    /// <summary>
    /// 개수와 자리가 고정된 아이템 슬롯을 가진 인벤토리 UI를 관리하는 클래스, InventoryUI 상속
    /// </summary>
    public class StaticInventoryUI : InventoryUI
    {
        #region Variables
        public GameObject[] staticSlots;
        #endregion

        #region Custom Method
        public override void CreateSlots()
        {
            slotUIs = new Dictionary<GameObject, ItemSlot>();

            for (int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                GameObject go = staticSlots[i];

                // 생성된 슬롯 오브젝트의 트리거에 이벤트 등록
                AddEvent(go, EventTriggerType.PointerEnter, delegate { OnEnter(go); });
                AddEvent(go, EventTriggerType.PointerExit, delegate { OnExit(go); });
                AddEvent(go, EventTriggerType.BeginDrag, delegate { OnStartDrag(go); });
                AddEvent(go, EventTriggerType.Drag, delegate { OnDrag(go); });
                AddEvent(go, EventTriggerType.EndDrag, delegate { OnEndDrag(go); });
                AddEvent(go, EventTriggerType.PointerClick, delegate { OnClick(go); });

                // slotUIs 등록
                inventoryObject.Slots[i].slotUI = go;
                slotUIs.Add(go, inventoryObject.Slots[i]);
            }
        }

        public override void UpdateSelectSlot(GameObject go)
        {
            base.UpdateSelectSlot(go);

            if (selectSlotObject == null)
            {
                itemInfoUI.gameObject.SetActive(false);
            }
            else
            {
                itemInfoUI.gameObject.SetActive(true);
                itemInfoUI.SetItemInfoUI(slotUIs[selectSlotObject], true);
            }
        }

        // 아이템 장착 해제
        public void Unequip()
        {
            // 선택 아이템 오브젝트 체크
            if (selectSlotObject == null)
                return;

            // 인벤토리에 제거하는 아이템 추가 - 인벤 풀 체크
            if(UIManager.Instance.AddItemInventory(slotUIs[selectSlotObject].item, 1))
            {
                // 아이템 제거
                slotUIs[selectSlotObject].RemoveItem();
                // 선택 해제
                UpdateSelectSlot(null);
            }
        }

        // 모든 아이템 장착 해제
        public void UnequipAll()
        {
            foreach (var slotObject in staticSlots)
            {
                // 인벤토리에 제거하는 아이템 추가 - 인벤 풀 체크
                if (UIManager.Instance.AddItemInventory(slotUIs[slotObject].item, 1))
                {
                    // 아이템 제거
                    slotUIs[slotObject].RemoveItem();
                }
            }
            // 선택 해제
            UpdateSelectSlot(null);
        }
        #endregion
    }
}

