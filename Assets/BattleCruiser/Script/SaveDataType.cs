using System.Collections;
using System.Collections.Generic;

public class ShipData
{
    public string className;//�Ա� �̸�            
    public float maxHp;//�ִ� ü�� 
    public float mass;//���� 
    public float armor;//�尩
    public float hoverPower;//���� ����
    public float strafePower;//�¿� ����
    public float horizontalRestorationPower;//���� ������

    public List<WeaponData> weaponDatas;//���� ����

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
    public int projectiletype;//�߻�ü Ÿ��
    public float projectiledVelocity;//�ʱ� �߻� �ӵ�
    public float dispersion;//�߻簢 ����(����)
    public float shellLifeTime;//ź �۵� �ð�    
    public float caliber;//����
    public float apDmgFactor;//���� ������ ����
    public float heDmgFactor;//ȭ�� ������ ����
    public float turningSpeedPerSecond;//�ͷ� ȸ�� �ӵ�
    public float coolDown;//�߻� ��Ÿ��
    //public Vector2 equipPosition;//���� ���� ���� ��ǥ 

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
    public List<string> stageShipDataList;//�Լ� ��ųʸ� Ű�� ������ �ִ� ����Ʈ

    public StageData(List<string> stageShipDataList)
    {
        this.stageShipDataList = stageShipDataList;
    }
}