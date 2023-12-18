﻿using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "NewActor", menuName = "ScriptableObjects/ActorScriptable")]

public class ActorScriptable : ScriptableObject
{

	[field: SerializeField]
	public float health { get; private set; } = 2F;

	[field: SerializeField]
	public float maxSpeed { get; private set; } = 25F;

	[field: SerializeField]
	public float moveSpeed { get; private set; } = 5F;

	[field: SerializeField]
	public float acceleration { get; private set; } = 0.5F;

	[field: SerializeField]
	public float deceleration { get; private set; } = 0.8F;
}