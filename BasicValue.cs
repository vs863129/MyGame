using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public class BasicValue : MonoBehaviour
{

    [Header("物件辨識")]
    public int ID;
    [Header("血量")]
    [SerializeField]
    private float _BaseHealth; //基礎血量
    float _ModflieHealth;//血量修飾符
    public float BaseHealth
    {
        get 
        {
            return _BaseHealth + _ModflieHealth;
        }
        set
        {
            _ModflieHealth = value;
            if(BaseHealth<Health)
            {
                Health = BaseHealth;
            }
            UI.UpdateHealthUI(Health, BaseHealth);
        }
    }
    float _Health;
    public float Health
    {
        get
        {
            return _Health;
        }
        set
        {
            _Health = value;
            if (_Health > _BaseHealth)
            {
                _Health = _BaseHealth;
            }
            if (_Health < 0)
            {
                _Health = 0;
            }
            UI.UpdateHealthUI(Health, BaseHealth);
            if(IsInvoking(nameof(CloseHealthBar)))
            {
                CancelInvoke(nameof(CloseHealthBar)); 
            }
            Invoke(nameof(CloseHealthBar), 2f);
        }
    }//浮動血量
    [Header("傷害")]
    public AttackType attacktype;//傷害類型
    [SerializeField]
    private float BaseDamage; //基礎傷害
    public float GotBaseDamage
    {
        get { return BaseDamage; }
    }
    float ModflieDamage;
    public float Damage
    {
        get
        {
            return ModflieDamage + GotBaseDamage;
        }
        set
        {
            ModflieDamage = value;
            if (Damage < 0)
            {
                ModflieDamage = 0;
            }
        }
    }//浮動傷害
    [SerializeField]
    private float BaseAttackCooldown;//基礎間隔
    public float GotBaseAttackCooldown
    {
        get { return BaseAttackCooldown; }
    }
    float _AttackCooldown;
    public float AttackCooldown//攻擊間隔
    {
        get
        {
            return _AttackCooldown;
        }
        set
        {
            _AttackCooldown = value;
            if (_AttackCooldown<0)
            {
                _AttackCooldown = 0;
            }
        }
    }
    [SerializeField]
    private float BaseAttackRange;
    public float GotBassAttackRange
    {
        get { return BaseAttackRange; }
    }
    float _AttackRange;
    public float AttackRange //攻擊範圍
    {
        get
        {
            return _AttackRange;
        }
        set
        {
            _AttackRange = value;
            if (_AttackRange<0)
            {
                _AttackRange = 0;
            }
        }
    }
    [Header("裝備")]
    public Transform L_Weapon;
    [Header("防禦")]
    [SerializeField]
    private float BaseDefense; //基礎防禦
    public float GotBaseDefense
    {
        get { return BaseDefense; }
    }
    float ModflieDefense;
    public float Defense//防禦
    {
        get 
        {
            return ModflieDefense + GotBaseDefense;
        }
        set
        {
            ModflieDefense = value;
        }
    }
    [Header("擊退抵抗")]
    [SerializeField]
    private float BaseRepelResistance;//基礎擊退抵抗值
    public float GotBaseRepelResistance
    {
        get { return BaseRepelResistance; }
    }
    float _RepelResistance;
    public float RepelResistance
    {
        get
        {
            return _RepelResistance;
        }
        set
        {
            _RepelResistance = value;
            if (_RepelResistance<0)
            {
                _RepelResistance = 0;
            }
        }
    }
    [Header("遠程")]
    public Transform Bow;//武器位置
    public float LaunchForce;//推力
    public GameObject BulletPerfab;//彈藥
    public Transform ShotPos;//發射點
    protected Vector2 ShotDirection; //射擊方向
    [Header("聲音")]
    [SerializeField]protected AudioSource PlayerAudio;
    [SerializeField] AudioClip[] MoveAudios;
    [SerializeField] AudioClip[] RunAudio;
    [Header("其他")]
    [SerializeField]
    protected Rigidbody2D Rigidbody;
    private IsTarget Type;
    public IsTarget GotType
    {
        get
        {
            return Type;
        }
    }
    public Color objColor; //本體顏色
    [SerializeField]
    private float BaseSpeed;//基礎速度
    public float GotBaseSpeed
    {
        get { return BaseSpeed; }
    }
    float _Speed;
    public float Speed
    {
        get
        {
            return _Speed;
        }
        set
        {
            _Speed = value;
            if(_Speed < 0)
            {
                _Speed = 0;
            }
        }
    }//速度
    public float Repel;//擊退強度
    //protected float RepelCooldown;//擊退時長
    //protected bool IsRepel;//是否被擊退
    public SpriteRenderer[] SpriteColor;
    protected bool Died;
    private float BaseHurtTime;
    bool _Flip;
    public  bool FlipX
    {
        get { return _Flip; }
        set
        {
            if(value)
            {
                gameObject.transform.eulerAngles = new Vector2(0, 180);
                UI.UI.localRotation = Quaternion.Euler(0,-180,0);
                _Flip = value;
            }
            else
            {
                gameObject.transform.eulerAngles = new Vector2(0, 0);
                UI.UI.localRotation = Quaternion.Euler(0, 0 , 0);
                _Flip = value;
            }
        }
    }
    float HurtTime;
    [Header("UI")]
    public PrefabeUI UI;
    public void Awake()
    {
        BaseHurtTime = 0.1f;
        Health = BaseHealth;
        AttackRange = BaseAttackRange;
        AttackCooldown = BaseAttackCooldown;
        Defense = BaseDefense;
        Speed = BaseSpeed;
        RepelResistance = BaseRepelResistance;
        Rigidbody = GetComponent<Rigidbody2D>();
        for (int i=0;i< SpriteColor.Length;i++)
        {
            SpriteColor[i].color = objColor;
        }
    }
    void CloseHealthBar()
    {
        UI.CloseBar();
    }
    protected virtual void Update()
    {
        HurtTime = _Cooldowning(HurtTime);
        //RepelCooldown = _Cooldowning(RepelCooldown);
        if (HurtTime<=0)
        {
            for (int i = 0; i < SpriteColor.Length; i++)
            {
                SpriteColor[i].color = objColor;
            }
        }
        //if(RepelCooldown<=0)
        //{
        //    IsRepel = false;
        //}
    }
    protected void IsFixedUpdate()
    {
        AttackCooldown = _Cooldowning(AttackCooldown);
    }
    #region 音效
    public void PlayMoveSound()
    {
        PlayerAudio.PlayOneShot(MoveAudios[Random.Range(0,MoveAudios.Length)]);
    }
    public void PlayRunSound()
    {
        PlayerAudio.PlayOneShot(RunAudio[Random.Range(0, RunAudio.Length)]);
    }
    #endregion
    public void HurtColor(Color color)
    {
        for (int i = 0; i < SpriteColor.Length; i++)
        {
            SpriteColor[i].color = color;
        }
        HurtTime = BaseHurtTime;
    }
    public virtual void Hurt(BasicValue Attacker)
    { 
        if (!Died)
        {
            float TakeDamage = Attacker.Damage;
            HurtColor(Color.red);
            Health -= TakeDamage * (1 - (Defense / (Defense + 100)));
            float TargetDis = Attacker.transform.position.x - transform.position.x;
            if (TargetDis > 0 && TakeDamage > 1 && Attacker.Repel > 0)
            {
                Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
                rigidbody2D.AddForce(new Vector2(-Attacker.Repel*100, Attacker.Repel*80));
                //rigidbody2D.velocity += new Vector2(-Attacker.Repel, Attacker.Repel);
                //RepelCooldown = 1;
                //IsRepel = true;
            }
            else
            {
                Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
                rigidbody2D.AddForce(new Vector2(Attacker.Repel*100, Attacker.Repel*80));
                //rigidbody2D.velocity += new Vector2(Attacker.Repel, Attacker.Repel);
                //RepelCooldown = 1;
                //IsRepel = true;
            }
            if (Health==0)
            {
                Dead();
            }
        }
    }
    public virtual void Dead()
    {
        Died = true;
        if (GotType == IsTarget.Enemy)
        {
            Enemy enemy = GetComponent<Enemy>();
            //enemy.QuestCheck();
        }
        Destroy(gameObject);
    }
    public void GotEffect(EffectClip effect)
    {
        if(UI.Effects.Count!=0)
        {
            for(int i=0;i< UI.Effects.Count;i++)
            {
                Effect Script = UI.Effects[i].GetComponent<Effect>();
                if (Script.GotClip == effect)
                {
                    Script.GotClip.DurationTime = Script.GotClip.GotBaseDurationTime;
                    break;
                }
                else if (i== UI.Effects.Count&& Script.GotClip != effect)
                {
                    GameObject EffectUI = Instantiate(effect.GotUI, UI.EffectList);
                    Script.GotEffectTarget(this, effect);
                    UI.Effects.Add(EffectUI);
                }
            }
        }
        else
        {
            GameObject EffectUI = Instantiate(effect.GotUI, UI.EffectList);
            Effect Script = EffectUI.GetComponent<Effect>();
            Script.GotEffectTarget(this, effect);
            UI.Effects.Add(EffectUI);
        }
    }
    protected float _Cooldowning(float value)
    {
        if(value!=0)
        {
            value -= Time.fixedDeltaTime;
            if (value<0)
            {
                return 0;
            }
        }
        return value;
    }
    protected bool MoveToPoint(Vector2 Point,ref Vector2 Move)//自動移動至地點
    {
        Vector2 TargetVelocity = new Vector2(Speed*16f, Rigidbody.velocity.y);
        Vector2 Velocity = Vector2.zero;
        double Distance = transform.position.x - Point.x;;
        if (Distance > -0.1f && Distance < 0.1f)
        {
            return true;
        }
        else
        {
            switch (System.Math.Sign(Distance))
            {
                case 1: //左移動
                    Move.x = -1;
                    if (!_Flip)
                    { FlipX = true; }
                    return false;
                case -1://右移動
                    Move.x = 1;
                    if (_Flip)
                    { FlipX = false; }
                    return false;
            }
            return false;
        }
    }
    public void AddModflie(Attribute buff, int value, bool IsModfile)
    {
        if (IsModfile)
        {
            switch (buff)
            {
                case Attribute.Health:
                    BaseHealth = _ModflieHealth + value;
                    break;
                case Attribute.Damage:
                    Damage = ModflieDamage + value;
                    break;
                case Attribute.Defense:
                    Defense = ModflieDefense + value;
                    break;
            }
        }
        else
        {
            switch (buff)
            {
                case Attribute.Health:
                    _BaseHealth += value;
                    break;
                case Attribute.Damage:
                    BaseDamage += value;
                    break;
                case Attribute.Defense:
                    BaseDefense += value;
                    break;
            }
            UI.UpdateHealthUI(Health, BaseHealth);
        }

    }
    #region 載入
    public void LoadDate(R_Player Date)
    {
        Date.LoadPos(out Vector3 vector3, out _Flip); transform.position = vector3;
        Date.LoadHealth(out _Health, out _BaseHealth, out _ModflieHealth); UI.UpdateHealthUI(Health, BaseHealth);
        Date.LoadDamage(out BaseDamage,out ModflieDamage);
        Date.LoadDefens(out BaseDefense,out ModflieDefense);

    }
    #endregion
    #region 儲存
    public void SaveHealth(out float Health,out float BaseHealth,out float ModflieHealth)
    {
        Health = this.Health;
        ModflieHealth=this._ModflieHealth;
        BaseHealth=_BaseHealth;
    }
    public void SaveDamage(out float Damage,out float ModflieDamage)
    {
        Damage = BaseDamage;
        ModflieDamage = this.ModflieDamage;
    }
    public void SaveDefense(out float BaseDefense,out float ModflieDefense)
    {
        BaseDefense = this.BaseDefense;
        ModflieDefense = this.ModflieDefense;
    }
    public void SavePos(out Vector3 vector,out bool Flip)
    {
        vector = transform.position;
        Flip = _Flip;
    }
    #endregion
    [System.Serializable]
    public class PrefabeUI
    {
        public RectTransform UI;
        [SerializeField] GameObject HealthBar;//血條UI
        [Header("效果")]
        public Transform EffectList;
        public List<GameObject> Effects = new List<GameObject>();
        public void UpdateHealthUI(float Health, float MaxHealth)
        {
            if(HealthBar)
            {
                Slider Bar= HealthBar.GetComponent<Slider>();
                Bar.maxValue = MaxHealth;
                Bar.value = Health;
                HealthBar.SetActive(true);
            }
        }
        public void CloseBar()
        {
            if(HealthBar)
            {
                HealthBar.SetActive(false);
            }
        }
    }
}