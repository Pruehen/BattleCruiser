using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour
{    
    Vector2 inputMovement = Vector2.zero;//이동 명령
    Vector2 aimPoint = Vector2.zero;//조준 명령
    Vehicle controlledShip;    
    public bool fireTrigger = true;

    Vehicle target;
    Vector3 targetVelocity;

    bool isInit = false;

    public void Init(ShipData shipData)
    {
        controlledShip = GetComponent<Vehicle>();//현재 함선 클래스        
        target = Player.Instance.gameObject.GetComponent<Vehicle>();        

        controlledShip.Init(true, shipData);//함선 데이터 초기화

        List<int> weaponSpriteIndexs = new List<int>();
        List<WeaponData> weaponDatas = new List<WeaponData>();
        foreach (string key in shipData.weaponDatas)
        {
            WeaponData weaponData = JsonDataManager.Instance.saveData.weaponDataDictionary[key];

            weaponDatas.Add(weaponData);
            weaponSpriteIndexs.Add(weaponData.sptiteIndex);
        }

        controlledShip.WeaponInit(weaponSpriteIndexs, weaponDatas);
        controlledShip.SetTarget(target);

        isInit = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInit)
        {
            controlledShip.SetControllVector(inputMovement);
            controlledShip.SetTrigger(fireTrigger);
            controlledShip.SetAimPosition(aimPoint);
        }
    }
    
    public void SetInputMovement(Vector2 vector2)
    {
        inputMovement = vector2;
    }
    public void SetAimPoint(Vector2 vector2)
    {
        aimPoint = vector2;
    }
}
