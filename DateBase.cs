using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System;

public class DateBase : MonoBehaviour 
{
    public bool IsFirstStart;
    static string directory = "/SaveDate";
    static string fileName = "/Data.json";
    public GameObject MainSettings;
    public GameManger GameManger;
    public ResearchClip[] A_ReserachClip;//所有研究列表
    public List<BuildingClip> AllBuildingClip = new List<BuildingClip>();
    public List<ItemClip> A_Items = new List<ItemClip>();
    public List<DropItem> IsDrops = new List<DropItem>();
    public List<ItemClip> A_Resource = new List<ItemClip>();
    public List<ResearchClip> A_ResearchClip = new List<ResearchClip>();
    public List<ProduceClip> A_ProduceClip = new List<ProduceClip>();
    public List<GameObject> A_Collecting = new List<GameObject>();
    public List<Sprite> A_InteractiveImage = new List<Sprite>();
    public GameObject[] D_Obj;//物件紀錄
    public Date GameData; //儲存用的資料
    public Image DebugImage;//測試用
    private void Awake()
    {
        Initialization();
    }
    void Initialization()//初始化
    {
        foreach(ResearchClip clip in A_ResearchClip)
        {
            clip.OriginalSetting();
        }
        foreach(ProduceClip clip in A_ProduceClip)
        {
            if(clip.Cost.Count!=0)
            {
                foreach(Cost_Resource Cost in clip.Cost)
                {
                    Cost.Reduction = 0;
                }
            }
        }
 
    }


    #region 時間運算區
    private void FixedUpdate()
    {
        if (!IsInvoking(nameof(Flow)))
        {
            InvokeRepeating(nameof(Flow), 1, 1);
        }
    }
    void Flow()
    {
        TimeDate date = GameData.timeDate;
        date.Sec += GameManger.TimeManger.TimePoint;
        if (date.Sec >= 60)
        {
            for (int i = 0; i < date.Sec / 60; i++)
            {
                date.Min++;
            }
            date.Sec %= 60;
            if (date.Min >= 60)
            {
                for (int i = 0; i < date.Min / 60; i++)
                {
                    date.Hour++;
                    int SunState= GameManger.TimeManger.SunState(date.Hour); //判斷目前時間決定太陽狀態
                    GameManger.VillagersManger.SetTime(SunState);
                }
                date.Min %= 60;
                if (date.Hour >= 24)
                {
                    date.Hour = 0;
                    NextDay(date);
                }
            }
            GameManger.TimeManger.SetLight(date.Min+(date.Hour*60));//控制光線用
        }
        GameManger.VillagersManger.Flow();//村民控制
    }
    public void NextDay(TimeDate date)
    {
        date.Day++;
        date.Set = false;
    }
    public void TimeCalculation(float Point) //計算用
    {
        float TrueTime, GameTime;
        TrueTime = 86400;
        GameTime = TrueTime / Point;
        Debug.Log("現實" + GameTime / 60 + "分鐘 等於 遊戲一日");
    }
    #endregion
    #region 存檔
    public void Save()
    {
        S_PlayerDate();
        S_EnemyDate();
        S_Building();
        S_Villager();
        S_DropItme();
        S_ResourceDate();
        S_ResearchDate();
        S_InventorySlots();
        //建立檔案 Json BUG
        //string json = JsonUtility.ToJson(GameData);
        //string Path = Application.persistentDataPath + directory;
        //File.WriteAllText(Path + fileName, json);
        //Debug.Log(Path);
        IFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Played.gamesave";
        Stream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, GameData);
        Debug.Log(path);
        stream.Close();
    }
    private void S_ResourceDate()
    {
        GameData.Resources.Clear();
        for(int i=0;i<GameManger.ResourceSystem.Store.Count;i++)
        {
            if(GameManger.ResourceSystem.Store[i]!=null)
            {
                ResourceUI Scrip = GameManger.ResourceSystem.Store[i].GetComponent<ResourceUI>();
                R_Resource Date = new R_Resource();
                Date.Save(ref Scrip);
                this.GameData.Resources.Add(Date);
            }
        }
    }
    private void S_EnemyDate()//怪物資料寫入
    {
        //Array.Clear(D_Obj, 0, D_Obj.Length);//清空物件
        //date.EnemybDateList.Clear();//清空數據
        //D_Obj = GameObject.FindGameObjectsWithTag("Enemy");
        //for (int i = 0; i < D_Obj.Length; i++)
        //{
        //        date.R_E_Pos(D_Obj[i]);
        //}
    }
    private void S_Villager()
    {
        GameData.Villagers.Clear();
        for(int i=0;i< GameManger.VillagersManger.villagers.Count;i++)
        {
            Villager Scrip = GameManger.VillagersManger.villagers[i];
            R_Villager Date = new R_Villager();
            Date.Save(Scrip);
            this.GameData.Villagers.Add(Date);
        }
    }
    private void S_Building()
    {
        GameData.CompletedBuilding.Clear();
        for (int i=0;i<GameManger.builderSystem.Complate.Count;i++)
        {
            R_Building Date = new R_Building();
            
            #region 判斷Script
            if (GameManger.builderSystem.Complate[i].GetComponent<Shop>())
            {
                Shop shop = GameManger.builderSystem.Complate[i].GetComponent<Shop>();
                Date.Save(shop);
                this.GameData.CompletedBuilding.Add(Date);
            }
            #endregion
            else
            {
                var Script = GameManger.builderSystem.Complate[i].GetComponent<Building>();
                Date.Save(Script);
                this.GameData.CompletedBuilding.Add(Date);
            }
        }
    }
    private void S_DropItme()
    {
        GameData.DropItme.Clear();
        for (int i=0;i< IsDrops.Count;i++)
        {
            R_DropItme NewDate = new R_DropItme();
            NewDate.Save(IsDrops[i]);
            GameData.DropItme.Add(NewDate);
        }
    }
    private void S_PlayerDate()//玩家資料寫入
    {
        GameData.Player_Date.SaveDate(GameManger.player.GetComponent<Player>());
    }
    private void S_ResearchDate()
    {
        GameData.Research.Clear();
        ResearchClip NowProject=null;
        if (GameManger.Research.NowProject != null)
        {
            NowProject = GameManger.Research.NowProject;
            R_Research Date = new R_Research();
            Date.Save(ref NowProject, true);
            this.GameData.Research.Add(Date);
        }
        for (int i=0;i<GameManger.Research.FinishedResearch.Count;i++)
        {
            ResearchClip clip = GameManger.Research.FinishedResearch[i];
            if(NowProject == GameManger.Research.FinishedResearch[i]&&NowProject!=null)
            {
                continue;
            }
            R_Research Date = new R_Research();
            Date.Save(ref clip, false);
            this.GameData.Research.Add(Date);
        }
    }
    private void S_InventorySlots()
    {
        GameData.Inventory.Slots.Clear();
        GameData.Inventory.Save(GameManger.InventorySystem);
        for (int i=0;i<GameManger.InventorySystem.SlotsUI.Count;i++)
        {
            if(GameManger.InventorySystem.SlotsUI[i].Item.Clip!=null)
            {
                Slot slot = GameManger.InventorySystem.SlotsUI[i];
                R_InventorySlots Date = new R_InventorySlots();
                Date.Save(ref slot);
                this.GameData.Inventory.Slots.Add(Date);
            }
        }
    }
#endregion
    #region 載入
    public void Load()
    {
        // json BUG
        //string FullPath = Application.persistentDataPath + directory + fileName;
        //if (File.Exists(FullPath))
        //{
        //    string json = File.ReadAllText(FullPath);
        //    GameData = JsonUtility.FromJson<Date>(json);
        //    LoadDate();
        //}
        //else
        //{
        //    Debug.Log("沒有存檔");
        //}
        string path = Application.persistentDataPath + "/Played.gamesave";
        if (File.Exists(path))
        {
            Debug.Log("Load Save: " + path);
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Open);
            GameData = (Date)formatter.Deserialize(stream);
            stream.Close();
            LoadDate();
        }
    }
    void LoadDate()
    {
        GameManger.player.GetComponent<Player>().LoadDate(GameData.Player_Date);
        L_Research_Date();
        L_ResourcesDate();
        L_Building();
        L_Villagers();
        L_DropItme();
        L_Inventory();
        L_TimeDate();
    }
    #region 已經編寫好
    private void L_DropItme()
    {
        C_DropItme();
        for (int i=0;i<GameData.DropItme.Count;i++)
        {
            GameData.DropItme[i].Load(out string ItemName, out Vector3 vector3, out int Value);
            for (int x=0;x< A_Items.Count;x++)
            {
                if(ItemName== A_Items[x].name)
                {
                    GameManger.DropItem(A_Items[x], vector3, Value);
                    break;
                }
            }
        }
    }
    private void C_DropItme()
    {
        for(int i=0;i<IsDrops.Count;i++)
        {
            IsDrops[i].Remove();
        }
        IsDrops.Clear();
    }
    private void L_Building()
    {
        C_Building();
        for (int i=0;i<GameData.CompletedBuilding.Count;i++)
        {
            GameManger.builderSystem.Load(GameData.CompletedBuilding[i]);
        }
    }
    private void C_Building()
    {
        for (int i = 0; i < GameManger.builderSystem.Complate.Count; i++)
        {
            GameManger.builderSystem.Complate[i].GetComponent<Building>().Remove();
        }
        GameManger.builderSystem.Complate.Clear();
    }
    private void L_Villagers()
    {
        C_Villagers();
        for(int i=0;i<GameData.Villagers.Count;i++)
        {
            GameManger.VillagersManger.Load(GameData.Villagers[i]);
        }
        GameManger.VillagersManger.LoadUI();
    }
    private void C_Villagers()
    {
        List<Villager> villager = GameManger.VillagersManger.villagers;
        for(int i=0;i< villager.Count;i++)
        {
            villager[i].Remove();
        }
        villager.Clear();
    }
    private void L_ResourcesDate()
    {
        C_ResourcesDate();
        for(int i=0; i<GameData.Resources.Count;i++)
        {
            GameManger.ResourceSystem.Load(GameData.Resources[i], A_Resource);
        }
    }
    private void C_ResourcesDate()
    {
        List<ResourceUI> ResourcesUI = GameManger.ResourceSystem.Store;
        for(int i=0;i< ResourcesUI.Count;i++)
        {
            if (ResourcesUI[i] != null)
            { ResourcesUI[i].Destroy(); }
        }
        ResourcesUI.Clear();
    }
    private void L_Research_Date()
    {
        GameManger.Research.FinishedResearch.Clear();
        for(int i=0;i<GameData.Research.Count;i++)
        {
            GameManger.Research.Load(GameData.Research[i],A_ResearchClip);
        }
    }
    private void L_Inventory()
    {
        C_Inventory();
        GameData.Inventory.LoadEquipmentSolts(A_Items, out ItemClip Head, out ItemClip Chest, out ItemClip Foots, out ItemClip LeftWeapon, out ItemClip RightWeapon, out ItemClip[] Accessories);
        GameManger.InventorySystem.Equipment.Load(ref Head, ref Chest, ref Foots, ref LeftWeapon, ref RightWeapon, ref Accessories);
        List<R_InventorySlots> DateSlots = GameData.Inventory.Slots;
        for(int i=0;i< DateSlots.Count;i++)
        {
            DateSlots[i].Load(A_Items, out int ID, out ItemClip itme, out int Value);
        }
    }
    private void C_Inventory()
    {
        for (int i = 0; i < GameManger.InventorySystem.SlotsUI.Count; i++)
        {
            GameManger.InventorySystem.SlotsUI[i].Clear();
        }
        GameManger.InventorySystem.Equipment.AllClear();
    }
    void L_TimeDate()
    {
        GameManger.TimeManger.Load(GameData.timeDate.Hour, GameData.timeDate.Min);
    }
    #endregion
    #region 正在編寫

    #endregion
    #endregion
}



