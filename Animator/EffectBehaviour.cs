using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBehaviour : StateMachineBehaviour
{
    public enum EffectType
    {
        L_Weapon,
        R_Weapon
    }

    public EffectType type;
    [SerializeReference]private TrailRenderer TrailRenderer;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        EquipmentSystem Equipment = animator.GetComponent<Player>().InventorySystem.Equipment;
        if (type==EffectType.L_Weapon)
        {
            TrailRenderer = Equipment.Obj_LeftWeapon.GetComponent<WeaponControl>().TrailRenderer;
            Equipment.Obj_LeftWeapon.GetComponent<WeaponControl>().PolygonCollider2D.enabled = true;
        }
        else
        {
            TrailRenderer =  Equipment.Obj_RightWeapon.GetComponent<WeaponControl>().TrailRenderer;
            Equipment.Obj_RightWeapon.GetComponent<WeaponControl>().PolygonCollider2D.enabled = true;
        }
        TrailRenderer.emitting = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TrailRenderer.emitting = false;
        if (type == EffectType.L_Weapon)
        {
            animator.GetComponent<Player>().InventorySystem.Equipment.Obj_LeftWeapon.GetComponent<WeaponControl>().NotHit();
        }
        else
        {
            animator.GetComponent<Player>().InventorySystem.Equipment.Obj_RightWeapon.GetComponent<WeaponControl>().NotHit();
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
