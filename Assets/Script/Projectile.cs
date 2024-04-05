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
    float apDmgFactor;
    float heDmgFactor;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 position, Vector2 velocity, Quaternion quaternion, float time, float caliber, float apDmgFactor, float heDmgFactor)
    {
        this.transform.position = position;
        this.transform.rotation = quaternion;
        rigidbody2D.velocity = velocity;//좌표, 회전, 속도 초기화
        rigidbody2D.drag = 3 / caliber;
        float size = Mathf.Sqrt(caliber) * 0.1f;        
        this.transform.localScale = new Vector3(size, size, size);

        isUse = true;
        usingTime = 0;
        selfDestructTime = time + Random.Range(-0.5f, 0.5f);  
        
        this.caliber = caliber;
        this.apDmgFactor = apDmgFactor;
        this.heDmgFactor = heDmgFactor;
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
            float kineticDmg = apDmgFactor * rigidbody2D.velocity.sqrMagnitude * 0.0005f * caliber;
            float explosiveDmg = caliber * caliber * caliber * heDmgFactor * 0.001f;

            Debug.Log($"물리 데미지 : {kineticDmg}");
            Debug.Log($"폭발 데미지 : {explosiveDmg}");
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
