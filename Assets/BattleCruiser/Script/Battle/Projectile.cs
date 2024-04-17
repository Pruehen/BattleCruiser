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
    bool guided;

    Transform target;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        trilRenderer = GetComponent<TrailRenderer>();
    }

    public void Init(Vector2 position, Vector2 shipVelocity, Vector2 projectiledVelocity, Quaternion quaternion, float time, float caliber, float apDmgFactor, float heDmgFactor, bool propulsion, bool guided, Vehicle target)
    {
        this.caliber = caliber;//���� ����
        this.apDmgFactor = apDmgFactor;//���� ������ ���� ����
        this.heDmgFactor = heDmgFactor;//ȭ�� ������ ���� ����

        lifeTime = 0;//������Ÿ�� ���� �ʱ�ȭ
        selfDestructTime = time + Random.Range(-0.5f, 0.5f);//ź �۵� �ð� ����

        this.transform.position = position;//��ǥ ����
        this.transform.rotation = quaternion;//ȸ�� ����

        rigidbody2D.drag = 1 / caliber;//�׷� ����
        float size = Mathf.Sqrt(caliber) * 0.1f;//ź�� ���� ������ ���� ����
        this.transform.localScale = new Vector3(size, size, size);//���� ������ ����

        if (propulsion)//��������ź
        {
            this.deltaV = projectiledVelocity.magnitude;//�ӵ�����
            this.propulsion = (deltaV * 5) / selfDestructTime;//�߷�
            rigidbody2D.velocity = shipVelocity + projectiledVelocity * 0.1f;//�ʱ� �ӵ� ����

            EffectManager.Instance.GenerateEngineEffect(this.transform, selfDestructTime * 0.2f, 0.5f);//�������� ����Ʈ ����            
        }
        else//�Ϲ�ź
        {
            deltaV = 0;
            this.propulsion = 0;
            rigidbody2D.velocity = shipVelocity + projectiledVelocity;//�ʱ� �ӵ� ����
        }        

        if(guided && target != null)//����ź
        {
            this.target = target.transform;//Ÿ�� ����
            rigidbody2D.drag *= 2;//�׷� 2���
        }
        else//������ź
        {
            this.target = null;
        }

        if(caliber < 100)//ź�� ������ 100 �̸�
        {
            trilRenderer.enabled = false;
        }
        else//ź�� ������ 100 �̻�
        {
            trilRenderer.enabled = true;//Ʈ���� ������ Ȱ��ȭ
            trilRenderer.Clear();//���� ���� ����
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

        if (deltaV > 0)//����
        {
            rigidbody2D.AddForce(this.transform.right * propulsion, ForceMode2D.Force);
            deltaV -= propulsion * Time.fixedDeltaTime;
        }
        else if (target == null)
        {
            rigidbody2D.AddTorque(GetAngleBetweenVectors(this.transform.right, rigidbody2D.velocity) * 0.1f, ForceMode2D.Force);//ź�� ���� �������� ź�� ȸ��
        }

        velocityTemp = rigidbody2D.velocity;
    }

    Vector2 angleError_temp = Vector2.zero;
    void Guided()//���� �κ�
    {
        float targetAngle = GetAngleBetweenVectors(this.transform.right, target.transform.position - this.transform.position);

        if (lifeTime > 0.5f)
        {
            Vector2 toTargetVec = (target.transform.position - this.transform.position).normalized;//���� ����

            Vector2 angleError_diff = (toTargetVec - angleError_temp) * 20;//�����Ӵ� ���� ��ȭ��
            angleError_temp = toTargetVec;

            Vector2 aed_local = this.transform.InverseTransformDirection(angleError_diff);
            float torque = Mathf.Clamp(aed_local.y * 5, -0.2f, 0.2f) * Mathf.Sqrt(caliber);

            if (Mathf.Abs(targetAngle) > 110)
            {
                torque *= -10;
            }            
            

            rigidbody2D.AddTorque(torque, ForceMode2D.Force);
        }

        Vector2 sideForce = (Vector2)this.transform.right * rigidbody2D.velocity.magnitude - rigidbody2D.velocity;
        rigidbody2D.AddForce(sideForce, ForceMode2D.Force);      
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
