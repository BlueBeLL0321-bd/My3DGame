using UnityEngine;
using My3DGame.Util;
using My3DGame.ItemSystem;
using My3DGame.InventorySystem;

namespace My3DGame.Manager
{
    /// <summary>
    /// 게임 플레이 중 UI를 관리하는 클래스
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        #region Variables
        public ItemDataBase itemDataBase;
        public InventorySO inventoryObject;

        public DynamicInventoryUI playerInventoryUI;
        public StaticInventoryUI playerEquipmentUI;
        
        // 치팅
        public int index = 2;
        #endregion

        #region Unity Event Method
        protected override void Awake()
        {
            base.Awake();

            // update select 이벤트 함수 등록
            playerInventoryUI.OnUpdateSelectSlot += playerEquipmentUI.UpdateSelectSlot;

            playerEquipmentUI.OnUpdateSelectSlot += playerInventoryUI.UpdateSelectSlot;
        }

        private void Update()
        {
            //
            if(Input.GetKeyDown(KeyCode.I))
            {
                TogglePlayerInventoryUI();
            }
            else if(Input.GetKeyDown(KeyCode.U))
            {
                TogglePlayerEquipmentUI();
            }

            // 치트키
            if (Input.GetKeyDown(KeyCode.M))
            {
                Item newItem = itemDataBase.itemObjects[index].CreateItem();
                inventoryObject.AddItem(newItem, 1);
            }
        }
        #endregion

        #region Custom Method
        private void Toggle(GameObject go)
        {
            go.SetActive(!go.activeSelf);

            // 마우스 커서 제어
            // UI Open 체크
            if(IsUIOpen())
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                Time.timeScale = 0f;
            }
            else // UI Close
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                Time.timeScale = 1f;
            }
        }

        // UI Open Check
        public bool IsUIOpen()
        {
            bool isOpen = false;

            isOpen |= playerInventoryUI.gameObject.activeSelf;
            isOpen |= playerEquipmentUI.gameObject.activeSelf;

            return isOpen;
        }

        public void TogglePlayerInventoryUI()
        {
            Toggle(playerInventoryUI.gameObject);
            if(playerInventoryUI.gameObject.activeSelf == false)
            {
                // 선택 해제
                playerInventoryUI.UpdateSelectSlot(null);
            }
        }

        public void TogglePlayerEquipmentUI()
        {
            Toggle(playerEquipmentUI.gameObject);
            if(playerEquipmentUI.gameObject.activeSelf == false)
            {
                // 선택 해제
                playerEquipmentUI.UpdateSelectSlot(null);
            }
        }

        // 인벤토리에 아이템 추가
        public bool AddItemInventory(Item newItem, int amount)
        {
            return inventoryObject.AddItem(newItem, amount);
        }

        // 장착 인벤토리에 아이템 장착
        public void EquipItemInventory(ItemSlot itemSlot)
        {
            playerEquipmentUI.Equip(itemSlot);
        }
        #endregion
    }
}

