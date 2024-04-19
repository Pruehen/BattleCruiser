using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

public class ShipData
{
    public string className;//함급 이름            
    public float maxHp;//최대 체력 
    public float mass;//질량 
    public float armor;//장갑
    public float hoverPower;//상하 가속
    public float strafePower;//좌우 가속
    public float horizontalRestorationPower;//수평 복원력

    public List<string> weaponDatas;//무기 키 정보

    public ShipData(string className, float maxHp, float mass, float armor, float hoverPower, float strafePower, float horizontalRestorationPower, List<string> weaponDatas)
    {
        this.className = className;
        this.maxHp = maxHp;
        this.mass = mass;
        this.armor = armor;
        this.hoverPower = hoverPower;
        this.strafePower = strafePower;
        this.horizontalRestorationPower = horizontalRestorationPower;
        this.weaponDatas = weaponDatas;
    }
}

public class WeaponData
{
    public string weaponName;//무기 이름    
    public string weaponKey;//무기 코드
    public int sptiteIndex;//무기의 스프라이트 인덱스
    public float projectiledVelocity;//초기 발사 속도
    public float dispersion;//발사각 분포(각도)
    public float shellLifeTime;//탄 작동 시간    
    public float caliber;//구경
    public float apDmgFactor;//물리 데미지 팩터
    public float heDmgFactor;//화학 데미지 팩터
    public float turningSpeedPerSecond;//터렛 회전 속도
    public float coolDown;//발사 쿨타임
    public int multiShot;//한번에 발사하는 수량
    public float multiShotDelay;//1살보시 발사 딜레이
    public float mass;//질량(kg)
    
    public bool isPropulsion;    
    public bool isGuided;

    [JsonConstructor]
    public WeaponData(string weaponName, string weaponKey, int sptiteIndex, float projectiledVelocity, float dispersion, float shellLifeTime, float caliber, float apDmgFactor, 
        float heDmgFactor, float turningSpeedPerSecond, float coolDown, int multiShot, float multiShotDelay, float mass, bool isPropulsion, bool isGuided)
    {
        this.weaponName = weaponName;
        this.weaponKey = weaponKey;
        this.sptiteIndex = sptiteIndex;
        this.projectiledVelocity = projectiledVelocity;
        this.dispersion = dispersion;
        this.shellLifeTime = shellLifeTime;
        this.caliber = caliber;
        this.apDmgFactor = apDmgFactor;
        this.heDmgFactor = heDmgFactor;
        this.turningSpeedPerSecond = turningSpeedPerSecond;
        this.coolDown = coolDown;
        this.multiShot = multiShot;
        this.multiShotDelay = multiShotDelay;
        this.mass = mass;
        this.isPropulsion = isPropulsion;
        this.isGuided = isGuided;
    }

    public WeaponData(WeaponData weaponData)
    {
        this.weaponName = weaponData.weaponName;
        this.weaponKey = weaponData.weaponKey;
        this.sptiteIndex = weaponData.sptiteIndex;
        this.projectiledVelocity = weaponData.projectiledVelocity;
        this.dispersion = weaponData.dispersion;
        this.shellLifeTime = weaponData.shellLifeTime;
        this.caliber = weaponData.caliber;
        this.apDmgFactor = weaponData.apDmgFactor;
        this.heDmgFactor = weaponData.heDmgFactor;
        this.turningSpeedPerSecond = weaponData.turningSpeedPerSecond;
        this.coolDown = weaponData.coolDown;
        this.multiShot = weaponData.multiShot;
        this.multiShotDelay = weaponData.multiShotDelay;
        this.mass = weaponData.mass;

        isPropulsion = weaponData.isPropulsion;
        isGuided = weaponData.isGuided;
    }
}

public class CustomWeaponData
{     
    public WeaponData weaponData;//저장할 무기 스펙
    //public string rarity { get; private set; }
    //public string baseWeaponKey { get; private set; }    
    public int rarityNum { get; private set; }//무기 희귀도. 0~7    

    public string[] GetData()
    {
        string[] data = new string[9];

        string rarityColorCode;

        switch (rarityNum)
        {
            case 0:
                rarityColorCode = RarityColor.tech0_Code;
                break;
            case 1:
                rarityColorCode = RarityColor.tech1_Code;
                break;
            case 2:
                rarityColorCode = RarityColor.tech2_Code;
                break;
            case 3:
                rarityColorCode = RarityColor.tech3_Code;
                break;
            case 4:
                rarityColorCode = RarityColor.tech4_Code;
                break;
            case 5:
                rarityColorCode = RarityColor.tech5_Code;
                break;
            case 6:
                rarityColorCode = RarityColor.tech6_Code;
                break;
            case 7: 
                rarityColorCode = RarityColor.tech7_Code;
                break;
            default:
                rarityColorCode = RarityColor.tech0_Code;
                break;
        }

        data[0] = $"<color={rarityColorCode}>{rarityNum + 1}티어 아이템</color>";
        data[1] = $"이름   : {weaponData.weaponName}";
        data[2] = $"탄속   : {(weaponData.projectiledVelocity * 10):N0}m/s";
        data[3] = $"분산도 : {weaponData.dispersion:N2}도";
        data[4] = $"구경   : {weaponData.caliber:N0}mm";
        data[5] = $"물리 피해 : {weaponData.apDmgFactor * weaponData.projectiledVelocity * weaponData.projectiledVelocity * 0.00005f * weaponData.caliber * weaponData.caliber:N0}";
        data[6] = $"화학 피해 : {weaponData.caliber * weaponData.caliber * weaponData.caliber * weaponData.heDmgFactor * 0.001f:N0}";
        data[7] = $"포신 회전 : {weaponData.turningSpeedPerSecond:N0}도/s";
        data[8] = $"RPM   : {weaponData.multiShot * 60 / weaponData.coolDown:N0}";

        return data;
    }

    public CustomWeaponData(string baseWeaponKey, int rarityNum)
    {
        //this.baseWeaponKey = baseWeaponKey;
        this.weaponData = new WeaponData(JsonDataManager.Instance.saveData.weaponDataDictionary[baseWeaponKey]);
        this.rarityNum = rarityNum;

        //switch (option1)
        //{
        //    case 0:
        //        prefix1 = "";
        //        break;
        //    case 1:
        //        prefix1 = "장포신";
        //        weaponData.projectiledVelocity *= 1.2f;
        //        weaponData.coolDown *= 1.5f;
        //        weaponData.dispersion *= 0.5f;
        //        weaponData.mass *= 1.5f;
        //        break;
        //    case 2:
        //        prefix1 = "단포신";
        //        weaponData.projectiledVelocity *= 0.8f;
        //        weaponData.coolDown *= 0.7f;
        //        weaponData.dispersion *= 2f;
        //        weaponData.mass *= 0.7f;
        //        break;
        //    case 3:
        //        prefix1 = "로켓추진";
        //        weaponData.projectiledVelocity *= 2f;
        //        weaponData.dispersion *= 3f;
        //        weaponData.apDmgFactor *= 0.2f;
        //        weaponData.mass *= 0.5f;
        //        weaponData.isPropulsion = true;
        //        break;
        //    default:
        //        break;
        //}

        //switch (option2)
        //{
        //    case 0:
        //        prefix2 = "";
        //        break;
        //    case 1:
        //        prefix2 = "철갑탄";
        //        weaponData.projectiledVelocity *= 1.1f;
        //        weaponData.apDmgFactor *= 2f;
        //        weaponData.heDmgFactor = 0;
        //        break;
        //    case 2:
        //        prefix2 = "고폭탄";
        //        weaponData.projectiledVelocity *= 0.9f;
        //        weaponData.apDmgFactor = 0;
        //        weaponData.heDmgFactor *= 3;
        //        break;
        //    case 3:
        //        prefix2 = "유도탄";
        //        weaponData.projectiledVelocity *= 0.8f;
        //        weaponData.apDmgFactor *= 0.5f;
        //        weaponData.heDmgFactor *= 0.5f;
        //        weaponData.isGuided = true;
        //        break;
        //    default:
        //        break;
        //}
        //weaponData.weaponName = string.Format($"{prefix1} {prefix2} {weaponData.weaponName}");

        switch (rarityNum)
        {
            case 0:                
                break;
            case 1:                
                weaponData.apDmgFactor *= 1.3f;
                weaponData.heDmgFactor *= 1.3f;                
                break;
            case 2:                
                weaponData.apDmgFactor *= 1.6f;
                weaponData.heDmgFactor *= 2f;
                weaponData.dispersion *= 0.9f;
                weaponData.coolDown *= 0.8f;
                weaponData.multiShotDelay *= 0.8f;
                break;
            case 3:                
                weaponData.apDmgFactor *= 2f;
                weaponData.heDmgFactor *= 3f;
                weaponData.dispersion *= 0.8f;
                weaponData.coolDown *= 0.7f;
                weaponData.multiShotDelay *= 0.7f;
                break;
            case 4:
                weaponData.apDmgFactor *= 2.5f;
                weaponData.heDmgFactor *= 4f;
                weaponData.dispersion *= 0.6f;
                weaponData.coolDown *= 0.6f;
                weaponData.multiShotDelay *= 0.6f;
                break;
            case 5:
                weaponData.apDmgFactor *= 3f;
                weaponData.heDmgFactor *= 5f;
                weaponData.dispersion *= 0.4f;
                weaponData.coolDown *= 0.5f;
                weaponData.multiShotDelay *= 0.5f;
                break;
            case 6:
                weaponData.apDmgFactor *= 3.5f;
                weaponData.heDmgFactor *= 6f;
                weaponData.dispersion *= 0.25f;
                weaponData.coolDown *= 0.4f;
                weaponData.multiShotDelay *= 0.4f;
                break;
            case 7:
                weaponData.apDmgFactor *= 3.5f;
                weaponData.heDmgFactor *= 6f;
                weaponData.dispersion *= 0.2f;
                weaponData.coolDown *= 0.2f;
                weaponData.multiShotDelay *= 0.2f;
                break;
            default:                
                break;
        }

        //weaponData.projectiledVelocity *= randomGain;
        //weaponData.coolDown *= randomGain;
        //weaponData.dispersion *= randomGain;
        //weaponData.apDmgFactor *= randomGain;
        //weaponData.heDmgFactor *= randomGain;
    }

    [JsonConstructor]
    public CustomWeaponData(WeaponData weaponData, int rarityNum)
    {
        this.weaponData = weaponData;
        //this.rarity = rarity;
        this.rarityNum = rarityNum;
        //this.baseWeaponKey = baseWeaponKey;        
    }
}

public class StageData
{
    public List<string> stageShipDataList;//함선 딕셔너리 키를 가지고 있는 리스트

    public StageData(List<string> stageShipDataList)
    {
        this.stageShipDataList = stageShipDataList;
    }
}

public class UserData
{
    public Dictionary<int, CustomWeaponData> customWeaponDatas;
    public Setting setting;
    public int nanobot { get; private set; }
    public int level { get; private set; }

    public UserData()
    {
        this.customWeaponDatas = new Dictionary<int, CustomWeaponData>();
        setting = new Setting(0, 0, 0, 0, 0, 0);
        nanobot = 0;
        level = 0;
    }

    [JsonConstructor]
    public UserData(Dictionary<int, CustomWeaponData> customWeaponDatas, Setting setting, int nanobot, int level)
    {
        this.customWeaponDatas = customWeaponDatas;
        this.setting = setting;
        this.nanobot = nanobot;
        this.level = level;
    }

    public void nanobotUp(int value)
    {
        nanobot += value;
    }
    public void levelUp(int value)
    {
        level += value;
    }
    public bool CustomWeaponDataAdd(CustomWeaponData data)//인벤토리에 데이터 추가
    {
        for (int i = 0; i < 128; i++)
        {
            if(customWeaponDatas.ContainsKey(i) == false)//키가 없을 경우
            {
                customWeaponDatas.Add(i, data);                
                return true;
            }
        }
        return false;
    }
}

public class Setting
{
    public float bgm;
    public float sfx;
    public float radarRange;
    public float wheelSens;
    public float camSpeed;
    public float camRange;

    [JsonConstructor]
    public Setting(float bgm, float sfx, float radarRange, float wheelSens, float camSpeed, float camRange)
    {
        this.bgm = bgm;
        this.sfx = sfx;
        this.radarRange = radarRange;
        this.wheelSens = wheelSens;
        this.camSpeed = camSpeed;
        this.camRange = camRange;
    }
    //public Setting() { }
}