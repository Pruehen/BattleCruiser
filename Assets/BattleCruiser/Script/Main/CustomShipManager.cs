using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomShipManager : SceneSingleton<CustomShipManager>
{
    public GameObject[] weaponPrfs;//무장의 프리팹

    public Transform customShipParentTrf;//커스텀쉽 전체의 루트 트랜스폼
    public List<CustomShip> customShips;//커스텀쉽 클래스(UI 부분) 리스트
    public List<GameObject> customShipGameObjects;//커스텀쉽 게임오브젝트(스프라이트) 리스트
    public List<Transform> turretPoints;//커스텀쉽에 달린 터렛 포인트의 리스트

    int selectedCustomShipIndex;//사용할 함선 인덱스.

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
    /// 선택할 함선을 index의 함선으로 선택.
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
    /// 터렛 등의 장비를 갱신. int 키와 커스텀웨폰데이터를 매개변수로 받음.
    /// </summary>
    public void UpdataEquipment(int keyIndex, CustomWeaponData equipData)
    {
        if (keyIndex < 1000)//장비된 무장의 인덱스가 아닐 경우 리턴.
            return;

        int positionIndex = (int)(keyIndex % 1000);//무장의 위치 인덱스. 0부터 터렛의 개수 -1까지의 값.
        int shipIndex = (keyIndex / 1000) - 1;//무장을 장착할 함선 인덱스. 0부터 시작.
        int weaponIndex;//무장의 고유 인덱스. 터렛의 형태를 결정.

        if (equipData != null)
        {
            weaponIndex = equipData.baseWeaponKey.Index();//무장의 고유 인덱스
        }
        else
        {
            weaponIndex = -1;
        }


        Transform targetTrf = turretPoints[shipIndex].GetChild(positionIndex);//무장을 장착할 부모 트랜스폼

        if (weaponIndex == -1)//무장이 없는 경우, 무장 제거.
        {
            if(targetTrf.childCount > 0)//무장이 달려있는 경우 무장 제거
            {
                Destroy(targetTrf.GetChild(0).gameObject);
            }
        }
        else//무장이 있는 경우, 무장 장비.
        {
            if (targetTrf.childCount > 0)//무장이 달려있는 경우 기존 무장 제거
            {
                Destroy(targetTrf.GetChild(0).gameObject);
            }

            Instantiate(weaponPrfs[weaponIndex], targetTrf);
        }
    }
}
