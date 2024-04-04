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
    float coolDown = 0.1f;//�߻� ��Ÿ��
    float delay = 0;

    bool coolDownComplete = false;//��Ÿ�� �Ϸ�
    bool fireAngleComplete = false;//�߻簢 �Ϸ�
    bool readyToFire = false;//�߻� �غ� �Ϸ�
    bool trigger = false;//Ʈ����

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
    
    bool TurretRotate()//�ͷ� ȸ�� �Լ�. �߻� �غ� �Ϸ�Ǹ� true ��ȯ
    {
        // Ÿ�� ���� ���� ���
        toTargetVector2 = targetPosition - (Vector2)this.transform.position;
        float angleErrer = GetAngleBetweenVectors(this.transform.right, toTargetVector2);

        if (Mathf.Abs(angleErrer) < 0.5f)//�� ������ 0.5�� �̸��� ��� ��ǥ�� ���� ����
        {
            // Ÿ�� ���� ������ ���� ��� (����)
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

    float GetAngleBetweenVectors(Vector2 v1, Vector2 v2)//�� ���� ���Ͱ��� ���� ���� ��ȯ (-180~180)
    {
        float angle1 = Mathf.Atan2(v1.y, v1.x) * Mathf.Rad2Deg;
        float angle2 = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;

        // ���� ���� ���
        float angleDifference = angle2 - angle1;

        // -180������ 180�� ������ ������ ��ȯ
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
