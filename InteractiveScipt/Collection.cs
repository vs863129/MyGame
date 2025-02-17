using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Collection : Interactive
{
    [SerializeField] Transform LeftCollectionPoint, RightCollectionPoint;
    [SerializeField] private BoxCollider2D InteractiveRange;
    [SerializeField] private float _BaseTime;
    public float BaseTime { get { return _BaseTime; } }
    public GameObject Bar;
    [SerializeField] Item CollectionItem;
    [SerializeField] GameObject DropItme;
    [SerializeField] GameObject SoundPrafbe;
    [SerializeField] Transform DropPoint;
    [HideInInspector]public float ChangeColorTime=0;
    public Transform GroundCheckPoint;
    float PlayerTime;
    [Header("音效")]
    public AudioClip[] CollectSound;
    [Range(0, 1f)]
    [SerializeField] float Volume;
    [Range(-3,3f)]
    [SerializeField] float Pitch;
    void FixedUpdate()
    {
        WhenBuilding();
        WhenPlayerIsDoing();
    }
    void WhenBuilding()//建造期間,建築物遇到該阻擋物
    {
        if (ChangeColorTime > 0.1f)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            ChangeColorTime -= Time.fixedDeltaTime;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
    #region 玩家採集
    public Transform OverPoint(Vector2 Player)
    {
        float Distance = transform.position.x - Player.x;
        if (Mathf.Sign(Distance) == 1) //往左
        {
            return LeftCollectionPoint;
        }
        else //往右
        {
            return RightCollectionPoint;
        }
    }
    public bool IsLeft(Transform Direction)//如果站在左邊則面向右邊
    {
        if (Direction == RightCollectionPoint)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public override void GotInteractive(Player Player)
    {
        if (!m_Player || !Player)
        {
            PlayerTime = BaseTime;
        }
        base.GotInteractive(Player);
    }
    void WhenPlayerIsDoing()
    {
        if(m_Player && IsFinish())
        {
            
            SpawnDropItme();
        }
    }
    bool IsFinish()
    {
        if(PlayerTime<=0.1f)
        {
            return true;
        }
        else
        {
            PlayerTime -= Time.fixedDeltaTime;
            return false;
        }
    }
    public void PlayerCollect(Player player)
    {
        m_Player = player;
        SpawnDropItme();
    }
    void SpawnDropItme()
    {
        PlayerTime = BaseTime;
        GameObject Obj = Instantiate(DropItme);
        if(CollectSound.Length!=0)
        {
            GameObject Sound = Instantiate(SoundPrafbe);
            Sound.transform.position = transform.position;
            Sound.GetComponent<TempSound>().Play(CollectSound[Random.Range(0, CollectSound.Length)], Volume, Pitch);
        }
        Obj.transform.position = DropPoint.position;
        Item Dropitem=new Item();
        Dropitem.Clip = CollectionItem.Clip;
        Dropitem.GotPassValue(CollectionItem,5);
        Obj.GetComponent<DropItem>().Spawn(Dropitem);
        if (CollectionItem.IsNone())
        {
            m_Player.Interactive = null;
            GM.DateBase.A_Collecting.Remove(gameObject);
            Destroy(gameObject);
        }
    }
    #endregion
    #region NPC運作
    public bool Isenough(float Acquisition)
    {
        if (CollectionItem.Value < Acquisition)
        {
            return false;
        }
        else
        {
            return true;
        }
        
    }
    public bool Villager_Complete(Villager villager,bool IsComplete)
    {
        if (IsComplete)
        {
            villager.V_Data.WorkOredr.GotItem = Villager_Gotitem(villager.V_Data.Stats);
            if (CollectionItem.IsNone())
            {
                Destroy(gameObject);
            }
            return true;
        }
        return false;
    }
    Item Villager_Gotitem(VillagerState State)
    {
            Item _item= new Item();
            _item.Clip = CollectionItem.Clip;
            _item.GotPassValue(CollectionItem, State.CollectPower);
            return _item;
    }
    #endregion
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")&&!NPC)
        {
            Player player = collision.GetComponent<Player>();
            if(!player.Interactive)
            {
                player.Interactive = this;
                Bar.SetActive(true);
            }
            else if(Type == CollectionType.Collect)
            {
                if (player.Interactive.Type != CollectionType.Collect)
                {
                    player.Interactive.GetComponent<Collection>().Bar.SetActive(false);
                    player.Interactive = this;
                    Bar.SetActive(true);
                }
            }
        }
    }
     void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag ("Player")&&!NPC)
        {
            Player player = collision.GetComponent<Player>();
            player.Interactive = null;
            m_Player = null;
            Bar.SetActive(false);
        }
        if(collision.CompareTag("NPC"))
        {
            m_Player = null;
            Bar.SetActive(false);
        }
    }

}

