using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New EnemyClip", menuName = "Type/Enemy")]
public class EnemyClip : ScriptableObject
{
    public string Name;//名稱
    [TextArea(8,10)]
    public string Description;//敘述
}
