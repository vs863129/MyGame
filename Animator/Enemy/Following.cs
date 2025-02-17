using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Following : StateMachineBehaviour
{
    private Transform PlayerPos;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerPos = animator.GetComponent<Enemy>().TargetPos;
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(animator.GetComponent<Enemy>().TargetPos!=null)
        {
            FollowTarget(animator);
        }
        else
        {
            animator.SetBool("IsFollowing", false);
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
    private void FollowTarget(Animator animator)
    {
        Enemy Enemydate = animator.GetComponent<Enemy>();
        float TargetDistance = PlayerPos.position.x - animator.transform.position.x;
        float FollowSpeed = Enemydate.Speed * 1.2f;
        if (TargetDistance < 0)
        {
            Enemydate.LeftMove(FollowSpeed, Vector2.zero);
        }
        else
        {
            Enemydate.RightMove(FollowSpeed, Vector2.zero);
         
        }
    }

}
