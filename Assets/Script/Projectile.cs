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
        rigidbody2D.velocity = velocity;//좌표, 회전, 속도 초기화

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
