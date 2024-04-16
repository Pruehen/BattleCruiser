using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomShipManager : SceneSingleton<CustomShipManager>
{
    public GameObject[] weaponPrfs;//������ ������

    public Transform customShipParentTrf;//Ŀ���ҽ� ��ü�� ��Ʈ Ʈ������
    public List<CustomShip> customShips;//Ŀ���ҽ� Ŭ����(UI �κ�) ����Ʈ
    public List<GameObject> customShipGameObjects;//Ŀ���ҽ� ���ӿ�����Ʈ(��������Ʈ) ����Ʈ
    public List<Transform> turretPoints;//Ŀ���ҽ��� �޸� �ͷ� ����Ʈ�� ����Ʈ

    int selectedCustomShipIndex;//����� �Լ� �ε���.

    private void Awake()
    {
        customShips = new List<CustomShip>();
        customShipGameObjects = new List<GameObject>();
        turretPoints = new List<Transform>();

        for (int i = 0; i < customShipParentTrf.childCount; i++)
        {
            customShips.Add(customShipParentTrf.GetChild(i).GetComponent<CustomShip>());
            customShipGameObjects.Add(this.transform.GetChild(i).gameObject);
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
        selectedCustomShipIndex = index;

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
        int weaponIndex;//������ ���� �ε���. �ͷ��� ���¸� ����.

        if (equipData != null)
        {
            weaponIndex = equipData.baseWeaponKey.Index();//������ ���� �ε���
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

            Instantiate(weaponPrfs[weaponIndex], targetTrf);
        }
    }
}
