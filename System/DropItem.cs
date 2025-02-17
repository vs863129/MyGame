using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] private Transform Player;
    [SerializeField] private GameManger GM;
    [SerializeField] private Item _item;
    [SerializeField] private SpriteRenderer SpriteRenderer { get {return GetComponent<SpriteRenderer>(); } }
    [SerializeField] float GotTime;
    bool CanGet
    {
        get
        {
            if(GotTime>0.1f)
            {
                GotTime -= Time.fixedDeltaTime;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
    public void Spawn(ItemClip item,int value)
    {
        GotTime = 5f;
        Player = GameObject.Find("Player").transform;
        GM = GameObject.Find("GameManger").GetComponent<GameManger>();
        _item =new Item();
        {
            _item.Clip = item;
            _item.AddValue(value);
        }
        SpriteRenderer.sprite = item.Icon;
        RandomShot();
        GM.DateBase.IsDrops.Add(this);
    }
    public void Spawn(Item item)
    {
        Player = GameObject.Find("Player").transform;
        GM = GameObject.Find("GameManger").GetComponent<GameManger>();
        _item = item;
        SpriteRenderer.sprite = item.Clip.Icon;
        RandomShot();
        GM.DateBase.IsDrops.Add(this);
    }
    void RandomShot()
    {
        Vector2 Shot = new Vector2(Mathf.Sign(Random.Range(-1,1))* Random.Range(30,50), Random.Range(200, 300));
        GetComponent<Rigidbody2D>().AddForce(Shot);
    }
    private void FixedUpdate()
    {
        if(CanGet)
        {
            FindPlayer();
        }
    }
    public void FindPlayer()
    {
        RaycastHit2D Hit = Physics2D.Raycast(gameObject.transform.position, Player.position, 1,1 << LayerMask.NameToLayer("Player"));
        if (Hit.collider)
        {
            if (GM.InventorySystem.GotItem(_item))
            {
                Remove();
            }
        }    
    }
    public void Remove()
    {
        for (int i = 0; i < GM.DateBase.IsDrops.Count; i++)
        {
            if (GM.DateBase.IsDrops[i].GetInstanceID() == GetInstanceID())
            {
                GM.DateBase.IsDrops.RemoveAt(i);
            }
        }
        Destroy(gameObject);
    }
    #region 存取
    public void SaveDate(out Item _item, out Vector3 vector3)
    {
        _item = this._item;
        vector3 = transform.position;
    }
    #endregion
}
