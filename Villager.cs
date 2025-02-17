using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Villager : MonoBehaviour
{
    public QuestClip Quest;
    public VillagerClip V_Data;
    public GameObject villager;
    public Transform Place;
    [Header("屬性")]
    public float Speed = 1;
    [Header("判定")]
    [SerializeField] private GameObject QuestUI;
    [SerializeField] private Collider2D GroundCheckPos;
    [SerializeField] private BoxCollider2D TalkRange;
    bool _Flip;
    public bool FlipX
    {
        get { return _Flip; }
        set
        {
            if (value)
            {
                villager.transform.eulerAngles = new Vector2(0, 180);
                //UI.UI.localRotation = Quaternion.Euler(0, -180, 0);
                _Flip = value;
            }
            else
            {
                villager.transform.eulerAngles = new Vector2(0, 0);
                //UI.UI.localRotation = Quaternion.Euler(0, 0, 0);
                _Flip = value;
            }
        }
    }
    [Header("隨機移動")]
    int m_States;
    float m_IdleMoveChangeTime;
    Vector2 m_RandomMovePoint;
    bool m_RandomMove;
    [Header("其他系統")]
    private Animator AM;
    private Rigidbody2D RB;
    public GameManger GM;
    [SerializeField] GameObject InteractiveBar;
    [SerializeField] VillagerApparel Aapparel;
    public VillagerApparel GetApparel
    {
        get { return Aapparel; }
    }
    private void Awake()
    {
       RB = villager.GetComponent<Rigidbody2D>();
       AM = villager.GetComponent<Animator>();
    }
    private void Start()
    {
        Aapparel.LoadBase(V_Data);
    }
    public void EnterPlace(Transform _Place)//進入區域
    {
        if(V_Data.Stats.AI == Villager_AI.Working&&!Place)
        {
            V_Data.Working.GetComponent<Building>().Enter(this,true);
        }
        Place = _Place;
        villager.SetActive(false);
    }
    public void LeavePlace()//離開區域
    {
        if(Place==V_Data.Working.transform)
        {
            V_Data.Working.GetComponent<Building>().Enter(this, false);
        }
        Aapparel.LoadBase(V_Data);
        villager.SetActive(true);
        Place = null;
    }
    private void Update()
    {
        if(villager.activeInHierarchy)
        {
            GroundCheck();
            Interactive();
        }
    }
    bool TalkCheck()
    {
        if (TalkRange.IsTouchingLayers(1 << LayerMask.NameToLayer("Player")))
        {
            return true;
        }
        return false;
    }
    private void Interactive()
    {
        if(Quest)
        {
            //if (Input.GetKey(KeyCode.E)&& TalkCheck())
            //{
            //    GM.QuestSystem.GiveQuest(Quest);
            //    Quest = null;
            //}
        }
    }
    private void FixedUpdate()
    {
        FindAnyBuilding();
        if (Quest&& villager.activeInHierarchy)
        {
            QuestUI.SetActive(true);
        }
        else
        {
            QuestUI.SetActive(false);
        }
        if(villager.activeSelf)
        {
            AM.SetFloat("Speed", RB.velocity.x);
        }
        switch (V_Data.Stats.AI)
        {
            case Villager_AI.Idle:
                { 
                    if(Place)
                    {
                        LeavePlace();
                    }
                    if(V_Data.Working)
                    {
                        V_Data.Stats.AI = Villager_AI.Working;
                    }
                    RandemMove();
                }
                break;
            case Villager_AI.GoHome:
                if (V_Data.Home)
                {
                    MoveToPlace(V_Data.Home.transform);
                }
                break;
            case Villager_AI.Working:

                if (V_Data.Working)
                {
                    if (!V_Data.WorkOredr)
                    {
                        MoveToPlace(V_Data.Working.transform);
                    }
                    else
                    {
                        V_Data.WorkOredr.villager = this;
                        V_Data.WorkOredr.Wokring();
                    }
                }
                else
                {
                    V_Data.Stats.AI = Villager_AI.Idle;
                }
                break;
        }
    }
    void RandemMove()
    {
        if (V_Data.Stats.IsGround)
        {
            if (m_IdleMoveChangeTime < 0.1f)
            {
                m_States = Random.Range(-1, 2);
                m_RandomMove = false;
                m_IdleMoveChangeTime = Random.Range(5, 10);
            }
            else
            {
                m_IdleMoveChangeTime -= Time.fixedDeltaTime;
                switch (m_States)
                {
                    case int State when State > 0:
                        if (!m_RandomMove)
                        {
                            float direction = Random.Range(-2, 2);
                            m_RandomMovePoint = new Vector2(villager.transform.position.x + direction, villager.transform.position.y);

                            m_RandomMove = true;
                        }
                        else
                        {
                            if (Move(m_RandomMovePoint))
                            {
                                m_IdleMoveChangeTime = 0;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
    void FindAnyBuilding()
    {
        #region 找居住地
        if(!V_Data.Home)
        {
            if(!IsInvoking(nameof(FindHome)))
            {
                InvokeRepeating(nameof(FindHome), 1, 1);
            }
        }
        else
        {
            if (IsInvoking(nameof(FindHome)))
            {
                CancelInvoke(nameof(FindHome));
            }
        }
        #endregion
        #region 找工作
        if (!V_Data.Working)
        {
            if (!IsInvoking(nameof(FindJob)))
            {
                InvokeRepeating(nameof(FindJob), 1, 1);
            }
        }
        else
        {
            if (IsInvoking(nameof(FindJob)))
            {
                CancelInvoke(nameof(FindJob));
            }
        }
        #endregion
    }
    public void MoveToPlace(Transform Place)
    {

        if (this.Place != Place)
        {
            if (this.Place)
            {
                LeavePlace();
            }
            if (Move(Place.position))
            {
                EnterPlace(Place.transform);
            }
        }
    }
    private void FindHome()
    {
        if (GM.builderSystem.Complate.Count == 0)
        {
            //Debug.Log("需要居住空間");
            return;
        }
        for (int i = 0; i < GM.builderSystem.Complate.Count; i++)
        {
            BuildingClip Building = GM.builderSystem.Complate[i].GetComponent<Building>().clip;
            if (Building.CheckVacancy(V_Data))
            {
                V_Data.Home = GM.builderSystem.Complate[i];
                CancelInvoke("FindHome");
                return;
            }
            else if(i == GM.builderSystem.Complate.Count-1&& Building.HabitablePeople.Length == 0)
            {
                //Debug.Log("需要居住空間");
            }
        }
    }
    private void FindJob()
    {
        if (GM.builderSystem.Complate.Count == 0)
        {
            //Debug.Log("尋找工作機會");
            return;
        }
        for (int i = 0; i < GM.builderSystem.Complate.Count; i++)
        {
            BuildingClip Building = GM.builderSystem.Complate[i].GetComponent<Building>().clip;
            if (Building.CheckJobVacancies(V_Data))
            {
                V_Data.Working = GM.builderSystem.Complate[i];
                CancelInvoke("FindJob");
                return;
            }
            else if (i == GM.builderSystem.Complate.Count - 1 && Building.JobVacancies.Length == 0)
            {
                //Debug.Log("尋找工作機會");
            }
        }
    }
    public void GroundCheck()
    {

        if (GroundCheckPos.IsTouchingLayers(1 << LayerMask.NameToLayer("Ground")))
        {
            V_Data.Stats.IsGround = true;
        
        }
        else
        {
            V_Data.Stats.IsGround = false;
        }
        AM.SetBool("Ground", V_Data.Stats.IsGround);
        AM.SetFloat("Air", RB.velocity.y);
    }
    public bool Move(Vector2 Target)
    {
        Vector2 TargetVelocity = new Vector2(Speed * 1.8f, RB.velocity.y);
        Vector2 Velocity = Vector2.zero;
        float Distance = villager.transform.position.x - Target.x;
        if (Distance > -0.1f && Distance < 0.1f)
        {
            return true;
        }
        switch (System.Math.Sign(Distance))
        {
            case 1: //左移動
                RB.velocity = Vector2.SmoothDamp(RB.velocity, -TargetVelocity, ref Velocity, .05f);
                if (!_Flip)
                { FlipX = true; }
                return false;
            case -1://右移動
                RB.velocity = Vector2.SmoothDamp(RB.velocity, TargetVelocity, ref Velocity, .05f);
                if (_Flip)
                { FlipX = false; }
                return false;
        }

        return true;
    }
    public void Remove()
    {
        Destroy(gameObject);
    }
    #region 設定滑桿
    public void SetInteractiveBar(Sprite Icon,float Max)
    {
        Slider Bar =InteractiveBar.GetComponent<Slider>();
        Bar.fillRect.GetComponent<Image>().sprite = Icon;
        Bar.maxValue = Max;
    }
    public void Interacting(float value)
    {
        Slider Bar = InteractiveBar.GetComponent<Slider>();
        InteractiveBar.SetActive(true);
        Bar.value = value;
        if (value < 0.1f)
        {
            InteractiveBar.SetActive(false);
        }
    }
    #endregion
    #region 存取
    public void Save(out Transform InBuilding ,out Vector3 Pos)
    {
        InBuilding = Place;
        Pos = villager.transform.position;
    }
    public void SaveClip(out VillagerClip villagerClip)
    {
        villagerClip = V_Data;
    }
    public void Load(R_Villager Date)
    {
        Date.Loadvillager(out bool Outside, out int PlaceID, out string PlaceName, out Vector3 Pos);
        if(Outside)
        {
            Place = null;
        }
        else
        {
            for(int i=0;i < GM.builderSystem.Complate.Count;i++)
            {
               Building Building = GM.builderSystem.Complate[i].GetComponent<Building>();
                if (Building.GotID==PlaceID&& Building.clip.name==PlaceName)
                {
                    Place = GM.builderSystem.Complate[i].transform;
                    villager.SetActive(false);
                    break;
                }
            }
        }
        villager.transform.position = Pos;
    }
    #endregion
}
[System.Serializable]
public class VillagerApparel
{
    [SerializeField] private SpriteRenderer Clothes;
    [SerializeField] private SpriteRenderer Headwear;
    [SerializeField] private SpriteRenderer[] Sleeves;
    [SerializeField] private SpriteRenderer[] Shoes;
    public void LoadBase(VillagerClip clip)
    {
        clip.LoadClothing(out Sprite Clothes, out Sprite Headwear,out Sprite Sleeves,out Sprite Shoes);
        this.Clothes.sprite = Clothes;
        this.Headwear.sprite = Headwear;
        for(int i=0;i< this.Sleeves.Length;i++)
        {
            this.Sleeves[i].sprite = Sleeves;
        }
        for (int i = 0; i < this.Shoes.Length; i++)
        {
            this.Shoes[i].sprite = Shoes;
        }
    }
    public void LoadSprite(out Sprite Headwear, out Sprite Clothes,out Sprite L_Sleeve,out Sprite R_Sleeve, out Sprite L_Shoe,out Sprite R_Shoe)
    {
        Headwear = this.Headwear.sprite;
        Clothes = this.Clothes.sprite;
        L_Sleeve = Sleeves[0].sprite;
        R_Sleeve = Sleeves[1].sprite;
        L_Shoe = Shoes[0].sprite;
        R_Shoe = Shoes[1].sprite;

    }
}
