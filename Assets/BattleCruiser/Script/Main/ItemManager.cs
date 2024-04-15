using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemManager : SceneSingleton<ItemManager>
{
    public Transform slotParentTrf;//인벤토리 슬롯 전체의 루트 트랜스폼
    public Transform customShipTrf;//커스텀 쉽의 루트 트랜스폼
    public Dictionary<int, Slot> slotData;//슬롯 데이터 딕셔너리
    public Dictionary<int, CustomWeaponData> weaponData;

    public GameObject rarityCurver;//레어도 커버
    public GameObject newItemCurver;//새로운 아이템 커버
    public GameObject[] weaponIcons;//무기 아이콘 프리팹

    public ItemDataViewer itemDataViewer;//무기 데이터를 표시하는 창    
    public bool isDataView = false;
    public Slot selectedSlot = null;


    void Awake()
    {
        slotData = new Dictionary<int, Slot>();

        for (int i = 0; i < slotParentTrf.childCount; i++)//인벤토리 슬롯 순회하며 초기화
        {
            Slot slot = slotParentTrf.GetChild(i).GetComponent<Slot>();
            slotData.Add(i, slot);//슬롯 생성
            slot.Init(i);//슬롯 초기화
        }
        for (int i = 0; i < customShipTrf.childCount; i++)
        {
            customShipTrf.GetChild(i).GetComponent<CustomShip>().Init();//커스텀쉽 데이터 초기화(슬롯 초기화 포함)
        }

        //SlotsInit();//슬롯에 이벤트 트리거 추가        

        weaponData = JsonDataManager.Instance.saveData.userData.customWeaponDatas;        
        foreach (var item in weaponData)//딕셔너리의 모든 키 조회
        {
            if (slotData[item.Key].slotData == null)
            {
                slotData[item.Key].AddData(item.Value);
            }
        }
    }

    void Update()
    {
        if(isDataView)
        {
            itemDataViewer.transform.position = new Vector2(Mathf.Clamp(Input.mousePosition.x, 0, Screen.width - 700), Mathf.Clamp(Input.mousePosition.y, 350, Screen.height));
        }
        if(selectedSlot != null)
        {
            selectedSlot.gameObject.transform.position = Input.mousePosition;
        }
    }

    public void AddItem(int weaponIndex, int rarity, Transform transform)//슬롯에 이미지 추가
    {
        GameObject itemData = new GameObject();
        itemData.transform.SetParent(transform);
        itemData.transform.localPosition = Vector3.zero;
        itemData.name = "Data";

        Image rarityCurverImage = Instantiate(rarityCurver, itemData.transform).GetComponent<Image>();
        switch (rarity)
        {
            case 0:
                rarityCurverImage.color = RarityColor.common;
                break;
            case 1:
                rarityCurverImage.color = RarityColor.rare;
                break;
            case 2:
                rarityCurverImage.color = RarityColor.epic;
                break;
            case 3:
                rarityCurverImage.color = RarityColor.legendary;
                break;
            default:
                rarityCurverImage.color = Color.black;
                break;
        }

        Instantiate(weaponIcons[weaponIndex], itemData.transform);
    }

    public void DataSave()
    {
        weaponData = new Dictionary<int, CustomWeaponData>();

        foreach (var item in slotData)//슬롯 데이터 순회
        {
            if(item.Value.slotData != null)//해당 슬롯에 데이터가 존재할 경우
            {
                weaponData.Add(item.Key, item.Value.slotData);//데이터 추가
            }
        }
        JsonDataManager.Instance.saveData.userData.customWeaponDatas = weaponData;
        JsonDataManager.Instance.DataSave();
    }
}


