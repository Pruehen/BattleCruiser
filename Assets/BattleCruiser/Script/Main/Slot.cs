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

    private void Start()//���� �� ��� ���Կ� �̺�Ʈ �ý��� �߰�
    {
        EventTrigger eventTrigger = this.GetComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;//�̺�Ʈ ���� ����
        entry.callback.AddListener((data) => { OnPointerEnter((PointerEventData)data); });//�̺�Ʈ�� ���� �޼��� ����
        eventTrigger.triggers.Add(entry);//�̺�Ʈ Ʈ���ſ� �̺�Ʈ �߰�

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

    void OnPointerEnter(PointerEventData eventData)//���Կ� ���콺 �÷��� ��
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
    void OnPointerExit()//���Կ��� ���콺�� �������� ��
    {
        ItemManager.Instance.isDataView = false;
        ItemManager.Instance.itemDataViewer.gameObject.SetActive(false);
    }
    void OnPointerDown()//���콺 Ŭ�� ���� ��
    {
        Debug.Log("Mouse clicked!");
    }
    void OnPointerUp()//���콺 Ŭ�� �� ��
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

    public void SwapData(Slot targetSlot)//Ÿ ���԰��� ��ġ �� ������ ��ȯ
    {
        Transform parentTemp = this.slotTrf.parent;
        CustomWeaponData customWeaponDataTemp = this.slotData;

        slotTrf.SetParent(targetSlot.slotTrf.parent);
        slotData = targetSlot.slotData;

        targetSlot.slotTrf.SetParent(parentTemp);
        targetSlot.slotData = customWeaponDataTemp;
    }
}
