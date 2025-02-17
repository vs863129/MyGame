using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    protected int ID;
    protected GameManger GM;
    [SerializeField] protected bool IsInteractive; //是否可互動
    [SerializeField] protected GameObject InteractiveUI;
    [SerializeField] protected GameObject VillagerObj;
    [SerializeField] protected VillagerAnmation AM_Villager;
    [SerializeField] SpriteRenderer[] AllImage;
    public BuildingClip clip;
    public float ChangeColorTime;
    bool IsChangeColor;
    private void Awake()
    {
        GM = GameObject.Find("GameManger").GetComponent<GameManger>();
    }
    protected virtual void Start()
    {
        ID = GetID();
    }
    int GetID()
    {
        int _ID = 0;
        for (int i = 0; i < GM.builderSystem.Complate.Count; i++)
        {
            Building building = GM.builderSystem.Complate[i].GetComponent<Building>();
            if (building.clip.name == clip.name)
            {
                if (building.GetInstanceID() == GetInstanceID())
                {
                    break;
                }
                else
                {
                    _ID++;
                }
            }
        }
        return _ID;
    }
    void Update()
    {
        if (IsInteractive && InteractiveUI.activeSelf && IsOpen())
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenUI();
            }
        }
        if(clip.HaveWorkPeople&& VillagerObj)
        {
            VillagerObj.SetActive(true);
        }
        else if(VillagerObj)
        {
            VillagerObj.SetActive(false);
        }
    }
    protected virtual void FixedUpdate()
    {
        WhenBuilding();
    }
    protected void WhenBuilding()//建造期間,建築物遇到該阻擋物
    {
        if (ChangeColorTime > 0.1f)
        {
            IsChangeColor = false;
            foreach (SpriteRenderer ObjColor in AllImage)
            {
                ObjColor.color = Color.red;
            }
            ChangeColorTime -= Time.fixedDeltaTime;
        }
        else
        {
            if(!IsChangeColor)
            {
                foreach (SpriteRenderer ObjColor in AllImage)
                {
                    ObjColor.color = Color.white;
                }
                IsChangeColor = true;
            }
        }
    }
    public void Enter(Villager villager,bool Enter)
    {
        if(Enter)
        {
            clip.EnterWorkingPlace(villager.V_Data);
            if(AM_Villager)
            {
                AM_Villager.SwapVillagerShow(villager.GetApparel);
            }
        }
        else
        {
            clip.LeaveWorkingPlace(villager.V_Data);
        }
    }
    protected  bool VillagerIsWorking { get { if (clip.WorkingPeple.Count != 0) { return true; } return false; } }
    protected void GotWorkOrder(OrderClip WorkOrder)
    {
        for (int i = 0; i < clip.JobVacancies.Length; i++)
        {
            if (clip.JobVacancies[i] != null)
            {
                if (!clip.JobVacancies[i].WorkOredr)
                {
                    clip.JobVacancies[i].WorkOredr = Instantiate(WorkOrder);
                    clip.JobVacancies[i].WorkOredr.WokrBuilding = transform;
                }
            }
        }
    }
    protected virtual void OpenUI() { }
    protected bool IsOpen()
    {
        if (clip.WorkingPeple.Count != 0)
        {
            return true;
        }
        return false;
    }
    #region 外部使用
    public void Remove()
    {
        Destroy(gameObject);
    }
    #region 存取
    public int GotID
    {
        get
        { return ID; }

    }
    public void Save(out int ID,out BuildingClip buildingclip, out Vector3 vector3)
    {
        ID = this.ID;
        buildingclip = clip;
        vector3 = transform.position;
    }
    public int LoadID
    {
        set
        {
            ID = value;
        }
    }
    #endregion
    #endregion
}