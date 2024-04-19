using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2D;
    TrailRenderer trilRenderer;
    Vector2 velocityTemp;

    bool isUse = false;
    float lifeTime = 0;
    float selfDestructTime = 0;
    float caliber;
    float apDmgFactor;
    float heDmgFactor;
    float deltaV;
    float propulsion;
    float hp;
    bool guided;

    Transform target;

    GameObject motorEffect;
    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        trilRenderer = GetComponent<TrailRenderer>();
    }

    public void Init(Vector2 position, Vector2 shipVelocity, Vector2 projectiledVelocity, Quaternion quaternion, float time, float caliber, float apDmgFactor, float heDmgFactor, bool propulsion, bool guided, Vehicle target)
    {
        this.caliber = caliber;//직경 설정
        this.hp = caliber * (apDmgFactor + 1);
        this.apDmgFactor = apDmgFactor;//물리 데미지 팩터 설정
        this.heDmgFactor = heDmgFactor;//화학 데미지 팩터 설정

        lifeTime = 0;//라이프타임 변수 초기화
        selfDestructTime = time + Random.Range(-0.5f, 0.5f);//탄 작동 시간 설정

        this.transform.position = position;//좌표 설정
        this.transform.rotation = quaternion;//회전 설정
       
        float size = Mathf.Sqrt(caliber) * 0.1f;//탄의 렌더 사이즈 변수 설정
        this.transform.localScale = new Vector3(size, size, size);//실제 사이즈 변경
        rigidbody2D.drag = 1 / caliber;//항력 설정
        rigidbody2D.mass = caliber * caliber * 0.001f * (apDmgFactor + 1);
        
        if (propulsion)//로켓추진탄
        {
            this.deltaV = projectiledVelocity.magnitude;//속도증분
            this.propulsion = (deltaV * 5) / selfDestructTime;//추력
            rigidbody2D.velocity = shipVelocity + projectiledVelocity * 0.1f;//초기 속도 설정

            if (motorEffect != null)
            {
                EffectManager.Instance.EffectEnqueue(motorEffect);//로켓추진 이펙트 제거
            }
            motorEffect = EffectManager.Instance.GenerateEngineEffect(this.transform, 0.5f);//로켓추진 이펙트 생성         
        }
        else//일반탄
        {
            deltaV = 0;
            this.propulsion = 0;
            rigidbody2D.velocity = shipVelocity + projectiledVelocity;//초기 속도 설정

            if(motorEffect != null)
            {
                EffectManager.Instance.EffectEnqueue(motorEffect);//로켓추진 이펙트 제거
                motorEffect = null;
            }
        }        

        if(guided && target != null)//유도탄
        {
            this.target = target.transform;//타겟 설정
            rigidbody2D.drag *= 2;//항력 2배로
        }
        else//무유도탄
        {
            this.target = null;
        }

        if(caliber < 100)//탄의 직경이 100 미만
        {
            trilRenderer.enabled = false;
        }
        else//탄의 직경이 100 이상
        {
            trilRenderer.enabled = true;//트레일 렌더러 활성화
            trilRenderer.Clear();//기존 레이 제거
        }

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
            rigidbody2D.AddForce(this.transform.right * propulsion * rigidbody2D.mass, ForceMode2D.Force);
            deltaV -= propulsion * Time.fixedDeltaTime;
            if(deltaV <= 0 && motorEffect != null)
            {
                EffectManager.Instance.EffectEnqueue(motorEffect);
                motorEffect = null;
            }
        }
        else if (target == null)
        {
            rigidbody2D.AddTorque(GetAngleBetweenVectors(this.transform.right, rigidbody2D.velocity) * 0.1f * rigidbody2D.mass, ForceMode2D.Force);//탄의 진행 방향으로 탄을 회전
        }

        velocityTemp = rigidbody2D.velocity;
    }

    Vector2 angleError_temp = Vector2.zero;
    void Guided()//유도 부분
    {
        float targetAngle = GetAngleBetweenVectors(this.transform.right, target.transform.position - this.transform.position);

        if (lifeTime > 0.5f)
        {
            Vector2 toTargetVec = (target.transform.position - this.transform.position).normalized;//방향 벡터

            Vector2 angleError_diff = (toTargetVec - angleError_temp) * 20;//프레임당 방향 변화량
            angleError_temp = toTargetVec;

            Vector2 aed_local = this.transform.InverseTransformDirection(angleError_diff);
            float torque = Mathf.Clamp(aed_local.y * 5, -0.2f, 0.2f) * Mathf.Sqrt(caliber);

            if (Mathf.Abs(targetAngle) > 110)
            {
                torque *= -10;
            }            
            

            rigidbody2D.AddTorque(torque * rigidbody2D.mass, ForceMode2D.Force);
        }

        Vector2 sideForce = (Vector2)this.transform.right * rigidbody2D.velocity.magnitude - rigidbody2D.velocity;
        rigidbody2D.AddForce(sideForce * rigidbody2D.mass, ForceMode2D.Force);      
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

        float dmgRangeSize = caliber * 0.02f * Mathf.Sqrt(heDmgFactor);
        float explosiveDmg = caliber * caliber * caliber * heDmgFactor * 0.001f;
        if (dmgRangeSize > 20)
        {
            //Debug.Log(dmgRangeSize);
            Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(this.transform.position, dmgRangeSize, 1 << 8);
            Collider2D[] collider2Ds2 = Physics2D.OverlapCircleAll(this.transform.position, dmgRangeSize, 1 << 6);
            //Debug.Log(collider2Ds.Length);
            for (int i = 0; i < collider2Ds.Length; i++)
            {
                Vehicle target;
                if (collider2Ds[i].gameObject.TryGetComponent(out target))
                {
                    float dmg = (0.1f * explosiveDmg) / Mathf.Log10((this.transform.position - target.transform.position).sqrMagnitude);
                    target.Demage(0, dmg);
                    //Debug.Log();
                }
            }
            for (int i = 0; i < collider2Ds2.Length; i++)
            {
                Vehicle target;
                if (collider2Ds2[i].gameObject.TryGetComponent(out target))
                {
                    float dmg = (0.1f * explosiveDmg) / Mathf.Log10((this.transform.position - target.transform.position).sqrMagnitude);
                    target.Demage(0, dmg);
                    //Debug.Log();
                }
            }
        }

        EffectManager.Instance.GenerateExplosion((Vector2)hitPosition, caliber * 0.01f * Mathf.Sqrt(heDmgFactor));
        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);
    }
    void ProjectileDemage(float dmg)
    {
        hp -= dmg;
        if (hp <= 0) 
        {
            hp = caliber * apDmgFactor;
            ProjectileDestroy(this.transform.position);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {        
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            float kineticEnergy = (velocityTemp - collision.gameObject.GetComponent<Rigidbody2D>().velocity).sqrMagnitude;            
            float kineticDmg = apDmgFactor * kineticEnergy * 0.00005f * caliber * caliber;
            float explosiveDmg = caliber * caliber * caliber * heDmgFactor * 0.001f;

            collision.gameObject.GetComponent<Vehicle>().Demage(kineticDmg, explosiveDmg);



            ProjectileDestroy(collision.contacts[0].point);
        }
        else if(collision.gameObject.CompareTag("Projectile"))
        {
            collision.gameObject.GetComponent<Projectile>().ProjectileDemage(this.hp);
            this.ProjectileDemage(collision.gameObject.GetComponent<Projectile>().hp);

            //rigidbody2D.velocity = velocityTemp;
        }            
        else
        {
            ProjectileDestroy(collision.contacts[0].point);
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
