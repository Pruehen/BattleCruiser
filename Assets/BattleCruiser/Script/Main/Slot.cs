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

    private void Awake()//시작 시 모든 슬롯에 이벤트 시스템 추가
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
        entryUp.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
        eventTrigger.triggers.Add(entryUp);
        
        //slotData = null;
    }

    void OnPointerEnter(PointerEventData eventData)//슬롯에 마우스 올렸을 시
    {        
        if (this.slotWeaponData != null)
        {
            ItemManager.Instance.isDataView = true;
            ItemManager.Instance.itemDataViewer.gameObject.SetActive(true);
            ItemManager.Instance.itemDataViewer.SetText(this.slotWeaponData.GetData());

            //Debug.Log(targetSlot.slotData.GetData()[0]);
        }
    }
    void OnPointerExit()//슬롯에서 마우스가 내려갔을 시
    {
        ItemManager.Instance.isDataView = false;
        ItemManager.Instance.itemDataViewer.gameObject.SetActive(false);
    }
    void OnPointerDown()//마우스 클릭 누를 시
    {
        if (this.slotWeaponData != null)
        {
            ItemManager.Instance.selectedSlot = this;

            this.transform.SetParent(GameObject.Find("Canvas").transform);
            this.GetComponent<Image>().raycastTarget = false;
        }
    }
    void OnPointerUp(PointerEventData eventData)//마우스 클릭 뗄 시
    {
        if (this.slotWeaponData != null && eventData != null)
        {
            Slot target;
            bool check = eventData.pointerEnter.TryGetComponent(out target);

            if (target != this && check)
            {
                if (CanMerge(this, target))
                {
                    Debug.Log("데이터 병합");
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

    public static void SwapData(Slot targetSlot1, Slot targetSlot2)//타 슬롯과의 위치 및 데이터 교환
    {
        Vector2 positionTemp = targetSlot1.startlocalPosition;
        Transform parentTemp = targetSlot1.slotParentTrf;
        int indexTemp = targetSlot1.index;

        targetSlot1.transform.SetParent(targetSlot2.slotParentTrf);//1번 슬롯의 부모를 2번 슬롯의 부모로 설정
        targetSlot1.slotParentTrf = targetSlot1.transform.parent;//부모 갱신
        targetSlot2.transform.SetParent(parentTemp);//2번 슬롯의 부모를 1번 슬롯의 부모로 설정
        targetSlot2.slotParentTrf = targetSlot2.transform.parent;//부모 갱신

        targetSlot1.transform.localPosition = targetSlot2.startlocalPosition;//1번 슬롯 위치를 2번 슬롯으로 이동
        targetSlot1.startlocalPosition = targetSlot1.transform.localPosition;//1번 슬롯의 시작 포인트를 현재 위치로 갱신
        targetSlot2.transform.localPosition = positionTemp;//2번 슬롯의 위치를 기존 1번 슬롯의 시작 포인트로 이동
        targetSlot2.startlocalPosition = targetSlot2.transform.localPosition;//2번 슬롯의 시작 포인트를 현재 위치로 갱신

        targetSlot1.index = targetSlot2.index;//1번 슬롯의 인덱스 갱신
        targetSlot2.index = indexTemp;//2번 슬롯의 인덱스 갱신

        ItemManager.Instance.slotData[targetSlot1.index] = targetSlot1;//딕셔너리의 데이터 갱신
        //Debug.Log(targetSlot1.index);
        ItemManager.Instance.slotData[targetSlot2.index] = targetSlot2;//딕셔너리의 데이터 갱신
        //Debug.Log(targetSlot2.index);        

        CustomShipManager.Instance.UpdataEquipment(targetSlot1.index, targetSlot1.slotWeaponData);
        CustomShipManager.Instance.UpdataEquipment(targetSlot2.index, targetSlot2.slotWeaponData);
    }

    public static void MergeData(Slot targetSlot1, Slot targetSlot2)//데이터 병합. Slot2 위치에 병합한 데이터를 생성함.
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
        if(targetSlot1.slotWeaponData != null && targetSlot2.slotWeaponData != null)//둘다 데이터가 있는 슬롯이고
        {
            if (targetSlot1.slotWeaponData.weaponData.weaponKey == targetSlot2.slotWeaponData.weaponData.weaponKey)//무기 타입이 같으며
            {
                if (targetSlot1.slotWeaponData.rarityNum == targetSlot2.slotWeaponData.rarityNum)//레어도까지 같으며
                {
                    if(targetSlot1.slotWeaponData.rarityNum < 7)//레어도가 7 미만일 경우
                    {
                        return true;//true 반환
                    }
                    
                }
            }
        }
        return false;
    }
}

