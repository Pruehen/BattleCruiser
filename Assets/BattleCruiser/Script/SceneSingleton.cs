using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�̱��� ���׸� Ŭ����. �̱������� ����� Ŭ�������� ��ӹ޾Ƽ� ���. �ٸ� ������ �Ѿ�� �������� ����.
public class SceneSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
                //DontDestroyOnLoad(instance.gameObject);

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<T>();
                }
            }

            return instance;
        }
    }
}