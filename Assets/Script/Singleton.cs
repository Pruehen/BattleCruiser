using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//싱글톤 제네릭 클래스. 싱글톤으로 사용할 클래스에서 상속받아서 사용.
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