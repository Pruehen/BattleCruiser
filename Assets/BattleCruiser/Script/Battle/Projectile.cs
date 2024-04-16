using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2D;
    Vector2 velocityTemp;

    bool isUse = false;
    float lifeTime = 0;
    float selfDestructTime = 0;
    float caliber;
    float apDmgFactor;
    float heDmgFactor;
    float deltaV;
    float propulsion;
    bool guided;

    Transform target;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 position, Vector2 shipVelocity, Vector2 projectiledVelocity, Quaternion quaternion, float time, float caliber, float apDmgFactor, float heDmgFactor, bool propulsion, bool guided, Vehicle target)
    {
        this.caliber = caliber;
        this.apDmgFactor = apDmgFactor;
        this.heDmgFactor = heDmgFactor;

        lifeTime = 0;
        selfDestructTime = time + Random.Range(-0.5f, 0.5f);

        if (propulsion)//로켓추진탄
        {
            this.deltaV = projectiledVelocity.magnitude * 0.9f;
            this.propulsion = (deltaV * 10) / selfDestructTime;
            rigidbody2D.velocity = shipVelocity + projectiledVelocity * 0.1f;//좌표, 회전, 속도 초기화
        }
        else//일반탄
        {
            deltaV = 0;
            this.propulsion = 0;
            rigidbody2D.velocity = shipVelocity + projectiledVelocity;//좌표, 회전, 속도 초기화
        }        
        if(guided && target != null)//유도탄
        {
            this.target = target.transform;
        }
        else//무유도탄
        {
            this.target = null;
        }
        this.transform.position = position;
        this.transform.rotation = quaternion;

        rigidbody2D.drag = 1 / caliber;
        float size = Mathf.Sqrt(caliber) * 0.1f;        
        this.transform.localScale = new Vector3(size, size, size);

        isUse = true;       
    }

    private void Update()
    {
        lifeTime += Time.deltaTime;        
        SelfDestroy();
    }

    private void FixedUpdate()
    {
        if(target != null)
        {
            Guided();
        }

        if (deltaV > 0)//추진
        {
            rigidbody2D.AddForce(this.transform.right * propulsion, ForceMode2D.Force);
            deltaV -= propulsion * Time.fixedDeltaTime;
        }
        else if (target == null)
        {
            rigidbody2D.AddTorque(GetAngleBetweenVectors(this.transform.right, rigidbody2D.velocity) * 0.1f, ForceMode2D.Force);//탄의 진행 방향으로 탄을 회전
        }

        velocityTemp = rigidbody2D.velocity;
    }

    Vector2 angleError_temp = Vector2.zero;
    void Guided()
    {
        float targetAngle = GetAngleBetweenVectors(this.transform.right, target.transform.position - this.transform.position);

        if (lifeTime > 0.5f)
        {
            Vector2 toTargetVec = (target.transform.position - this.transform.position).normalized;//방향 벡터

            Vector2 angleError_diff = (toTargetVec - angleError_temp) * 20;//프레임당 방향 변화량
            angleError_temp = toTargetVec;

            Vector2 aed_local = this.transform.InverseTransformDirection(angleError_diff);
            float torque = Mathf.Clamp(aed_local.y * 2, -0.02f, 0.02f) * caliber;

            rigidbody2D.AddTorque(torque, ForceMode2D.Force);
        }

        Vector2 sideForce = (Vector2)this.transform.right * rigidbody2D.velocity.magnitude - rigidbody2D.velocity;
        rigidbody2D.AddForce(sideForce, ForceMode2D.Force);

        
        if (targetAngle > 150)
        {
            target = null;
        }
    }
    void SelfDestroy()
    {
        if (lifeTime >= selfDestructTime)
        {
            ProjectileDestroy(this.transform.position);
        }        
    }
    void ProjectileDestroy(Vector2 hitPosition)
    {
        lifeTime = 0;
        isUse = false;

        EffectManager.Instance.GenerateExplosion((Vector2)hitPosition, caliber);
        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            float kineticEnergy = (velocityTemp - collision.gameObject.GetComponent<Rigidbody2D>().velocity).sqrMagnitude;            
            float kineticDmg = apDmgFactor * kineticEnergy * 0.00005f * caliber * caliber;            
            float explosiveDmg = caliber * caliber * caliber * heDmgFactor * 0.001f;

            collision.gameObject.GetComponent<Vehicle>().Demage(kineticDmg, explosiveDmg);
        }
        ProjectileDestroy(collision.contacts[0].point);
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
