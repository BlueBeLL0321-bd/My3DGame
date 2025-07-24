using System;
using System.Linq;
using My3DGame.ItemSystem;

namespace My3DGame.InventorySystem
{
    /// <summary>
    /// 인벤토리를 관리하는 클래스
    /// 속성 : 슬롯 목록
    /// 기능 : 인벤토리 비우기, 아이템 존재 여부 체크
    /// </summary>
    [System.Serializable]
    public class Inventory
    {
        #region Variables
        public ItemSlot[] slots = new ItemSlot[16];
        #endregion

        #region Custom Method
        // 인벤토리 비우기
        public void Clear()
        {
            foreach (var itemSlot in slots)
            {
                itemSlot.UpdateSlot(new Item(), 0);
            }
        }

        public bool IsContain(ItemSO itemObject)
        {
            return IsContain(itemObject.data.id);
            // return Array.Find(slots, i => i.item.id == itemObject.data.id) != null;
        }

        public bool IsContain(int id)
        {
            return slots.FirstOrDefault(i => i.item.id == id) != null;
        }
        #endregion
    }
}

