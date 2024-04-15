using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemManager : SceneSingleton<ItemManager>
{
    public Transform slotParentTrf;//�κ��丮 ���� ��ü�� ��Ʈ Ʈ������
    public Transform customShipTrf;//Ŀ���� ���� ��Ʈ Ʈ������
    public Dictionary<int, Slot> slotData;//���� ������ ��ųʸ�
    public Dictionary<int, CustomWeaponData> weaponData;

    public GameObject rarityCurver;//��� Ŀ��
    public GameObject newItemCurver;//���ο� ������ Ŀ��
    public GameObject[] weaponIcons;//���� ������ ������

    public ItemDataViewer itemDataViewer;//���� �����͸� ǥ���ϴ� â    
    public bool isDataView = false;
    public Slot selectedSlot = null;


    void Awake()
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

        weaponData = JsonDataManager.Instance.saveData.userData.customWeaponDatas;        
        foreach (var item in weaponData)//��ųʸ��� ��� Ű ��ȸ
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

    public void AddItem(int weaponIndex, int rarity, Transform transform)//���Կ� �̹��� �߰�
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

        foreach (var item in slotData)//���� ������ ��ȸ
        {
            if(item.Value.slotData != null)//�ش� ���Կ� �����Ͱ� ������ ���
            {
                weaponData.Add(item.Key, item.Value.slotData);//������ �߰�
            }
        }
        JsonDataManager.Instance.saveData.userData.customWeaponDatas = weaponData;
        JsonDataManager.Instance.DataSave();
    }
}


