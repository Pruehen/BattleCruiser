using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomShip : MonoBehaviour
{
    public int startIndex;//�ش� Ŀ���ҽ��� ���� �ε���. 1000������ ������. ������ 4���� ��� 1003���� �ε���.
    int slotCount;//������ �� ����

    //public Dictionary<int, Slot> slotData;

    // Start is called before the first frame update
    public void Init()
    {
        slotCount = this.transform.childCount;
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Slot slot = this.transform.GetChild(i).GetComponent<Slot>();
            ItemManager.Instance.slotData.Add(startIndex + i, slot);
            slot.Init(i + startIndex);//���� �ʱ�ȭ
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
