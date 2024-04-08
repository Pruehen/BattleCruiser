using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipWeaponData : MonoBehaviour
{
    public List<WeaponData> weaponDatas = new List<WeaponData>();    
}

public enum ProjectileType
{
    Shell, Missile
}

public class WeaponData
{
    public ProjectileType projectiletype = ProjectileType.Shell;//발사체 타입
    public float projectiledVelocity = 100;//초기 발사 속도
    public float dispersion = 3;//발사각 분포(각도)
    public float shellLifeTime = 6;//탄 작동 시간    
    public float caliber = 100f;//구경
    public float apDmgFactor = 1;//물리 데미지 팩터
    public float heDmgFactor = 1;//화학 데미지 팩터
    public float turningSpeedPerSecond = 90;//터렛 회전 속도
    public float coolDown = 0.1f;//발사 쿨타임
    public Vector2 equipPosition = Vector2.zero;//무장 장착 로컬 좌표 

    public WeaponData(ProjectileType projectiletype, float projectiledVelocity, float dispersion, float shellLifeTime, float caliber, float apDmgFactor, float heDmgFactor, float turningSpeedPerSecond, float coolDown, Vector2 equipPosition)
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
        this.equipPosition = equipPosition;
    }
}