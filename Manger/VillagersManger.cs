using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class VillagersManger : MonoBehaviour
{   [Header("初始化")]
    [Tooltip("基礎村民數量")]
    [SerializeField] int BaseValue;
    [Header("狀態")]
    VillagerManger_Time _NowTime;
    public VillagerManger_Time NowTime { get => _NowTime; }
    [Header("居民生成")]
    public GameObject villager_Prefab;//預置物
    public Transform Prefab_List;//Villager集合區
    public Transform Spawn;//出生點
    public List<Villager> villagers = new List<Villager>();
    [SerializeField] float DefaultFoodFlow;
    [SerializeField] float DefaultFatigueFlow;
    [Header("村民列表")]
    [SerializeField] GameObject UIPrfab;
    [SerializeField] List<GameObject> VillagerUI = new List<GameObject>();
    [SerializeField] Transform UIList;
    [SerializeField] List<string> FistName = new List<string>();
    [SerializeField] List<string> LastName = new List<string>();
    public ToggleGroup UIGroup;
    [Header("招募")]
    public List<GameObject> RecruitList = new List<GameObject>();
    public GameObject RecruitButton;
    public Transform list;
    [Header("系統")]
    public VillagerListUI_States StateUI;
    [SerializeField] GameManger GM;
    [SerializeField] private VillagerAapparelSprite AllAapparelImage; //所有村民服裝
    
    #region 暫時拔除功能
    //public void AddRecruitUI()//生成招募選項
    //{
    //    VillagerClip Clip = ScriptableObject.CreateInstance<VillagerClip>();
    //    Clip.Stats.Name = "TEST";
    //    Clip.Generate(AllAapparelImage);
    //    GameObject obj = Instantiate(RecruitButton, list);
    //    obj.transform.localScale = new Vector3(1, 1, 1);
    //    obj.transform.SetAsFirstSibling();
    //    RecruitUI UI = obj.GetComponent<RecruitUI>();
    //    UI.Recruit_Button.onClick.AddListener(() => Recruit(Clip));
    //    UI.Recruit_Button.onClick.AddListener(() => UI.RecruitFinish());
    //}
    public void Recruit(VillagerClip Clip) //招募
    {
        GameObject Prefab = Instantiate(villager_Prefab, Prefab_List);
        Villager Script = Prefab.GetComponent<Villager>();
        villagers.Add(Script);
        Script.villager = Instantiate(villager_Prefab, Prefab.transform);
        Script.villager.transform.position = Spawn.position;
        Script.V_Data = Clip;
    }
    #endregion
    public void initialize()
    {
        int A = 0;
        while(A< BaseValue)
        {
            GenerateVIllager();
            A++;
        }
    }
    public void Flow()
    {
            for (int i = 0; i < villagers.Count; i++)
            {
                villagers[i].V_Data.Stats.Flow();
            }
    }
    public void GenerateVIllager()
    {
        GameObject Obj = Instantiate(villager_Prefab, Prefab_List);
        Villager Script = Obj.GetComponent<Villager>();
        Obj.transform.SetAsFirstSibling();
        Script.villager.transform.position = new Vector2(Spawn.position.x+Random.Range(-5,5),Spawn.position.y);
        Script.GM = GM;
        villagers.Add(Script);
        VillagerClip V_Data = ScriptableObject.CreateInstance<VillagerClip>();
        Script.V_Data = V_Data;
        string Name = FistName[Random.Range(0, FistName.Count)] +" "+ LastName[Random.Range(0, LastName.Count)];
        V_Data.Generate(Name,AllAapparelImage, DefaultFoodFlow, DefaultFatigueFlow);
        Obj.GetComponent<SortingGroup>().sortingOrder = VillagerUI.Count;
        GenerateUI(V_Data);
    }
    #region 每小時判斷狀態
    public void SetTime(int i)
    {
        if((int)_NowTime!=i)
        {
            _NowTime = (VillagerManger_Time)i;
            SwitchVillagerState();
        }
    }
    void SwitchVillagerState()
    {
        switch (_NowTime)
        {
            case VillagerManger_Time.Sun:
                ActionAllVillagerState(Villager_AI.Working);
                break;
            case VillagerManger_Time.SunDown:
                ActionAllVillagerState(Villager_AI.GoHome);
                break;
            case VillagerManger_Time.Night:
                break;
            case VillagerManger_Time.SunUp:
                break;
        }
    }
    void ActionAllVillagerState(Villager_AI AI)
    {
        for(int i=0;i< villagers.Count;i++)
        {
            villagers[i].V_Data.Stats.AI = AI;
        }
    }
    #endregion
    #region 生成記錄檔
    public void Load(R_Villager date)
    {
        GameObject gameObject = Instantiate(villager_Prefab, Prefab_List);
        Villager Script = gameObject.GetComponent<Villager>();
        Script.GM = GM;
        Script.V_Data = LoadVillagerClip(date);
        Script.Load(date);
        villagers.Add (Script);
    }
    private VillagerClip LoadVillagerClip(R_Villager date)
    {
        VillagerClip clip = ScriptableObject.CreateInstance<VillagerClip>();
        date.LoadVillagerClip(out string ClipName, out int HomeID, out string HomeClip, out int WorkBuildingID, out string WorkBuildingClip);
        clip.Stats = date.LoadState();
        clip.name = ClipName;
        clip.Home = FindBuilding(clip,HomeID, HomeClip,false);
        clip.Working = FindBuilding(clip,WorkBuildingID, WorkBuildingClip,true);
        AllAapparelImage.Load(date, out Sprite Headwear, out Sprite Clothes, out Sprite Sleeve, out Sprite Shoes);
        clip.LoadAapparel(ref Headwear, ref Clothes,ref Sleeve,ref Shoes);
        return clip;
    }
    private GameObject FindBuilding(VillagerClip Villager,int ID, string Clip,bool WorkPlace)
    {
        for (int i = 0; i < GM.builderSystem.Complate.Count; i++)
        {
            Building building = GM.builderSystem.Complate[i].GetComponent<Building>();
            if (building.GotID == ID && building.clip.name == Clip)
            {
                if(WorkPlace)
                {
                    building.clip.CheckJobVacancies(Villager);
                }
                return GM.builderSystem.Complate[i];
            }
        }
        return null;
    }
    #endregion
    #region 生成UI
    public void GenerateUI(VillagerClip Data)
    {
        VillagerUI.Add(VillagerUi(Data));
    }
    public void LoadUI() 
    {
        RemoveUI();
        for(int i=0;i< villagers.Count;i++)
        {
            VillagerUI.Add(VillagerUi(villagers[i].V_Data));
        }
    }
    void RemoveUI()
    {
        for(int i=0;i< VillagerUI.Count;i++)
        {
            Destroy(VillagerUI[i]);
        }
        VillagerUI.Clear();
    }
    GameObject VillagerUi(VillagerClip Data)
    {
        GameObject _ui = Instantiate(UIPrfab, UIList);
        VillagerFormUI Script = _ui.GetComponent<VillagerFormUI>();
        Script.Generate(Data, UIGroup);
        Script.VM = this;
        return _ui;
    }
    #endregion
}
[System.Serializable]
public class VillagerAapparelSprite
{
    [SerializeField] private List<Sprite> Headwears = new List<Sprite>();
    [SerializeField] private List<Sprite> Clothes = new List<Sprite>();
    [SerializeField] private List<Sprite> Sleeves = new List<Sprite>();
    [SerializeField] private List<Sprite> Shoes = new List<Sprite>();
    public void Generate(out Sprite Clothe,out Sprite Sleeve,out Sprite Shoe)
    {
        Clothe = Clothes[Random.Range(0, Clothes.Count)];
        Sleeve = Sleeves[Random.Range(0, Sleeves.Count)];
        Shoe = Shoes[Random.Range(0, Sleeves.Count)];
    }
    public void Load(R_Villager date,out Sprite Headwear, out Sprite Clothes, out Sprite Sleeves,out Sprite Shoes)
    {
        date.LoadVillagerAapparelClip(out string _Clothes, out string _HeadWear, out string _Sleeves, out string _Shoes);
        Clothes = FindClothe(_Clothes);
        Headwear = FindHeadwear(_HeadWear);
        Sleeves = FindSleeves(_Sleeves);
        Shoes = FindShoes(_Shoes);
    }
    private Sprite FindClothe(string Clothe)
    {
            for (int i = 0; i < Clothes.Count; i++)
            {
                if (Clothe == Clothes[i].name)
                {
                    return Clothes[i];
                }
            }
        return null;
    }
    private Sprite FindHeadwear(string Headwear)
    {
        for (int i = 0; i < this.Headwears.Count; i++)
        {
            if (Headwear == this.Headwears[i].name)
            {
                return this.Headwears[i];
            }
        }
        return null;
    }
    private Sprite FindSleeves(string Sleeves)
    {
        for (int i = 0; i < this.Sleeves.Count; i++)
        {
            if (Sleeves == this.Sleeves[i].name)
            {
                return this.Sleeves[i];
            }
        }
        return null;
    }
    private Sprite FindShoes(string Shoes)
    {
        for (int i = 0; i < this.Shoes.Count; i++)
        {
            if (Shoes == this.Shoes[i].name)
            {
                return this.Shoes[i];
            }
        }
        return null;
    }
}
