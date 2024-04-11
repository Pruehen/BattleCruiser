using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour
{    
    Vector2 inputMovement = Vector2.zero;//�̵� ���
    Vector2 aimPoint = Vector2.zero;//���� ���
    Vehicle controlledShip;
    ShipData shipData;
    public bool fireTrigger = true;

    Transform target;
    Vector3 targetVelocity;

    private void Start()
    {
        controlledShip = GetComponent<Vehicle>();//���� �Լ� Ŭ����
        shipData = JsonDataManager.Instance.saveData.shipDataDictionary["Ship_001"];//�Լ��� ����� �Լ� ������
        target = Player.Instance.gameObject.transform;

        controlledShip.Init(true, shipData);//�Լ� ������ �ʱ�ȭ
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
