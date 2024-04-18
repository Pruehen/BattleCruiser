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
    List<Vehicle> teamVehicles;//�Ʊ� �Լ� ����Ʈ

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

        StartCoroutine(Init());
        StartCoroutine(MoveOrderRepeat());
    }

    IEnumerator Init()
    {
        yield return new WaitForSeconds(1);
        maxWeaponVelocity = controllVehicle.maxWeaponVelocity;
        teamVehicles = BattleSceneManager.Instance.activeEnemyList;
    }
    IEnumerator MoveOrderRepeat()//0.5�ʸ��� �̵� ����Ʈ�� ������
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            movePoint = moveStrategy.Order(controllVehicle, targetVehicle);//�⺻ �̵� ��� ����

            if (teamVehicles != null)//�Ʊ� �Լ����� �Ÿ� ���� ���
            {
                Vehicle mostCloseVehicle = null;
                float closeVehicleDistance = float.MaxValue;
                for (int i = 0; i < teamVehicles.Count; i++)
                {
                    if (teamVehicles[i] != controllVehicle)//�˻��� �Լ��� �ڽ��� �ƴ� ��� 
                    {
                        float distance = (teamVehicles[i].transform.position - controllVehicle.transform.position).magnitude;//�ش� �Լ����� �Ÿ� ���

                        if (distance < closeVehicleDistance)//�Ÿ��� ���� �ּҰ����� ���� �� ����
                        {
                            closeVehicleDistance = distance;
                            mostCloseVehicle = teamVehicles[i];
                        }
                    }
                }

                if (mostCloseVehicle != null && closeVehicleDistance < 50)//�Ÿ��� 50 �̸��� ���, ����� �Ʊ����� �Ÿ� ���� ������� ��������Ʈ
                {
                    Vector2 myPos = controllVehicle.transform.position;
                    movePoint = myPos.GenerateDistanceKeepVector(mostCloseVehicle.transform.position, 80);
                    //Debug.Log("�Ÿ� ���� ���");
                }
            }

            SetAim();
        }        
    }

    // Update is called once per frame
    void Update()
    {                
        MovePID();
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

        float Weapondistance = (controllVehicle.maxEffectiveRange + controllVehicle.minEffectiveRange) * 0.6f;

        if(distance > Weapondistance)
        {
            controllUnit.fireTrigger = false;
        }
        else
        {
            controllUnit.fireTrigger = true;
        }
    }
}


interface IMoveStrategy
{
    Vector2 Order(Vehicle myVehicle, Vehicle target);
    void EnterStage();
}

class DistanceKeep : IMoveStrategy//�ڽ��� ��ȿ ��Ÿ� �߰����� �ش��ϴ� �Ÿ��� �δ� ���.
{
    public Vector2 Order(Vehicle myVehicle, Vehicle target)
    {
        Vector2 myPos = myVehicle.transform.position;
        float distance = (myVehicle.maxEffectiveRange + myVehicle.minEffectiveRange) * 0.5f;
        //Debug.Log(distance);
        //Debug.Log("�Ÿ� ���� ���");
        return myPos.GenerateDistanceKeepVector(target.transform.position, distance);
    }    
    public void EnterStage()
    {

    }
}

class HoverAvoidance: IMoveStrategy
{
    public Vector2 Order(Vehicle myVehicle, Vehicle target)
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
    public Vector2 Order(Vehicle myVehicle, Vehicle target)
    {
        Debug.Log("���� ���");
        return Vector2.zero;
    }
    public void EnterStage()
    {

    }
}


