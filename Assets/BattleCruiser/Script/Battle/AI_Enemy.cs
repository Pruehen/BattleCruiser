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
    Vehicle mustCloseTeamVehicle;//�ڽŰ� ���� ����� �Ʊ� �Լ�

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
    }

    // Start is called before the first frame update
    void Start()
    {
        targetVehicle = Player.Instance.GetComponent<Vehicle>();
        moveStrategy = new DistanceKeep();

        randomGain1 = Random.Range(-100f, 100f);
        randomGain2 = Random.Range(-100f, 100f);

        maxWeaponVelocity = 300;

        StartCoroutine(MaxWeaponVelocitySet());
        StartCoroutine(MoveOrderRepeat());
    }

    IEnumerator MaxWeaponVelocitySet()
    {
        yield return new WaitForSeconds(1);
        maxWeaponVelocity = controllVehicle.maxWeaponVelocity;        
    }
    IEnumerator MoveOrderRepeat()//1�ʸ��� �̵� ����Ʈ�� ������
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            movePoint = moveStrategy.Order(controllVehicle, targetVehicle, randomGain1, randomGain2);//�⺻ �̵� ��� ����
        }        
    }

    // Update is called once per frame
    void Update()
    {                
        MovePID();
        SetAim();
    }

    Vector2 moveTargetTemp;
    void MovePID()//���� ��ġ�� ��ǥ ��ġ���� ���̸� ���ӵ� ���� ������� ��ȯ�ϴ� �Լ�
    {
        Vector2 moveOrder = Vector2.zero;//�̵� ��� ����

        Vector2 moveTarget = movePoint - (Vector2)controllVehicle.transform.position;//��ǥ ��ġ���� ���� ����                
        moveTargetTemp = moveTarget;

        moveOrder = moveTarget * p - controllVehicle.Rigidbody2D().velocity * d;        

        moveOrder = moveOrder.normalized;
        controllUnit.SetInputMovement(moveOrder);
    }
    void SetAim()//��ǥ ��ġ�� ��������Ʈ�� �ű�� �Լ�. Ÿ�� �Լ��� �̵��� ź�� Ư���� ����ؾ� ��
    {
        float distance = ((Vector2)targetVehicle.transform.position - (Vector2)controllVehicle.transform.position).magnitude;//��� �Ÿ�
        float eta = distance / maxWeaponVelocity;//��� ź ���� �ӵ�

        Vector2 targetAimPosition = (Vector2)targetVehicle.transform.position + 
            (targetVehicle.Rigidbody2D().velocity - controllVehicle.Rigidbody2D().velocity) * eta;

        aimPoint = targetAimPosition;
        controllUnit.SetAimPoint(aimPoint);
    }
}


interface IMoveStrategy
{
    Vector2 Order(Vehicle myVehicle, Vehicle target, float randomGain1, float randomGain2);
    void EnterStage();
}

class DistanceKeep : IMoveStrategy//�ڽ��� ��ȿ ��Ÿ� �߰����� �ش��ϴ� �Ÿ��� �δ� ���.
{
    public Vector2 Order(Vehicle myVehicle, Vehicle target, float randomGain1, float randomGain2)
    {
        Vector2 toTargetDir = (target.transform.position - myVehicle.transform.position).normalized;
        float distance = (myVehicle.maxEffectiveRange + myVehicle.minEffectiveRange) * 0.5f;
        Vector2 movePosition = (Vector2)target.transform.position - (toTargetDir * distance);

        while (movePosition.y <= 50)
        {
            // ȸ���� ���� ���� (����� ��� �ݽð� ����, ������ ��� �ð� ����)
            float angle = (toTargetDir.x > 0) ? -15f : 15f;

            // ���ʹϾ����� ȸ��
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            toTargetDir = rotation * toTargetDir;

            // ���ο� ��ǥ ��ġ �ٽ� ���
            movePosition = (Vector2)target.transform.position - (toTargetDir * distance);
        }

        //Debug.Log("�Ÿ� ���� ���");
        return movePosition;
    }    
    public void EnterStage()
    {

    }
}

class HoverAvoidance: IMoveStrategy
{
    public Vector2 Order(Vehicle myVehicle, Vehicle target, float randomGain1, float randomGain2)
    {
        //Vector2 movePositon = new Vector2(200 + randomGain1 + targetPositon.x, Mathf.Clamp(randomGain2 + targetPositon.y, 50, 10000));

        //Debug.Log("���� ȸ�� ���");
        return Vector2.zero;
    }
    public void EnterStage()
    {

    }
}

class Banzai : IMoveStrategy
{
    public Vector2 Order(Vehicle myVehicle, Vehicle target, float randomGain1, float randomGain2)
    {
        Debug.Log("���� ���");
        return Vector2.zero;
    }
    public void EnterStage()
    {

    }
}


