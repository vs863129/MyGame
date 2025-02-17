using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New MeltProduce", menuName = "Type/MeltProduce")]
public class MeltProduceClip : ProduceClip
{
    private void Awake()
    {
        Type = ProduceType.Melt;
    }
    public override bool Making(float Power)
    {
        Power = 1;
        return base.Making(Power);
    }
}
