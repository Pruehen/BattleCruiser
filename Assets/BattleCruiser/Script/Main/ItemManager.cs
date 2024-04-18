using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemManager : SceneSingleton<ItemManager>
{
    public Transform slotParentTrf;//인벤토리 슬롯 전체의 루트 트랜스폼
    public Transform customShipTrf;//커스텀 쉽의 루트 트랜스폼
    public Dictionary<int, Slot> slotData;//슬롯 데이터 딕셔너리
    public ToggleGroup sortTypeToggleGroup;
    //public Dictionary<int, CustomWeaponData> weaponData;

    //public GameObject rarityCurver;//레어도 커버
    //public GameObject newItemCurver;//새로운 아이템 커버
    //public GameObject[] weaponIcons;//무기 아이콘 프리팹

    public ItemDataViewer itemDataViewer;//무기 데이터를 표시하는 창    
    public bool isDataView = false;
    public Slot selectedSlot = null;


    void Start()
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

        Dictionary<int, CustomWeaponData> weaponData = JsonDataManager.Instance.saveData.userData.customWeaponDatas;        
        foreach (var item in weaponData)//딕셔너리의 모든 키 조회
        {
            if (slotData[item.Key].slotWeaponData == null)
            {
                slotData[item.Key].SetData(item.Value);

                if(item.Key >= 1000)//장착된 장비일 경우
                {
                    CustomShipManager.Instance.UpdataEquipment(item.Key, item.Value);
                }
            }
        }
    }

    void Update()
    {
        if(isDataView)
        {
            itemDataViewer.transform.position = new Vector2(Mathf.Clamp(Input.mousePosition.x, 0, Screen.width - 700), Mathf.Clamp(Input.mousePosition.y, 700, Screen.height));
        }
        if(selectedSlot != null)
        {
            selectedSlot.gameObject.transform.position = Input.mousePosition;
        }
    }

    public void AddItemImage(int weaponSpriteIndex, int rarity, Transform transform)//슬롯에 이미지 추가
    {
        GameObject itemData = new GameObject();
        itemData.transform.SetParent(transform);
        itemData.transform.localPosition = Vector3.zero;
        itemData.name = "Data";

        Image rarityCurverImage = Instantiate(PrefabManager.Instance.rarityCurver, itemData.transform).GetComponent<Image>();
        switch (rarity)
        {
            case 0:
                rarityCurverImage.color = RarityColor.tech0;
                break;
            case 1:
                rarityCurverImage.color = RarityColor.tech1;
                break;
            case 2:
                rarityCurverImage.color = RarityColor.tech2;
                break;
            case 3:
                rarityCurverImage.color = RarityColor.tech3;
                break;
            case 4:
                rarityCurverImage.color = RarityColor.tech4;
                break;
            case 5:
                rarityCurverImage.color = RarityColor.tech5;
                break;
            case 6:
                rarityCurverImage.color = RarityColor.tech6;
                break;
            case 7:
                rarityCurverImage.color = RarityColor.tech7;
                break;
            default:
                rarityCurverImage.color = Color.black;
                break;
        }

        Instantiate(PrefabManager.Instance.weapons_Image[weaponSpriteIndex], itemData.transform);
    }
    public void TryRemoveItemImage(Transform transform)
    {
        if(transform.childCount != 0)
        {
            Destroy(transform.GetChild(0).gameObject);
        }        
    }

    public void DataSave()//슬롯데이터를 웨폰데이터로 변환해서 저장. 외부 버튼 클릭해서 호출함.
    {
        Dictionary<int, CustomWeaponData> weaponData = new Dictionary<int, CustomWeaponData>();

        foreach (var item in slotData)//슬롯 데이터 순회
        {
            if(item.Value.slotWeaponData != null)//해당 슬롯에 데이터가 존재할 경우
            {
                weaponData.Add(item.Key, item.Value.slotWeaponData);//데이터 추가
            }
        }
        JsonDataManager.Instance.saveData.userData.customWeaponDatas = weaponData;
        JsonDataManager.Instance.DataSave();
    }

    public void SortSlotData()
    {
        string useSortType = sortTypeToggleGroup.ActiveToggles().ToArray()[0].name;

        switch (useSortType)
        {
            case "Rarity":
                SlotDataSorter.InsertionSort(slotData, slot => slot.slotWeaponData.rarityNum);
                break;
            case "Type":
                SlotDataSorter.InsertionSort(slotData, slot => slot.slotWeaponData.weaponData.weaponKey.Index());
                break;
            case "Caliber":
                SlotDataSorter.InsertionSort(slotData, slot => slot.slotWeaponData.weaponData.caliber);
                break;
            case "Mass":
                SlotDataSorter.InsertionSort(slotData, slot => slot.slotWeaponData.weaponData.mass);
                break;
            default:
                break;
        }                                
    }
}


public class SlotDataSorter
{
    //public static void InsertionSort_Rarity(Dictionary<int, Slot> slotData)
    //{
    //    int count = slotData.Count;
    //    for (int index = 1; index < count; ++index)
    //    {
    //        int key = slotData[index].slotWeaponData.rarityNum;//비교할 대상이 되는 키            
    //        int insertIndex = index - 1;//삽입 위치 인덱스

    //        // key보다 큰 원소를 오른쪽으로 이동
    //        while (insertIndex >= 0 && slotData[insertIndex].slotWeaponData.rarityNum > key)
    //        {                
    //            insertIndex--;
    //        }
    //        if (insertIndex + 1 != index)
    //        {
    //            Slot.SwapData(slotData[insertIndex + 1], slotData[index]);
    //        }
    //    }
    //}
    //public static void InsertionSort_Type(Dictionary<int, Slot> slotData)
    //{
    //    int count = slotData.Count;
    //    for (int index = 1; index < count; ++index)
    //    {
    //        int key = slotData[index].slotWeaponData.weaponData.weaponKey.Index();//비교할 대상이 되는 키            
    //        int insertIndex = index - 1;//삽입 위치 인덱스

    //        // key보다 큰 원소를 오른쪽으로 이동
    //        while (insertIndex >= 0 && slotData[insertIndex].slotWeaponData.weaponData.weaponKey.Index() > key)
    //        {
    //            insertIndex--;
    //        }
    //        if (insertIndex + 1 != index)
    //        {
    //            Slot.SwapData(slotData[insertIndex + 1], slotData[index]);
    //        }
    //    }
    //}

    //public static void InsertionSort_Caliber(Dictionary<int, Slot> slotData)
    //{
    //    int count = slotData.Count;
    //    for (int index = 1; index < count; ++index)
    //    {
    //        float key = slotData[index].slotWeaponData.weaponData.caliber;//비교할 대상이 되는 키            
    //        int insertIndex = index - 1;//삽입 위치 인덱스

    //        // key보다 큰 원소를 오른쪽으로 이동
    //        while (insertIndex >= 0 && slotData[insertIndex].slotWeaponData.weaponData.caliber > key)
    //        {
    //            insertIndex--;
    //        }
    //        if (insertIndex + 1 != index)
    //        {
    //            Slot.SwapData(slotData[insertIndex + 1], slotData[index]);
    //        }
    //    }
    //}

    //public static void InsertionSort_Mass(Dictionary<int, Slot> slotData)
    //{
    //    int count = slotData.Count;
    //    for (int index = 1; index < count; ++index)
    //    {


    //        float key = slotData[index].slotWeaponData.weaponData.mass;//비교할 대상이 되는 키            
    //        int insertIndex = index - 1;//삽입 위치 인덱스

    //        // key보다 큰 원소를 오른쪽으로 이동
    //        while ((insertIndex >= 0 && slotData[insertIndex].slotWeaponData.weaponData.mass > key))
    //        {
    //            insertIndex--;
    //        }
    //        if (insertIndex + 1 != index)
    //        {
    //            Slot.SwapData(slotData[insertIndex + 1], slotData[index]);
    //        }
    //    }
    //}

    public static void InsertionSort(Dictionary<int, Slot> slotData, Func<Slot, IComparable> keySelector)
    {
        int count = 128;

        for (int index = 1; index < count; index++)
        {
            if (slotData[index].slotWeaponData == null)
                continue;

            int insertIndex = index - 1; // 삽입 위치 인덱스

            //빈 칸일 경우 칸을 당김.
            while (insertIndex >= 0 && slotData[insertIndex].slotWeaponData == null)
            {
                insertIndex--;
            }
            if (insertIndex + 1 != index)
            {
                Slot.SwapData(slotData[insertIndex + 1], slotData[index]);
            }
        }

        for (int index = 1; index < count; index++)
        {
            if (slotData[index].slotWeaponData == null)
                continue;

            IComparable key = keySelector(slotData[index]); // 비교할 대상이 되는 키            
            int insertIndex = index - 1; // 삽입 위치 인덱스

            // key보다 작은 원소를 오른쪽으로 이동
            while (insertIndex >= 0 && (keySelector(slotData[insertIndex]).CompareTo(key) < 0 || slotData[insertIndex].slotWeaponData == null))
            {
                Slot.SwapData(slotData[insertIndex + 1], slotData[insertIndex]);
                insertIndex--;
            }
            //Slot.SwapData(slotData[insertIndex], slotData[index]);
        }

    }
}