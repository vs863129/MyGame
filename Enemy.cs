using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BasicValue
{
    [Header("怪物屬性")]
    public Transform TargetPos;
    public Transform CheckGround;
    public float PatrolTime;//巡邏一次時長
    float _PatrolTime;//運算用
    protected override void Update()
    {
        base.Update();
        IsPatrol();
    }
    private void FixedUpdate()
    {
        IsFixedUpdate();
        CooldownTIme();
    }
    public void LeftMove(float Speed, Vector2 Velocity)
    {
        FlipX = false;
        Vector2 TargetVelocity = new Vector2(Speed, Rigidbody.velocity.y);
        Rigidbody.velocity = Vector2.SmoothDamp(Rigidbody.velocity, TargetVelocity, ref Velocity, .05f);
    }
    public void RightMove(float Speed, Vector2 Velocity)
    {
        FlipX = true;
        Vector2 TargetVelocity = new Vector2(-Speed, Rigidbody.velocity.y);
        Rigidbody.velocity = Vector2.SmoothDamp(Rigidbody.velocity, TargetVelocity, ref Velocity, .05f);
    }
    private void IsPatrol()
    {
        Animator animator= GetComponent<Animator>();
        if(TargetPos==null&& _PatrolTime == 0)
        {
           if(RandomBool(Random.Range(-1,2)))
            {
                animator.SetBool("IsPatrol",true);
            }
            else
            {
                animator.SetBool("IsPatrol", false);
            }
            _PatrolTime = PatrolTime;//重置時間
        }
    }
    private void CooldownTIme()
    {
        if(_PatrolTime>0.1f)
        {
            _PatrolTime -= Time.deltaTime;
        }
        else
        {
            _PatrolTime = 0;
        }
    }
    private bool RandomBool(int Number)
    {
            if (Number <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(CheckGround.position, 0.1f);
    }
}
