using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�̱��� ���׸� Ŭ����. �̱������� ����� Ŭ�������� ��ӹ޾Ƽ� ���.
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<T>();

                    DontDestroyOnLoad(singletonObject);
                }
            }

            return instance;
        }
    }
}