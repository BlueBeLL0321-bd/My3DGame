using UnityEngine;
using My3DGame.InventorySystem;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using My3DGame.ItemSystem;

namespace My3DGame
{
    public class ShopUI : InventoryUI
    {
        #region Variables
        public InventorySO playerInventory;
        public StatsSO playerStats;

        public GameObject[] staticSlots;

        // 상점에서 판매되는 아이템 목록
        public ItemSO[] itemObjects;

        public GameObject buyButton;
        #endregion

        #region Unity Event Method
        #endregion

        #region Custom Method
        public override void CreateSlots()
        {
            slotUIs = new Dictionary<GameObject, ItemSlot>();

            for (int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                GameObject go = staticSlots[i];

                // 생성된 슬롯 오브젝트의 트리거에 이벤트 등록
                AddEvent(go, EventTriggerType.PointerClick, delegate { OnClick(go); });

                // slotUIs 등록
                inventoryObject.Slots[i].slotUI = go;
                slotUIs.Add(go, inventoryObject.Slots[i]);

                // 슬롯에 판매되는 아이템 세팅
                Item shopItem = itemObjects[i].CreateItem();
                slotUIs[go].UpdateSlot(shopItem, 1);
            }
        }

        //
        public override void UpdateSelectSlot(GameObject go)
        {
            base.UpdateSelectSlot(go);

            if(selectSlotObject == null)
            {
                buyButton.SetActive(false);
            }
            else
            {
                buyButton.SetActive(true);
            }
        }

        // 아이템 구매
        public void BuyItem()
        {
            // Debug.Log("선택된 아이템 구매해서 인벤토리에 넣는다");
            if(selectSlotObject == null)
            {
                return;
            }

            int price = slotUIs[selectSlotObject].ItemObject.shopPrice;

            // 소지금 체크
            if(playerStats.EnoughGold(price))
            {
                Item newItem = slotUIs[selectSlotObject].ItemObject.CreateItem();
                if(playerInventory.AddItem(newItem, 1))
                {
                    playerStats.UseGold(price);
                    UpdateSelectSlot(null);
                }
            }
            else
            {
                Debug.Log("소지금 부족");
            }
        }
        #endregion
    }
}

