using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourceUI : MonoBehaviour
{
    private ItemClip _Clip;//資源
    public ItemClip Clip
    {
        get
        {
            return _Clip;
        }
    }
    private int Value;//數量
    [SerializeField]public TextMeshProUGUI Show;
    [SerializeField]private Image Image;
    public Color BaseColor;
    #region 需求使用
    public bool Isenough 
    {
        get
        {
            if(Show.color== BaseColor)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    #endregion
    public void Awake()
    {
        BaseColor = Show.color;
    }
    public bool CheckIsEnough(ResourceSystem RS,InventorySystem PlayerInv)//檢查倉庫和玩家背包是否有足夠的資源
    {
        List<ResourceUI> Store = RS.Store;
        int HaveValue = 0;
        int Inv = 0;
        while (Inv < PlayerInv.SlotsUI.Count)
        {
            if (PlayerInv.SlotsUI[Inv].Item.Clip!=null)
            {
                if(PlayerInv.SlotsUI[Inv].Item.Clip==Clip)
                {
                    HaveValue += PlayerInv.SlotsUI[Inv].Item.Value;
                }
            }
            Inv++;
        }
        if(Store.Count!=0)
        {
            int i = 0;
            while(i<Store.Count)
            {
                if(Store[i].Clip==Clip)
                {
                    HaveValue += Store[i].GotValue;
                    break;
                }
                i++;
            }
        }
        return CheckIsEnough(HaveValue);
    }

    public bool CheckIsEnough(List<ResourceUI> Store) //只檢查倉庫是否有足夠的資源
    {
        bool IsEnough = false;
        if(Store.Count!=0)
        {
            for (int i = 0; i < Store.Count; i++)
            {
                if (Store[i].Clip == Clip)
                {
                    if(CheckIsEnough(Store[i].GotValue))
                    {
                        IsEnough = true;
                        break;
                    }
                }
                else if(i==Store.Count-1&& Store[i].Clip!=Clip)
                {
                    CheckIsEnough(0);
                }
            }
        }
        else
        {
            CheckIsEnough(0);
        }
        return IsEnough;
    }
    bool CheckIsEnough(int value) //需求顯示
    {
        if(value >= Value)
        {
            Show.color = BaseColor;
            return true;
        }
        else
        {
            Show.color = Color.red;
            return false;
        }
    }
    public void SpawnUI( ItemClip Clip, int value)
    {
        _Clip = Clip;
        Image.sprite= Clip.Icon;
        Show.text =""+ value;
        Value = value;
    }
    public void AddValue(int value)
    {
        Value += value;
        Show.text = "" + Value;
    }
    public void CostValue(int value,ref List<ResourceUI> List)
    {
        Value -= value;
        Show.text = "" + Value;
        if(Value == 0)
        {
            Destroy(ref List);
        }
    }
    public void Destroy()
    {
        Destroy(gameObject);
    }
    public void Destroy(ref List<ResourceUI> Store)
    {
        int i = 0;
        while(i< Store.Count)
        {
            if(Store[i]==this)
            {
                Store.RemoveAt(i);
                break;
            }
            i++;
        }
        Destroy(gameObject);
    }
    public int GotValue //取得數量
    {
        get { return Value; }
    }
}
