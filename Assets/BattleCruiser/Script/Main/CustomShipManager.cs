using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomShipManager : SceneSingleton<CustomShipManager>
{
    //public GameObject[] weaponPrfs;//������ ������

    public Transform customShipParentTrf;//Ŀ���ҽ� Ŭ������ ��Ʈ Ʈ������
    public Transform customShipGameObjectParentTrf;//Ŀ���ҽ� ���ӿ�����Ʈ�� ��Ʈ Ʈ������

    public List<CustomShip> customShips;//Ŀ���ҽ� Ŭ����(UI �κ�) ����Ʈ
    public List<GameObject> customShipGameObjects;//Ŀ���ҽ� ���ӿ�����Ʈ(��������Ʈ) ����Ʈ

    public List<Transform> turretPoints;//Ŀ���ҽ��� �޸� �ͷ� ����Ʈ�� ����Ʈ
    int selectedShipIndex;

    private void Awake()
    {
        customShips = new List<CustomShip>();
        customShipGameObjects = new List<GameObject>();
        turretPoints = new List<Transform>();

        for (int i = 0; i < customShipParentTrf.childCount; i++)
        {
            customShips.Add(customShipParentTrf.GetChild(i).GetComponent<CustomShip>());//UI�κ� Ŀ���ҽ�            
            customShipGameObjects.Add(customShipGameObjectParentTrf.GetChild(i).gameObject);//���ӿ�����Ʈ �κ� Ŀ���ҽ�            
            turretPoints.Add(customShipGameObjects[i].transform.GetChild(customShipGameObjects[i].transform.childCount - 1));
        }

        SelectCustomShip(0);
    }

    /// <summary>
    /// ������ �Լ��� index�� �Լ����� ����.
    /// </summary>
    /// <param name="index"></param>
    public void SelectCustomShip(int index)
    {
        selectedShipIndex = index;
        for (int i = 0; i < customShips.Count; i++)
        {
            if(index == i)
            {
                customShips[i].gameObject.SetActive(true);
                customShipGameObjects[i].SetActive(true);
            }
            else
            {
                customShips[i].gameObject.SetActive(false);
                customShipGameObjects[i].SetActive(false);
            }
        }

        //GameManager.Instance.SetShipData(selectedShipIndex);
    }

    /// <summary>
    /// �ͷ� ���� ��� ����. int Ű�� Ŀ���ҿ��������͸� �Ű������� ����.
    /// </summary>
    public void UpdataEquipment(int keyIndex, CustomWeaponData equipData)
    {
        if (keyIndex < 1000)//���� ������ �ε����� �ƴ� ��� ����.
            return;

        int positionIndex = (int)(keyIndex % 1000);//������ ��ġ �ε���. 0���� �ͷ��� ���� -1������ ��.
        int shipIndex = (keyIndex / 1000) - 1;//������ ������ �Լ� �ε���. 0���� ����.
        int weaponIndex;//������ ��������Ʈ �ε���. �ͷ��� ���¸� ����.

        if (equipData != null)
        {
            weaponIndex = equipData.weaponData.sptiteIndex;//������ ��������Ʈ �ε���
        }
        else
        {
            weaponIndex = -1;
        }


        Transform targetTrf = turretPoints[shipIndex].GetChild(positionIndex);//������ ������ �θ� Ʈ������

        if (weaponIndex == -1)//������ ���� ���, ���� ����.
        {
            if(targetTrf.childCount > 0)//������ �޷��ִ� ��� ���� ����
            {
                Destroy(targetTrf.GetChild(0).gameObject);
            }
        }
        else//������ �ִ� ���, ���� ���.
        {
            if (targetTrf.childCount > 0)//������ �޷��ִ� ��� ���� ���� ����
            {
                Destroy(targetTrf.GetChild(0).gameObject);
            }

            Instantiate(PrefabManager.Instance.weapons[weaponIndex], targetTrf);
        }
    }

    public void SetShipData()//���� ���¸� ���ӸŴ����� ����. �ܺ� ��ư ������ ȣ��.
    {
        GameManager.Instance.SetShipData(selectedShipIndex);
    }
}
