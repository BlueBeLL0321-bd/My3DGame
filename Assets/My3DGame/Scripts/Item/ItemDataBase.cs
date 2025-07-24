using UnityEngine;

namespace My3DGame.ItemSystem
{
    /// <summary>
    /// 입력된 아이템 정보들을 관리하는 스크립터블 오브젝트
    /// </summary>
    [CreateAssetMenu(fileName = "New ItemDataBase", menuName = "Inventory System/ItemDataBase")]
    public class ItemDataBase : ScriptableObject
    {
        #region Variables
        public ItemSO[] itemObjects;
        #endregion

        #region Unity Event Method
        // 인스펙터 창 변경 시 호출되는 함수
        private void OnValidate()
        {
            // itemObjects 배열을 반복문으로 돌려서 ID값 세팅
            for (int i = 0; i < itemObjects.Length; i++)
            {
                if (itemObjects[i] == null)
                    continue;

                itemObjects[i].data.id = i;
            }
        }
        #endregion
    }
}

