using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Villager", menuName = "Type/Villager")]
public class VillagerClip : ScriptableObject
{
    public GameObject Working;
    public GameObject Home;
    [SerializeField]private VillagerAapparelClip Aapparel=new VillagerAapparelClip();
    public VillagerState Stats;
    public OrderClip WorkOredr;
    public InventoryClip inventory;

    public void LoadClothing(out Sprite Clothes, out Sprite Headwear, out Sprite Sleeve, out Sprite Shoe)
    {
        Aapparel.Base(out Clothes,out Headwear,out Sleeve,out Shoe);
    }
    public void Generate(string Name,VillagerAapparelSprite AapparelData,float FoodFlow,float FatigueFlow)//生成
    {
        Stats = new VillagerState();
        Stats.Name = Name;
        Stats.Generate(FoodFlow, FatigueFlow);
        InventoryClip _inventory = CreateInstance<InventoryClip>();
        _inventory.GenerateSlot(4);
        inventory = _inventory;
        Aapparel.Generate(AapparelData);
    }
    #region 存取
    public void Save(out VillagerAapparelClip Aapparel)
    {
        Aapparel = this.Aapparel;
    }
    public void LoadAapparel(ref Sprite Headwear, ref Sprite Clothes, ref Sprite Sleeves,ref Sprite Shoes)
    {
        Aapparel.Load(ref Headwear, ref Clothes, ref Sleeves, ref Shoes);
    }
    public Sprite GotHat()
    {
        return Aapparel.LoadHeadWear();
    }
    #endregion
}
//foreach (KeyValuePair<string  , int> debug in Stats)//偵錯Dictionary用
//{
//    Debug.Log(debug.Key.ToString() + "" + debug.Value.ToString());
//}
[System.Serializable]
public class VillagerAapparelClip
{
    [SerializeField] private Sprite Headwear;
    [SerializeField] private Sprite Clothes;
    [SerializeField] private Sprite Sleeves;
    [SerializeField] private Sprite Shoes;
    public void Generate(VillagerAapparelSprite data)
    {
        data.Generate(out Clothes,out Sleeves,out Shoes);
    }
    public void Base(out Sprite Clothes, out Sprite Headwear, out Sprite Sleeve, out Sprite Shoe)
    {
        Clothes = this.Clothes;
        Headwear = this.Headwear;
        Sleeve = this.Sleeves;
        Shoe = this.Shoes;
    }
    #region 存取
    public void Save(out string Clothes, out string Headwear, out string Sleeves, out string Shoes)
    {
        if(this.Clothes!=null)
        {
            Clothes = this.Clothes.name;
        }
        else
        {
            Clothes = "";
        }
        if (this.Headwear != null)
        {
            Headwear = this.Headwear.name;
        }
        else
        {
            Headwear = "";
        }
        if (this.Sleeves != null)
        {
            Sleeves = this.Sleeves.name;
        }
        else
        {
            Sleeves = "";
        }
        if (this.Shoes != null)
        {
            Shoes = this.Shoes.name;
        }
        else
        {
            Shoes = "";
        }
    }
    public void Load(ref Sprite Headwear,ref Sprite Clothes, ref Sprite Sleeves,ref Sprite Shoes)
    {
        this.Headwear = Headwear;
        this.Clothes = Clothes;
        this.Sleeves = Sleeves;
        this.Shoes = Shoes;
    }
    public Sprite LoadHeadWear()
    {
        return Headwear;
    }
    #endregion
}
[System.Serializable]
public class VillagerState
{
    int _ID;
    public int ID
    {
        get
        {
            return _ID;
        }
    }
    public string Name;//村民名字
    public int Age;//年齡
    #region 屬性
    int _Strength;
    public int Strength//力量
    {
        get
        {
            return _Strength;
        }
    }
    int _Agile;
    public int Agile//敏捷
    {
        get
        {
            return _Agile;
        }
    }
    int _Intelligence;
    public int Intelligence//智力
    {
        get
        {
            return _Intelligence;
        }
    }
    int _Lucky;
    public int Lucky//幸運
    {
        get
        {
            return _Lucky;
        } 
    }
    float _Health;
    public float Health { get { return _Health; } set { _Health = value; if (_Health < 0) { _Health = 0; } } }//血量
    float _Food;
    public float Food { get { return _Food; } set { _Food = value; if (_Food < 0) { _Food = 0; }if (_Food > 100) { _Food = 100; } } }//飢餓度
    float _Fatigue;
    public float Fatigue { get { return _Fatigue; } set { _Fatigue = value; if (_Fatigue < 0) { _Fatigue = 0; } if (_Fatigue > 100) { _Fatigue = 100; } } }//疲勞度
    public int CollectPower {get { return (int)Mathf.Ceil( Strength*1.2f); } } //單次採集量
    public float CollectSpeed { get { return Agile*.1f + Strength*.3f; } } //採集速度
    public float ProducePower { get { return Strength * .2f + Agile * .3f;  } } //單次製作量
    #endregion
    #region 流逝速度
    float FoodFlow;
    float FatigueFlow;
    #endregion
    public Villager_AI AI;
    #region 判斷
    public bool IsGround;
    #endregion
    public void Flow()
    {
        Food -= FoodFlow;
        Fatigue -= FatigueFlow;
    }
    public void Generate(float DefaultFoodFlow,float DefaultFatigueFlow)
    {
        FoodFlow = DefaultFoodFlow;
        FatigueFlow = DefaultFatigueFlow;
        _Health = 100;
        _Food = 100;
        _Fatigue = 100;
        _Strength = Random.Range(1, 10);
        _Agile = Random.Range(1, 10);
        _Intelligence = Random.Range(1, 10);
        _Lucky = Random.Range(1, 10);
    }
    public VillagerState StateData()
    {
        return (VillagerState)MemberwiseClone();
    }
}

