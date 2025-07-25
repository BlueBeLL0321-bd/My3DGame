using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

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

        // 슬롯 선택
        protected GameObject selectSlotObject = null;       // 현재 선택된 슬롯 오브젝트

        public Action<GameObject> OnUpdateSelectSlot;       // 슬롯 선택 시 등록된 함수를 호출하는 이벤트 함수

        public ItemInfoUI itemInfoUI;                       // 선택된 슬롯의 아이템 정보 창
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

            // 슬롯 초기화
            UpdateSelectSlot(null);

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
            // 아이템 슬롯 체크
            if(slot == null || slot.slotUI == null)
            {
                return;
            }

            slot.slotUI.transform.GetChild(0).GetComponent<Image>().sprite
                = slot.item.id < 0 ? null : slot.ItemObject.icon;
            slot.slotUI.transform.GetChild(0).GetComponent<Image>().color
                = slot.item.id < 0 ? new Color(1, 1, 1, 0) : new Color(1, 1, 1, 1);
            slot.slotUI.GetComponentInChildren<TextMeshProUGUI>().text
                = slot.item.id < 0 ? string.Empty :
                 (slot.amount == 1 ? string.Empty : slot.amount.ToString());

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

        // 슬롯 UI 오브젝트에 마우스가 들어가면 호출
        public void OnEnter(GameObject go)
        {
            Debug.Log($"OnEnter SlotUI Object : {go.name}");
        }

        // 슬롯 UI 오브젝트에 마우스가 나가면 호출
        public void OnExit(GameObject go)
        {
            Debug.Log($"OnExit SlotUI Object : {go.name}");
        }

        // 슬롯 UI를 가지고 마우스 드래그 시작할 때 호출
        public void OnStartDrag(GameObject go)
        {
            Debug.Log($"OnStartDrag SlotUI Object : {go.name}");
        }

        // 슬롯 UI를 가지고 마우스 드래그 중일 때 호출
        public void OnDrag(GameObject go)
        {
            Debug.Log($"OnDrag SlotUI Object : {go.name}");
        }

        // 슬롯 UI를 가지고 마우스 드래그 끝날 때 호출
        public void OnEndDrag(GameObject go)
        {
            Debug.Log($"OnEndDrag SlotUI Object : {go.name}");
        }

        // 슬롯 UI를 마우스가 선택 시 호출
        public void OnClick(GameObject go)
        {
            // 이벤트 함수에 등록된 함수 먼저 호출
            // OnUpdateSelectSlot?.Invoke(go);

            // 슬롯 선택
            ItemSlot slot = slotUIs[go];

            // 아이템 체크
            if(slot.item.id >= 0)
            {
                if(selectSlotObject == go)
                {
                    UpdateSelectSlot(null);
                }
                else
                {
                    UpdateSelectSlot(go);
                }
            }
            else
            {
                UpdateSelectSlot(null);
            }
        }

        // 슬롯 선택
        public virtual void UpdateSelectSlot(GameObject go)
        {
            // 선택된 슬롯 오브젝트 저장
            selectSlotObject = go;

            // 선택 UI 활성화
            foreach (KeyValuePair<GameObject, ItemSlot> slot in slotUIs)
            {
                if(slot.Key == go)
                {
                    slot.Value.slotUI.transform.GetChild(1).GetComponent<Image>().enabled = true;
                }
                else
                {
                    slot.Value.slotUI.transform.GetChild(1).GetComponent<Image>().enabled = false;
                }
            }
        }
        #endregion
    }
}