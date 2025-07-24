using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace My3DGame.InventorySystem
{
    /// <summary>
    /// 인벤토리 UI를 관리하는 클래스들의 부모 (추상) 클래스
    /// 속성 : 인벤토리 오브젝트, UI에 있는 슬롯 오브젝트를 관리하는 목록 (Dictionary)
    /// 필수(abstract) 기능 : 슬롯 오브젝트 생성(인벤토리에 있는 슬롯 숫자만큼)
    /// </summary>
    [RequireComponent(typeof(EventTrigger))]
    public abstract class InventoryUI : MonoBehaviour
    {
        #region Variables
        public InventorySO inventoryObject;

        public Dictionary<GameObject, ItemSlot> slotUIs = new Dictionary<GameObject, ItemSlot>();
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 인벤토리 오브젝트에 있는 아이템 슬롯으로 슬롯 오브젝트 생성
            CreateSlots();

            // 인벤토리 오브젝트에 있는 아이템 슬롯 값 설정
            for (int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                inventoryObject.Slots[i].parent = inventoryObject;
                inventoryObject.Slots[i].OnPostUpdate += OnPostUpdate;

                // 강제로 슬롯 업데이트 실행
                inventoryObject.Slots[i].OnPostUpdate?.Invoke(inventoryObject.Slots[i]);
            }

            // 이벤트 추가
            AddEvent(this.gameObject, EventTriggerType.PointerEnter,
                delegate { OnEnterInterface(this.gameObject); });
            AddEvent(this.gameObject, EventTriggerType.PointerExit,
                delegate { OnExitInterface(this.gameObject); });
        }
        #endregion

        #region Custom Method
        // 필수(abstract) 기능 정의 : 슬롯 오브젝트 생성
        public abstract void CreateSlots();

        // ItemSlot Update 시 변경 후 호출되는 이벤트 함수에 등록
        public void OnPostUpdate(ItemSlot slot)
        {

        }

        // 이벤트 함수 등록
        protected void AddEvent(GameObject go, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            // 이벤트 오브젝트 체크
            EventTrigger trigger = go.GetComponent<EventTrigger>();
            if(trigger == null)
            {
                Debug.Log("No Event Trigger Component Found");
                return;
            }

            // 이벤트 엔트리 구성
            EventTrigger.Entry eventTrigger = new EventTrigger.Entry { eventID = type };
            eventTrigger.callback.AddListener(action);
            // 이벤트 엔트리를 등록
            trigger.triggers.Add(eventTrigger);
        }

        // 인벤토리 UI에 마우스가 들어오면 호출
        public void OnEnterInterface(GameObject go)
        {
            Debug.Log($"OnEnterInterface Object : {go.name}");
        }

        // 인벤토리 UI에 마우스가 나가면 호출
        public void OnExitInterface(GameObject go)
        {
            Debug.Log($"OnExitInterface Object : {go.name}");
        }
        #endregion
    }
}