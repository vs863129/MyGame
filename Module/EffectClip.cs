using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Effect", menuName = "Type/Effect")]
public class EffectClip : ScriptableObject
{
    [SerializeField] private GameObject UIPrefabe;
    public GameObject GotUI
    {
        get { return UIPrefabe; }
    }
    public enum Type
    {
        Poisoning,
        Frozen,
        Heal,
    }
    [SerializeField] private string Name;

    [SerializeField][TextArea(8, 10)] private string Description;

    [SerializeField] private Type _type;//類型

    [SerializeField] private float Value;//數值

    [SerializeField] private float _ActionTime;//每幾秒執行一次

    [SerializeField] private float BaseDurationTime;
    public float GotBaseDurationTime
    {
        get
        {
            return BaseDurationTime;
        }
    }
    float _DurationTime;
    public float DurationTime//持續多久
    {
        get
        {
            return _DurationTime;
        }
        set
        {
            _DurationTime = value;
            if (_DurationTime<0)
            {
                _DurationTime = 0;
            }
        }
    }
    public float GotValue
    {
        get
        {
            return Value;
        }
    }
    public float GotRepeatTime
    {
        get
        {
            return _ActionTime;
        }
    }
    public Type GotType
    {
        get
        {
            return _type;
        }
    }
}
