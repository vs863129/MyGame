using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Build", menuName = "Type/Building")]
public class BuildingClip : ScriptableObject {
	public GameObject Building;//建造物
	public string Name;//建築物名稱
	public Sprite Icon;//圖片
	public Sprite BuildingImage;
	[TextArea(10, 5)]
	public string Description;//描述
	[Header("空間")]
	public VillagerClip[] HabitablePeople;
	public VillagerClip[] JobVacancies; //職缺
	public WorkPosition m_WorkPosition;
	public List<VillagerClip> WorkingPeple = new List<VillagerClip>();
	[Header("碰撞範圍")]
	public int HCellsize;//格高
	public int WCellsize;//格寬
	[Header("資源")]
	public List<Cost_Resource> CostRes;//消耗資源
	#region 村民判斷
	public bool CheckVacancy(VillagerClip villager)
	{
		if (HabitablePeople != null)
		{
			for (int i = 0; i < HabitablePeople.Length; i++)
			{
				if (HabitablePeople[i] == null)
				{
					HabitablePeople[i] = villager;
					return true;
				}
			}
		}
		return false;
	}
	public bool CheckJobVacancies(VillagerClip villager)
	{
		if (JobVacancies.Length != 0)
		{
			for (int i = 0; i < JobVacancies.Length; i++)
			{
				if (JobVacancies[i] == null)
				{
					JobVacancies[i] = villager;
					return true;
				}
			}
		}
		return false;
	}
	public void EnterWorkingPlace(VillagerClip villager)
	{
		WorkingPeple.Add(villager);
	}
	public void LeaveWorkingPlace(VillagerClip villager)
	{
		for (int i = 0; i < WorkingPeple.Count; i++)
		{
			if (WorkingPeple[i].Stats.ID == villager.Stats.ID)
			{
				WorkingPeple.RemoveAt(i);
			}
		}
	}
	public bool HaveWorkPeople
    {
        get
        {
			if (WorkingPeple.Count != 0)
			{
				return true;
			}
			return false;
		}
    }
	#endregion
	public GameObject Build(Vector3 Pos, List<ResourceUI> Cost)
	{
		foreach(ResourceUI AllCost in Cost)
        {
			if(!AllCost.Isenough)
            {
				Debug.Log("資源不足");
				return null;
            }
        }
		return SpawnBuilding(Pos); 
	}
	public GameObject SpawnBuilding(Vector3 Pos)
    {
		Vector2 ColliderSize = new Vector2(WCellsize, HCellsize);
		GameObject obj = Instantiate(Building, Pos, Quaternion.identity);
		obj.GetComponent<BoxCollider2D>().size = ColliderSize;
		obj.GetComponent<Building>().clip = Instantiate(this);
		return obj;
    }
}

