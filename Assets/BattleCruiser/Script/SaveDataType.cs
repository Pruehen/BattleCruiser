using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

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

    [JsonIgnore]
    public bool isPropulsion = false;
    [JsonIgnore]
    public bool isGuided = false;
    //public Vector2 equipPosition;//무장 장착 로컬 좌표 

    public WeaponData(string name, float projectiledVelocity, float dispersion, float shellLifeTime, float caliber, float apDmgFactor, float heDmgFactor, float turningSpeedPerSecond, float coolDown, int multiShot, float multiShotDelay)
    {
        this.weaponName = name;        
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
        //this.equipPosition = equipPosition;
    }

    public WeaponData(WeaponData weaponData)
    {
        this.weaponName = weaponData.weaponName;        
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
    }
}

public class CustomWeaponData
{     
    public WeaponData weaponData;//저장할 무기 스펙
    public Option1 option1;
    public Option2 option2;
    string prefix1;
    string prefix2;
    

    public enum Option1
    {
        장포신,
        단포신,
        로켓추진
    }
    public enum Option2
    {
        철갑탄,
        고폭탄,
        유도탄
    }

    public CustomWeaponData(string baseWeaponKey, Option1 option1, Option2 option2)
    {        
        this.weaponData = new WeaponData(JsonDataManager.Instance.saveData.weaponDataDictionary[baseWeaponKey]);
        this.option1 = option1;
        this.option2 = option2;

        switch (option1)
        {
            case Option1.장포신:
                prefix1 = "장포신";
                weaponData.projectiledVelocity *= 1.2f;
                weaponData.coolDown *= 1.5f;
                weaponData.dispersion *= 0.5f;
                break;
            case Option1.단포신:
                prefix1 = "단포신";
                weaponData.projectiledVelocity *= 0.8f;
                weaponData.coolDown *= 0.7f;
                weaponData.dispersion *= 2f;
                break;
            case Option1.로켓추진:
                prefix1 = "로켓추진";
                weaponData.projectiledVelocity *= 2f;                
                weaponData.dispersion *= 3f;
                weaponData.apDmgFactor *= 0.2f;
                break;
            default:
                break;
        }

        switch (option2)
        {
            case Option2.철갑탄:
                prefix2 = "철갑탄";
                weaponData.projectiledVelocity *= 1.1f;
                weaponData.apDmgFactor *= 2f;
                weaponData.heDmgFactor = 0;
                break;
            case Option2.고폭탄:
                prefix2 = "고폭탄";
                weaponData.projectiledVelocity *= 0.9f;
                weaponData.apDmgFactor = 0;
                weaponData.heDmgFactor *= 3;
                break;
            case Option2.유도탄:
                prefix2 = "유도탄";
                weaponData.projectiledVelocity *= 0.8f;
                weaponData.apDmgFactor *= 0.5f;
                weaponData.heDmgFactor *= 0.5f;
                break;
            default:
                break;
        }
        weaponData.weaponName = string.Format($"{prefix1} {prefix2} {weaponData.weaponName}");
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