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
        #endregion

        #region Custom Method
        public void SetItemInfoUI(ItemSlot itemSlot)
        {
            itemName.text = itemSlot.ItemObject.name;
            itemDescription.text = itemSlot.ItemObject.description;

            // ...
        }
        #endregion
    }
}
