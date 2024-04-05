using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EffectManager : Singleton<EffectManager>
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

    public void GenerateMuzzleFlash(Transform parent, bool isLocalPosition)//�ѱ� ȭ�� ����
    {        
        int randomIndex = Random.Range(0, muzzleFlashes.Length);//���� �ε���
        GameObject item = ObjectPoolManager.Instance.DequeueObject(muzzleFlashes[randomIndex]);//������ �ѱ� ȭ�� ����Ʈ ����
        Transform parentTemp = item.transform.parent;//������Ʈ Ǯ�� Ʈ������
        if (isLocalPosition)
        {
            item.transform.SetParent(parent);//���� ������ ��� Ʈ������ ��ȯ
        }
        item.transform.position = parent.position;
        item.transform.rotation = parent.rotation;
        item.GetComponent<ParticleSystem>().Play();//��ġ, ȸ�� ���� �� ��ƼŬ �÷���
        StartCoroutine(EffectEnqueue(0.05f, item, parentTemp));//�ڷ�ƾ���� ���� �ð� ��� �� ��ť
    }
    public void GenerateExplosion(Vector2 position, float caliber)//���� ����Ʈ ����
    {
        GameObject item;
        if(caliber < 100)
        {
            item = ObjectPoolManager.Instance.DequeueObject(explosionEffect[0]);//���� ���� ����Ʈ ����
        }
        else
        {
            item = ObjectPoolManager.Instance.DequeueObject(explosionEffect[1]);//���� ���� ����Ʈ ����
        }
        item.transform.position = position;
        float size = Mathf.Sqrt(caliber) * 0.1f;
        item.transform.localScale = new Vector3(size, size, size);
        item.GetComponent<ParticleSystem>().Play();//��ġ, ȸ�� ���� �� ��ƼŬ �÷���
        StartCoroutine(EffectEnqueue(1, item));//�ڷ�ƾ���� ���� �ð� ��� �� ��ť
    }
    public void GenerateDemageEffect(Transform parent, int index)//�ջ� ����Ʈ ����
    {        
        GameObject item = ObjectPoolManager.Instance.DequeueObject(demagedEffect[index]);//�ջ� ȭ�� �Ǵ� ���� ����Ʈ ����        

        item.transform.SetParent(parent);//Ʈ������ ��ȯ
        item.transform.position = parent.position;
        item.transform.rotation = Quaternion.identity;

        item.GetComponent<ParticleSystem>().Play();//��ġ, ȸ�� ���� �� ��ƼŬ �÷���        
    }

    IEnumerator EffectEnqueue(float time, GameObject item, Transform defaultParent = null)//��ť �� Ʈ������ �ʱ�ȭ
    {
        yield return new WaitForSeconds(time);
        if (defaultParent != null)
        {
            item.transform.SetParent(defaultParent);
        }
        ObjectPoolManager.Instance.EnqueueObject(item);
    }
}
