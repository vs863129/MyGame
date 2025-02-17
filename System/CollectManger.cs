using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectManger : MonoBehaviour
{
    [SerializeField] GameManger GM;
    [SerializeField] int MaxObj;
    [SerializeField] int MaxBaseObj;
    [SerializeField] Transform OneSpawnPoint,TwoSpawnPoint;
    #region 預置物
    [SerializeField] GameObject[] CollectPrafab;
    [SerializeField] GameObject[] BaseCollectPrafb;
    #endregion
    public void SpawnAllCollect()
    {
        if(!CheckIsFull(CollectPrafab,out int HaveValue))
        {
            int NeedSpawnValue = MaxObj- HaveValue;
            int i = 0;
            while(i!= NeedSpawnValue)
            {
                GM.DateBase.A_Collecting.Add(SpawnCollect(CollectPrafab));
                i++;
            }
        }
        if (!CheckIsFull(BaseCollectPrafb, out int HaveBaseValue))
        {
            int NeedSpawnValue = MaxBaseObj - HaveBaseValue;
            int i = 0;
            while (i != NeedSpawnValue)
            {
                GM.DateBase.A_Collecting.Add(SpawnCollect(BaseCollectPrafb));
                i++;
            }
        }
    }
    bool CheckIsFull(GameObject[] Objs,out int NowObj)
    {
        List<GameObject> A_Collecting= GM.DateBase.A_Collecting;
        int i = 0;
        int HaveObj = 0;
        while(i < A_Collecting.Count )
        {
            int CheckHaveObj = 0;
            while (CheckHaveObj< Objs.Length)
            {
                if(A_Collecting[i].GetComponent<SpriteRenderer>().sprite== Objs[CheckHaveObj].GetComponent<SpriteRenderer>().sprite)
                {
                    HaveObj++;
                    switch (Objs)
                    {
                        case GameObject[] A when A == CollectPrafab:
                            if(HaveObj >= MaxObj)
                            {
                                NowObj = HaveObj;
                                Debug.Log("Full");
                                return true;
                            }
                            break;
                        case GameObject[] A when A == BaseCollectPrafb:
                            if (HaveObj >= MaxBaseObj)
                            {
                                NowObj = HaveObj;
                                Debug.Log("Full");
                                return true;
                            }
                            break;
                    } 
                    break;
                }
                CheckHaveObj++;
            }
            i++;
        }
        NowObj = HaveObj;
        return false;
    }
    GameObject SpawnCollect(GameObject[] Objs)
    {
        int DecideValue= Random.Range(0, Objs.Length);
        GameObject Obj = Instantiate(Objs[DecideValue]);
        Obj.transform.position = new Vector2(Random.Range(OneSpawnPoint.position.x, TwoSpawnPoint.position.x), OneSpawnPoint.transform.position.y);
        Collection Script = Obj.GetComponent<Collection>();
        RaycastHit2D hit = Physics2D.Raycast(Script.transform.position, Vector3.down, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground"));
        if(hit.collider)
        {
            //Debug.Log(Obj.transform.position.y + "|" + Script.GroundCheckPoint.position.y + "|" + hit.point.y);
            Obj.transform.position = new Vector2(Obj.transform.position.x, OverPoint(Obj.transform.position.y, Script.GroundCheckPoint.position.y, hit.point.y));
            Script.Bar.SetActive(false);
        }
        return Obj;
    }
    float OverPoint(float ObjY,float CheckPointY,float HitPointY) //使主物件的下方對齊地板
    {
        float TruePoint = CheckPointY - ObjY;
        return HitPointY - TruePoint;
    }
}
