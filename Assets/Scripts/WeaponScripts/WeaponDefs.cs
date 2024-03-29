﻿using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public static class WeaponDefs
{
	public static string OBJECT_WEAPON_TAG = "ObjectWeapon";
	public static string EQUIPPED_WEAPON_TAG = "EquippedWeapon";

	public static float GLOBAL_PICKUP_RANGE = 1F;

	public static float THROW_ROTATE_LOW = 100.0F;
	public static float THROW_ROTATE_MID = 350.0F;
	public static float THROW_ROTATE_HIGH = 600.0F;

	public static bool canWeaponBePickedUp(GameObject target)
	{
		return target.tag.StartsWith("Object") && target.tag.Equals(WeaponDefs.OBJECT_WEAPON_TAG);
	}
}
public enum WeaponType
{
	UNARMED,
	SWING,
	STAB,
	SHOOT
};