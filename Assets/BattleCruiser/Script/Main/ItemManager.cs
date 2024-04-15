using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemManager : SceneSingleton<ItemManager>
{
    private void Awake()
    {
        Debug.Log($"{Instance.name} �ν��Ͻ� �Ϸ�");
    }

    public Transform slotParentTrf;//���� ��ü�� ��Ʈ Ʈ������
    List<Slot> slotList;//���� ������ ����Ʈ    

    public GameObject rarityCurver;//��� Ŀ��
    public GameObject newItemCurver;//���ο� ������ Ŀ��
    public GameObject[] weaponIcons;//���� ������ ������

    public ItemDataViewer itemDataViewer;//���� �����͸� ǥ���ϴ� â    
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
        //SlotsInit();//���Կ� �̺�Ʈ Ʈ���� �߰�

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


}


