using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="New Tool",menuName = "Type/Items/Tool")]
public class ToolClip : ItemClip
{
    [Header("工具屬性")]
    public Sprite ToolImage;
    public Tool_Type Type;
    private void Awake()
    {
        type = ItemType.Tool;
    }
}
