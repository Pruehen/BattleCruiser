using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class EffectManager : SceneSingleton<EffectManager>
{
    public GameObject[] muzzleFlashes;
    public GameObject[] explosionEffect;
    public GameObject[] demagedEffect;
    public GameObject engineEffect;
    private void Awake()
    {
        foreach (GameObject item in muzzleFlashes)
        {
            ObjectPoolManager.Instance.CreatePool(item);
        }
    }

    public void GenerateMuzzleFlash(Transform parent, bool isLocalPosition, float caliber)//�ѱ� ȭ�� ����
    {        
        int randomIndex = Random.Range(0, muzzleFlashes.Length);//���� �ε���
        GameObject item = ObjectPoolManager.Instance.DequeueObject(muzzleFlashes[randomIndex]);//������ �ѱ� ȭ�� ����Ʈ ����        
        if (isLocalPosition)
        {
            item.transform.SetParent(parent);//���� ������ ��� Ʈ������ ��ȯ
        }
        float size = Mathf.Sqrt(caliber) * 0.2f;
        item.transform.localScale = new Vector3(size, size, size);

        item.transform.position = parent.position;
        item.transform.rotation = parent.rotation;
        item.GetComponent<ParticleSystem>().Play();//��ġ, ȸ�� ���� �� ��ƼŬ �÷���
        StartCoroutine(EffectEnqueue(0.15f, item));//�ڷ�ƾ���� ���� �ð� ��� �� ��ť
    }
    public void GenerateExplosion(Vector2 position, float size)//���� ����Ʈ ����
    {
        GameObject item;
        if(size < 2)//2 �̸��� ���
        {
            item = ObjectPoolManager.Instance.DequeueObject(explosionEffect[0]);//���� ���� ����Ʈ ����
            StartCoroutine(EffectEnqueue(0.5f, item));//�ڷ�ƾ���� ���� �ð� ��� �� ��ť
        }
        else if(size < 20)//20 �̸��� ���
        {
            item = ObjectPoolManager.Instance.DequeueObject(explosionEffect[1]);//���� ���� ����Ʈ ����
            StartCoroutine(EffectEnqueue(2f, item));//�ڷ�ƾ���� ���� �ð� ��� �� ��ť
        }
        else
        {
            item = ObjectPoolManager.Instance.DequeueObject(explosionEffect[2]);//���� ���� ����Ʈ ����
            StartCoroutine(EffectEnqueue(11f, item));//�ڷ�ƾ���� ���� �ð� ��� �� ��ť
        }
        item.transform.position = position;
        //float size = Mathf.Sqrt(caliber) * 0.2f;
        item.transform.localScale = new Vector3(size, size, size);
        item.GetComponent<ParticleSystem>().Play();//��ġ, ȸ�� ���� �� ��ƼŬ �÷���        
    }
    public void GenerateDemageEffect(Transform parent, Vector2 position, int index)//�ջ� ����Ʈ ����
    {        
        GameObject item = ObjectPoolManager.Instance.DequeueObject(demagedEffect[index]);//�ջ� ȭ�� �Ǵ� ���� ����Ʈ ����        
        
        item.transform.SetParent(parent.parent);//Ʈ������ ��ȯ
        item.transform.localScale = new Vector3(1, 1, 1);
        item.transform.position = position;
        item.transform.rotation = Quaternion.identity;        

        item.GetComponent<ParticleSystem>().Play();//��ġ, ȸ�� ���� �� ��ƼŬ �÷���        
    }
    public GameObject GenerateEngineEffect(Transform parent)
    {
        GameObject item = ObjectPoolManager.Instance.DequeueObject(engineEffect);

        item.transform.SetParent(parent);//Ʈ������ ��ȯ
        item.transform.localScale = new Vector3(1, 1, 1);
        item.transform.position = parent.position;
        item.transform.rotation = Quaternion.identity;

        return item;
    }
    public GameObject GenerateEngineEffect(Transform parent, float scale)
    {
        GameObject item = ObjectPoolManager.Instance.DequeueObject(engineEffect);

        item.transform.SetParent(parent);//Ʈ������ ��ȯ
        item.transform.localScale = new Vector3(scale, scale * 2, scale);
        item.transform.position = parent.position;
        item.transform.rotation = parent.rotation;
        item.transform.Rotate(0, 0, -90);

        //StartCoroutine(EffectEnqueue(usingTime, item));//�ڷ�ƾ���� ���� �ð� ��� �� ��ť
        return item;
    }


    IEnumerator EffectEnqueue(float time, GameObject item)//��ť �� Ʈ������ �ʱ�ȭ
    {
        yield return new WaitForSeconds(time);
        ObjectPoolManager.Instance.EnqueueObject(item);
    }
    public void EffectEnqueue(GameObject item)
    {
        ObjectPoolManager.Instance.EnqueueObject(item);
    }
}
