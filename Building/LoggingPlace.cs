using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggingPlace : Building
{
    [SerializeField]
    AI_CollectionClip WorkGoals;
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (clip.WorkingPeple.Count!=0)
        {
            for(int i=0;i< clip.WorkingPeple.Count;i++)
            {
                StartCoroutine(DispatchWork(clip.WorkingPeple[i]));
            }
        }
    }
    IEnumerator DispatchWork(VillagerClip villager)
    {
        yield return new WaitForSeconds(1);
        if (!villager.WorkOredr)
        {
            AI_CollectionClip GotGoals = Instantiate(WorkGoals);
            GotGoals.WokrBuilding = transform;
            villager.WorkOredr = GotGoals;
        }
    }
}
