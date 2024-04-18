using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PrefabManager : GlobalSingleton<PrefabManager>
{
    public GameObject[] playerPrfs;
    public GameObject[] enemyPrfs;

    public GameObject projectile;
    public GameObject projectile_Enemy;
    public GameObject[] weapons;

    public GameObject[] weapons_Image;
    public GameObject rarityCurver;//레어도 커버
    public GameObject newItemCurver;//새로운 아이템 커버

    public GameObject dropItem;
    public GameObject slot;
    

    private void Awake()
    {
        Debug.Log($"{Instance.name} 전역 인스턴싱 완료");
    }
}
