using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour
{    
    Vector2 inputMovement = Vector2.zero;//이동 명령
    Vector2 aimPoint = Vector2.zero;//조준 명령
    Vehicle controlledShip;
    ShipData shipData;
    public bool fireTrigger = true;

    Transform target;
    Vector3 targetVelocity;

    private void Start()
    {
        controlledShip = GetComponent<Vehicle>();//현재 함선 클래스
        shipData = JsonDataManager.Instance.saveData.shipDataDictionary["Ship_001"];//함선이 사용할 함선 데이터
        target = Player.Instance.gameObject.transform;

        controlledShip.Init(true, shipData);//함선 데이터 초기화
        controlledShip.WeaponInit(shipData.weaponDatas);
    }

    // Update is called once per frame
    void Update()
    {        
        controlledShip.SetControllVector(inputMovement);
        controlledShip.SetTrigger(fireTrigger);
        controlledShip.SetAimPosition(aimPoint);
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
