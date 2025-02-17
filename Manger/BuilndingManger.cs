using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BuilndingManger : MonoBehaviour
{
    public GameObject Move;
    public float CellSize; //格線
    [Header("已建築")]
    public List<GameObject> Complate = new List<GameObject>();
    [Header("替換區")]
    public BuildingClip Building_Clip;
    public GameObject NowBuilding;
    [Header("介面")]
    public GameObject ShowText;
    public GameObject ShopUI;
    public GameObject N_UI;
    //[Header("顯示區")]
    //public GameObject TextPrefab;
    //public Transform TextParent;
    //public GameObject Line;
    [Header("偵測區")]
    public Transform Mouse;
    public Transform CheckPos;
    [Header("微調區")]
    public Vector2 value;//位移
    [Header("其他系統")]
    public BuildingUI_Setting UISetting;//生成所有建築按鈕和部分資源判定
    public GameManger GM;//主系統
    private float T_Show;
    [Header("需要解鎖的按鈕")]
    [Tooltip("0 研究")]
    [SerializeField] GameObject[] Buttons;
    //[Header("網格")] //暫時用不到
    //public Grid Grid;
    public void OnBuiliding(bool InBuilding)
    {
        if(InBuilding)
        {
            GM.UIManger.InBuilding = this;
        }
        else
        {
            Building_Clip = null;
            Cancel(true);
        }

    }
    public void Switch(BuildingClip Clip) //替換
    {
        if (Building_Clip != Clip)
        {
            UISetting.TotalCost(Clip.Name,Clip.CostRes);
            Building_Clip = Clip;
            NowBuilding.SetActive(true);
            ShowText.transform.position = new Vector3(Clip.WCellsize / 2, Clip.HCellsize) + transform.position;
            NowBuilding.GetComponent<SpriteRenderer>().sprite = Clip.BuildingImage;
            NowBuilding.GetComponent<SpriteRenderer>().color = Color.white;
            NowBuilding.GetComponent<BoxCollider2D>().size = new Vector2(Clip.WCellsize, Clip.HCellsize);
            NowBuilding.GetComponent<BoxCollider2D>().enabled = true;
            NowBuilding.transform.position = Vector3.zero;
            NowBuilding.transform.position = (Move.transform.position - NowBuilding.transform.position) + new Vector3(0.5f * Clip.WCellsize, 0.5f * Clip.HCellsize);
            //NowBuilding.transform.position = Move.transform.position - NowBuilding.transform.position + new Vector3(0.5f* Clip.W_Cellsize, -0.5f * Clip.H_Cellsize);
            CheckPos.position = NowBuilding.transform.position + new Vector3(0, -0.5f * Clip.HCellsize, 0);
            if(!IsInvoking(nameof(R_CheckResource)))
            {
                InvokeRepeating(nameof(R_CheckResource), 0, 0.5f);
            }
        }
        else
        {
            Cancel(false);
        }

        //value = new Vector2(0.5f * Clip.W_Cellsize, 0.5f * Clip.H_Cellsize);
        //Grid.DarwGrid(TextPrefab, TextParent, Line, Clip.W_Cellsize + 1, Clip.H_Cellsize + 1, 1f); //暫時不使用
    }
    public void Cancel(bool External)
    {
        if (External)
        {
            UISetting.ToggleGroup.SetAllTogglesOff();
            CancelInvoke(nameof(R_CheckResource));
        }
        Building_Clip = null;
        NowBuilding.SetActive(false);
        UISetting.Remove();
    }
    private void Update()
    {
        ShowTime();
        if (Building_Clip)
        {
            MouseSettings();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel(true);
        }
    }
    #region 持續確認需求資源
    void R_CheckResource()
    {
        Debug.Log("BuildingCheck");
        if(Building_Clip!=null)
        {
            UISetting.Isenough(GM);
        }
        else
        {
            CancelInvoke(nameof(R_CheckResource));
        }
    }
    #endregion
    private void ShowTime()
    {
        if(T_Show!=0)
        {
            T_Show -= Time.fixedDeltaTime;
            if(T_Show<0.1f)
            {
                T_Show = 0;
                ShowText.SetActive(false);
            }
        }
    }
    private void LateUpdate()
    {

        if (Building_Clip)
        {
            RaycastHit2D hit = Physics2D.Raycast(CheckPos.position, Vector3.down, 1, 1 << LayerMask.NameToLayer("Ground"));
            if (hit.collider)
            {
                CheackGround(hit.point.y);
            }
            else
            {
                NowBuilding.GetComponent<BuildingCheck>().Check(false);
                Vector2 OverPos = new Vector2(Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).x / CellSize) * CellSize - (int)value.x, Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).y / CellSize) * CellSize - (int)value.y);
                Move.transform.position = OverPos;
            }
            if (NowBuilding.GetComponent<BuildingCheck>().CanBuilding && Input.GetMouseButtonDown(0)&&!GM.UIManger.MousePointInUI)//建立建築
            {
                Vector3 OverBuildingPos = new Vector3(0.5f * Building_Clip.WCellsize, 0.5f * Building_Clip.HCellsize);
                GameObject Isenough = Building_Clip.Build(Move.transform.position + OverBuildingPos, UISetting.CostList);
                if (Isenough)//符合條件則生成建築與編入Data
                {
                    GM.PayResource(UISetting.CostList);
                    UISetting.Isenough(GM);
                    Complate.Add(Isenough);
                    UnLockButton(Isenough.GetComponent<Building>().clip);
                }
                else
                {
                    ShowText.SetActive(true);
                    T_Show = 3;
                }
            }
        }
        else
        {
            NowBuilding.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            NowBuilding.GetComponent<BoxCollider2D>().enabled = false;
        }
        
    }
    void UnLockButton(BuildingClip Clip)
    {
        switch (Clip.m_WorkPosition)
        {
            case WorkPosition.Researcher:
                Buttons[0].SetActive(true);
                Buttons[0].GetComponent<Button>().interactable = true;
                break;
        }
    }
    public void CheackGround(float hitposY)
    {
        Vector2 OverPos = new Vector2(Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).x / CellSize) * CellSize - (int)value.x, Mathf.Floor(Camera.main.ScreenToWorldPoint(Input.mousePosition).y / CellSize) * CellSize - (int)value.y);
        Move.transform.position = OverPos;
        if (!NowBuilding.GetComponent<BuildingCheck>().Isobstructed)
        {
            NowBuilding.GetComponent<BuildingCheck>().Check(true);
        }
    }
    public void MouseSettings()
    {
        Mouse.transform.position = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
    }
    #region 載入
    public void Load(R_Building date)
    {
        date.Load(out int ID, out string ClipName, out Vector3 vector3, out int HabitablePeople, out int JobVacancies);
        GameObject Building = BuildingLoadDate(ClipName, vector3);
        Building.GetComponent<Building>().LoadID = ID;
        Building.GetComponent<Building>().clip.HabitablePeople = new VillagerClip[HabitablePeople];
        Building.GetComponent<Building>().clip.JobVacancies = new VillagerClip[JobVacancies];
        #region 其他建築設定載入
        if(Building.GetComponent<Shop>())
        {
            Shop LoadDate = Building.GetComponent<Shop>();
            LoadDate.ShopCargo = date.CargoList(GM.DateBase.A_Items);
        }
        #endregion
        Complate.Add(Building);
    }
    private GameObject BuildingLoadDate(string ClipName,Vector3 Pos)
    {
        for (int i = 0; i<GM.DateBase.AllBuildingClip.Count;i++)
        {
            if(GM.DateBase.AllBuildingClip[i].name== ClipName)
            {
                return GM.DateBase.AllBuildingClip[i].SpawnBuilding(Pos);
            }
        }
        return null;
    }
    #endregion
}

[System.Serializable]
public class Grid //暫時不用
{
    private int width;
    private int height;
    private float cellSize;
    private int[,] gridArray;
    private Vector2 GridPoint;
    public List<LineClip> LineList = new List<LineClip>();
    public void DarwGrid(GameObject TextObj, Transform TextObjparent,GameObject Line, int width, int height, float cellSize)
    {
        this.width = width; 
        this.height = height;
        this.cellSize = cellSize;
        gridArray = new int[width, height];
        Clear();
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                if (y == gridArray.GetLength(1) - 1)
                {
                    var objX = Object.Instantiate(Line);
                    objX.name = "X";
                    objX.GetComponent<LineRenderer>().SetPosition(0, new Vector3(x, 0, 0));
                    objX.GetComponent<LineRenderer>().SetPosition(1, new Vector3(x, y, 0));
                    LineList.Add(new LineClip {Prefab= objX,FristPos = objX.GetComponent<LineRenderer>().GetPosition(0), SecondPos = objX.GetComponent<LineRenderer>().GetPosition(1) });
                }
                if (x == gridArray.GetLength(0) - 1)
                {
                    var objY = Object.Instantiate(Line);
                    objY.name = "Y";
                    objY.GetComponent<LineRenderer>().SetPosition(0, new Vector3(0, y, 0));
                    objY.GetComponent<LineRenderer>().SetPosition(1, new Vector3(x, y, 0));
                    LineList.Add(new LineClip { Prefab = objY, FristPos = objY.GetComponent<LineRenderer>().GetPosition(0), SecondPos = objY.GetComponent<LineRenderer>().GetPosition(1) });
                }
            }
        }
        //for(int x=0;x<gridArray.GetLength(0);x++)
        // {
        //     for(int y=0;y<gridArray.GetLength(1);y++)
        //     {
        //         CreateText(TextObj, parent,x+":"+y, GetWorldPositon(x,y)+new Vector3(cellSize, cellSize)*0.5f);
        //         Debug.DrawLine(GetWorldPositon(x, y), GetWorldPositon(x, y + 1), Color.white, 100f);
        //         Debug.DrawLine(GetWorldPositon(x, y), GetWorldPositon(x + 1, y), Color.white, 100f);

        //     }
        // }
        // Debug.DrawLine(GetWorldPositon(0, height), GetWorldPositon(width, height), Color.white, 100f);
        // Debug.DrawLine(GetWorldPositon(width, 0), GetWorldPositon(width, height), Color.white, 100f);
    }
    public void UpdateGrid(Vector2 obj)
    {
        if(LineList.Count!=0)
        {
        }
        //var GridPoint = obj.transform.position - new Vector3(1.3f, 1.3f,0);
        //Debug.Log((int)GridPoint.x+" "+ (int)GridPoint.y);

    }

    public void Clear()
    {
        for (int List = 0; List < LineList.Count; List++)
        {
            Object.Destroy(LineList[List].Prefab);
        }
        LineList.Clear();

    }
    public void CreateText(GameObject TextObj, Transform parent, string text,Vector3 localPosition)
    {
     var obj= Object.Instantiate(TextObj);
     obj.transform.localPosition = localPosition;
     obj.transform.parent = parent;
     obj.transform.localScale = new Vector3(1, 1, 1);
     var ChangeText = obj.GetComponent<TextMeshProUGUI>();
     ChangeText.text = text;
     
    }
    private Vector3 GetWorldPositon(int x,int y)
    {
        return new Vector3(x, y) * cellSize;
    }

}
[System.Serializable]
public class LineClip//暫時用不到
{
    public GameObject Prefab;
    public Vector3 FristPos;//第一點
    public Vector3 SecondPos;//第二點
    public void UpdatePos(Vector2 obj)
    {

    }
}
[System.Serializable]
public class BuildingUI_Setting //建築面板設定
{
    [Header("建築")]
    public GameObject P_Building_Button;//按鈕預置物
    public List<BuildingClip> CanBuilding=new List<BuildingClip>();//可建築
    public Transform B_parent;//生成點
    public List<GameObject> Button_List = new List<GameObject>();//生成好的按鈕
    public ToggleGroup ToggleGroup; 
    [Header("資源成本")]
    public GameObject P_Resource;
    public List<ResourceUI> CostList = new List<ResourceUI>();
    [SerializeField]
    Transform CostParent;
    public void Start(GameManger GM)
    {
        for (int i = 0; i < CanBuilding.Count; i++)
        {
            AddBuildingButton(GM.builderSystem, CanBuilding[i]);
        }
    }
    public void TotalCost(string BuildingName,List<Cost_Resource> Cost) //顯示成本
    {
        if(CostList.Count!=0)
        {
            Remove();
            LoadCostDate(Cost);
        }
        else
        {
            LoadCostDate(Cost);
        }
    }
    #region 確認資源
    public void Isenough(GameManger GM)
    {
        int A = 0;
        while(A < CostList.Count)
        {
            GM.CheckResource(CostList[A]);
            A++;
        }
    }
    void LoadCostDate(List<Cost_Resource> Cost)
    {
        for(int i=0;i<Cost.Count;i++)
        {
            GameObject CostDate = Object.Instantiate(P_Resource, CostParent);
            CostDate.transform.localScale = new Vector3(1, 1, 1);//避免變形
            ResourceUI Script = CostDate.GetComponent<ResourceUI>();
            Script.SpawnUI( Cost[i].Clip, Cost[i].Value);
            Script.Show.color = Color.red;
            CostList.Add(Script);
        }
    }
 #endregion
    public void Remove()
    {
        foreach (ResourceUI obj in CostList)
        {
            obj.Destroy();
        }
        CostList.Clear();
    }
    public void AddBuildingButton(BuilndingManger BM, BuildingClip clip) //生成按鈕
    {
        GameObject button = Object.Instantiate(P_Building_Button);
        button.transform.SetParent(B_parent);
        button.transform.localScale = new Vector3(1, 1, 1);
        button.GetComponent<Image>().sprite = clip.Icon;
        BuildingButton Setting = button.GetComponent<BuildingButton>();
        Setting.data = clip;
        Setting.Toggle.group = ToggleGroup;
        Setting.Toggle.onValueChanged.AddListener((value) => BM.Switch(clip));
        LanguageSystem.SetText(Setting.Localize, "BuildingName," + clip.name);
        Button_List.Add(button);
    }
}
