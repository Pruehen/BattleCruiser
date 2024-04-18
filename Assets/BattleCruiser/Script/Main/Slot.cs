using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Transform slotParentTrf;
    public Vector2 startlocalPosition;
    public int index;
    public CustomWeaponData slotWeaponData { get; private set; }

    private void Awake()//���� �� ��� ���Կ� �̺�Ʈ �ý��� �߰�
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
        entryUp.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
        eventTrigger.triggers.Add(entryUp);
        
        //slotData = null;
    }

    void OnPointerEnter(PointerEventData eventData)//���Կ� ���콺 �÷��� ��
    {        
        if (this.slotWeaponData != null)
        {
            ItemManager.Instance.isDataView = true;
            ItemManager.Instance.itemDataViewer.gameObject.SetActive(true);
            ItemManager.Instance.itemDataViewer.SetText(this.slotWeaponData.GetData());

            //Debug.Log(targetSlot.slotData.GetData()[0]);
        }
    }
    void OnPointerExit()//���Կ��� ���콺�� �������� ��
    {
        ItemManager.Instance.isDataView = false;
        ItemManager.Instance.itemDataViewer.gameObject.SetActive(false);
    }
    void OnPointerDown()//���콺 Ŭ�� ���� ��
    {
        if (this.slotWeaponData != null)
        {
            ItemManager.Instance.selectedSlot = this;

            this.transform.SetParent(GameObject.Find("Canvas").transform);
            this.GetComponent<Image>().raycastTarget = false;
        }
    }
    void OnPointerUp(PointerEventData eventData)//���콺 Ŭ�� �� ��
    {
        if (this.slotWeaponData != null && eventData != null)
        {
            Slot target;
            bool check = eventData.pointerEnter.TryGetComponent(out target);

            if (target != this && check)
            {
                if (CanMerge(this, target))
                {
                    Debug.Log("������ ����");
                    MergeData(this, target);

                    this.gameObject.transform.SetParent(this.slotParentTrf);
                    this.gameObject.transform.localPosition = this.startlocalPosition;
                }
                else
                {
                    SwapData(this, target);
                }
            }
            else
            {
                this.gameObject.transform.SetParent(this.slotParentTrf);
                this.gameObject.transform.localPosition = this.startlocalPosition;                
            }
            this.GetComponent<Image>().raycastTarget = true;
            ItemManager.Instance.selectedSlot = null;
        }
    }
    public void Init(int index)
    {        
        this.index = index;
        this.slotParentTrf = this.gameObject.transform.parent;
        startlocalPosition = this.transform.localPosition;
        slotWeaponData = null;
    }

    public void SetData(CustomWeaponData data)
    {
        slotWeaponData = data;

        ItemManager.Instance.TryRemoveItemImage(this.transform);
        if (data != null)
        {
            ItemManager.Instance.AddItemImage(data.weaponData.sptiteIndex, data.rarityNum, this.transform);
        }        
    }

    public static void SwapData(Slot targetSlot1, Slot targetSlot2)//Ÿ ���԰��� ��ġ �� ������ ��ȯ
    {
        Vector2 positionTemp = targetSlot1.startlocalPosition;
        Transform parentTemp = targetSlot1.slotParentTrf;
        int indexTemp = targetSlot1.index;

        targetSlot1.transform.SetParent(targetSlot2.slotParentTrf);//1�� ������ �θ� 2�� ������ �θ�� ����
        targetSlot1.slotParentTrf = targetSlot1.transform.parent;//�θ� ����
        targetSlot2.transform.SetParent(parentTemp);//2�� ������ �θ� 1�� ������ �θ�� ����
        targetSlot2.slotParentTrf = targetSlot2.transform.parent;//�θ� ����

        targetSlot1.transform.localPosition = targetSlot2.startlocalPosition;//1�� ���� ��ġ�� 2�� �������� �̵�
        targetSlot1.startlocalPosition = targetSlot1.transform.localPosition;//1�� ������ ���� ����Ʈ�� ���� ��ġ�� ����
        targetSlot2.transform.localPosition = positionTemp;//2�� ������ ��ġ�� ���� 1�� ������ ���� ����Ʈ�� �̵�
        targetSlot2.startlocalPosition = targetSlot2.transform.localPosition;//2�� ������ ���� ����Ʈ�� ���� ��ġ�� ����

        targetSlot1.index = targetSlot2.index;//1�� ������ �ε��� ����
        targetSlot2.index = indexTemp;//2�� ������ �ε��� ����

        ItemManager.Instance.slotData[targetSlot1.index] = targetSlot1;//��ųʸ��� ������ ����
        //Debug.Log(targetSlot1.index);
        ItemManager.Instance.slotData[targetSlot2.index] = targetSlot2;//��ųʸ��� ������ ����
        //Debug.Log(targetSlot2.index);        

        CustomShipManager.Instance.UpdataEquipment(targetSlot1.index, targetSlot1.slotWeaponData);
        CustomShipManager.Instance.UpdataEquipment(targetSlot2.index, targetSlot2.slotWeaponData);
    }

    public static void MergeData(Slot targetSlot1, Slot targetSlot2)//������ ����. Slot2 ��ġ�� ������ �����͸� ������.
    {
        targetSlot2.SetData(new CustomWeaponData(targetSlot2.slotWeaponData.weaponData.weaponKey, targetSlot2.slotWeaponData.rarityNum + 1));
        targetSlot1.SetData(null);

        ItemManager.Instance.slotData[targetSlot2.index] = targetSlot2;
        ItemManager.Instance.slotData[targetSlot2.index] = targetSlot2;

        CustomShipManager.Instance.UpdataEquipment(targetSlot1.index, targetSlot1.slotWeaponData);
        CustomShipManager.Instance.UpdataEquipment(targetSlot2.index, targetSlot2.slotWeaponData);

        ItemManager.Instance.itemDataViewer.SetText(targetSlot2.slotWeaponData.GetData());
    }

    public static bool CanMerge(Slot targetSlot1, Slot targetSlot2)
    {
        if(targetSlot1.slotWeaponData != null && targetSlot2.slotWeaponData != null)//�Ѵ� �����Ͱ� �ִ� �����̰�
        {
            if (targetSlot1.slotWeaponData.weaponData.weaponKey == targetSlot2.slotWeaponData.weaponData.weaponKey)//���� Ÿ���� ������
            {
                if (targetSlot1.slotWeaponData.rarityNum == targetSlot2.slotWeaponData.rarityNum)//������� ������
                {
                    if(targetSlot1.slotWeaponData.rarityNum < 7)//����� 7 �̸��� ���
                    {
                        return true;//true ��ȯ
                    }
                    
                }
            }
        }
        return false;
    }
}

