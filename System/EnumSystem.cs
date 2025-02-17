using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum VillagerManger_Time
{
    Sun,
    SunDown,
    Night,
    SunUp,
}
public enum Villager_Stats_Type
{
    Strength,//力量
    Agile,//敏捷
    Intelligence,//智力
    Lucky,//幸運
}
public enum Villager_AI
{
    Idle,
    GoHome,
    Working,
};
public enum BuildingScrip
{
    none,
    Shop,
    Research,
}
public enum Attribute
{
    Health,
    Defense,
    Damage,
    [Tooltip("製作資源減免(%)")]
    ProduceCostResourceReduction,//製作成本減免(%)
    [Tooltip("建造資源減免(%)")]
    BuilingCostResourceReduction,//建造成本減免(%)
}
public enum ItemType
{
    [Tooltip("資源或建材")]
    Resource,
    [Tooltip("雜物")]
    Sundries,
    [Tooltip("工具")]
    Tool,
    [Tooltip("裝備")]
    Equipment,
}
public enum Researchtype
{
    [Tooltip("預設")]
    none,    
    [Tooltip("經濟研究")]
    Economic, 
    [Tooltip("建設研究")]
    Construct,
    [Tooltip("戰爭研究")]
    Combat,   
}
public enum Tool_Type
{
    Axe,
    Pickaxe,
    Hammer,
    Handmade,
}
public enum Equipment_Type
{
    Weapon,//武器
    Head,//頭部
    Chest,//胸甲
    Foots,//腳
    Gloves,//手套
    Accessories,//飾品
}
public enum Weapon_Type
{
    SingleHanded,//單手
    DoubleHanded,//雙手
    DualWield,//雙持
}
public enum AttackType
{
    Melee,
    Remote,
}
public enum IsTarget
{
    Enemy,
    Player,
}
public enum CollectionType
{
    Felling,//伐木
    Mining,//採礦/石
    Collect,//拾取
}
public enum ProduceBuildingType
{
    [Tooltip("普通生產")]
    GeneralProductionPlace,
    [Tooltip("鍛造場")]
    Smithy,
}
public enum ProduceType
{
    [Tooltip("一般製造物")]
    Normal,
    [Tooltip("鑄融製造物")]
    Melt,
    [Tooltip("手工製作")]
    Handicraft,
}
public enum WorkPosition
{
    [Tooltip("無業")]
    none,
    [Tooltip("伐木工")]
    Lumberjack,
    [Tooltip("採礦工")]
    Miner,
    [Tooltip("製作者")]
    Maker,
    [Tooltip("研究者")]
    Researcher,
    [Tooltip("鐵匠")]
    Blacksmith,
}
public enum Tutorial_Quest_Type
{
    [Tooltip("蒐集")]
    CollectItem,
    [Tooltip("建築")]
    Buidling,
}
public enum UseItemTips
{
    None,
    Use,
    Equip,
    Store,
}

