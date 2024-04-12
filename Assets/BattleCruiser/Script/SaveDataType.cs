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

    public List<WeaponData> weaponDatas;//무기 정보

    public ShipData(string className, float maxHp, float mass, float armor, float hoverPower, float strafePower, float horizontalRestorationPower, List<WeaponData> weaponDatas)
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
    public int projectiletype;//발사체 타입
    public float projectiledVelocity;//초기 발사 속도
    public float dispersion;//발사각 분포(각도)
    public float shellLifeTime;//탄 작동 시간    
    public float caliber;//구경
    public float apDmgFactor;//물리 데미지 팩터
    public float heDmgFactor;//화학 데미지 팩터
    public float turningSpeedPerSecond;//터렛 회전 속도
    public float coolDown;//발사 쿨타임
    //public Vector2 equipPosition;//무장 장착 로컬 좌표 

    public WeaponData(int projectiletype, float projectiledVelocity, float dispersion, float shellLifeTime, float caliber, float apDmgFactor, float heDmgFactor, float turningSpeedPerSecond, float coolDown)
    {
        this.projectiletype = projectiletype;
        this.projectiledVelocity = projectiledVelocity;
        this.dispersion = dispersion;
        this.shellLifeTime = shellLifeTime;
        this.caliber = caliber;
        this.apDmgFactor = apDmgFactor;
        this.heDmgFactor = heDmgFactor;
        this.turningSpeedPerSecond = turningSpeedPerSecond;
        this.coolDown = coolDown;
        //this.equipPosition = equipPosition;
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