using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour {
	public SpriteRenderer Player;
	public float HP;
	public float GotHurt;
	public int PoisonChance;
	public int FireChance;
	private bool Poisoning;
	private bool Fireing;
	private float P_CooldDownTime;
	private float F_CooldDownTime;


	void FixedUpdate () {
		if (P_CooldDownTime > 1)
		{
			P_CooldDownTime -= Time.deltaTime;
		}
		else
		{
			Poisoning = false;
		}
		if(F_CooldDownTime>1)
		{
			F_CooldDownTime -= Time.deltaTime;
		}
		else
		{
			Fireing = false;
		}
		if (P_CooldDownTime < 1 && F_CooldDownTime < 1)
		{
			Player.color = new Color(1, 1, 1, 1);
		}
	}
	public void S_GotHurt(float Damage)
	{
		HP -= Damage;
		if (PoisonChance >= Random.Range(0, 1000)&& PoisonChance!=0)
		{
			PoisonHurt(10);
		}
		if (FireChance >= Random.Range(0, 1000)&& FireChance!=0)
		{
			FireHurt(5);
		}

	}
	public void PoisonHurt(float Coolfloat)
	{
		Poisoning = true;
		P_CooldDownTime = Coolfloat;
		S_PoisonHurt();
	}
	public void FireHurt(float CooldTime)
	{
		Fireing = true;
		F_CooldDownTime = CooldTime;
		S_FireHurt();
	}
	void S_PoisonHurt()
	{
		if (Poisoning)
		{
			Invoke("S_PoisonHurt", 1);
			HP -= GotHurt;
			Player.color = Color.green;
		}

	}
	void S_FireHurt()
	{
		if (Fireing)
		{
			Invoke("S_FireHurt", 1);
			HP -= GotHurt;
			Player.color = Color.red;
		}
	}
	
}
