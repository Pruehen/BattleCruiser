using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class Player : Singleton<Player>
{
    Rigidbody2D rigidbody2D;
    float mass = 0;
    bool isDead = false;

    Vector2 controllVector = Vector2.zero;//��Ʈ�� �̵� ����
    public float hoverPower = 5;//���� �̵� �߷�
    public float strafePower = 10;//�¿� �̵� �߷�

    public Vector2 screenAimPoint = Vector2.zero;//���� ��ġ(��ũ�� ����)
    public Vector2 aimPosition = Vector2.zero;//���� ��ġ(���� ��ǥ)
    public Vector2 aimDirection = Vector2.zero;//���� ����(��� ��ǥ)

    public bool fireTrigger = false;

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

        aimPosition = Camera.main.ScreenToWorldPoint(screenAimPoint);//���� ���ͷ� ��ȯ
        aimDirection = aimPosition - (Vector2)transform.position;//�÷��̾� ���� ���� ���ͷ� ��ȯ
    }

    private void FixedUpdate()
    {
        AltitudeHoldPropulsion();
        ControllPropulsion();

        SetTurretParentVelocity(rigidbody2D.velocity);
    }
    void ControllPropulsion()//�����¿� ����
    {
        rigidbody2D.AddForce(controllVector * mass, ForceMode2D.Force);
    }
    void AltitudeHoldPropulsion()//������ ����
    {
        rigidbody2D.AddForce(Vector2.up * 9.8f * mass, ForceMode2D.Force);
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
    void SetTurretParentVelocity(Vector2 velocity)
    {
        foreach (Weapon turret in childWeaponList)
        {
            turret.SetParentVelocity(velocity);
        }
    }


    void OnMove(InputValue inputValue)//WASD ����
    {
        Vector2 inputMovement = inputValue.Get<Vector2>();//��ǲ ���� �޾ƿ�
        controllVector = new Vector2(inputMovement.x * strafePower, inputMovement.y * hoverPower);//���� ���ͷ� ��ȯ
    }
    void OnAim(InputValue inputValue)//���콺 ��ġ
    {
        screenAimPoint = inputValue.Get<Vector2>();//���콺 ��ġ �޾ƿ�
        PlayerUI.Instance.SetAimPointPosition(screenAimPoint);//��������Ʈ ��ġ����
    }
    void OnLeftClick(InputValue inputValue)//���콺 ��Ŭ��
    {
        float isClick = inputValue.Get<float>();

        if(isClick == 1)//������ ��
        {
            fireTrigger = true;
        }
        else//�� ��
        {
            fireTrigger = false;
        }
        SetTurretTrigger(fireTrigger);
    }
    void OnRightClick(InputValue inputValue)//���콺 ��Ŭ��
    {
        Debug.Log(inputValue);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

    }
}
