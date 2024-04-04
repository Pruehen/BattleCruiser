using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public GameObject projectile;
    public Transform firePoint;
    float projectiledVelocity = 110;
    Vector2 parentVelocity = Vector2.zero;

    float turningSpeedPerSecond = 90;
    float coolDown = 0.1f;//발사 쿨타임
    float delay = 0;

    bool coolDownComplete = false;//쿨타임 완료
    bool fireAngleComplete = false;//발사각 완료
    bool readyToFire = false;//발사 준비 완료
    bool trigger = false;//트리거

    Vector2 targetPosition;
    Vector2 toTargetVector2;

    public void SetTargetPoint(Vector2 targetPos)
    {
        targetPosition = targetPos;
    }
    public void SetTrigger(bool value)
    {
        trigger = value;
    }
    public void SetParentVelocity(Vector2 velocity)
    {
        parentVelocity = velocity;
    }

    private void Update()
    {
        if(delay < coolDown)
        {
            delay += Time.deltaTime;
            if(delay >= coolDown)
            {
                coolDownComplete = true;
            }
        }        

        fireAngleComplete = TurretRotate();
        readyToFire = fireAngleComplete && coolDownComplete;

        if(trigger)
        {
            Fire();
        }    
    }
    void Fire()
    {
        if (readyToFire == false)
            return;

        Projectile item = ObjectPoolManager.Instance.DequeueObject(projectile).GetComponent<Projectile>();
        item.Init(firePoint.position, (Vector2)transform.right * projectiledVelocity + parentVelocity, firePoint.rotation, 3);

        delay = 0;
        coolDownComplete = false;
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
