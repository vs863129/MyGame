using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Date
{
    public List<R_Quest> questlist=new List<R_Quest>();
    public List<R_EnemybPos> EnemybDateList;
    public List<R_DropItme> DropItme=new List<R_DropItme>();
    public List<R_Building> CompletedBuilding = new List<R_Building>();
    public List<R_Villager> Villagers = new List<R_Villager>();
    public List<R_Resource> Resources = new List<R_Resource>();
    public List<R_Research> Research = new List<R_Research>();
    public R_Inventory Inventory;
    public R_Player Player_Date;
    public TimeDate timeDate;
    public void R_Quests(QuestClip Quest)
    {
        R_Quest _quest = new R_Quest();
        _quest.ClipName = Quest.name;
        _quest.IsDoing = Quest.Doing;
        _quest.IsComplete = Quest.Complete;
        _quest.C_value = Quest.C_value;
        _quest.G_value = new int[Quest.Goals.Length];
        for (int i=0;i< Quest.Goals.Length;i++)
        {
            for(int x=0;x< _quest.G_value.Length;x++)
            {
                _quest.G_value[x] = Quest.Goals[i].Amount;
            }
        }
        questlist.Add(_quest);
    } //紀錄任務
    //public void R_E_Pos(GameObject PrefabPos)//紀錄怪物數據
    //{
    //    R_EnemybPos Obj = new R_EnemybPos();
    //    Obj.SetPos(PrefabPos.transform.position);
    //    Obj.PrefabName = PrefabPos.GetComponent<Enemy>().clip.name;
    //    Obj.Health = PrefabPos.GetComponent<BasicValue>().Health;
    //    if (PrefabPos.GetComponent<Enemy>().Target)
    //    {
    //        Obj.TargetTag = PrefabPos.GetComponent<Enemy>().Target.tag;
    //        Obj.TargetID = PrefabPos.GetComponent<Enemy>().Target.GetComponent<BasicValue>().ID;
    //        Obj.IsAlert = PrefabPos.GetComponent<Enemy>().IsAlert;
    //        Obj.ChaseCooldown = PrefabPos.GetComponent<Enemy>().ChaseCooldown;
    //    }
    //    EnemybDateList.Add(Obj);
    //}
}
[System.Serializable]
public class R_EnemybPos
{
    public string PrefabName;
    public float Health;
    public double X, Y, Z;
    [Header("發現敵人")]
    public string TargetTag;
    public int TargetID;
    public bool IsAlert;//警戒中
    public float ChaseCooldown;//追逐冷卻
    public void SetPos(Vector3 pos)
    {
        X = pos.x;
        Y = pos.y;
        Z = pos.z;
    }
}
[System.Serializable]
public class R_Player
{
    private float Health, BaseMaxHealth, ModflieHealth;
    private float Damage, ModflieDamage;
    private float Defense, ModflieDefense;
    private float X, Y, Z;
    private bool Flip;
    #region 儲存(傳入)
    public void SaveDate(BasicValue Player)
    {
        Player.SaveHealth(out Health, out BaseMaxHealth, out ModflieHealth);
        Player.SaveDamage(out Damage, out ModflieDamage);
        Player.SaveDefense(out Defense, out ModflieDefense);
        Player.SavePos(out Vector3 vector3, out Flip);
        X = vector3.x; Y = vector3.y; Z = vector3.z;
    }
    #endregion
    #region 載入(傳出)
    public void LoadHealth(out float Health, out float BaseHealth, out float ModflieHealth)
    {
        Health = this.Health;
        BaseHealth = this.BaseMaxHealth;
        ModflieHealth = this.ModflieHealth;
    }
    public void LoadDamage(out float Damage, out float ModflieDamage)
    {
        Damage = this.Damage;
        ModflieDamage = this.ModflieDamage;
    }
    public void LoadDefens(out float Defense, out float ModflieDefense)
    {
        Defense = this.Defense;
        ModflieDefense = this.ModflieDefense;
    }
    public void LoadPos(out Vector3 vector,out bool Flip)
    {
        vector = new Vector3(X, Y, Z);
        Flip = this.Flip;
    }
    #endregion
}
[System.Serializable]
public class R_Quest
{
    public string ClipName;
    public bool Repetitive;//是否為重複
    public bool IsComplete;//是否完成
    public bool IsDoing;//是否正在進行中
    public int C_value;//完成次數
    public int[] G_value;//以達成目標量
}
[System.Serializable]
public class R_DropItme
{
    private string ItmeName;
    private float X, Y, Z;
    private int Value;
    public void Save(DropItem item)
    {
        item.SaveDate(out Item Date, out Vector3 vector3);
        ItmeName = Date.Clip.name;
        Value= Date.Value;
        X = vector3.x; Y = vector3.y; Z = vector3.z;
    }
    public void Load(out string ItmeName,out Vector3 vector,out int Value)
    {
        ItmeName = this.ItmeName;
        vector = new Vector3(X, Y, Z);
        Value = this.Value;
    }
}
[System.Serializable]
public class R_Villager
{
    string ClipName;
    VillagerState State;
    bool Outside;//是否在特定地點
    int PlaceID;
    string Place;
    float X, Y, Z;
    //服裝
    string Clothes, HeadWear, Sleeves,Shoes;
    //居住地
    int HomeID;
    string HomeClip;
    //工作地
    int WorkBuildingID;
    string WorkBuildingClip;
    public void Save(Villager villager)
    {
        //vilager
        villager.Save(out Transform Inbuilding, out Vector3 Pos);
        if (Inbuilding != null)
        {
            Outside = false;
            Place = Inbuilding.name;
            PlaceID = Inbuilding.GetComponent<Building>().GotID;
        }
        else
        {
            Outside = true;
        }
        X = Pos.x;Y = Pos.y;Z = Pos.z;
        //clip
        villager.SaveClip(out VillagerClip date);
        ClipName = date.name;
        SaveBuilding(date.Home, out HomeID, out HomeClip);
        SaveBuilding(date.Working, out WorkBuildingID, out WorkBuildingClip);
        State = date.Stats.StateData();
        date.Save(out VillagerAapparelClip Aapparel);
        Aapparel.Save(out Clothes,out HeadWear,out Sleeves,out Shoes);
    }
    private void SaveBuilding(GameObject building,out int ID,out string Name)
    {
        if(building!=null)
        {
            ID = building.GetComponent<Building>().GotID;
            Name = building.GetComponent<Building>().clip.name;
        }
        else
        {
            ID = 0;
            Name = "";
        }
    }
    #region 載入
    public void Loadvillager(out bool Outside,out int PlaceID ,out string Place, out Vector3 Pos)
    {
        Outside = this.Outside;
        Place = this.Place;
        PlaceID = this.PlaceID;
        Pos = new Vector3(X, Y, Z);
    }
    public void LoadVillagerClip(out string ClipName, out int HomeID,out string HomeClip,out int WorkBuildingID,out string WorkBuildingClip)
    {
        ClipName = this.ClipName;
        HomeID = this.HomeID;
        HomeClip = this.HomeClip;
        WorkBuildingID = this.WorkBuildingID;
        WorkBuildingClip = this.WorkBuildingClip;
    }
    public VillagerState LoadState()
    {
        return State;
    }
    public void LoadVillagerAapparelClip(out string Clothes, out string HeadWear, out string Sleeves, out string Shoes)
    {
        Clothes = this.Clothes;
        HeadWear = this.HeadWear;
        Sleeves = this.Sleeves;
        Shoes = this.Shoes;
    }
    #endregion
}
#region 建築設定
[System.Serializable]
public class R_Building
{
    int ID;
    string ClipName;
    float X, Y, Z;
    int HabitablePeople;
    int JobVacancies;
    #region 商店
    List< R_Cargo> Cargos = new List<R_Cargo>();
    #endregion
    public void Save(Building building)
    {
        building.Save(out ID,out BuildingClip clip, out Vector3 vector3);
        X = vector3.x;Y = vector3.y;Z = vector3.z;
        ClipName=clip.name;
        HabitablePeople = clip.HabitablePeople.Length;
        JobVacancies = clip.JobVacancies.Length;
    }
    public void Save(Shop Shop)
    {
        Building building = Shop;
        Save(building);
        for(int i=0;i< Shop.ShopCargo.Count;i++)
        {
            R_Cargo Date = new R_Cargo();
            Date.Save(Shop.ShopCargo[i]);
            Cargos.Add(Date);
        }
    }
    public void Load(out int ID,out string ClipName,out Vector3 vector3,out int HabitablePeople,out int JobVacancies)
    {
        ID = this.ID;
        ClipName = this.ClipName;
        HabitablePeople = this.HabitablePeople;
        JobVacancies = this.JobVacancies;
        vector3 = new Vector3(X, Y, Z);
    }
    public List<Cargo> CargoList(List<ItemClip> Allitem)
    {
        List<Cargo> LoadList = new List<Cargo>();
        for(int i=0;i< Cargos.Count;i++)
        {
            LoadList.Add(CheckItmeDate(Allitem, Cargos[i]));
        }
        return LoadList;
    }
    Cargo CheckItmeDate(List<ItemClip> Allitem, R_Cargo Date)
    {
        Cargo LoadCargo=new Cargo();
        LoadCargo.item = new Item();
        for (int i=0;i<Allitem.Count;i++)
        {
            Date.Load(out string ClipName, out int Value, out int Cost);
            if(Allitem[i].name==ClipName)
            {
                LoadCargo.item.Clip = Allitem[i];
                LoadCargo.item.AddValue(Value);
                LoadCargo.Cost = Cost;
            }
        }
        return LoadCargo;
    }
}
[System.Serializable]
class R_Cargo
{
    string ItemName;
    int Value;
    int Cost;
    public void Save(Cargo Cargo)
    {
        ItemName = Cargo.item.Clip.name;
        Value = Cargo.item.Value;
        Cost = Cargo.Cost;
    }
    public void Load(out string ItemName,out int Value,out int Cost)
    {
        ItemName = this.ItemName;
        Value = this.Value;
        Cost = this.Cost;
    }
}
#endregion
[System.Serializable]
public class R_Resource
{
    string Clip;
    int Value;
    public void Save(ref ResourceUI date)
    {
        Clip = date.Clip.name;
        Value = date.GotValue;
    }
    public void Load(ref List<ItemClip> A_Clips ,out ItemClip ClipName, out int Value)
    {
        ClipName = FindClip(A_Clips);
        Value = this.Value;
    }
    private ItemClip FindClip(List<ItemClip> A_Clips)
    {
        for (int i = 0; i < A_Clips.Count; i++)
        {
            if (A_Clips[i].name == this.Clip)
            {
                return A_Clips[i];
            }
        }
        return null;
    }
}
[System.Serializable]
public class R_Research
{
    bool IsResearch;
    string Clip;
    int Level;
    float Point;
    public void Save(ref ResearchClip clip,bool IsResearch)
    {
        Clip = clip.name;
        Level = clip.level;
        Point = clip.Point;
        this.IsResearch = IsResearch;
    }
    public void Load(ref List<ResearchClip>A_Clips,out ResearchClip Clip,out bool IsResearch)
    {
        Clip = FindClip(A_Clips);
        IsResearch = this.IsResearch;
    }
    private ResearchClip FindClip(List<ResearchClip> A_Clip)
    {
        for (int i = 0; i < A_Clip.Count; i++)
        {
            if(A_Clip[i].name==Clip)
            {
                A_Clip[i].level = Level;
                A_Clip[i].Point = Point;
                return A_Clip[i];
            }
        }
        return null;
    }
} 
[System.Serializable]
public class R_Inventory
{
    string Head;
    string Chest;
    string Foots;
    string LeftWeapon;
    string RightWeapon;
    string[] Accessories=new string[4];
    float Weight;
    float MaxWeight;
    public List<R_InventorySlots> Slots = new List<R_InventorySlots>();
    public void Save(InventorySystem system)
    {
        system.Equipment.Save(out Head, out Chest, out Foots, out LeftWeapon, out RightWeapon, out Accessories);
        system._Date.GotCapacityDate(out Weight,out MaxWeight);
    }
    public void LoadEquipmentSolts(List<ItemClip> AllClips,out ItemClip Head,out ItemClip Chest,out ItemClip Foots,out ItemClip LeftWeapon,out ItemClip RightWeapon,out ItemClip[] Accessories)
    {
        Head = FindClip(AllClips, this.Head);
        Chest = FindClip(AllClips, this.Chest);
        Foots = FindClip(AllClips, this.Foots);
        LeftWeapon = FindClip(AllClips, this.LeftWeapon);
        RightWeapon = FindClip(AllClips, this.RightWeapon);
        ItemClip[] AccessoriesList=new ItemClip[4];
        for (int i=0;i< this.Accessories.Length;i++)
        {
            AccessoriesList[i] = FindClip(AllClips, this.Accessories[i]);
        }
        Accessories = AccessoriesList;
    }
    public void LoadWeight(out float Weight,out float MaxWeight)
    {
        Weight = this.Weight;
        MaxWeight = this.MaxWeight;
    }
    private ItemClip FindClip(List<ItemClip> AllClips,string Clip)
    {
        if(Clip!="")
        {
            for (int i = 0; i < AllClips.Count; i++)
            {
                if (AllClips[i].name== Clip)
                {
                    return AllClips[i];
                }
            }
        }
        return null;
    }
}
[System.Serializable]
public class R_InventorySlots
{
    int ID;
    string ItemClip;
    int Value;
    public void Save(ref Slot slot)
    {
        ID = slot.GotID;
        ItemClip = slot.Item.Clip.name;
        Value = slot.Item.Value;
    }
    public void Load(List<ItemClip> AllClips,out int ID,out ItemClip clip,out int Value)
    {
        clip = FindClip(AllClips, ItemClip);
        ID = this.ID;
        Value = this.Value;
    }
    private ItemClip FindClip(List<ItemClip> AllClips, string Clip)
    {
        if (Clip != "")
        {
            for (int i = 0; i < AllClips.Count; i++)
            {
                if (AllClips[i].name == Clip)
                {
                    return AllClips[i];
                }
            }
        }
        return null;
    }
}
[System.Serializable]
public class TimeDate
{
    //一日時間 86400 秒
    //現實48分鐘 = 遊戲一日(30Point)
    //現實2小時 = 遊戲一日 (12Point)
    public int Sec;
    public int Min;
    public int Hour;
    public int Day;
    public bool Set = true;
}
