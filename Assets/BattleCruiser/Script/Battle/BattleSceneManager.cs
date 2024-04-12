using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneManager : SceneSingleton<BattleSceneManager>
{
    int stage;
    public List<Vehicle> activeEnemyList;

    public Transform vehicleTrf;

    private void Awake()
    {
        Debug.Log($"{Instance.name} ���� �ν��Ͻ� �Ϸ�");
        stage = GameManager.Instance.selectedStage;

        float minX = 300;
        float minY = 50;
        float maxX = 1000 + stage * 1000;
        float maxY = 200 + stage * 20;

        StageData stageData = JsonDataManager.Instance.saveData.stageList[stage];//����� json�������� �������� �����͸� �����ؼ� �� ���� ��ǥ�� ����.

        for (int i = 0; i < stageData.stageShipDataList.Count; i++)
        {
            ShipData shipData = JsonDataManager.Instance.saveData.shipDataDictionary[stageData.stageShipDataList[i]];//������ �Լ� ������

            GameObject createEnemy = Instantiate(PrefabManager.Instance.enemy, new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY)), Quaternion.identity, vehicleTrf);
            createEnemy.name = shipData.className;
            Enemy enemy = createEnemy.GetComponent<Enemy>();
            enemy.Init(shipData);//������ �Լ� �ʱ�ȭ

            activeEnemyList.Add(createEnemy.GetComponent<Vehicle>());
        }
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
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime *= 0.2f;
        yield return new WaitForSecondsRealtime(5);

        Time.fixedDeltaTime *= 5;
        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(1);

        GameUI.Instance.OnResultWdw(isWin);
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
}
