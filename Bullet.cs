using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public  Rigidbody2D rb;
    public float LifeTime;
    public enum _Launcher //發射者類別
    {
        Player, //玩家
        Enemy, //敵人
    }
    public EffectClip effect;
    public BasicValue Attacker;
    bool hasHit;
    private void Update()
    {
        if (!hasHit)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
        else
        {
            LifeTime -= Time.fixedDeltaTime;
            if (LifeTime < 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }
    private void EffectCheck(BasicValue GotObject)
    {
        if(effect)
        {
            GotObject.GotEffect(effect);
        }
    }
}