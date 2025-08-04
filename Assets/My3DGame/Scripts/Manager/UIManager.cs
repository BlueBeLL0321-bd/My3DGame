using UnityEngine;
using My3DGame.Util;
using My3DGame.ItemSystem;
using My3DGame.InventorySystem;
using My3DGame.Common;

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

        public DrawDialog dialogUI;
        public QuestUI questUI;
        
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
            else if(Input.GetKeyDown(KeyCode.Q))
            {
                if(questUI.gameObject.activeSelf)
                {
                    CloseQuestUI();
                }
                else
                {
                    OpenPlayerQuestUI();
                }
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
            isOpen |= dialogUI.gameObject.activeSelf;
            isOpen |= questUI.gameObject.activeSelf;

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

        public void OpenDialogUI(int dialogIndex, NpcType npcType = NpcType.None)
        {
            Toggle(dialogUI.gameObject);
            Time.timeScale = 1f;

            dialogUI.OnCloseDialog += CloseDialogUI;
            // 퀘스트를 가지고 있는 NPC이면
            if (npcType == NpcType.QuestGiver)       
            {
                dialogUI.OnCloseDialog += OpenQuestUI;
            }

            dialogUI.StartDialog(dialogIndex);
        }

        public void CloseDialogUI()
        {
            Toggle(dialogUI.gameObject);
        }

        // 대화창 종료 시 오픈되는 Quest UI
        public void OpenQuestUI()
        {
            if(questUI.OpenQuestUI() == true)
            {
                Toggle(questUI.gameObject);
                questUI.OnCloseQuestUI += ToggleQuestUI;
            }
        }

        // 현재 진행 중인 플레이어의 Quest 정보를 확인하는 Quest UI
        public void OpenPlayerQuestUI()
        {
            if(questUI.OpenPlayerQuestUI())
            {
                Toggle(questUI.gameObject);
                questUI.OnCloseQuestUI += ToggleQuestUI;
            }
        }

        private void ToggleQuestUI()
        {
            Toggle(questUI.gameObject);
        }

        public void CloseQuestUI()
        {
            questUI.CloseQuestUI();
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

