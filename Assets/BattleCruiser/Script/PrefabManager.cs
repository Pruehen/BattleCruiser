using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PrefabManager : SceneSingleton<PrefabManager>
{
    public GameObject[] playerPrfs;
    public GameObject[] enemyPrfs;

    public GameObject projectile;
    public GameObject projectile_Enemy;
    public GameObject[] weapons;

    private void Awake()
    {
        Debug.Log($"{Instance.name} ���� �ν��Ͻ� �Ϸ�");
    }
}
