using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolBehaviour : StateMachineBehaviour
{
    float ChangeTime;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ChangeTime = Random.Range(5, 20);
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Patrol(animator, animator.GetComponent<Enemy>().CheckGround);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
    private void Patrol(Animator animator,Transform CheckPos)
    {
        Enemy enemy = animator.GetComponent<Enemy>();
        RaycastHit2D Hit = Physics2D.Raycast(CheckPos.position, Vector2.left, 0.1f, 1 << LayerMask.NameToLayer("Ground"));
        if(Hit)
        {
            enemy.FlipX = !enemy.FlipX;
            ChangeTime = Random.Range(5, 20);
        }
        else
        {
            if(ChangeTime == 0)
            {
                RandomMoveDirection(enemy);
            }
            else
            {
                ChangeTime -= Time.fixedDeltaTime;
                if(ChangeTime<0.1f)
                {
                    ChangeTime = 0;
                }
            }
           if(enemy.FlipX)
            {
               enemy. RightMove(enemy.Speed, Vector2.zero);
            }
            else
            {
                enemy.LeftMove(enemy.Speed, Vector2.zero);
            }
        }
    }
    private void RandomMoveDirection(Enemy enemy)
    {
        int IsChange = Random.Range(-1, 2);
        if(IsChange >= 0)
        {
            enemy.FlipX = !enemy.FlipX;
            ChangeTime = Random.Range(5, 20);
        }
    }
}
