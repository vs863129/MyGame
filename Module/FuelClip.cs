using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Fuel", menuName = "Type/Smithy/Fuel")]
public class FuelClip : ScriptableObject
{
    public ItemClip Item;
    public float CanBurnTime;
    public int MaxQuantity;
    public List<Cost_Resource> Cost;
}
