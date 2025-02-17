using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private Transform PlayerPos;
    private GameManger GameManger;
    private Player player;
    public GameObject UI;
    public float TriggerRange;
    public void Awake()
    {
        PlayerPos = GameObject.Find("Player").transform;
        GameManger = GameObject.Find("GameManger").GetComponent<GameManger>();
    }
    public void Open()
    {
        if (!UI.activeInHierarchy)
        {
            UI.SetActive(true);
        }
        else
        {
            UI.SetActive(false);
        }
    }
    public void Update()
    {
        if (PlayerPos != null)
        {
            Flip();
        }
        LeaveRange();
    }
    public void TriggerEvent(RaycastHit2D hit)
    {
        if (!hit)
        {
            player = null;
        }
        else
        {
            if (hit.collider.tag == "Player")
            {
                player = hit.transform.GetComponent<Player>();
                if (Input.GetKeyDown(KeyCode.F) && player)
                {
                    Open();
                }
            }
        }

    }
    public void Flip()
    {
        float Distance = transform.position.x - PlayerPos.position.x;
        if(Distance>0)
        {
            transform.localScale =new Vector3(transform.localScale.x, transform.localScale.y, Mathf.Abs(transform.localScale.z));
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, TriggerRange, 1 << LayerMask.NameToLayer("Player"));
            TriggerEvent(hit);
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, -Mathf.Abs(transform.localScale.z));
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, TriggerRange, 1 << LayerMask.NameToLayer("Player"));
            TriggerEvent(hit);
        }

    }
    public void LeaveRange()
    {
        if (!player)
        {
            UI.SetActive(false);
        }
    }//離開對話範圍

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, TriggerRange);
    }

}
