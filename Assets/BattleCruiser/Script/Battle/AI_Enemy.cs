using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Enemy : MonoBehaviour
{
    [SerializeField] Vector2 movePoint;//이동할 위치 (월드 벡터)
    [SerializeField] Vector2 aimPoint;//조준할 위치 (월드 벡터)
    Enemy controllUnit;//조종할 유닛

    Vehicle controllVehicle;//자신의 함선(참조용)
    Vehicle targetVehicle;//타겟 함선(참조용)

    Vector2 currentPosition;//현재 위치
    Vector2 currentVelocity;//현재 속도

    Vector2 targetPosition;//타겟 위치
    Vector2 targetVelocity;//타겟 속도

    float maxWeaponVelocity;

    [Header("PID")]
    float p = 0.001f;//비례 제어 변수
    float i = 1;//적분 제어 변수
    float d = 0.001f;//미분 제어 변수

    IMoveStrategy moveStrategy;//조종 전략
    float randomGain1;//유닛별로 다른 랜덤값
    float randomGain2;//유닛별로 다른 랜덤값

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
    void SetData()//명령에 사용할 데이터 셋.
    {
        currentPosition = this.transform.position;
        currentVelocity = controllVehicle.Rigidbody2D().velocity;

        targetPosition = targetVehicle.gameObject.transform.position;
        targetVelocity = targetVehicle.Rigidbody2D().velocity;
    }

    Vector2 moveTargetTemp;
    void MovePID()//현재 위치와 목표 위치간의 차이를 가속도 생성 명령으로 변환하는 함수
    {
        Vector2 moveOrder = Vector2.zero;//이동 명령 벡터

        Vector2 moveTarget = movePoint - currentPosition;//목표 위치로의 로컬 벡터                
        moveTargetTemp = moveTarget;

        moveOrder = moveTarget * p - currentVelocity * d;
        //Debug.Log(moveOrder);

        moveOrder = moveOrder.normalized;
        controllUnit.SetInputMovement(moveOrder);
    }
    void SetAim()//목표 위치에 에임포인트를 옮기는 함수. 타겟 함선의 이동과 탄도 특성을 고려해야 함
    {
        float distance = (targetPosition - currentPosition).magnitude;//약식 거리
        float eta = distance / maxWeaponVelocity;//약식 탄 도달 속도

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

        //Debug.Log("거리 유지 명령");
        return movePositon;
    }
}

class Broadside : IMoveStrategy
{
    public Vector2 Order(Vector2 myPosition, Vector2 myVelocity, Vector2 targetPositon, Vector2 targetVelocity, float randomGain1, float randomGain2)
    {
        Debug.Log("브로드사이드 명령");
        return Vector2.zero;
    }
}

class Banzai : IMoveStrategy
{
    public Vector2 Order(Vector2 myPosition, Vector2 myVelocity, Vector2 targetPositon, Vector2 targetVelocity, float randomGain1, float randomGain2)
    {
        Debug.Log("돌진 명령");
        return Vector2.zero;
    }
}
