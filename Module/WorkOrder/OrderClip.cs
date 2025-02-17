using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderClip : ScriptableObject
{
    public bool IsOut { get => _IsOut; }
    [SerializeField]
    bool _IsOut;
    public Transform WokrBuilding;
    public Villager villager;
    public virtual void Wokring() { }
    protected virtual void EnterWorkPlace() { villager.MoveToPlace(WokrBuilding); }
    public Item GotItem;
}
