using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    Rigidbody2D rigidbody2D;
    public Rigidbody2D Rigidbody2D() { return rigidbody2D; }
    float mass = 0;
    bool isDead = false;

    Vector2 controllVector = Vector2.zero;//��Ʈ�� �̵� ����
    float hoverPower = 5;//���� �̵� �߷�
    float strafePower = 10;//�¿� �̵� �߷�
    float horizontalRestorationPower = 2;//������

    //public Vector2 screenAimPoint = Vector2.zero;//���� ��ġ(��ũ�� ����)
    Vector2 aimPosition = Vector2.zero;//���� ��ġ(���� ��ǥ)
    Vector2 aimDirection = Vector2.zero;//���� ����(��� ��ǥ)

    bool fireTrigger = false;

    public Transform weaponsTrf;
    List<Weapon> childWeaponList;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        mass = rigidbody2D.mass;

        childWeaponList = new List<Weapon>();
        for (int i = 0; i < weaponsTrf.childCount; i++)
        {
            childWeaponList.Add(weaponsTrf.GetChild(i).gameObject.GetComponent<Weapon>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetTurretTargetPos(aimPosition);
        SetTurretParentVelocity(rigidbody2D.velocity);        

        aimDirection = aimPosition - (Vector2)transform.position;//��� �߽� ���� ���� ���ͷ� ��ȯ
    }

    private void FixedUpdate()
    {
        AltitudeHoldPropulsion();//������ ����
        ControllPropulsion();//�����¿� ����
        HorizontalRestoration();//���򺹿� ȸ��
    }
    void ControllPropulsion()//�����¿� ����
    {
        rigidbody2D.AddForce(controllVector * mass, ForceMode2D.Force);
    }
    void AltitudeHoldPropulsion()//������ ����
    {
        rigidbody2D.AddForce(this.transform.up * 9.8f * mass, ForceMode2D.Force);
    }
    void HorizontalRestoration()//���� ����
    {
        rigidbody2D.AddTorque(this.transform.up.x * horizontalRestorationPower * mass * 360, ForceMode2D.Force);
    }
    void SetTurretTargetPos(Vector2 aimPoint)//�ͷ� ��ǥ ������ ȸ��
    {
        foreach (Weapon turret in childWeaponList)
        {
            turret.SetTargetPoint(aimPoint);
        }
    }
    void SetTurretTrigger(bool value)//�ͷ� Ʈ���� ����
    {
        foreach (Weapon turret in childWeaponList)
        {
            turret.SetTrigger(value);
        }
    }
    void SetTurretParentVelocity(Vector2 velocity)//�ͷ��� ������ �ӵ� ����
    {
        foreach (Weapon turret in childWeaponList)
        {
            turret.SetParentVelocity(velocity);
        }
    }


    public void SetControllVector(Vector2 controllVector)//���� ��ǲ
    {
        this.controllVector = new Vector2(controllVector.x * strafePower, controllVector.y * hoverPower);
    }
    public void SetAimPosition(Vector2 aimPosition)//���� ��ǲ
    {
        this.aimPosition = aimPosition;
    }
    public void SetTrigger(bool value)//�߻� Ʈ���� ��ǲ
    {
        fireTrigger = value;
        SetTurretTrigger(fireTrigger);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

    }
}
