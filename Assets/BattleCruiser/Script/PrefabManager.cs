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
    public GameObject rarityCurver;//��� Ŀ��
    public GameObject newItemCurver;//���ο� ������ Ŀ��

    public GameObject dropItem;
    public GameObject slot;
    

    private void Awake()
    {
        Debug.Log($"{Instance.name} ���� �ν��Ͻ� �Ϸ�");
    }
}
