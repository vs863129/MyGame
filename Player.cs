using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : BasicValue
{
    [SerializeField] GameObject Prafbe;
    [Header("替換")]
    [SerializeField] SpriteRenderer L_Hand_Item;
    [SerializeField] SpriteRenderer R_Hand_Item;
    [SerializeField] Sprite L_Hand_TempItem;
    [SerializeField] Sprite R_Hand_TempItem;
    [SerializeField] GameObject ShowTool; 
    [Header("墜落判斷")]
    public GameObject Point;
    protected GameObject[] Points;
    public int NumberOfPoints;
    public float SpaceBetweenPoints;
    Animator animator;
    public Animator GotAnimator
    { get { return animator; } }
    private Vector3 Velocity = Vector3.zero;
    public Vector2 Movement;
    [Header("判斷")]
    public Transform DropPos;
    public Collider2D GroundCheckPoint;
    [SerializeField] private GameObject InteractiveIcon;
    [SerializeField] Transform AutoMovePoint;
    public float Jump;
    bool isGround;
    bool IsJump;
    bool Run;
    bool Collecting;
    [Header("其他系統")]
    public Interactive Interactive;
    public InventorySystem InventorySystem;
    [SerializeField]private GameManger GameManger;
    void Start()
    {
        animator = GetComponent<Animator>();
        Points = new GameObject[NumberOfPoints];
        for (int i = 0; i < NumberOfPoints; i++)
        {
            Points[i] = Instantiate(Point, ShotPos.position, Quaternion.identity);
        }
    }
    #region 內部
    protected override void Update()
    {
        if (!Died)
        {
            animator.SetFloat("Speed", Movement.x);
            if (!Collecting) //如果沒在採集狀態
            {
                animator.SetBool("Collecting", false);
                if (!GameManger.UIManger.InBuilding)
                {
                    base.Update();
                    Collecting = CanInteractive();
                    MoveUpdate();
                    //    if (EquipmentSystem.Weapon.DamageType == ItemClip.Attack_Type.Remote && EquipmentSystem.Weapon)
                    //    {
                    //        ShotCount();
                    //    }
                    //if (InventorySystem.Equipment.LeftWeapon.Item.Clip || InventorySystem.Equipment.RightWeapon.Item.Clip)
                    //{
                    //    if (Input.GetMouseButtonDown(0) && AttackCooldown == 0 && !GameManger.UIManger.NowUI && !GameManger.UIManger.MousePointInUI)
                    //    {
                    //        MeleeAttack();
                    //    }
                    //}
                }
                else
                {
                    Movement = Vector2.zero; //停止動作
                }
            }
            else
            {
                if (Interactive != null)
                {
                    ActionCollect();
                }
                else
                {
                    ReturnHandItem();
                    Collecting = false;
                }
            }
        }
    }
    public override void Hurt(BasicValue Attacker)
    {
        base.Hurt(Attacker);
        InventorySystem.StatsUI.UpdateStats();
    }
    void ActionCollect()
    {
        Collection Script = Interactive.GetComponent<Collection>();
        if (Script.Type != CollectionType.Collect)
        {

            R_Hand_Item.sprite = InventorySystem.Equipment.GotToolSprite((int)Script.Type);
            if (AutoMovePoint)
            {
                FlipX = Script.IsLeft(AutoMovePoint);
                Interactive.GotInteractive(this);
                Movement.x = 0;
                SetCollectingAnimtion();
            }
            else
            {
                if (MoveToPoint(Script.OverPoint(transform.position).position, ref Movement))
                {
                    AutoMovePoint = Script.OverPoint(transform.position);
                }
            }
            if (Input.anyKeyDown&&!Input.GetKeyDown(KeyCode.Mouse0))
            {
                Collecting = false;
                AutoMovePoint = null;
                Interactive.GotInteractive(null);
                ReturnHandItem();
            }
        }
        else
        {
            SetCollectingAnimtion();
            Movement.x = 0;
        }
    }
    void TemporaryHandItem()
    {
        L_Hand_TempItem = L_Hand_Item.sprite;
        R_Hand_TempItem = R_Hand_Item.sprite;
    }
    void ReturnHandItem()
    {
        L_Hand_Item.sprite = L_Hand_TempItem;
        R_Hand_Item.sprite = R_Hand_TempItem;
        Debug.Log("停止採集");
    }
    #region Animator
    public void Collect()
    {
        Collecting = false;
        Interactive.GetComponent<Collection>().PlayerCollect(this);
        if (Interactive)
        {
            Interactive.GotInteractive(null);
        }
        ReturnHandItem();
    }
    void SetCollectingAnimtion()
    {
        CollectionType C_Type = Interactive.Type;
        animator.SetBool("Collecting", true);
        switch (C_Type)
        {
            case CollectionType.Felling:
                animator.SetBool("Collect", false);
                animator.SetBool("Mining", false);
                animator.SetBool("Felling", true);
                break;
            case CollectionType.Collect:
                animator.SetBool("Mining", false);
                animator.SetBool("Felling", false);
                animator.SetBool("Collect", true);
                break;
            case CollectionType.Mining:
                animator.SetBool("Collect", false);
                animator.SetBool("Felling", false);
                animator.SetBool("Mining", true);
                break;
            default:
                break;
        }
    }
    #endregion
    #region 互動設定
    private bool CanInteractive()
    {
        if (Interactive != null && isGround)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (InventorySystem.Equipment.CheckHaveTool((int)Interactive.Type) || Interactive.Type == CollectionType.Collect)
                {
                    TemporaryHandItem();
                    return true;
                }
                else
                {
                    StartCoroutine(ShowMissTool(Interactive.InteractiveImage));
                    Debug.Log("需要對應工具");
                    return false;
                }

            }
        }
        return false;
    }
    IEnumerator ShowMissTool(Sprite Image)
    {
        ShowTool.GetComponent<Image>().sprite = Image;
        ShowTool.SetActive(true);
        yield return new WaitForSeconds(1);
        ShowTool.SetActive(false);
    }
    #endregion
    private void MeleeAttack()
    {
        if (AttackCooldown == 0)
        {
            animator.SetTrigger("Attack");
            AttackCooldown = GotBaseAttackCooldown;
        }
    }
    private void MoveUpdate()
    {
        GroundCheck();
        IsJump = false;
        Movement.x = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space) && isGround)//跳躍
        {
            Rigidbody.velocity = transform.TransformDirection(new Vector2(0, Jump));
            isGround = false;
            IsJump = true;
        }
        if (Movement.x!=0)
        {
            if (Run)
            {
                Speed = GotBaseSpeed + 0.5f;
                animator.SetBool("Running", Run);
            }
            else
            {
                Speed = GotBaseSpeed;
                animator.SetBool("Running", Run);
            }
        }
        else
        {
            Speed = GotBaseSpeed;
            animator.SetBool("Running", false);
        }
        if (Prafbe.activeInHierarchy)
        {
            Move(Movement.x * Speed * Time.fixedDeltaTime);
        }
    }
    private void GroundCheck()
    {
        if(GroundCheckPoint.IsTouchingLayers(1 << LayerMask.NameToLayer("Ground")))
        {
            if(!IsJump)
            {
                isGround = true;
            }
        }
        else
        {
            isGround = false;
        }
        animator.SetBool("Ground", isGround);
        animator.SetFloat("Air", Rigidbody.velocity.y);
    }
    private void LateUpdate()
    {
        if (attacktype == AttackType.Remote)
        {
            for (int i = 0; i < NumberOfPoints; i++)
            {
                Points[i].SetActive(true);
                Points[i].transform.position = PointPos(i * SpaceBetweenPoints);
            }
        }
        else
        {
            for (int i = 0; i < NumberOfPoints; i++)
            {
                Points[i].SetActive(false);
            }
        }

    }
    Vector2 PointPos(float t)
    {
        Vector2 postion = (Vector2)ShotPos.position + (ShotDirection.normalized * LaunchForce * t) + 0.5f * Physics2D.gravity*(t*t);
        return postion;
    }
    private void FixedUpdate()
    {

        IsFixedUpdate();
    }
#endregion
    public void HidePrafab(bool IsHidde)
    {
        Prafbe.SetActive(IsHidde);
    }
    void Move(float move)
    {
        Flip();
        Vector3 TargetVelocity = new Vector2(move*100f, Rigidbody.velocity.y);
        if(Input.GetKey(KeyCode.LeftShift))
        {
            Run = true;
        }
        else
        {
            Run = false;
        }
        Rigidbody.velocity = Vector3.SmoothDamp(Rigidbody.velocity, TargetVelocity, ref Velocity, .05f);
    }
    public void Flip()
    {
        if(Input.GetKey(KeyCode.LeftArrow)|| Input.GetKey(KeyCode.A) && Movement.x<0) //往左
        {
            FlipX = true;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) && Movement.x > 0)//往右
        {
            FlipX = false;
        }
    }
    public void ShotCount()
    {
        Vector2 BowPos = Bow.position;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ShotDirection = mousePos - BowPos;
        Bow.right = ShotDirection;
        if(Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }
    void Fire()
    {
        GameObject NewBullet = Instantiate(BulletPerfab, ShotPos.position, ShotPos.rotation);
        NewBullet.GetComponent<Rigidbody2D>().velocity = Bow.right * LaunchForce;
        NewBullet.GetComponent<Bullet>().Attacker = this;
        NewBullet.layer = 12;
    }
    public override void Dead()
    {
        Died = true;
        gameObject.SetActive(false);
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(L_Weapon.position, AttackRange);
    }
}