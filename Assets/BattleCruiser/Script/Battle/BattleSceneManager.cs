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

        SkyboxChanger.Instance.ChangeSkybox();

        stage = GameManager.Instance.selectedStage;
        PlayerShipData playerShipData = GameManager.Instance.playerShipData;//�÷��̾��� �Լ� ������

        Player player = Instantiate(PrefabManager.Instance.playerPrfs[playerShipData.shipIndex], new Vector2(0, 50), Quaternion.identity, vehicleTrf).GetComponent<Player>();
        player.Init(playerShipData);//������ �Լ� �ʱ�ȭ

        float minX = 300;
        float minY = 50;
        float maxX = 1000 + stage * 200;
        float maxY = 200 + stage * 20;

        StageData stageData = JsonDataManager.Instance.saveData.stageList[stage];//����� json�������� �������� �����͸� �����ؼ� �� ���� ��ǥ�� ����.
        activeEnemyList = new List<Vehicle>();

        for (int l = 0; l < Mathf.Pow(2, GameManager.Instance.Setting.difficulty); l++)
        {
            for (int i = 0; i < stageData.stageShipDataList.Count; i++)
            {
                string key = stageData.stageShipDataList[i];
                ShipData shipData = JsonDataManager.Instance.saveData.shipDataDictionary[key];//������ �Լ� ������

                GameObject createEnemy = Instantiate(PrefabManager.Instance.enemyPrfs[key.Index()], new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY)), Quaternion.identity, vehicleTrf);
                createEnemy.name = shipData.className;
                Enemy enemy = createEnemy.GetComponent<Enemy>();
                enemy.Init(shipData);//������ �Լ� �ʱ�ȭ

                activeEnemyList.Add(createEnemy.GetComponent<Vehicle>());
            }
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
        yield return new WaitForSecondsRealtime(5);

        if(stage == 10)
        {
            yield return new WaitForSecondsRealtime(10);
        }


        if (stage != 9 || !isWin)
        {
            for (int i = 0; i < dropItemDatas.Count; i++)//��� �������� �߰�
            {
                JsonDataManager.Instance.saveData.userData.CustomWeaponDataAdd(new CustomWeaponData(dropItemDatas[i].weaponKey, dropItemDatas[i].rarity));
            }
            Debug.Log($"{dropItemDatas.Count}���� ������ ���");
            JsonDataManager.Instance.DataSave();

            GameUI.Instance.OnResultWdw(isWin);
        }
        else
        {
            stage = 10;

            StageData stageData = JsonDataManager.Instance.saveData.stageList[stage];//����� json�������� �������� �����͸� �����ؼ� �� ���� ��ǥ�� ����.
            string key = stageData.stageShipDataList[0];
            ShipData shipData = JsonDataManager.Instance.saveData.shipDataDictionary[key];//������ �Լ� ������

            GameObject createEnemy = Instantiate(PrefabManager.Instance.enemyPrfs[key.Index()], new Vector2(Player.Instance.transform.position.x + 1000, 2000), Quaternion.identity, vehicleTrf);
            createEnemy.name = shipData.className;
            Enemy enemy = createEnemy.GetComponent<Enemy>();
            enemy.Init(shipData);//������ �Լ� �ʱ�ȭ

            activeEnemyList.Add(createEnemy.GetComponent<Vehicle>());
            StartCoroutine(GameEndCheck());
        }
    }

    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
    }


    float totalKineticDmg = 0;
    float totalChemicalDmg = 0;

    float totalKineticDmg_Enemy = 0;
    float totalChemicalDmg_Enemy = 0;


    public void DmgUp(float kntDmg, float expDmg)
    {
        totalKineticDmg += kntDmg;
        totalChemicalDmg += expDmg;             
        GameUI.Instance.SetDmgText(totalKineticDmg, totalChemicalDmg);
    }
    public void DmgUp_Enemy(float kntDmg, float expDmg)
    {
        totalKineticDmg_Enemy += kntDmg;
        totalChemicalDmg_Enemy += expDmg;
        GameUI.Instance.SetEnemyDmgText(totalKineticDmg_Enemy, totalChemicalDmg_Enemy);
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
