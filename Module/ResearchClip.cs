using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New ResearchClip", menuName = "Type/Research")]
public class ResearchClip : ScriptableObject
{
    public Sprite ICON;
    public Researchtype type; //類別
    public string Name;//科技名稱
    [TextArea(8, 10)]
    public string Description; //敘述
    public float Point;//已研究點數
    public float MaxPoint //需要的科研點數
    {
        get
        {
            if (level == MaxLevel) 
            {
                return SetMaxPoint[MaxLevel - 1];
            }
            else
            {
                return SetMaxPoint[level];
            }

        }
    }
    [Tooltip("同步最大等級")]
    [SerializeField] float[] SetMaxPoint;
    public int level; //目前等級
    [Tooltip("建議優先調整BUFF數量")]
    public int MaxLevel;//最大等級
    [SerializeField] ResearchCondition[] Condition; //前置研究
    [Header("增益效果")]
    public UnlockBuilding UnlockBuilding;
    [Tooltip("研究完成可影響的數值數量")]
    public ResearchBuffs[] Buff; //數值影響
    public ResearchCondition GotCondition(int ID)
    {
        return Condition[ID];
    }

    public void OriginalSetting()
    {
        Point = 0;
        level = 0;
    }
    public bool CheckCondition
    {
        get
        {
            if (Condition.Length != 0)
            {
                return true;
            }
            return false;
        }
    }
    public int GotConditionLength
    {
        get { return Condition.Length; }
    }
}
[System.Serializable]
public class UnlockBuilding
{
    [SerializeField] BuildingClip[] A_UnlockBuilding;
    public void GenerateBuildingButton(BuilndingManger BM)
    {
        foreach(BuildingClip clip in A_UnlockBuilding)
        {
            BM.UISetting.CanBuilding.Add(clip);
            BM.UISetting.AddBuildingButton(BM, clip);
        }
    }
}
[System.Serializable]
public class ResearchBuffs
{
    public Attribute Type;
    [Tooltip("請同步最大等級數量")]
    [SerializeField]List<int> BonusValue=new List<int>();
    //public void GenerateNewBouns(int MaxLevel)
    //{
    //    if(BonusValue.Count!=MaxLevel)
    //    {
    //        if(BonusValue.Count<MaxLevel)
    //        {
    //            BonusValue.Add(0);
    //        }
    //        else
    //        {
    //            int i = BonusValue.Count-1;
    //            while(i> MaxLevel-1)
    //            {
    //                BonusValue.RemoveAt(i);
    //                i--;
    //            }
    //        }
    //    }
    //}
    public int Value(int Level)
    {
        return BonusValue[Level-1];
    }
}
[System.Serializable]
public class ResearchCondition
{
    public ResearchClip R_clip;
    public int level;
}

