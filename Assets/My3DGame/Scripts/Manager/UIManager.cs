using UnityEngine;
using My3DGame.ItemSystem;
using My3DGame.InventorySystem;

namespace My3DGame.Manager
{
    /// <summary>
    /// 게임 플레이 중 UI를 관리하는 클래스
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        #region Variables
        public ItemDataBase itemDataBase;
        public InventorySO inventoryObject;

        public int index = 2;
        #endregion

        #region Unity Event Method
        private void Update()
        {
            // 치트키
            if(Input.GetKeyDown(KeyCode.M))
            {
                Item newItem = itemDataBase.itemObjects[index].CreateItem();
                inventoryObject.AddItem(newItem, 1);
            }
        }
        #endregion
    }
}

