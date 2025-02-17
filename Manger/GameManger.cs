using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class GameManger : MonoBehaviour
{
    public GameObject player;//玩家
    [Header("研發")]
    public ResearchSystem Research;//研究系統
    [Header("時間")]
    public TimeManger TimeManger;
    [Header("背包")]
    public InventorySystem InventorySystem;//背包系統
    [Header("倉庫")]
    public ResourceSystem ResourceSystem;//倉庫系統
    [Header("商店")]
    public ShopManger ShopManger;//商店控制
    [Header("製造")]
    public ProduceSystem ProduceSystem;//製造系統
    [Header("攝影機")]
    public CameraSetting CameraSetting;
    [Header("教程")]
    [SerializeField] TutorialManger TutorialManger;
    [Header("外部系統")]
    public CollectManger CollectManger;//自然資源管理
    public BuilndingManger builderSystem;//建築系統
    public VillagersManger VillagersManger;//村民控制
    public UIManger UIManger;//UI管理
    [Header("紀錄系統")]
    public DateBase DateBase;//存檔系統
    [Header("預置物")]
    [SerializeField] private GameObject GMButton;
    [SerializeField] private GameObject P_DropItem;
    #region 倉庫與玩家資源
    public bool CheckResource(ResourceUI CostUI)
    {
        return CostUI.CheckIsEnough(ResourceSystem, InventorySystem);
    }
    public void PayResource(List<ResourceUI> AllCostUI)
    {
        int CostUI = 0;
        while(CostUI< AllCostUI.Count)
        {
            int NeedCostValue = AllCostUI[CostUI].GotValue;
            int PlayerInv = 0;
            while (PlayerInv < InventorySystem.SlotsUI.Count)
            {
                if (InventorySystem.SlotsUI[PlayerInv].Item.Clip == AllCostUI[CostUI].Clip)
                {
                    InventorySystem.RemoveItem(InventorySystem.SlotsUI[PlayerInv], NeedCostValue);
                    if (NeedCostValue <= 0)
                    {
                        break;
                    }
                }
                PlayerInv++;
            }
            if (NeedCostValue > 0)
            {
                int Store = 0;
                while (Store < ResourceSystem.Store.Count)
                {
                    if (ResourceSystem.Store[Store].Clip == AllCostUI[CostUI].Clip)
                    {
                        ResourceSystem.Store[Store].CostValue(NeedCostValue, ref ResourceSystem.Store);
                        break;
                    }
                    Store++;
                }
            }
            CostUI++;
        }
    }
    #endregion
    #region 未整理
    private void Awake()
    {
       InventorySystem.AddSlot();
       LoadMainSettings();
    }
    #region 讀取主選單的設定
    void LoadMainSettings()
    {
        if (GameObject.Find("M_Settings"))
        {
            DateBase.MainSettings = GameObject.Find("M_Settings");
            MainSettings Script = DateBase.MainSettings.GetComponent<MainSettings>();
            if (Script.IgnoreTutorial)
            {
                TutorialManger.IgnoreSetting();
            }
            Destroy(DateBase.MainSettings);
        }
    }
    #endregion
    private void Start()
    {
        //InvokeRepeating("Time", 1, 1);
        Research.Start(this);
        builderSystem.UISetting.Start(this);
        TimeManger.Load(DateBase.GameData.timeDate.Hour, DateBase.GameData.timeDate.Min);
        if(DateBase.IsFirstStart)
        {
            VillagersManger.initialize();
            CollectManger.SpawnAllCollect();
            DateBase.IsFirstStart = false;
        }
    }
    private void FixedUpdate()
    {
        Research.FixedUpdate();
    }
    private void Update()
    {
        CameraSetting.Update();
    }
    public void DebugSomething()//測試所以東西 可任意更改
    {
    }
    [ContextMenu("Save")]
    public void Save()
    {
        DateBase.Save();
    }
    [ContextMenu("Load")]
    public void Load()
    {
        DateBase.Load();
    }
    public void SelectResearch(ResearchClip clip)//按鈕用(選擇研究)
    {
        if(!Research.NowProject)
        {
            if (Research.SelectProject != null)
            {
                if (Research.SelectProject == clip)
                {
                    Research.SelectProject = null;
                    Research.UpdateUI(null);
                }
                else
                {
                    Research.SelectProject = clip;
                    Research.UpdateUI(clip);
                }
            }
            else
            {
                Research.SelectProject = clip;
                Research.UpdateUI(clip);
            }
        }
    }
    public void StartResearch()//按鈕用(開始研究)
    {
        Research.Identify();
    }
    public void DropItem(ItemClip item, Vector3 SpawnPoint, int Value)//生成掉落物
    {
        DropItem dropItem = Instantiate(P_DropItem).GetComponent<DropItem>();
        dropItem.transform.position = SpawnPoint;
        dropItem.Spawn(item, Value);
        DateBase.IsDrops.Add(dropItem);
    }
    #endregion
}
#region 事件系統
[System.Serializable]
public class EventSystem 
{

}
#endregion
#region 研究系統
[System.Serializable]
public class ResearchSystem
{
    GameManger GM;
    public float NowPoint;//目前擁有的研究點
    public ResearchClip SelectProject;//選擇的研究項目
    public ResearchClip NowProject;//目前研究項目
    [Tooltip("0 標題\n1 等級\n2 進度要求\n3 內文 ")]
    [SerializeField] TextMeshProUGUI[] Text;//文字
    [SerializeField] private Slider ReserachingBar;
    public Button StartButton;//開始研究按鈕
    [SerializeField] ResearchUI[] AllUI;
    public List<ResearchClip> FinishedResearch = new List<ResearchClip>(); //已完成的研究
    public ResearchUIManger UI;
    public void Start(GameManger GM)
    {
        this.GM = GM;
        UI.Check(FinishedResearch);
        UpdateUI(null);
    }
    public void FixedUpdate()
    {
        UI.UpdateLine();
    }
    public void Identify()//確定研究
    {
        if (SelectProject&& SelectProject.level< SelectProject.MaxLevel)
        {
            NowProject = SelectProject;
            StartButton.interactable = false;
            UpdateUI(NowProject);
        }
    }
    public void ToPoint(float Point)
    {
        if (NowProject != null)
        {
            NowProject.Point += Point;
            if (NowProject.Point >= NowProject.MaxPoint)
            {
                FinishResearch();
            }
            UpdateUI(NowProject);
        }
    }
    private void FinishResearch()
    {
        NowProject.level += 1;
        NowPoint = NowProject.Point - NowProject.MaxPoint;
        NowProject.Point = 0;
        switch (NowProject.type)
        {
            case Researchtype.none:
                break;
            case Researchtype.Economic:
                break;
            case Researchtype.Construct:
                NowProject.UnlockBuilding.GenerateBuildingButton(GM.builderSystem);
                break;
            case Researchtype.Combat:
                ToValue(NowProject);
                break;
        }
        UIUpdate();
        if (FinishedResearch.Count != 0)
        {
            for (int i = 0; i < FinishedResearch.Count; i++)
            {
                if (NowProject == FinishedResearch[i])
                {
                    break;
                }
                else if (NowProject != FinishedResearch[i] && i == FinishedResearch.Count - 1)
                {
                    FinishedResearch.Add(NowProject);
                    break;
                }
            }
        }
        else
        {
            FinishedResearch.Add(NowProject);
        }
        NowProject = null;
        SelectProject = null;
        UpdateUI(NowProject);
        UI.Check(FinishedResearch);
    }
    void UIUpdate()
    {
        foreach(ResearchUI UI in AllUI)
        {
            if(UI.Clip==NowProject)
            {
                UI.SetLevel();
            }
        }
    }
    void ToValue(ResearchClip clip)
    {
        int i = 0;
        while(i< clip.Buff.Length)
        {
            switch (clip.Buff[i].Type)
            {
                case Attribute.Health:
                    ToPlayer(clip.Buff, i, clip.level);
                    break;
                case Attribute.Defense:
                    ToPlayer(clip.Buff, i, clip.level);
                    break;
                case Attribute.Damage:
                    ToPlayer(clip.Buff, i, clip.level);
                    break;
                case Attribute.ProduceCostResourceReduction:
                    ToProduce(clip.Buff, i, clip.level);
                    break;
                case Attribute.BuilingCostResourceReduction:
                    break;
            }
            i++;
        }
    }
    void ToProduce(ResearchBuffs[] Buffs, int Index, int Level)
    {
        int A = 0;
        while(A<GM.DateBase.A_ProduceClip.Count)
        {
            if(GM.DateBase.A_ProduceClip[A].Cost.Count!=0)
            {
                foreach (Cost_Resource Cost in GM.DateBase.A_ProduceClip[A].Cost)
                {
                    Cost.Reduction = Buffs[Index].Value(Level);
                }
            }
            A++;
        }
    }
    void ToPlayer(ResearchBuffs[] Buffs,int Index,int Level)
    {
        GM.player.GetComponent<Player>().AddModflie(Buffs[Index].Type, Buffs[Index].Value(Level), false);
        GM.InventorySystem.StatsUI.UpdateStats();
    }
    public void UpdateUI(ResearchClip Clip)
    {
        if (NowProject || SelectProject)
        {
            Text[0].text = Clip.Name;//標題 
            Text[1].text = "" + Clip.level + "/" + Clip.MaxLevel;
            if (NowProject)
            {
                Text[2].text = ((double)Clip.Point / Clip.MaxPoint).ToString("P0"); // 需要的研究點
            }
            Text[3].text = Clip.Description;//內文
            ReserachingBar.maxValue = Clip.MaxPoint;
            ReserachingBar.value = Clip.Point;
        }
        else
        {
            Text[0].text ="None";//標題 和 等級
            Text[1].text = "";
            Text[2].text ="Start"; // 需要的研究點
            Text[3].text = "";//內文
            StartButton.interactable = true;
            ReserachingBar.maxValue = 1;
            ReserachingBar.value = 0;
        }
    }
    #region 存取
    public void Load(R_Research Date,List<ResearchClip>A_Clips)
    {
        Date.Load(ref A_Clips, out ResearchClip clip, out bool IsResearch);
        if(IsResearch)
        {
            SelectProject = clip;
            Identify();
        }
        else
        {
            FinishedResearch.Add(clip);
            UI.Check(FinishedResearch);
        }
    }
    #endregion
}
#endregion
#region 資源系統
[System.Serializable]
public class ResourceSystem
{
    [SerializeField]
    GameManger GM;
    public List<ResourceUI> Store = new List<ResourceUI>();
    [Header("介面")]
    public GameObject P_Resource;//預製物
    public Transform Res_parent;
    public void Add(ItemClip Clip,int value)//加入物品
    {
        if(Store.Count!=0)
        {
            for(int i=0;i< Store.Count;i++)
            {
                if (Store[i] != null)
                {
                    if (Store[i].GetComponent<ResourceUI>().Clip == Clip)
                    {
                        Store[i].GetComponent<ResourceUI>().AddValue(value);
                        break;
                    }
                    else if (i == Store.Count - 1 && Store[i].GetComponent<ResourceUI>().Clip != Clip)
                    {
                        Store.Add(ResUI(Clip, value));
                        break;
                    }
                }
                else
                {
                    Store[i] = ResUI(Clip, value);
                    break;
                }
            }
        }
        else
        {
            Store.Add(ResUI(Clip,value));
        }
    }

    public bool Pay(List <Cost_Resource> CostRes) //回傳扣除成功
    {
        for (int i = 0; i < CostRes.Count; i++)
        {
            for (int x = 0; x < Store.Count; x++)
            {
                if (Store[x].GetComponent<ResourceUI>().Clip == CostRes[i].Clip)
                {
                    Store[x].GetComponent<ResourceUI>().CostValue(CostRes[i].Value,ref Store);
                    break;
                }
            }
        }
        return true;
    }
    public bool CheckResource(List<Cost_Resource> AllCostResource) //只要有資源不足直接返回False
    {
        if(Store.Count!=0)
        {
            if(AllCostResource.Count!=0)
            {
                int A = 0;//儲藏
                while (A < Store.Count)
                {
                    int B = 0;//需求
                    while (B < AllCostResource.Count)
                    {
                        Debug.Log("Store:" + A + "," + "Cost" + B);
                        if(Store[A].Clip==AllCostResource[B].Clip)
                        {
                            if(!AllCostResource[B].IsEnough(Store[A].GotValue))
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else if(A == Store.Count-1)
                        {
                            return false;
                        }
                        B++;
                    }
                    A++;
                }
            }
        }
        else if(AllCostResource.Count!=0)
        {
            return false;
        }
        return true;
    }
    private ResourceUI ResUI(ItemClip Clip,int value)
    {
        GameObject New = Object.Instantiate(P_Resource, Res_parent);
        New.transform.localScale = new Vector3(1, 1, 1);
        New.name = Clip.name;
        ResourceUI Script = New.GetComponent<ResourceUI>();
        Script.SpawnUI( Clip, value);
        return Script;
    }
    #region 存取
    public void Load(R_Resource Date,List<ItemClip> A_ResourceClip)
    {
        Date.Load(ref A_ResourceClip, out ItemClip ClipName, out int Value);
        Add(ClipName, Value);
    }
    #endregion
}
[System.Serializable]
public class Cost_Resource //資源需求
{
    public ItemClip Clip;//資源
    [SerializeField] int _Value;
    public int Value
    {
        get
        {
            return (int)(_Value - (_Value * (Reduction * 0.01f)));
        }
    }
    //數量
    int _Reduction;
    public int Reduction//減免(%)
    { get { return _Reduction; }set { if (value > 99) { _Reduction = 99; } else { _Reduction = value; }} }
    public bool IsEnough(int _value)//檢查資源
    {
        if (_value >= Value)
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
#region 商店系統
[System.Serializable]
public class ShopManger 
{
    public GameManger GM;
    public GameObject ShopUI;
    [Header("預製物")]
    public GameObject CommodityUIPrefab;
    public Transform List;
    Shop NowShop;
    List<GameObject> CommodityUIList = new List<GameObject>();
    public void LoadCommodityData(Shop Shop)
    {
        if (NowShop != null)
        {
            if (NowShop != Shop)
            {
                ClearUI();
                LoadCommodityUI(Shop);
            }
            else
            {
                return;
            }
        }
        else
        {
            LoadCommodityUI(Shop);
        }
    }
     void LoadCommodityUI(Shop Shop)
    {
        NowShop = Shop;
        for (int i = 0; i < Shop.ShopCargo.Count; i++)
        {
            GameObject UI = Object.Instantiate(CommodityUIPrefab, List);
            ShopItemUI UIScript = UI.GetComponent<ShopItemUI>();
            UI.GetComponent<Button>().onClick.AddListener(() => UIScript.Buy(GM));
            UIScript.Commodity = Shop.ShopCargo[i];
            UIScript.UpdateUI();
            CommodityUIList.Add(UI);
        }
    }
    void ClearUI()
    {
        for(int i=0;i< CommodityUIList.Count;i++)
        {
            Object.Destroy(CommodityUIList[i]);
        }
        CommodityUIList.Clear();
    }
}
#endregion
#region 時間系統
[System.Serializable]
public class TimeManger
{
    [Tooltip("現實1小時 = 遊戲一日 (24Point)\n現實2小時 = 遊戲一日 (12Point)\n現實1分鐘 = 遊戲一日 (1440Point)")]
    public int TimePoint;
    [Tooltip("圖片放置:\n1 Sun \n2 SunDown \n3 Night \n4 Sun Up")]
    [SerializeField] Sprite[] TimeUI;  //0 Sun 1 SunDown 2 Night 3 SunUp
    [SerializeField] Image UI;
    [SerializeField] Animator SunTime;
    public void Load(int Hour,int Min)
    {
        SetLight(Min + (Hour * 60));
        SunState(Hour);
    }
    public void SetLight(int Min)
    {
        SunTime.SetFloat("Minute",Min);
    }
    public int SunState(int Hour)
    {
        UI.sprite = TimeUI[SetSun(Hour)];
        return SetSun(Hour);
    }
     int SetSun(int hour)
    {
        switch(hour) 
        {
            case int x when x == 7:
                return 3; // 日出時間 7點
            case int x when x >= 19 || x <= 6: //晚上時間 7點 至 隔日 凌晨6點 
                return 2;
            case int x when x == 18: // 日落 下午 6點
                return 1;
            default:
                return 0; //早上時間 
        }
    }
}
#endregion
#region 攝影機設定
[System.Serializable]
public class CameraSetting
{
    [SerializeField] GameManger GM;
    [SerializeField] Transform LeftPoint, RightPoint;
    [SerializeField] Cinemachine.CinemachineVirtualCamera MainCamer;
    public void Update()
    {
        if(GM.player.transform.position.x<= LeftPoint.position.x|| GM.player.transform.position.x >= RightPoint.position.x)
        {
            Cinemachine.CinemachineFramingTransposer SetDeadZone = MainCamer.GetCinemachineComponent<Cinemachine.CinemachineFramingTransposer>();
            SetDeadZone.m_DeadZoneWidth = OverDistance(GM.player.transform.position);
            if (SetDeadZone.m_DeadZoneWidth<0.1)
            {
                SetDeadZone.m_DeadZoneWidth = 0;
            }
        }
    }
    float OverDistance(Vector3 PlayerPoint)
    {
        float Distance;
        switch (Mathf.Sign(PlayerPoint.x))
        {

            case 1:
                Distance = Mathf.Abs(PlayerPoint.x - RightPoint.position.x);
                if (Distance>1)
                {
                    return 1;
                }
                else
                {
                    return Distance;
                }

            default:
                Distance = Mathf.Abs(PlayerPoint.x - LeftPoint.position.x);
                if (Distance > 1)
                {
                    return 1;
                }
                else
                {
                    return Distance;
                }
        }
    }
}
#endregion


