using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : GlobalSingleton<GameManager>
{
    public int selectedStage { get; private set; }
    public PlayerShipData playerShipData { get; private set; }
    Setting setting;
    public Setting Setting
    {
        get
        {
            if (setting == null)
                setting = JsonDataManager.Instance.saveData.userData.setting;
            return setting;
        }
        set
        {
            setting = value;
        }
    }


    private void Awake()
    {
        Debug.Log($"{Instance.name} 인스턴싱 완료");
        playerShipData = new PlayerShipData();        
    }
    public void SetStageNum(int num)
    {
        selectedStage = num;
        Debug.Log($"{selectedStage} 스테이지로 설정");
    }
    public void SetShipData(int index)//함선 세팅 창을 닫을 때 1회 호출됨.
    {
        playerShipData.shipIndex = index;//함선 코드        
        playerShipData.shipData = JsonDataManager.Instance.saveData.shipDataDictionary[index.ShipKey()];//함선의 기본 데이터        
        playerShipData.weaponSpriteIndexs.Clear();//기존 장착 무기 코드 제거
        playerShipData.weaponDatas.Clear();//기존 무기 데이터 제거


        int slotSize = playerShipData.shipData.weaponDatas.Count;//무장의 최대 수량
        int startWeaponIndex = (index + 1) * 1000;//해당 함선에 장착된 무기 딕셔너리의 시작 키
        for (int i = startWeaponIndex; i < startWeaponIndex + slotSize; i++)
        {
            Slot slot = ItemManager.Instance.slotData[i];//슬롯 캐싱

            if (slot.slotWeaponData != null)//무기가 장착된 경우
            {
                playerShipData.weaponSpriteIndexs.Add(slot.slotWeaponData.weaponData.sptiteIndex);//무기 스프라이트 키 추가
                playerShipData.weaponDatas.Add(slot.slotWeaponData.weaponData);//무기 데이터 추가
            }
            else//무기가 장착되지 않은 경우
            {
                playerShipData.weaponSpriteIndexs.Add(-1);//무기 키 -1 추가 (무기 없음을 나타냄)
                playerShipData.weaponDatas.Add(null);//null값 추가
            }
        }

        //Debug.Log($"현재 사용 중인 무기 수량 : {playerShipData.weaponIndexs.Count}, {playerShipData.weaponDatas.Count}");
    }

    public class PlayerShipData
    {
        public int shipIndex;//함선의 고유 코드
        public ShipData shipData;//함선 데이터
        public List<int> weaponSpriteIndexs;//사용할 무기들의 스프라이트 코드
        public List<WeaponData> weaponDatas;//사용할 무기 데이터

        public PlayerShipData()
        {
            weaponSpriteIndexs = new List<int>();
            weaponDatas = new List<WeaponData>();
        }
    }
}
