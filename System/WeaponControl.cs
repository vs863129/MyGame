using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponControl : MonoBehaviour
{
    [SerializeField]private Player player;
    public TrailRenderer TrailRenderer;
    public PolygonCollider2D PolygonCollider2D;
    // Start is called before the first frame update
    public void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            BasicValue enemy = collision.GetComponent<BasicValue>();
            enemy.Hurt(player);
            PolygonCollider2D.enabled = false;
            Debug.Log("Hit");
        }

    }
    public void NotHit()
    {
        PolygonCollider2D.enabled = false;
    }
}
