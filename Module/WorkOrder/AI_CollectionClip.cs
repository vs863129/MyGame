using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CollectionOredr", menuName = "WorkOrder/Collection")]
public class AI_CollectionClip : OrderClip
{
    public CollectionType Type;
    [SerializeField]
    Transform Target;
    [SerializeField]
    bool Got;
    float WaitTime;
    float CollectionNeedTime;
    List<GameObject> Trees=new List<GameObject>();
    public override void Wokring()
    {
        if (!Target)
        {
            if(SearchCollections(villager))
            {
                if (!Target.GetComponent<Collection>().Isenough(villager.V_Data.Stats.CollectPower))
                {
                    Target.GetComponent<Collection>().NPC = villager;
                }
            }
        }
        else 
        {
            if (villager.Move(Target.transform.position))
            {
                if (!Got)
                {
                    WaitTime = 1;//卸貨後開始倒數
                    Got = Target.GetComponent<Collection>().Villager_Complete(villager, IsComplete);
                }
                else
                {
                    StoreCollections(villager.GM.ResourceSystem);
                }
            }
        }
        if(villager.Place && IsOut)
        {
            villager.LeavePlace();
        }
        if(Got) //返回
        {
            Target = WokrBuilding;
        }
    }
    #region 判斷
    bool IsComplete //完成採集
    {
        get
        {
            if (CollectionNeedTime < 0.1f)
            {
            return true;
            }
            CollectionNeedTime -= villager.V_Data.Stats.CollectSpeed * Time.fixedDeltaTime;
            villager.Interacting(CollectionNeedTime);
            return false;
        }
    }
    bool SearchCollections(Villager villager) //判斷是否有可採集物
    {
        Target = Tree(villager.villager.transform.position);
        if (Target)
        {
            CollectionNeedTime = Target.GetComponent<Collection>().BaseTime;
            villager.SetInteractiveBar(Target.GetComponent<Collection>().InteractiveImage, CollectionNeedTime);
            return true;
        }
        else
        {
            Debug.Log("沒有採集物");
            return false;
        }
    }
    Transform Tree(Vector2 origin)
    {
        Trees.Clear();
        GameObject[] Find = GameObject.FindGameObjectsWithTag("Tree");
        for (int i=0;i< Find.Length;i++)
        {
            if(!Find[i].GetComponent<Collection>().NPC)
            {
                Trees.Add(Find[i]);
            }
        }
        if (Trees.Count != 0)
        {
            GameObject _Tree = Trees[0];
            float DistanceA = Mathf.Sqrt((Mathf.Pow(_Tree.transform.position.x - origin.x, 2) + Mathf.Pow(_Tree.transform.position.y - origin.y, 2)));
            for (int i = 1; i < Trees.Count; i++)
            {
                float DistanceB = Mathf.Sqrt((Mathf.Pow(Trees[i].transform.position.x - origin.x, 2) + Mathf.Pow(Trees[i].transform.position.y - origin.y, 2)));
                if (DistanceA > DistanceB)
                {
                    _Tree = Trees[i];
                    DistanceA = DistanceB;
                }
            }
            return _Tree.transform;
        }
        else
        {
            return null;
        }
    } //判斷最近的採集物
    #endregion
    void StoreCollections(ResourceSystem Store)
    {
        if(WaitTime<=0)
        {
            if (GotItem != null)
            {
                Store.Add(GotItem.Clip, GotItem.Value);
                GotItem = null;
                Got = false;
                Target = null;
            }
        }
        else
        {
            WaitTime -= Time.fixedDeltaTime;
        }

    }//儲藏採集物
}
