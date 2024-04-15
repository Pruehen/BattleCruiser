using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour
{
    public Transform slotTrf { get; private set; }
    //public int index { get; private set; }
    public CustomWeaponData slotData { get; private set; }

    private void Start()//시작 시 모든 슬롯에 이벤트 시스템 추가
    {
        EventTrigger eventTrigger = this.GetComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;//이벤트 유형 설정
        entry.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });//이벤트에 대한 메서드 설정
        eventTrigger.triggers.Add(entry);//이벤트 트리거에 이벤트 추가

        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { OnPointerExit(); });
        eventTrigger.triggers.Add(entryExit);

        EventTrigger.Entry entryDown = new EventTrigger.Entry();
        entryDown.eventID = EventTriggerType.PointerDown;
        entryDown.callback.AddListener((data) => { OnPointerDown(); });
        eventTrigger.triggers.Add(entryDown);

        EventTrigger.Entry entryUp = new EventTrigger.Entry();
        entryUp.eventID = EventTriggerType.PointerUp;
        entryUp.callback.AddListener((data) => { OnPointerUp(); });
        eventTrigger.triggers.Add(entryUp);
    }

    void OnPointerEnter(PointerEventData eventData)//슬롯에 마우스 올렸을 시
    {
        Slot targetSlot = eventData.pointerEnter.GetComponent<Slot>();

        if (targetSlot.slotData != null)
        {
            ItemManager.Instance.isDataView = true;
            ItemManager.Instance.itemDataViewer.gameObject.SetActive(true);
            ItemManager.Instance.itemDataViewer.SetText(targetSlot.slotData.GetData());

            //Debug.Log(targetSlot.index);
        }
    }
    void OnPointerExit()//슬롯에서 마우스가 내려갔을 시
    {
        ItemManager.Instance.isDataView = false;
        ItemManager.Instance.itemDataViewer.gameObject.SetActive(false);
    }
    void OnPointerDown()//마우스 클릭 누를 시
    {
        Debug.Log("Mouse clicked!");
    }
    void OnPointerUp()//마우스 클릭 뗄 시
    {
        Debug.Log("Mouse clicked!");
    }
    public void Init()
    {
        //this.index = index;
        this.slotTrf = this.gameObject.transform;
        slotData = null;
    }

    public void AddData(CustomWeaponData data)
    {
        slotData = data;
        ItemManager.Instance.AddItem(data.baseWeaponKey.Index(), data.rarityNum, slotTrf);
    }

    public void SwapData(Slot targetSlot)//타 슬롯과의 위치 및 데이터 교환
    {
        Transform parentTemp = this.slotTrf.parent;
        CustomWeaponData customWeaponDataTemp = this.slotData;

        slotTrf.SetParent(targetSlot.slotTrf.parent);
        slotData = targetSlot.slotData;

        targetSlot.slotTrf.SetParent(parentTemp);
        targetSlot.slotData = customWeaponDataTemp;
    }
}
