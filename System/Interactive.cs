using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactive : MonoBehaviour
{
    protected GameManger GM;
    [SerializeField] public CollectionType Type;
    protected Player m_Player;
    public Villager NPC;
    public Sprite InteractiveImage
    {
        get
        {
            return GM.DateBase.A_InteractiveImage[(int)Type];
        }

    }
    private void Awake()
    {
        GM = GameObject.Find("GameManger").GetComponent<GameManger>();
    }
    public virtual void GotInteractive(Player Player)
    {
        m_Player = Player;
    }

}
