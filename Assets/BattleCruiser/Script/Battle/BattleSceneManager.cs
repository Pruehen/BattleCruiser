using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class BattleSceneManager : SceneSingleton<BattleSceneManager>
{
    public int stage { get; private set; }
    public List<Vehicle> activeEnemyList { get; private set; }
    List<DropItemData> dropItemDatas;

    public Transform vehicleTrf;
    public ResultWdw resultWdw;

    private void Awake()
    {
        Debug.Log($"{Instance.name} ���� �ν��Ͻ� �Ϸ�");
        stage = GameManager.Instance.selectedStage;
        PlayerShipData playerShipData = GameManager.Instance.playerShipData;//�÷��̾��� �Լ� ������

        Player player = Instantiate(PrefabManager.Instance.playerPrfs[playerShipData.shipIndex], new Vector2(0, 50), Quaternion.identity, vehicleTrf).GetComponent<Player>();
        player.Init(playerShipData);//������ �Լ� �ʱ�ȭ

        float minX = 300;
        float minY = 50;
        float maxX = 1000 + stage * 1000;
        float maxY = 200 + stage * 20;

        StageData stageData = JsonDataManager.Instance.saveData.stageList[stage];//����� json�������� �������� �����͸� �����ؼ� �� ���� ��ǥ�� ����.
        activeEnemyList = new List<Vehicle>();

        for (int i = 0; i < stageData.stageShipDataList.Count; i++)
        {
            ShipData shipData = JsonDataManager.Instance.saveData.shipDataDictionary[stageData.stageShipDataList[i]];//������ �Լ� ������

            GameObject createEnemy = Instantiate(PrefabManager.Instance.enemyPrfs[0], new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY)), Quaternion.identity, vehicleTrf);
            createEnemy.name = shipData.className;
            Enemy enemy = createEnemy.GetComponent<Enemy>();
            enemy.Init(shipData);//������ �Լ� �ʱ�ȭ

            activeEnemyList.Add(createEnemy.GetComponent<Vehicle>());
        }

        dropItemDatas = new List<DropItemData>();
    }
    public void AddDropItem(string weaponKey, int rarity, Vector2 position)
    {
        dropItemDatas.Add(new DropItemData(weaponKey, rarity));
        GameObject dropItem = Instantiate(PrefabManager.Instance.dropItem, position, Quaternion.identity);

        Color dropItemColor;
        switch (rarity)
        {
            case 0:
                dropItemColor = RarityColor.tech0;
                break;
            case 1:
                dropItemColor = RarityColor.tech1;
                break;
            case 2:
                dropItemColor = RarityColor.tech2;
                break;
            case 3:
                dropItemColor = RarityColor.tech3;
                break;
            case 4:
                dropItemColor = RarityColor.tech4;
                break;
            case 5:
                dropItemColor = RarityColor.tech5;
                break;
            case 6:
                dropItemColor = RarityColor.tech6;
                break;
            case 7:
                dropItemColor = RarityColor.tech7;
                break;
            default:
                dropItemColor = Color.black;
                break;
        }

        dropItem.GetComponent<SpriteRenderer>().color = dropItemColor;
        resultWdw.AddRewardItem(weaponKey, dropItemColor);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GameEndCheck());
    }

    IEnumerator GameEndCheck()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1);

            if (Player.Instance.controlledShip.isDead)
            {
                //�÷��̾� �й� ó��
                StartCoroutine(GameEnd(false));
                yield break;
            }
            else
            {
                bool allEnemyDead = true;
                foreach (Vehicle enemy in activeEnemyList)
                {
                    if (enemy.isDead == false)
                    {
                        allEnemyDead = false;
                    }
                }

                if (allEnemyDead)
                {
                    //�÷��̾� �¸� ó��
                    StartCoroutine(GameEnd(true));
                    yield break;
                }
            }
        }
    }

    IEnumerator GameEnd(bool isWin)
    {
        SetTimeScale(0.2f);
        Time.fixedDeltaTime *= 0.2f;
        yield return new WaitForSecondsRealtime(5);

        SetTimeScale(1);
        Time.fixedDeltaTime *= 5;
        yield return new WaitForSecondsRealtime(3);

        for (int i = 0; i < dropItemDatas.Count; i++)//��� �������� �߰�
        {
            JsonDataManager.Instance.saveData.userData.CustomWeaponDataAdd(new CustomWeaponData(dropItemDatas[i].weaponKey, dropItemDatas[i].rarity));
        }
        Debug.Log($"{dropItemDatas.Count}���� ������ ���");
        JsonDataManager.Instance.DataSave();

        GameUI.Instance.OnResultWdw(isWin);
    }

    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
    }


    float totalKineticDmg = 0;
    float totalChemicalDmg = 0;

    float dmgViewTime = 10;
    float dmgViewDelay = 0;

    bool isDmgView = false;

    private void Update()
    {
        if (isDmgView)
        {
            dmgViewDelay += Time.deltaTime;
            if (dmgViewDelay >= dmgViewTime)
            {
                isDmgView = false;
                GameUI.Instance.DisableDmgText();

                totalKineticDmg = 0;
                totalChemicalDmg = 0;
            }
        }
    }

    public void KineticDmgUp(float dmg)
    {
        totalKineticDmg += dmg;
        dmgViewDelay = 0;
        isDmgView = true;
        GameUI.Instance.SetKineticDmgText(totalKineticDmg);
    }
    public void ChemicalDmgUp(float dmg)
    {
        totalChemicalDmg += dmg;
        dmgViewDelay = 0;
        isDmgView = true;
        GameUI.Instance.SetChemicalDmgText(totalChemicalDmg);
    }

    public class DropItemData
    {
        public string weaponKey { get; private set; }
        public int rarity { get; private set; }

        public DropItemData(string weaponKey, int rarity)
        {
            this.weaponKey = weaponKey;
            this.rarity = rarity;
        }
    }
}
