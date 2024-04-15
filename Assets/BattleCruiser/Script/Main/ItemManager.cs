using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemManager : SceneSingleton<ItemManager>
{
    private void Awake()
    {
        Debug.Log($"{Instance.name} 인스턴싱 완료");
    }

    public Transform slotParentTrf;//슬롯 전체의 루트 트랜스폼
    List<Slot> slotList;//슬롯 데이터 리스트    

    public GameObject rarityCurver;//레어도 커버
    public GameObject newItemCurver;//새로운 아이템 커버
    public GameObject[] weaponIcons;//무기 아이콘 프리팹

    public ItemDataViewer itemDataViewer;//무기 데이터를 표시하는 창    
    public bool isDataView = false;

    void Start()
    {        
        slotList = new List<Slot>();        

        for (int i = 0; i < slotParentTrf.childCount; i++)
        {
            Slot slot = slotParentTrf.GetChild(i).GetComponent<Slot>();
            slotList.Add(slot);
            slot.Init();
        }
        //SlotsInit();//슬롯에 이벤트 트리거 추가

        List<CustomWeaponData> customWeaponDatas = JsonDataManager.Instance.saveData.userData.customWeaponDatas;

        for (int i = 0; i < customWeaponDatas.Count; i++)
        {
            slotList[i].AddData(customWeaponDatas[i]);
        }
    }

    void Update()
    {
        if(isDataView)
        {
            itemDataViewer.transform.position = Input.mousePosition;
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


}


