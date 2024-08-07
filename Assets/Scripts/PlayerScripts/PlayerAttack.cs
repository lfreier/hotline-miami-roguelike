﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
	public Actor player;
	public PlayerInputs inputs;

	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (inputs.attackInput() > 0)
		{
			player.attack();
		}

		else if(inputs.secondaryAttackInput() > 0)
		{
			player.attackSecondary();
		}
	}
}