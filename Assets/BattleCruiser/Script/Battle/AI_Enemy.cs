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
    List<Vehicle> teamVehicles;//아군 함선 리스트

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
    IEnumerator MoveOrderRepeat()//0.5초마다 이동 포인트를 갱신함
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            movePoint = moveStrategy.Order(controllVehicle, targetVehicle);//기본 이동 명령 생성

            if (teamVehicles != null)//아군 함선과의 거리 유지 명령
            {
                Vehicle mostCloseVehicle = null;
                float closeVehicleDistance = float.MaxValue;
                for (int i = 0; i < teamVehicles.Count; i++)
                {
                    if (teamVehicles[i] != controllVehicle)//검색한 함선이 자신이 아닌 경우 
                    {
                        float distance = (teamVehicles[i].transform.position - controllVehicle.transform.position).magnitude;//해당 함선과의 거리 계산

                        if (distance < closeVehicleDistance)//거리가 기존 최소값보다 작을 시 갱신
                        {
                            closeVehicleDistance = distance;
                            mostCloseVehicle = teamVehicles[i];
                        }
                    }
                }

                if (mostCloseVehicle != null && closeVehicleDistance < 50)//거리가 50 미만일 경우, 명령을 아군과의 거리 유지 명령으로 오버라이트
                {
                    Vector2 myPos = controllVehicle.transform.position;
                    movePoint = myPos.GenerateDistanceKeepVector(mostCloseVehicle.transform.position, 80);
                    //Debug.Log("거리 유지 명령");
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
    void MovePID()//현재 위치와 목표 위치간의 차이를 가속도 생성 명령으로 변환하는 함수
    {
        Vector2 moveOrder = Vector2.zero;//이동 명령 벡터

        Vector2 moveTarget = movePoint - (Vector2)controllVehicle.transform.position;//목표 위치로의 로컬 벡터                
        moveTargetTemp = moveTarget;

        moveOrder = moveTarget * p - controllVehicle.Rigidbody2D().velocity * d;        

        moveOrder = moveOrder.normalized;
        controllUnit.SetInputMovement(moveOrder);
    }
    void SetAim()//목표 위치에 에임포인트를 옮기는 함수. 타겟 함선의 이동과 탄도 특성을 고려해야 함
    {
        float distance = ((Vector2)targetVehicle.transform.position - (Vector2)controllVehicle.transform.position).magnitude;//약식 거리
        float eta = distance / maxWeaponVelocity;//약식 탄 도달 속도

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

class DistanceKeep : IMoveStrategy//자신의 유효 사거리 중간값에 해당하는 거리를 두는 명령.
{
    public Vector2 Order(Vehicle myVehicle, Vehicle target)
    {
        Vector2 myPos = myVehicle.transform.position;
        float distance = (myVehicle.maxEffectiveRange + myVehicle.minEffectiveRange) * 0.5f;
        //Debug.Log(distance);
        //Debug.Log("거리 유지 명령");
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

        //Debug.Log("상하 회피 명령");
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
        Debug.Log("돌진 명령");
        return Vector2.zero;
    }
    public void EnterStage()
    {

    }
}


