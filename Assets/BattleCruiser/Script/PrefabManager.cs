using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    static PrefabManager instance;
    public static PrefabManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (PrefabManager)FindObjectOfType(typeof(PrefabManager));
                DontDestroyOnLoad(instance.gameObject);

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    singletonObject.name = "PrefabManager";
                    instance = singletonObject.AddComponent<PrefabManager>();
                }
            }

            return instance;
        }
    }

    public GameObject enemy;
    public GameObject projectile;
    public GameObject projectile_Enemy;
    public GameObject weapon;

    private void Awake()
    {
        Debug.Log($"{Instance.name} 인스턴싱 완료");
    }
}
