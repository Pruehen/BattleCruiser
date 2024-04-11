using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour
{    
    Vector2 inputMovement = Vector2.zero;//�̵� ���
    Vector2 aimPoint = Vector2.zero;//���� ���
    Vehicle controlledShip;
    EquipWeaponData weaponData;
    public bool fireTrigger = true;

    Transform target;
    Vector3 targetVelocity;

    private void Awake()
    {
        controlledShip = GetComponent<Vehicle>();//���� �Լ� Ŭ����
        weaponData = GetComponent<EquipWeaponData>();//�Լ��� ���� ���� ������ Ŭ���� (���ⵥ���� �ʱ�ȭ�� ���)
        target = CombatPlayer.Instance.gameObject.transform;
    }

    private void Start()
    {
        controlledShip.Init(true, 500000, 10000, 50, 50, 100, 3);//�Լ� ������ �ʱ�ȭ

        weaponData.weaponDatas.Add(new WeaponData(ProjectileType.Shell, 100, 3, 6, 40, 1, 1, 45, 0.1f, Vector2.zero));
        weaponData.weaponDatas.Add(new WeaponData(ProjectileType.Shell, 100, 3, 6, 40, 1, 1, 45, 0.1f, Vector2.zero));
        weaponData.weaponDatas.Add(new WeaponData(ProjectileType.Shell, 100, 3, 6, 40, 1, 1, 45, 0.1f, Vector2.zero));
        weaponData.weaponDatas.Add(new WeaponData(ProjectileType.Shell, 100, 3, 6, 40, 1, 1, 45, 0.1f, Vector2.zero));

        controlledShip.WeaponInit(weaponData.weaponDatas);
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
