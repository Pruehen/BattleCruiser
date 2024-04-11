using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform firePoint;
    float projectiledVelocity = 100;//�߻� �ӵ�
    float dispersion = 3;//�߻簢 ����(����)
    float shellLifeTime = 6;//ź �۵� �ð�
    Vector2 parentVelocity = Vector2.zero;
    float caliber = 100f;
    float apDmgFactor = 1;
    float heDmgFactor = 1;

    float turningSpeedPerSecond = 90;
    float coolDown = 0.1f;//�߻� ��Ÿ��
    float delay = 0;

    bool coolDownComplete = false;//��Ÿ�� �Ϸ�
    bool fireAngleComplete = false;//�߻簢 �Ϸ�
    bool readyToFire = false;//�߻� �غ� �Ϸ�
    bool trigger = false;//Ʈ����

    Vector2 targetPosition;//���� ��ǥ(����)
    Vector2 toTargetVector2;//���� ��ǥ(����)

    bool isEnemy = false;
    bool isInit = false;

    public void SetTargetPoint(Vector2 targetPos)//���� ��ǥ ���� �� �Ÿ�, �߷�, ������ ����� ���� ����
    {
        float distance = (targetPos - (Vector2)this.transform.position).magnitude;//��ǥ���� �Ÿ�
        float eta = distance / projectiledVelocity;//��ǥ������ ���� ���� �ð�
        Vector2 calcTargetPos = new Vector2(targetPos.x, targetPos.y + (eta * 4.9f * eta));//�ʱ� ��ǥ ����

        float drag = 10/caliber;//���� ����
        float finalVelocity = projectiledVelocity * Mathf.Exp(-drag * eta);//���� ���׿� ���� ���� ź�� ���� �ӵ�
        eta = (calcTargetPos - (Vector2)this.transform.position).magnitude / ((projectiledVelocity + finalVelocity) * 0.5f);//������ ���� �� ���� ���׿� ���� ���� ���� �ð� ����
        finalVelocity = projectiledVelocity * Mathf.Exp(-drag * eta);//���� ���׿� ���� ���� ź�� ���� �ӵ��� ��ȭ�� eta���� ���� ����
        eta = (calcTargetPos - (Vector2)this.transform.position).magnitude / ((projectiledVelocity + finalVelocity) * 0.5f);//������ ���� �� ���� ���׿� ���� ���� ���� �ð� ����
        calcTargetPos = new Vector2(targetPos.x, targetPos.y + Mathf.Clamp(Mathf.Pow(eta, 2 + (eta * 0.002f)) * 4.9f, 0, distance));//���� ����

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
    public void Init(bool isEnemy, WeaponData weaponData, Vector2 localPosition)
    {
        this.isEnemy = isEnemy;
        this.projectiledVelocity = weaponData.projectiledVelocity;
        this.dispersion = weaponData.dispersion;
        this.shellLifeTime = weaponData.shellLifeTime;
        this.caliber = weaponData.caliber;
        this.apDmgFactor = weaponData.apDmgFactor;
        this.heDmgFactor = weaponData.heDmgFactor;
        this.turningSpeedPerSecond = weaponData.turningSpeedPerSecond;
        this.coolDown = weaponData.coolDown;
        this.transform.localPosition = localPosition;

        isInit = true;
    }

    private void Update()
    {
        if (isInit == false)
        {
            Debug.LogWarning("�ʱ�ȭ���� ���� ���� ���");
        }

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

        GameObject projectilePrf;
        if(isEnemy)
        {
            projectilePrf = PrefabManager.Instance.projectile_Enemy;
        }
        else
        {
            projectilePrf = PrefabManager.Instance.projectile;
        }

        Projectile item = ObjectPoolManager.Instance.DequeueObject(projectilePrf).GetComponent<Projectile>();//ź ����
        Quaternion dispersionAngle = Quaternion.Euler(0, 0, Random.Range(-(dispersion * 0.5f), dispersion * 0.5f));//�߻簢�� ���� ����
        Vector2 fireVelocity = dispersionAngle * (Vector2)transform.right * projectiledVelocity;//�߻� ���� ����
        item.Init(firePoint.position, fireVelocity + parentVelocity, firePoint.rotation, shellLifeTime, caliber, apDmgFactor, heDmgFactor);//�߻�

        delay = 0;
        coolDownComplete = false;

        EffectManager.Instance.GenerateMuzzleFlash(firePoint, true);
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
