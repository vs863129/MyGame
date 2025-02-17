using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoneEnemy : BasicValue
{
    [Header("怪物屬性")]
    public EnemyClip clip; //怪物片段
    private GameManger GameManger;
    [SerializeField]
    public GameObject Target;
    public Transform Left, Right;
    private enum Aistats
    {
        Idle,//閒置
        Patrol,//巡邏
        Attack,//攻擊
        Chase,//追逐
        Alert,//警戒
    };
    [SerializeField]
    private Aistats AiStats;
    [SerializeField]
    private Vector2 TargetDistance; //與目標之間距離
    public float checkRange; //追逐範圍
    public float SearchRange; //搜尋範圍
    [SerializeField]
    public float ChaseCooldown; //追逐時長
    private float AlertCooldown; //警戒時長
    public float RandomStatsCooldown;//狀態維持長度
    public bool IsAlert;
    private void Start()
    {
        GameManger = GameObject.Find("GameManger").GetComponent<GameManger>();
        SearchRange = AttackRange + 1;
    }
    private void FixedUpdate()
    {
        IsFixedUpdate();
        AI();
        if (!Target&&!IsAlert)
        {
            RandomStats();
        }
        else if(ChaseCooldown!=0)
        {
            TargetDistance.x = Mathf.Abs(transform.position.x - Target.transform.position.x); //左右
            TargetDistance.y = Mathf.Abs(transform.position.y - Target.transform.position.y); //上下
            if (TargetDistance.x < AttackRange)
            {
                if (attacktype == AttackType.Melee && TargetDistance.y < 0.8f)
                {
                    AiStats = Aistats.Attack;
                }
                else if (attacktype == AttackType.Remote)
                {
                    AiStats = Aistats.Attack;
                }
            }
            else
            {
                AiStats = Aistats.Chase;
            }
        }
    }
    private void RandomStats()
    {
        if (RandomStatsCooldown==0)
        {
            int Stats= Random.Range(1, 3);
            if (Stats == 1)
            {
                if (AiStats != Aistats.Idle)
                {
                    RandomStatsCooldown = Random.Range(3, 5);
                    AiStats = Aistats.Idle;
                }
                else
                {
                    RandomStatsCooldown = Random.Range(5, 10);
                    AiStats = Aistats.Patrol;
                }
            }
            else
            {
                RandomStatsCooldown = Random.Range(5, 10);
                AiStats = Aistats.Patrol;
                int isflip = Random.Range(1, 2);
                if (isflip == 1)
                {
                    FlipX = !FlipX;
                }
            }
        }
        else
        {
            RandomStatsCooldown = _Cooldowning(RandomStatsCooldown);
        }
    }
    private void OnAlert()
    {
        AlertCooldown = _Cooldowning(AlertCooldown);
        if(AlertCooldown==0)
        {
            IsAlert = false;
        }
    }
    private void AI()
    {
        //if (!IsRepel)
        //{
        //    switch (AiStats)
        //    {
        //        case Aistats.Idle:
        //            Detect();
        //            break;
        //        case Aistats.Patrol:
        //            Speed = GotBaseSpeed;
        //            Patrol();
        //            Detect();
        //            break;
        //        case Aistats.Attack:
        //            Speed = GotBaseSpeed * 1.5f;
        //            OnAttack();
        //            break;
        //        case Aistats.Chase:
        //            Speed = GotBaseSpeed * 1.5f;
        //            OnChase();
        //            Detect();
        //            break;
        //        case Aistats.Alert:
        //            OnAlert();
        //            break;
        //    }
        //}
    }
    public void OnChase()
    {
        IsAlert = true;
        ChaseCooldown = _Cooldowning(ChaseCooldown);
        if (ChaseCooldown!=0)
        {
            OnChaseMove();
        }
        if (ChaseCooldown <= 0)
        {
            RandomStatsCooldown = 0;
            ChaseCooldown = 0;
            AlertCooldown = 2;
            Target = null;
            AiStats = Aistats.Alert;
        }
    }
    public void OnChaseMove() //追逐狀態移動
    {
        float ChaseDistance = transform.position.x - Target.transform.position.x;
        if (ChaseDistance < 0)
        {
            RightMove();
        }
        else
        {
            LeftMove();
        }
    }
    public void OnAttack() //攻擊
    {
        Bow.transform.localScale = new Vector2(Mathf.Abs(Bow.transform.localScale.x), Bow.transform.localScale.y);
        if (attacktype== AttackType.Remote)
        {
            ShotCount();
            if(AttackCooldown==0)
            {
                Shot();
                ChaseCooldown = 10f;
            }
        }
        else if (attacktype == AttackType.Melee)
        {
            if (AttackCooldown == 0)
            {
                //TargetAttack(IsTarget.Player, this, attacktype);
                ChaseCooldown = 10f;
            }
        }
    }
    void ShotCount() //射擊方位
    {
        Vector2 TargetPos = Target.transform.position;
        Vector2 BowPos = Bow.position;
        ShotDirection = TargetPos - BowPos;
        Vector2 overShotDirection = new Vector2(ShotDirection.x, ShotDirection.y+(ShotDirection.y/10));
        Bow.right = overShotDirection;
    }
    void Shot() //射擊
    {
        GameObject NewBullet = Instantiate(BulletPerfab, ShotPos.position, ShotPos.rotation);
        NewBullet.GetComponent<Rigidbody2D>().velocity = Bow.right * LaunchForce;
        NewBullet.GetComponent<Bullet>().Attacker = this;
        NewBullet.layer = 15;
        AttackCooldown = GotBaseAttackCooldown;
    }
    public void Detect()//範圍內發現玩家
    {
        if (!FlipX)//右
        {
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, Vector2.right, SearchRange, 1 << LayerMask.NameToLayer("Player"));
            if(Hit)
            {
                Target = Hit.collider.gameObject;
                ChaseCooldown = 10f;
            }
        }
        else
        {
            RaycastHit2D Hit = Physics2D.Raycast(transform.position, Vector2.left, SearchRange, 1 << LayerMask.NameToLayer("Player"));
            if (Hit)
            {
                Target = Hit.collider.gameObject;
                ChaseCooldown = 10f;
            }
        }
    }
    public void Patrol()//巡邏
    {
        if (!FlipX)//往左
        {
            RaycastHit2D Hit = Physics2D.Raycast(Right.position, Vector2.right, checkRange, 1 << LayerMask.NameToLayer("Ground"));
            if (Hit)
            {
                FlipX = true;
            }
            Move();
        }
        else
        {
            RaycastHit2D Hit = Physics2D.Raycast(Left.position, Vector2.left, checkRange, 1 << LayerMask.NameToLayer("Ground"));
            if (Hit)
            {
                FlipX=false;
            }
            Move();
        }

    }
    public void Move()//普通移動
    {
        if (!FlipX)//右
        {
            RightMove();
        }
        else //左
        {
            LeftMove();
        }
    }
    public void RightMove()
    {
        FlipX=false;
        var _rigidbody = GetComponent<Rigidbody2D>();
        Vector2 TargetVelocity = new Vector2(Speed, _rigidbody.velocity.y);
        Vector2 Velocity = Vector2.zero;
        _rigidbody.velocity = Vector2.SmoothDamp(_rigidbody.velocity, TargetVelocity, ref Velocity, .05f);
        if(AiStats== Aistats.Patrol)
        {
            Bow.transform.localScale = new Vector3(Mathf.Abs( Bow.transform.localScale.x), Bow.transform.localScale.y,0);
        }

    }
    public void LeftMove()
    {
        FlipX=true;
        var _rigidbody = GetComponent<Rigidbody2D>();
        Vector2 TargetVelocity = new Vector2(-Speed, _rigidbody.velocity.y);
        Vector2 Velocity = Vector2.zero;
        _rigidbody.velocity = Vector2.SmoothDamp(_rigidbody.velocity, TargetVelocity, ref Velocity, .05f);
        if (AiStats == Aistats.Patrol)
        {
            Bow.transform.localScale = new Vector3(-Mathf.Abs(Bow.transform.localScale.x), Bow.transform.localScale.y,0);
        }
        else
        {
            Bow.transform.localScale = new Vector2(Mathf.Abs(Bow.transform.localScale.x), Bow.transform.localScale.y);
        }
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Left.position, checkRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Right.position, checkRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SearchRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, GotBassAttackRange);
    }

}
