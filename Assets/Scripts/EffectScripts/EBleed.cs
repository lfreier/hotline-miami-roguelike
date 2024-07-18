﻿using System.Collections;
using UnityEngine;

public class EBleed : MonoBehaviour, EffectInterface
{
	public float timer;
	public float tickTimer;

	public float bleedDamage;

	public Actor attachedActor;

	public EffectScriptable effectScriptable;

	// Use this for initialization
	void Start()
	{
		GameManager manager = GameManager.Instance;
		effectScriptable = manager.getEffectScriptable(GameManager.EFCT_SCRIP_ID_BLEED1);
	}

	// Update is called once per frame
	void Update()
	{
		timer -= Time.deltaTime;
		tickTimer -= Time.deltaTime;

		if (tickTimer <= 0)
		{
			tick(effectScriptable.tickLength);
		}
	}

	public float getTimer()
	{
		return timer;
	}

	public void init(EffectScriptable scriptable)
	{
		effectScriptable = scriptable;
	}

	public bool isPermanent()
	{
		return false;
	}

	public void tick(float seconds)
	{
		attachedActor.takeDamage(bleedDamage);
		tickTimer = effectScriptable.tickLength;

		Debug.Log(attachedActor.name + "bleeds for " + bleedDamage + " damage");

		if (timer <= 0)
		{
			Destroy(this);
		}
	}

	public void start(Actor target)
	{
		attachedActor = target;

		timer = effectScriptable.effectLength;
		tickTimer = effectScriptable.tickLength;

		bleedDamage = effectScriptable.effectStrength;
	}
}