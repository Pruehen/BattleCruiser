using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EffectManager : SceneSingleton<EffectManager>
{
    public GameObject[] muzzleFlashes;
    public GameObject[] explosionEffect;
    public GameObject[] demagedEffect;

    private void Awake()
    {
        foreach (GameObject item in muzzleFlashes)
        {
            ObjectPoolManager.Instance.CreatePool(item);
        }
    }

    public void GenerateMuzzleFlash(Transform parent, bool isLocalPosition)//총구 화염 생성
    {        
        int randomIndex = Random.Range(0, muzzleFlashes.Length);//랜덤 인덱스
        GameObject item = ObjectPoolManager.Instance.DequeueObject(muzzleFlashes[randomIndex]);//랜덤한 총구 화염 이펙트 생성        
        if (isLocalPosition)
        {
            item.transform.SetParent(parent);//로컬 생성일 경우 트랜스폼 변환
        }
        item.transform.position = parent.position;
        item.transform.rotation = parent.rotation;
        item.GetComponent<ParticleSystem>().Play();//위치, 회전 지정 후 파티클 플레이
        StartCoroutine(EffectEnqueue(0.05f, item));//코루틴으로 일정 시간 경과 후 인큐
    }
    public void GenerateExplosion(Vector2 position, float caliber)//폭발 이펙트 생성
    {
        GameObject item;
        if(caliber < 100)
        {
            item = ObjectPoolManager.Instance.DequeueObject(explosionEffect[0]);//소형 폭발 이펙트 생성
        }
        else
        {
            item = ObjectPoolManager.Instance.DequeueObject(explosionEffect[1]);//중형 폭발 이펙트 생성
        }
        item.transform.position = position;
        float size = Mathf.Sqrt(caliber) * 0.2f;
        item.transform.localScale = new Vector3(size, size, size);
        item.GetComponent<ParticleSystem>().Play();//위치, 회전 지정 후 파티클 플레이
        StartCoroutine(EffectEnqueue(1, item));//코루틴으로 일정 시간 경과 후 인큐
    }
    public void GenerateDemageEffect(Transform parent, Vector2 position, int index)//손상 이펙트 생성
    {        
        GameObject item = ObjectPoolManager.Instance.DequeueObject(demagedEffect[index]);//손상 화염 또는 연기 이펙트 생성        
        
        item.transform.SetParent(parent.parent);//트랜스폼 변환
        item.transform.localScale = new Vector3(1, 1, 1);
        item.transform.position = position;
        item.transform.rotation = Quaternion.identity;        

        item.GetComponent<ParticleSystem>().Play();//위치, 회전 지정 후 파티클 플레이        
    }

    IEnumerator EffectEnqueue(float time, GameObject item)//인큐 및 트랜스폼 초기화
    {
        yield return new WaitForSeconds(time);
        ObjectPoolManager.Instance.EnqueueObject(item);
    }
}
