using UnityEngine;
using TMPro;

namespace My3DGame.InventorySystem
{
    /// <summary>
    /// 선택된 아이템의 정보 보여 주기
    /// </summary>
    public class ItemInfoUI : MonoBehaviour
    {
        #region Variables
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI itemDescription;

        // ...

        public GameObject equipButton;
        public GameObject useButton;
        public GameObject sellButton;
        public GameObject unequipButton;
        #endregion

        #region Custom Method
        public void SetItemInfoUI(ItemSlot itemSlot, bool isEquipInven)
        {
            itemName.text = itemSlot.ItemObject.name;
            itemDescription.text = itemSlot.ItemObject.description;

            // ...

            // 버튼 세팅
            ResetButton();
            if(isEquipInven)
            {
                unequipButton.SetActive(true);
            }
            else
            {
                if(itemSlot.ItemObject.type == Common.ItemType.Food
                    || itemSlot.ItemObject.type == Common.ItemType.Default)
                {
                    useButton.SetActive(true);
                }
                else
                {
                    equipButton.SetActive(true);
                }
                sellButton.SetActive(true);
            }
        }

        private void ResetButton()
        {
            unequipButton.SetActive(false);
            useButton.SetActive(false);
            equipButton.SetActive(false);
            sellButton.SetActive(false);
        }
        #endregion
    }
}
