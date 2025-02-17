using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Effect : MonoBehaviour
{
    private BasicValue Target;
    private EffectClip Clip;
    public EffectClip GotClip
    {
        get { return Clip; }
    }
    private float DurationTime;
    public void GotEffectTarget(BasicValue _Target,EffectClip clip)
    {
        if (!Target)
        {
            Clip = clip;
            Target = _Target;
            DurationTime = Clip.GotBaseDurationTime;
            InvokeRepeating("Action", 1f, Clip.GotRepeatTime);
        }
    }
    private void Update()
    {
        if(IsInvoking("Action"))
        {
            DurationTime -= Time.deltaTime;
            if (DurationTime <= 0)
            {
                CancelInvoke("Action");
                RemoveEffect();
            }
            if (Target.Health == 0)
            {
                Target.Dead();
            }
        }
    }
    private void RemoveEffect()
    {
        for(int i=0;i< Target.UI.Effects.Count;i++)
        {
            EffectClip TargetEffect = Target.UI.Effects[i].GetComponent<Effect>().Clip;
            if(TargetEffect== Clip)
            {
                Target.UI.Effects.RemoveAt(i);
                break;
            }
        }
        Destroy(gameObject);
    }
    private void Action()
    {
        switch (Clip.GotType)
        {
            case EffectClip.Type.Poisoning:
                Target.Health -= Clip.GotValue;
                Target.HurtColor(Color.green);
                break;
            case EffectClip.Type.Frozen:
                break;
            case EffectClip.Type.Heal:
                Target.Health += Clip.GotValue;
                Target.HurtColor(Color.white);
                break;
        }
    }
}
