using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigidbody2D;

    bool isUse = false;
    float usingTime = 0;
    float selfDestructTime = 0;
    float caliber;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 position, Vector2 velocity, Quaternion quaternion, float time, float caliber)
    {
        this.transform.position = position;
        this.transform.rotation = quaternion;
        rigidbody2D.velocity = velocity;//��ǥ, ȸ��, �ӵ� �ʱ�ȭ

        isUse = true;
        usingTime = 0;
        selfDestructTime = time + Random.Range(-0.5f, 0.5f);    
        this.caliber = caliber;
    }

    private void Update()
    {
        usingTime += Time.deltaTime;
        SelfDestroy();
    }

    private void FixedUpdate()
    {
        rigidbody2D.AddTorque(GetAngleBetweenVectors(this.transform.right, rigidbody2D.velocity));
    }

    void SelfDestroy()
    {
        if (usingTime >= selfDestructTime)
        {
            ProjectileDestroy();
        }        
    }
    void ProjectileDestroy()
    {
        usingTime = 0;
        isUse = false;
        EffectManager.Instance.GenerateExplosion(this.transform.position, caliber);
        ObjectPoolManager.Instance.EnqueueObject(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Vehicle"))
        {
            
        }
        ProjectileDestroy();
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
