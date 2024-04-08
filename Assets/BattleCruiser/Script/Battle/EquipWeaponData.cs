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
    public ProjectileType projectiletype = ProjectileType.Shell;//�߻�ü Ÿ��
    public float projectiledVelocity = 100;//�ʱ� �߻� �ӵ�
    public float dispersion = 3;//�߻簢 ����(����)
    public float shellLifeTime = 6;//ź �۵� �ð�    
    public float caliber = 100f;//����
    public float apDmgFactor = 1;//���� ������ ����
    public float heDmgFactor = 1;//ȭ�� ������ ����
    public float turningSpeedPerSecond = 90;//�ͷ� ȸ�� �ӵ�
    public float coolDown = 0.1f;//�߻� ��Ÿ��
    public Vector2 equipPosition = Vector2.zero;//���� ���� ���� ��ǥ 

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