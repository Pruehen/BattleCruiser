using Newtonsoft.Json;
using System;
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

    [JsonConstructor]
    public WeaponData(string weaponName, float projectiledVelocity, float dispersion, float shellLifeTime, float caliber, float apDmgFactor, 
        float heDmgFactor, float turningSpeedPerSecond, float coolDown, int multiShot, float multiShotDelay)
    {
        this.weaponName = weaponName;        
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
        isPropulsion = false;
        isGuided = false;
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
        isPropulsion = false;
        isGuided = false;
    }
}

public class CustomWeaponData
{     
    public WeaponData weaponData;//������ ���� ����
    public string rarity { get; private set; }
    public string baseWeaponKey { get; private set; }    
    public int rarityNum { get; private set; }//���� ��͵�. 0~3    

    int option1;
    int option2;
    string prefix1;
    string prefix2;


    public string[] GetData()
    {
        string[] data = new string[9];

        string rarityColorCode;

        switch (rarityNum)
        {
            case 0:
                rarityColorCode = RarityColor.commonCode;
                break;
            case 1:
                rarityColorCode = RarityColor.rareCode;
                break;
            case 2:
                rarityColorCode = RarityColor.epicCode;
                break;
            case 3:
                rarityColorCode = RarityColor.legendaryCode;
                break;
            default:
                rarityColorCode = $"FFFFFF";
                break;
        }

        data[0] = $"<color={rarityColorCode}>{rarity} ������</color>";
        data[1] = $"�̸�   : {weaponData.weaponName}";
        data[2] = $"ź��   : {weaponData.projectiledVelocity:N0}m/s";
        data[3] = $"�л굵 : {weaponData.dispersion:N2}��";
        data[4] = $"����   : {weaponData.caliber:N0}mm";
        data[5] = $"���� ���� : {weaponData.apDmgFactor * weaponData.projectiledVelocity * weaponData.projectiledVelocity * 0.00005f * weaponData.caliber * weaponData.caliber:N0}";
        data[6] = $"ȭ�� ���� : {weaponData.caliber * weaponData.caliber * weaponData.caliber * weaponData.heDmgFactor * 0.001f:N0}";
        data[7] = $"���� ȸ�� : {weaponData.turningSpeedPerSecond:N0}��/s";
        data[8] = $"RPM   : {weaponData.multiShot * 60 / weaponData.coolDown:N0}";

        return data;
    }

    public CustomWeaponData(string baseWeaponKey, int option1, int option2, int rarityNum, float randomGain)
    {
        this.baseWeaponKey = baseWeaponKey;
        this.weaponData = new WeaponData(JsonDataManager.Instance.saveData.weaponDataDictionary[this.baseWeaponKey]);
        this.option1 = option1;
        this.option2 = option2;
        this.rarityNum = rarityNum;

        switch (option1)
        {
            case 0:
                prefix1 = "";
                break;
            case 1:
                prefix1 = "������";
                weaponData.projectiledVelocity *= 1.2f;
                weaponData.coolDown *= 1.5f;
                weaponData.dispersion *= 0.5f;
                break;
            case 2:
                prefix1 = "������";
                weaponData.projectiledVelocity *= 0.8f;
                weaponData.coolDown *= 0.7f;
                weaponData.dispersion *= 2f;
                break;
            case 3:
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
            case 0:
                prefix2 = "";
                break;
            case 1:
                prefix2 = "ö��ź";
                weaponData.projectiledVelocity *= 1.1f;
                weaponData.apDmgFactor *= 2f;
                weaponData.heDmgFactor = 0;
                break;
            case 2:
                prefix2 = "����ź";
                weaponData.projectiledVelocity *= 0.9f;
                weaponData.apDmgFactor = 0;
                weaponData.heDmgFactor *= 3;
                break;
            case 3:
                prefix2 = "����ź";
                weaponData.projectiledVelocity *= 0.8f;
                weaponData.apDmgFactor *= 0.5f;
                weaponData.heDmgFactor *= 0.5f;
                break;
            default:
                break;
        }
        weaponData.weaponName = string.Format($"{prefix1} {prefix2} {weaponData.weaponName}");

        switch (rarityNum)
        {
            case 0:
                rarity = "�Ϲ�";
                break;
            case 1:
                rarity = "����";
                weaponData.apDmgFactor *= 1.1f;
                weaponData.heDmgFactor *= 1.1f;
                break;
            case 2:
                rarity = "����";
                weaponData.apDmgFactor *= 1.1f;
                weaponData.heDmgFactor *= 1.3f;
                weaponData.projectiledVelocity *= 1.1f;
                weaponData.coolDown *= 0.9f;
                break;
            case 3:
                rarity = "��������";
                weaponData.apDmgFactor *= 1.3f;
                weaponData.heDmgFactor *= 2f;
                weaponData.projectiledVelocity *= 1.2f;
                weaponData.coolDown *= 0.8f;
                break;            
            default:
                rarity = "����";
                break;
        }

        weaponData.projectiledVelocity *= randomGain;
        weaponData.coolDown *= randomGain;
        weaponData.dispersion *= randomGain;
        weaponData.apDmgFactor *= randomGain;
        weaponData.heDmgFactor *= randomGain;
    }

    [JsonConstructor]
    public CustomWeaponData(WeaponData weaponData, string rarity, int rarityNum, string baseWeaponKey)
    {
        this.weaponData = weaponData;
        this.rarity = rarity;
        this.rarityNum = rarityNum;
        this.baseWeaponKey = baseWeaponKey;
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

public class UserData
{
    public Dictionary<int, CustomWeaponData> customWeaponDatas;
    public int nanobot { get; private set; }
    public int level { get; private set; }

    public UserData()
    {
        this.customWeaponDatas = new Dictionary<int, CustomWeaponData>();
        nanobot = 0;
        level = 0;
    }
    public UserData(Dictionary<int, CustomWeaponData> customWeaponDatas, int nanobot, int level)
    {
        this.customWeaponDatas = customWeaponDatas;
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
    public bool CustomWeaponDataAdd(CustomWeaponData data)//�κ��丮�� ������ �߰�
    {
        for (int i = 0; i < 128; i++)
        {
            if(customWeaponDatas.ContainsKey(i) == false)//Ű�� ���� ���
            {
                customWeaponDatas.Add(i, data);                
                return true;
            }
        }
        return false;
    }
}