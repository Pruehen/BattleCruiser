using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Vehicle parentVehicle;

    public string weaponKey { get; private set; }

    public Transform firePoint;
    float projectiledVelocity = 100;//발사 속도
    float dispersion = 3;//발사각 분포(각도)
    float shellLifeTime = 6;//탄 작동 시간
    Vector2 parentVelocity = Vector2.zero;
    float caliber = 100f;//탄 구경
    float apDmgFactor = 1;//물리 데미지 계수
    float heDmgFactor = 1;//화학 데미지 계수
    public float effectiveRange { get; private set; }//유효 사거리

    float turningSpeedPerSecond = 90;//초당 터렛 회전 속도
    float coolDown = 0.1f;//발사 쿨타임
    float delay = 0;
    int multiShot;//한번에 발사하는 수량
    float multiShotDelay;//1살보 발당 딜레이
    bool projectileIsPropulsion = false;//탄이 추진력을 가지고 있는지
    bool projectileIsGuided = false;//탄이 유도력을 가지고 있는지


    bool coolDownComplete = false;//쿨타임 완료
    bool fireAngleComplete = false;//발사각 완료
    bool readyToFire = false;//발사 준비 완료
    bool trigger = false;//트리거

    Vector2 targetPosition;//조준 좌표(월드)
    Vector2 toTargetVector2;//조준 좌표(로컬)

    bool isEnemy = false;
    bool isInit = false;

    public void SetTargetPoint(Vector2 targetPos)//조준 좌표 세팅 및 거리, 중력, 저항을 고려한 조준 보정
    {
        float distance = (targetPos - (Vector2)this.transform.position).magnitude;//목표와의 거리
        float eta = distance / projectiledVelocity;//목표까지의 도달 예상 시간
        Vector2 calcTargetPos = new Vector2(targetPos.x, targetPos.y + (eta * 4.9f * eta));//초기 좌표 세팅

        float drag = 1/caliber;//공기 저항
        float finalVelocity = projectiledVelocity * Mathf.Exp(-drag * eta);//공기 저항에 따른 최종 탄착 예상 속도
        eta = (calcTargetPos - (Vector2)this.transform.position).magnitude / ((projectiledVelocity + finalVelocity) * 0.5f);//포물선 궤적 및 공기 저항에 따른 도달 예상 시간 재계산
        finalVelocity = projectiledVelocity * Mathf.Exp(-drag * eta);//공기 저항에 따른 최종 탄착 예상 속도를 변화한 eta값에 맞춰 재계산
        eta = (calcTargetPos - (Vector2)this.transform.position).magnitude / ((projectiledVelocity + finalVelocity) * 0.5f);//포물선 궤적 및 공기 저항에 따른 도달 예상 시간 재계산
        calcTargetPos = new Vector2(targetPos.x, targetPos.y + Mathf.Clamp(Mathf.Pow(eta, 2 + (eta * 0.002f)) * 4.9f, 0, distance));//최종 세팅

        targetPosition = calcTargetPos;
    }
    public void SetTrigger(bool value)
    {
        trigger = value;
    }
    public void SetParentVelocity(Vector2 velocity)
    {
        parentVelocity = velocity;
    }
    public void Init(bool isEnemy, WeaponData weaponData, Vector2 localPosition, Vehicle vehicle)
    {        
        this.isEnemy = isEnemy;

        this.weaponKey = weaponData.weaponKey;
        this.projectiledVelocity = weaponData.projectiledVelocity;
        this.dispersion = weaponData.dispersion;
        this.shellLifeTime = weaponData.shellLifeTime;
        this.caliber = weaponData.caliber;
        this.apDmgFactor = weaponData.apDmgFactor;
        this.heDmgFactor = weaponData.heDmgFactor;
        this.turningSpeedPerSecond = weaponData.turningSpeedPerSecond;
        this.coolDown = weaponData.coolDown;
        this.transform.localPosition = localPosition;
        this.multiShot = weaponData.multiShot;
        this.multiShotDelay = weaponData.multiShotDelay;
        this.projectileIsPropulsion = weaponData.isPropulsion;
        this.projectileIsGuided = weaponData.isGuided;

        effectiveRange = projectiledVelocity * shellLifeTime * Mathf.Log10(caliber) * 0.2f;

        parentVehicle = vehicle;
        delay = this.coolDown;
        isInit = true;
    }

    private void Update()
    {
        if (isInit == false)
        {
            //Debug.LogWarning("초기화되지 않은 무기 사용");
            return;
        }

        if(delay <= coolDown)
        {
            delay += Time.deltaTime;
            if(delay > coolDown)
            {
                coolDownComplete = true;
            }
        }        

        fireAngleComplete = TurretRotate();
        readyToFire = fireAngleComplete && coolDownComplete;

        if(trigger)
        {
            FireTrigger();
        }    
    }
    void FireTrigger()
    {
        if (readyToFire == false)
            return;

        GameObject projectilePrf;
        if(isEnemy)
        {
            projectilePrf = PrefabManager.Instance.projectile_Enemy;
        }
        else
        {
            projectilePrf = PrefabManager.Instance.projectile;
        }
        delay = 0;
        coolDownComplete = false;

        StartCoroutine(Fire(multiShotDelay, multiShot, projectilePrf));
    }

    IEnumerator Fire(float delayTime, int count, GameObject projectilePrf)
    {
        for (int i = 0; i < count; i++)
        {
            Projectile item = ObjectPoolManager.Instance.DequeueObject(projectilePrf).GetComponent<Projectile>();//탄 생성
            Quaternion dispersionAngle = Quaternion.Euler(0, 0, Random.Range(-(dispersion * 0.5f), dispersion * 0.5f));//발사각도 오차 생성
            Vector2 fireVelocity = dispersionAngle * (Vector2)transform.right * projectiledVelocity;//발사 벡터 생성
            item.Init(firePoint.position, parentVelocity, fireVelocity, firePoint.rotation, shellLifeTime, caliber, apDmgFactor, heDmgFactor, projectileIsPropulsion, projectileIsGuided, parentVehicle.GetTarget());//발사
            EffectManager.Instance.GenerateMuzzleFlash(firePoint, true, caliber);

            yield return new WaitForSeconds(delayTime);
        }
    }
    
    bool TurretRotate()//터렛 회전 함수. 발사 준비가 완료되면 true 반환
    {
        // 타겟 방향 벡터 계산
        toTargetVector2 = targetPosition - (Vector2)this.transform.position;
        float angleErrer = GetAngleBetweenVectors(this.transform.right, toTargetVector2);

        if (Mathf.Abs(angleErrer) < 0.5f)//각 오차가 0.5도 미만일 경우 목표로 직접 고정
        {
            // 타겟 방향 벡터의 각도 계산 (라디안)
            float angle = Mathf.Atan2(toTargetVector2.y, toTargetVector2.x) * Mathf.Rad2Deg;
            this.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            return true;
        }
        else
        {
            this.transform.Rotate(Vector3.forward, Mathf.Clamp(angleErrer, -1, 1) * turningSpeedPerSecond * Time.deltaTime);
            return false;
        }
    }

    float GetAngleBetweenVectors(Vector2 v1, Vector2 v2)//두 방향 벡터간의 각도 차이 반환 (-180~180)
    {
        float angle1 = Mathf.Atan2(v1.y, v1.x) * Mathf.Rad2Deg;
        float angle2 = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;

        // 각도 차이 계산
        float angleDifference = angle2 - angle1;

        // -180도에서 180도 사이의 각도로 변환
        if (angleDifference > 180f)
        {
            angleDifference -= 360f;
        }
        else if (angleDifference < -180f)
        {
            angleDifference += 360f;
        }

        return angleDifference;
    }

}
