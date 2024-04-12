using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

public class ShipData
{
    public string className;//�Ա� �̸�            
    public float maxHp;//�ִ� ü�� 
    public float mass;//���� 
    public float armor;//�尩
    public float hoverPower;//���� ����
    public float strafePower;//�¿� ����
    public float horizontalRestorationPower;//���� ������

    public List<string> weaponDatas;//���� Ű ����

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
    public string weaponName;//���� �̸�    
    public float projectiledVelocity;//�ʱ� �߻� �ӵ�
    public float dispersion;//�߻簢 ����(����)
    public float shellLifeTime;//ź �۵� �ð�    
    public float caliber;//����
    public float apDmgFactor;//���� ������ ����
    public float heDmgFactor;//ȭ�� ������ ����
    public float turningSpeedPerSecond;//�ͷ� ȸ�� �ӵ�
    public float coolDown;//�߻� ��Ÿ��
    public int multiShot;//�ѹ��� �߻��ϴ� ����
    public float multiShotDelay;//1�캸�� �߻� ������

    [JsonIgnore]
    public bool isPropulsion = false;
    [JsonIgnore]
    public bool isGuided = false;
    //public Vector2 equipPosition;//���� ���� ���� ��ǥ 

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
    public WeaponData weaponData;//������ ���� ����
    public Option1 option1;
    public Option2 option2;
    string prefix1;
    string prefix2;
    

    public enum Option1
    {
        ������,
        ������,
        ��������
    }
    public enum Option2
    {
        ö��ź,
        ����ź,
        ����ź
    }

    public CustomWeaponData(string baseWeaponKey, Option1 option1, Option2 option2)
    {        
        this.weaponData = new WeaponData(JsonDataManager.Instance.saveData.weaponDataDictionary[baseWeaponKey]);
        this.option1 = option1;
        this.option2 = option2;

        switch (option1)
        {
            case Option1.������:
                prefix1 = "������";
                weaponData.projectiledVelocity *= 1.2f;
                weaponData.coolDown *= 1.5f;
                weaponData.dispersion *= 0.5f;
                break;
            case Option1.������:
                prefix1 = "������";
                weaponData.projectiledVelocity *= 0.8f;
                weaponData.coolDown *= 0.7f;
                weaponData.dispersion *= 2f;
                break;
            case Option1.��������:
                prefix1 = "��������";
                weaponData.projectiledVelocity *= 2f;                
                weaponData.dispersion *= 3f;
                weaponData.apDmgFactor *= 0.2f;
                break;
            default:
                break;
        }

        switch (option2)
        {
            case Option2.ö��ź:
                prefix2 = "ö��ź";
                weaponData.projectiledVelocity *= 1.1f;
                weaponData.apDmgFactor *= 2f;
                weaponData.heDmgFactor = 0;
                break;
            case Option2.����ź:
                prefix2 = "����ź";
                weaponData.projectiledVelocity *= 0.9f;
                weaponData.apDmgFactor = 0;
                weaponData.heDmgFactor *= 3;
                break;
            case Option2.����ź:
                prefix2 = "����ź";
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
    public List<string> stageShipDataList;//�Լ� ��ųʸ� Ű�� ������ �ִ� ����Ʈ

    public StageData(List<string> stageShipDataList)
    {
        this.stageShipDataList = stageShipDataList;
    }
}