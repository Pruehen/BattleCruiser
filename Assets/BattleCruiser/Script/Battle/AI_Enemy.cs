using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Enemy : MonoBehaviour
{
    [SerializeField] Vector2 movePoint;//�̵��� ��ġ (���� ����)
    [SerializeField] Vector2 aimPoint;//������ ��ġ (���� ����)
    Enemy controllUnit;//������ ����

    Vehicle controllVehicle;//�ڽ��� �Լ�(������)
    Vehicle targetVehicle;//Ÿ�� �Լ�(������)

    Vector2 currentPosition;//���� ��ġ
    Vector2 currentVelocity;//���� �ӵ�

    Vector2 targetPosition;//Ÿ�� ��ġ
    Vector2 targetVelocity;//Ÿ�� �ӵ�

    float maxWeaponVelocity;

    [Header("PID")]
    float p = 0.001f;//��� ���� ����
    float i = 1;//���� ���� ����
    float d = 0.001f;//�̺� ���� ����

    IMoveStrategy moveStrategy;//���� ����
    float randomGain1;//���ֺ��� �ٸ� ������
    float randomGain2;//���ֺ��� �ٸ� ������

    private void Awake()
    {
        controllUnit = GetComponent<Enemy>();
        controllVehicle = GetComponent<Vehicle>();
        targetVehicle = Player.Instance.GetComponent<Vehicle>();
    }

    // Start is called before the first frame update
    void Start()
    {
        moveStrategy = new DistanceKeep();
        randomGain1 = Random.Range(-100f, 100f);
        randomGain2 = Random.Range(-100f, 100f);
        maxWeaponVelocity = 300;
        StartCoroutine(maxWeaponVelocitySet());
    }

    IEnumerator maxWeaponVelocitySet()
    {
        yield return new WaitForSeconds(1);
        maxWeaponVelocity = controllVehicle.maxWeaponVelocity;
    }

    // Update is called once per frame
    void Update()
    {        
        SetData();
        movePoint = moveStrategy.Order(currentPosition, currentVelocity, targetPosition, targetVelocity, randomGain1, randomGain2);

        MovePID();
        SetAim();
    }
    void SetData()//��ɿ� ����� ������ ��.
    {
        currentPosition = this.transform.position;
        currentVelocity = controllVehicle.Rigidbody2D().velocity;

        targetPosition = targetVehicle.gameObject.transform.position;
        targetVelocity = targetVehicle.Rigidbody2D().velocity;
    }

    Vector2 moveTargetTemp;
    void MovePID()//���� ��ġ�� ��ǥ ��ġ���� ���̸� ���ӵ� ���� ������� ��ȯ�ϴ� �Լ�
    {
        Vector2 moveOrder = Vector2.zero;//�̵� ��� ����

        Vector2 moveTarget = movePoint - currentPosition;//��ǥ ��ġ���� ���� ����                
        moveTargetTemp = moveTarget;

        moveOrder = moveTarget * p - currentVelocity * d;
        //Debug.Log(moveOrder);

        moveOrder = moveOrder.normalized;
        controllUnit.SetInputMovement(moveOrder);
    }
    void SetAim()//��ǥ ��ġ�� ��������Ʈ�� �ű�� �Լ�. Ÿ�� �Լ��� �̵��� ź�� Ư���� ����ؾ� ��
    {
        float distance = (targetPosition - currentPosition).magnitude;//��� �Ÿ�
        float eta = distance / maxWeaponVelocity;//��� ź ���� �ӵ�

        Vector2 targetAimPosition = targetPosition + targetVelocity * eta;

        aimPoint = targetAimPosition;
        controllUnit.SetAimPoint(aimPoint);
    }
}


interface IMoveStrategy
{
    Vector2 Order(Vector2 myPosition, Vector2 myVelocity, Vector2 targetPositon, Vector2 targetVelocity, float randomGain1, float randomGain2);
}

class DistanceKeep : IMoveStrategy
{
    public Vector2 Order(Vector2 myPosition, Vector2 myVelocity, Vector2 targetPositon, Vector2 targetVelocity, float randomGain1, float randomGain2)
    {
        Vector2 movePositon = new Vector2(200 + randomGain1 + targetPositon.x, Mathf.Clamp(randomGain2 + targetPositon.y, 50, 10000));        

        //Debug.Log("�Ÿ� ���� ���");
        return movePositon;
    }
}

class Broadside : IMoveStrategy
{
    public Vector2 Order(Vector2 myPosition, Vector2 myVelocity, Vector2 targetPositon, Vector2 targetVelocity, float randomGain1, float randomGain2)
    {
        Debug.Log("��ε���̵� ���");
        return Vector2.zero;
    }
}

class Banzai : IMoveStrategy
{
    public Vector2 Order(Vector2 myPosition, Vector2 myVelocity, Vector2 targetPositon, Vector2 targetVelocity, float randomGain1, float randomGain2)
    {
        Debug.Log("���� ���");
        return Vector2.zero;
    }
}
