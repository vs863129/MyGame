using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
public class ProduceSystem : MonoBehaviour
{
    public ProduceBuilding NowBuilding;
    public GameObject ProduceUITotal
    {
        get
        {
            if (NowBuilding)
            {
                switch (NowBuilding.Type)
                {
                    case ProduceBuildingType.GeneralProductionPlace:
                        ProduceUIPrefab = ProductionUI.ProduceUIPrefab;
                        ProduceList = ProductionUI.ProduceList;
                        CostList = ProductionUI.CostList;
                        QueueList = ProductionUI.QueueList;
                        Group = ProductionUI.ProduceGroup;
                        return ProductionUI.UITotal;
                    case ProduceBuildingType.Smithy:
                        ProduceUIPrefab = SmithyUI.ProduceUIPrefab;
                        ProduceList = SmithyUI.ProduceList;
                        CostList = SmithyUI.CostList;
                        QueueList = SmithyUI.QueueList;
                        Group = SmithyUI.ProduceGroup;
                        return SmithyUI.UITotal;
                }
            }
            else
            {
                ProduceUIPrefab = ProductionUI.ProduceUIPrefab;
                ProduceList = ProductionUI.ProduceList;
                CostList = ProductionUI.CostList;
                QueueList = ProductionUI.QueueList;
                Group = ProductionUI.ProduceGroup;
                return ProductionUI.UITotal;
            }
            return null;
        }
    }
    [SerializeField] ProduceClip NowProduce;
    [Header("可製作項目")]
    [SerializeField] GameObject ProduceUIPrefab;
    [SerializeField] Transform ProduceList;
    List<GameObject> AllProduceUI = new List<GameObject>();
    [Header("需求資源")]
    [SerializeField] GameObject CostResourceUI;
    [SerializeField] Transform CostList;
     List<ResourceUI> AllCostList = new List<ResourceUI>();
    [Header("隊列")]
    [SerializeField] GameObject QueuePrefab;
    [SerializeField] Transform QueueList;
    List<QueueProduceUI> AllQueueList = new List<QueueProduceUI>();
    public List<QueueProduceUI> GotAllQueueList
    {
        get
        {
            return AllQueueList;
        }
    }
    [Header("製造介面")]
    [SerializeField] GeneralProductionUI ProductionUI;//普通製造介面
    [SerializeField] SmithyUI SmithyUI;//鍛造介面
    [Header("UI")]
    [Tooltip("0 Name 1 製造介紹")]
    [SerializeField] LocalizeStringEvent[] UI_Text;
    [Header("其他系統")]
    [SerializeField] HandicraftSystem Handicraft;
    [SerializeField] ToggleGroup Group;
    [SerializeField] GameManger GM;
    public void OpenUI(bool Open)
    {
        if (NowBuilding)
        {
            if(Open)
            {
                NowBuilding.GotAllList(out List<ProduceClip> AllProduceList, out List<ProduceClip> AllQueueList);
                switch (NowBuilding.Type)
                {
                    case ProduceBuildingType.GeneralProductionPlace:
                        LoadUI( AllProduceList, AllQueueList);
                        break;
                    case ProduceBuildingType.Smithy:
                        var Script= NowBuilding.GetComponent<SmithyBuilding>();
                        SmithyUI.GenerateFuelSlotsUI(Script);
                        LoadUI( AllProduceList, AllQueueList);
                        break;
                }
            }
            else
            {
                CancelInvoke(nameof(CheckResource));
                CancelInvoke(nameof(UpdateBar));
                NowProduce = null;
            }
        }
    }
    #region 切換頁面
    public void SwitchPage(bool IsOn)
    {
        if (NowBuilding)
        {
            var SmithyScript = NowBuilding.GetComponent<SmithyBuilding>();
            if (IsOn)
            {
                ClearPage();
                SmithyUI.ClearRecipeUI();
                switch (SmithyUI.GotActionToggleName)
                {
                    case "ForgePage":
                        LoadUI(SmithyScript.ForgeList);
                        break;
                    case "MeltPage":
                        LoadUI(SmithyScript.MeltList);
                        break;
                    case "FuelPage":
                        LoadUI(SmithyScript);
                        break;
                    default:
                        Debug.Log("名稱有誤請檢查");
                        break;
                }
            }
        }
    }
    void ClearPage()
    {
        foreach (GameObject gameObject in AllProduceUI)
        {
            Destroy(gameObject);
        }
        AllProduceUI.Clear();
        ClearCostUI();
    }
    #endregion
    #region 生成Produce選項
    public void GenerateProduceUI(ProduceClip ProduceClip,HandicraftSystem handicraftSystem)
    {
        GameObject NewProduceUI = Instantiate(ProduceUIPrefab, ProduceList);
        ProduceUI Script = NewProduceUI.GetComponent<ProduceUI>();
        Script.Generate(ProduceClip);
        Script.GetComponent<Toggle>().group = Group;
        Script.GetComponent<Toggle>().onValueChanged.AddListener((value) => Script.GenerateCostResoruceUI(GM,ref UI_Text, ref NowProduce, CostResourceUI, CostList, AllCostList, value));
        switch (ProduceClip.Type)
        {
            case ProduceType.Handicraft:
                Script.GetComponent<Toggle>().onValueChanged.AddListener((value) => HandIcraft_Action(handicraftSystem, Script));
                break;
            default:
                Script.GetComponent<Toggle>().onValueChanged.AddListener((value) => Action(Script));
                break;
        }
        
        AllProduceUI.Add(NewProduceUI);
    }
    public void GenerateQueueUI(ProduceClip ProduceClip,bool IsUI)
    {
        GameObject NewQueueProduce = Instantiate(QueuePrefab, QueueList);
        QueueProduceUI Script = NewQueueProduce.GetComponent<QueueProduceUI>();
        if(IsUI)
        {
            Script.GenerateUI(ProduceClip, GM.ResourceSystem, this);
        }
        else
        {
            Script.Generate(ProduceClip, GM.ResourceSystem, this);
        }
        AllQueueList.Add(Script);
    }
    #endregion
    #region ProduceUI讀取各類方式
    void LoadUI( List<ProduceClip> ProduceList, List<ProduceClip> QueueList)
    {
        ClearAllUI();
        int A = 0;
        int B = 0;
        while (A < ProduceList.Count)
        {
            GenerateProduceUI(ProduceList[A],null);
            A++;
        }
        while(B < QueueList.Count)
        {
            GenerateQueueUI(QueueList[B],true);
            B++;
        }
        InvokeRepeating(nameof(UpdateBar), .1f, 1);
        InvokeRepeating(nameof(CheckResource), 1, 1);
    }
    void LoadUI(List<ProduceClip> ProduceList)
    {
        ClearProduceUI();
        int i = 0;
        while(i< ProduceList.Count)
        {
            GenerateProduceUI(ProduceList[i],null);
            i++;
        }
        if(!IsInvoking(nameof(CheckResource)))
        {
            InvokeRepeating(nameof(CheckResource), 1, 1);
        }
    }
    public void LoadUI(HandicraftSystem handicraft)
    {
        ClearAllUI();
        handicraft.GotProduceList(out List<ProduceClip> ProduceList, out List<ProduceClip> QueueList);
        int A = 0;
        int B = 0;
        while (A < ProduceList.Count)
        {
            GenerateProduceUI(ProduceList[A], handicraft);
            A++;
        }
        while (B < QueueList.Count)
        {
            GenerateQueueUI(QueueList[B], true);
            B++;
        }
        InvokeRepeating(nameof(CheckResource), 1, 1);
    }
    void LoadUI(SmithyBuilding Smithy )
    {
        SmithyUI.GenerateFuelUI(Smithy, CostResourceUI, AllCostList,GM.ResourceSystem);
    }
    #endregion
    void UpdateBar()
    {
        if(NowBuilding)
        {
            NowBuilding.UpdateQueueBar(ref AllQueueList);
        }
        else
        {
            CancelInvoke(nameof(UpdateBar));
        }

    }
    bool CheckResource()
    {
        if (NowProduce)
        {
            bool IsEnough = true;
            if (AllCostList.Count != 0)
            {
                int i = 0;
                while (i < AllCostList.Count)
                {
                    if(!GM.CheckResource(AllCostList[i]))
                    {
                        IsEnough = false;
                    }
                    i++;
                }
            }
            return IsEnough;
        }
        return false;
    }
    #region 開始製作
    public void Action(ProduceUI UI)
    {
        if (UI.IsDoublePress())
        {
            if (NowProduce != null)
            {
                if (CheckResource())
                {
                    if (NowBuilding.GotQueueList.Count < NowBuilding.MaxQueue)
                    {
                        CostResource();
                        if (AllQueueList.Count != 0)
                        {
                            for (int i = 0; i < AllQueueList.Count; i++)
                            {
                                if (AllQueueList[i].Produce.item == NowProduce.item && !AllQueueList[i].Produce.IsFull)
                                {
                                    AllQueueList[i].Produce.GotItem();
                                    AllQueueList[i].UpdateBar();
                                    return;
                                }
                            }
                        }
                        ProduceClip produce = Instantiate(NowProduce);
                        GenerateQueueUI(produce, false);
                        NowBuilding.AddProduce(produce);

                    }
                    else
                    {
                        Debug.Log("隊列已滿");
                    }
                }
                else
                {
                    Debug.Log("手速很快喔");
                }
            }
        }
    }
    public void HandIcraft_Action(HandicraftSystem handicraft,ProduceUI UI)
    {
        if (UI.IsDoublePress())
        {
            if (CheckResource())
            {
                CostResource();
                if (AllQueueList.Count != 0)
                {
                    for (int i = 0; i < AllQueueList.Count; i++)
                    {
                        if (AllQueueList[i].Produce.item == NowProduce.item && !AllQueueList[i].Produce.IsFull)
                        {
                            AllQueueList[i].Produce.GotItem();
                            AllQueueList[i].UpdateBar();
                            return;
                        }
                    }
                }
                ProduceClip produce = Instantiate(NowProduce);
                GenerateQueueUI(produce, false);
                handicraft.Action(produce);
            }
        }
    }
    void CostResource()
    {
        int Cost = 0;
        while(Cost<NowProduce.Cost.Count)
        {
            int NeedCostValue = NowProduce.Cost[Cost].Value;
            int PlayerInv = 0;
            while(PlayerInv<GM.InventorySystem.SlotsUI.Count)
            {
                if(GM.InventorySystem.SlotsUI[PlayerInv].Item.Clip== NowProduce.Cost[Cost].Clip)
                {
                    GM.InventorySystem.RemoveItem(GM.InventorySystem.SlotsUI[PlayerInv], NeedCostValue);
                    if (NeedCostValue<=0)
                    {
                        break;
                    }
                }
                PlayerInv++;
            }
            if(NeedCostValue>0)
            {
                int Store = 0;
                while (Store < GM.ResourceSystem.Store.Count)
                {
                    if (GM.ResourceSystem.Store[Store].Clip == NowProduce.Cost[Cost].Clip)
                    {
                        GM.ResourceSystem.Store[Store].CostValue(NeedCostValue, ref GM.ResourceSystem.Store);
                        break;
                    }
                    Store++;
                }
            }
            Cost++;
        }
    }
    #endregion
    public void RmoveQueueUI(QueueProduceUI Obj)
    {
        int i = 0;
        while (i < AllQueueList.Count)
        {
            if (AllQueueList[i] == Obj)
            {
                switch (Obj.Produce.Type)
                {
                    case ProduceType.Handicraft:
                        Handicraft.GotProduceList(out List<ProduceClip> ProduceList, out List<ProduceClip> QueueList);
                        QueueList.RemoveAt(i);
                        AllQueueList[i].Destroy();
                        AllQueueList.Remove(Obj);
                        break;
                    default:
                        NowBuilding.GotQueueList.RemoveAt(i);
                        AllQueueList[i].Destroy();
                        AllQueueList.Remove(Obj);
                        break;
                }
                break;
            }
            i++;
        }
    }
    void ClearCostUI()
    {   int i = 0;
        while(i<AllCostList.Count)
        {
            AllCostList[i].Destroy();
            i++;
        }
        AllCostList.Clear();
    }
    void ClearProduceUI()
    {
        int i = 0;
        while(i< AllProduceUI.Count)
        {
            Destroy(AllProduceUI[i]);
            i++;
        }
        AllProduceUI.Clear();
    }
    void ClearAllUI()
    {
        int A = 0;
        int C = 0;
        while(A< AllProduceUI.Count)
        {
            Destroy(AllProduceUI[A]);
            A++;
        }
        AllProduceUI.Clear();
        ClearCostUI();
        AllCostList.Clear();
        while(C<AllQueueList.Count)
        {
            AllQueueList[C].Destroy();
            C++;
        }
        AllQueueList.Clear();
    }
}
