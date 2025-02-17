using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

[CreateAssetMenu(fileName = "New item", menuName = "Type/Items/Item")]
public class ItemClip : ScriptableObject
{
    public string Name;
    [SerializeField] private float _W;
    [SerializeField] private bool _IsStaceed;
    public Sprite Icon;

    #region 外部獲取

    public float W
    {
        get
        {
            return _W;
        }
    }
    public bool IsStaceed
    {
        get { return _IsStaceed; }
    }
    #endregion
    #region 武器
    [HideInInspector] public Weapon_Type WeaponType;
    [HideInInspector] public AttackType DamageType;
    [HideInInspector] public Equipment_Type EquipmentType;
    #endregion
    #region 通用屬性

    [HideInInspector]public List<AttributeBuff> Buffs=new List<AttributeBuff>();

    #endregion
    public ItemType type;//物品類型
    public string Description;//敘述
    [Tooltip("使用提示")]
    [SerializeField] UseItemTips Hints;
    string _TIP
    {
        get
        {
            return Hints.ToString();
        }
    }
    public string Tip
    { 
        get
        {
            if(Hints== UseItemTips.None)
            {
                return "";
            }
            return "UI," + _TIP;
        }
    }
    #region Editor
#if UNITY_EDITOR
    [CustomEditor(typeof(ItemClip))]
    public class ItemClipEditor : Editor
    {
        SerializedProperty EditorAttribute;
        ReorderableList List;
        ItemClip itemClip;
        private void OnEnable()
        {
            EditorAttribute = serializedObject.FindProperty("Buffs");
            List = new ReorderableList(serializedObject, EditorAttribute, true, true, true, true);

            List.drawElementCallback = DrawListItems;
            List.drawHeaderCallback = DrawHeader;

        }
        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = List.serializedProperty.GetArrayElementAtIndex(index);
            {
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("Type"),
                    GUIContent.none
                    );
                EditorGUI.PropertyField(
                    new Rect(rect.x+120, rect.y, 100, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("Value"),
                    GUIContent.none
                    );
            }
        }
        void DrawHeader(Rect rect)
        {
            string Name = "附加數值";
            EditorGUI.LabelField(rect, Name);

        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            itemClip = (ItemClip)target;
            if(itemClip.type== ItemType.Equipment)
            {
                EditorGUILayout.LabelField("屬性");
                EditorGUILayout.Space();
                itemClip.EquipmentType = (Equipment_Type)EditorGUILayout.EnumPopup("裝備類型", itemClip.EquipmentType);
                switch (itemClip.EquipmentType)
                {
                    case Equipment_Type.Weapon:

                        itemClip.WeaponType = (Weapon_Type)EditorGUILayout.EnumPopup("手持方式", itemClip.WeaponType);
                        itemClip.DamageType = (AttackType)EditorGUILayout.EnumPopup("攻擊方式", itemClip.DamageType);
                        break;
                    case Equipment_Type.Chest:
                        Armor();
                        break;
                    case Equipment_Type.Foots:
                        Armor();
                        break;
                    case Equipment_Type.Head:
                        Armor();
                        break;
                    case Equipment_Type.Accessories:
                        break;
                }
                EditorGUILayout.Space(20);
                serializedObject.Update();
                List.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
            }

            //測試用
            //EditorGUILayout.Space(); 
            //EditorGUILayout.LabelField("Name");
        }
        public void Armor()
        {

        }
    }
#endif
    #endregion
}
[System.Serializable]
public class AttributeBuff
{
    public Attribute Type;
    public int Value;
}
[System.Serializable]
public class Item//物品
{
    public ItemClip Clip;//物品
   [SerializeField] int _Value;//數量
    public void AddValue(ItemClip clip, int value)
    {
        Clip = clip;
        _Value += value;
    }

    public void AddValue(int value)
    {
        _Value += value;
    }
    public void CostValue(int value)
    {
        _Value -= value;
    }
    public void GotPassValue(Item FromWherInHere,int PassValue) //將A轉移至本地
    {
        if(Clip==null)
        {
            Clip = FromWherInHere.Clip;
        }
        if (FromWherInHere.Value>0)
        {
            if (FromWherInHere .Value> PassValue)
            {
                FromWherInHere.CostValue(PassValue);
            }
            else
            {
                PassValue = FromWherInHere.Value;
                FromWherInHere.CostValue(FromWherInHere.Value);
                FromWherInHere.Clear();
            }
            AddValue(PassValue);
        }
    }
    public void Clear()
    {
        _Value = 0;
        Clip = null;
    }
    public int Value
    {
        get { return _Value; }
    }
    public bool IsNone()
    {
        if(_Value==0)
        {
            return true;
        }
        return false;
    }
}
