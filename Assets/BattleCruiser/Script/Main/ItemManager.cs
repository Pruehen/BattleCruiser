using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemManager : SceneSingleton<ItemManager>
{
    public Transform slotParentTrf;//�κ��丮 ���� ��ü�� ��Ʈ Ʈ������
    public Transform customShipTrf;//Ŀ���� ���� ��Ʈ Ʈ������
    public Dictionary<int, Slot> slotData;//���� ������ ��ųʸ�
    public ToggleGroup sortTypeToggleGroup;
    //public Dictionary<int, CustomWeaponData> weaponData;

    //public GameObject rarityCurver;//��� Ŀ��
    //public GameObject newItemCurver;//���ο� ������ Ŀ��
    //public GameObject[] weaponIcons;//���� ������ ������

    public ItemDataViewer itemDataViewer;//���� �����͸� ǥ���ϴ� â    
    public bool isDataView = false;
    public Slot selectedSlot = null;


    void Start()
    {
        slotData = new Dictionary<int, Slot>();

        for (int i = 0; i < slotParentTrf.childCount; i++)//�κ��丮 ���� ��ȸ�ϸ� �ʱ�ȭ
        {
            Slot slot = slotParentTrf.GetChild(i).GetComponent<Slot>();
            slotData.Add(i, slot);//���� ����
            slot.Init(i);//���� �ʱ�ȭ
        }
        for (int i = 0; i < customShipTrf.childCount; i++)
        {
            customShipTrf.GetChild(i).GetComponent<CustomShip>().Init();//Ŀ���ҽ� ������ �ʱ�ȭ(���� �ʱ�ȭ ����)
        }

        //SlotsInit();//���Կ� �̺�Ʈ Ʈ���� �߰�        

        Dictionary<int, CustomWeaponData> weaponData = JsonDataManager.Instance.saveData.userData.customWeaponDatas;        
        foreach (var item in weaponData)//��ųʸ��� ��� Ű ��ȸ
        {
            if (slotData[item.Key].slotWeaponData == null)
            {
                slotData[item.Key].SetData(item.Value);

                if(item.Key >= 1000)//������ ����� ���
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

    public void AddItemImage(int weaponSpriteIndex, int rarity, Transform transform)//���Կ� �̹��� �߰�
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

    public void DataSave()//���Ե����͸� ���������ͷ� ��ȯ�ؼ� ����. �ܺ� ��ư Ŭ���ؼ� ȣ����.
    {
        Dictionary<int, CustomWeaponData> weaponData = new Dictionary<int, CustomWeaponData>();

        foreach (var item in slotData)//���� ������ ��ȸ
        {
            if(item.Value.slotWeaponData != null)//�ش� ���Կ� �����Ͱ� ������ ���
            {
                weaponData.Add(item.Key, item.Value.slotWeaponData);//������ �߰�
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
    //        int key = slotData[index].slotWeaponData.rarityNum;//���� ����� �Ǵ� Ű            
    //        int insertIndex = index - 1;//���� ��ġ �ε���

    //        // key���� ū ���Ҹ� ���������� �̵�
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
    //        int key = slotData[index].slotWeaponData.weaponData.weaponKey.Index();//���� ����� �Ǵ� Ű            
    //        int insertIndex = index - 1;//���� ��ġ �ε���

    //        // key���� ū ���Ҹ� ���������� �̵�
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
    //        float key = slotData[index].slotWeaponData.weaponData.caliber;//���� ����� �Ǵ� Ű            
    //        int insertIndex = index - 1;//���� ��ġ �ε���

    //        // key���� ū ���Ҹ� ���������� �̵�
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


    //        float key = slotData[index].slotWeaponData.weaponData.mass;//���� ����� �Ǵ� Ű            
    //        int insertIndex = index - 1;//���� ��ġ �ε���

    //        // key���� ū ���Ҹ� ���������� �̵�
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

            int insertIndex = index - 1; // ���� ��ġ �ε���

            //�� ĭ�� ��� ĭ�� ���.
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

            IComparable key = keySelector(slotData[index]); // ���� ����� �Ǵ� Ű            
            int insertIndex = index - 1; // ���� ��ġ �ε���

            // key���� ���� ���Ҹ� ���������� �̵�
            while (insertIndex >= 0 && (keySelector(slotData[insertIndex]).CompareTo(key) < 0 || slotData[insertIndex].slotWeaponData == null))
            {
                Slot.SwapData(slotData[insertIndex + 1], slotData[insertIndex]);
                insertIndex--;
            }
            //Slot.SwapData(slotData[insertIndex], slotData[index]);
        }

    }
}