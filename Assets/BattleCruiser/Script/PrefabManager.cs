using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PrefabManager : SceneSingleton<PrefabManager>
{
    public GameObject enemy;
    public GameObject projectile;
    public GameObject projectile_Enemy;
    public GameObject weapon;

    private void Awake()
    {
        Debug.Log($"{Instance.name} 로컬 인스턴싱 완료");
    }
}
