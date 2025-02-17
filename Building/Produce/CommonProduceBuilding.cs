using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonProduceBuilding : ProduceBuilding
{
    [Header("配方")]
    [SerializeField] List<ProduceClip> AllProduce = new List<ProduceClip>();
    public override void GotAllList(out List<ProduceClip> ProduceList, out List<ProduceClip> QueueList)
    {
        ProduceList = AllProduce;
        QueueList = GotQueueList;
    }
}
