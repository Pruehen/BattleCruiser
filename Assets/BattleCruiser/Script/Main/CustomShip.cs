using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomShip : MonoBehaviour
{
    public int startIndex;//해당 커스텀쉽의 시작 인덱스. 1000번부터 시작함. 슬롯이 4개일 경우 1003까지 인덱싱.
    int slotCount;//슬롯의 총 수량

    //public Dictionary<int, Slot> slotData;

    // Start is called before the first frame update
    public void Init()
    {
        slotCount = this.transform.childCount;
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Slot slot = this.transform.GetChild(i).GetComponent<Slot>();
            ItemManager.Instance.slotData.Add(startIndex + i, slot);
            slot.Init(i + startIndex);//슬롯 초기화
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
