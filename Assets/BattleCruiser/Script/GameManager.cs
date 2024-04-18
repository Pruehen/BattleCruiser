using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : GlobalSingleton<GameManager>
{
    public int selectedStage { get; private set; }
    public PlayerShipData playerShipData { get; private set; }
    Setting setting;
    public Setting Setting
    {
        get
        {
            if (setting == null)
                setting = JsonDataManager.Instance.saveData.userData.setting;
            return setting;
        }
        set
        {
            setting = value;
        }
    }


    private void Awake()
    {
        Debug.Log($"{Instance.name} �ν��Ͻ� �Ϸ�");
        playerShipData = new PlayerShipData();        
    }
    public void SetStageNum(int num)
    {
        selectedStage = num;
        Debug.Log($"{selectedStage} ���������� ����");
    }
    public void SetShipData(int index)//�Լ� ���� â�� ���� �� 1ȸ ȣ���.
    {
        playerShipData.shipIndex = index;//�Լ� �ڵ�        
        playerShipData.shipData = JsonDataManager.Instance.saveData.shipDataDictionary[index.ShipKey()];//�Լ��� �⺻ ������        
        playerShipData.weaponSpriteIndexs.Clear();//���� ���� ���� �ڵ� ����
        playerShipData.weaponDatas.Clear();//���� ���� ������ ����


        int slotSize = playerShipData.shipData.weaponDatas.Count;//������ �ִ� ����
        int startWeaponIndex = (index + 1) * 1000;//�ش� �Լ��� ������ ���� ��ųʸ��� ���� Ű
        for (int i = startWeaponIndex; i < startWeaponIndex + slotSize; i++)
        {
            Slot slot = ItemManager.Instance.slotData[i];//���� ĳ��

            if (slot.slotWeaponData != null)//���Ⱑ ������ ���
            {
                playerShipData.weaponSpriteIndexs.Add(slot.slotWeaponData.weaponData.sptiteIndex);//���� ��������Ʈ Ű �߰�
                playerShipData.weaponDatas.Add(slot.slotWeaponData.weaponData);//���� ������ �߰�
            }
            else//���Ⱑ �������� ���� ���
            {
                playerShipData.weaponSpriteIndexs.Add(-1);//���� Ű -1 �߰� (���� ������ ��Ÿ��)
                playerShipData.weaponDatas.Add(null);//null�� �߰�
            }
        }

        //Debug.Log($"���� ��� ���� ���� ���� : {playerShipData.weaponIndexs.Count}, {playerShipData.weaponDatas.Count}");
    }

    public class PlayerShipData
    {
        public int shipIndex;//�Լ��� ���� �ڵ�
        public ShipData shipData;//�Լ� ������
        public List<int> weaponSpriteIndexs;//����� ������� ��������Ʈ �ڵ�
        public List<WeaponData> weaponDatas;//����� ���� ������

        public PlayerShipData()
        {
            weaponSpriteIndexs = new List<int>();
            weaponDatas = new List<WeaponData>();
        }
    }
}
