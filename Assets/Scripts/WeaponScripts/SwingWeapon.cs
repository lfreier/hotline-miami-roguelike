﻿using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class SwingWeapon : MonoBehaviour, WeaponInterface
{
	[SerializeField] public Animator anim;

	LayerMask lastTargetLayer;
	string id;

	private const float equipPosX = 0.38F;
	private const float equipPosY = 0.15F;
	private const float equipRotZ = -67.5F;

	private bool attackTriggered;

	public Controller2D controller;
	public CircleCollider2D arc;
	public BoxCollider2D hitbox;

	public WeaponScriptable _weaponScriptable;
	public WeaponPhysics _weaponPhysics;

	void Start()
	{
		id = this.gameObject.name;
		_weaponPhysics.linkInterface(this);
	}

	void FixedUpdate()
	{
		// was just thrown, so give it initial speed
		_weaponPhysics.calculateThrow();
	}

	public bool attack(LayerMask targetLayer)
	{
		anim.SetTrigger("Attack");
		lastTargetLayer = targetLayer;
		attackTriggered = true;


		return true;
	}

	public bool canBeDropped()
	{
		if (_weaponScriptable.weaponType == WeaponType.UNARMED)
		{
			return false;
		}

		return true;
	}

	public WeaponScriptable getScriptable()
	{
		return _weaponScriptable;
	}

	public float getSpeed()
	{
		return _weaponScriptable.atkSpeed;
	}

	public WeaponType getType()
	{
		return _weaponScriptable.weaponType;
	}

	public bool inRange(Vector3 target)
	{
		return Vector3.Distance(transform.position, target) <= arc.radius;
	}

	public bool isActive()
	{
		return (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Idle") || _weaponPhysics.isBeingThrown());
	}

	public void physicsMove(Vector3 velocity)
	{
		controller.MoveRect(velocity);
	}

	public void setHitbox(bool toggle)
	{
		hitbox.enabled = toggle;
	}

	public void setStartingPosition()
	{
		transform.parent.SetLocalPositionAndRotation(new Vector3(equipPosX, equipPosY, 0), Quaternion.Euler(0, 0, equipRotZ));
	}

	/* Only deal with the movement of the throw */
	public void throwWeapon(Vector3 target)
	{
		_weaponPhysics.startThrow(target);
		hitbox.enabled = true;
	}

	public bool toggleCollider()
	{
		return arc.enabled = !arc.enabled;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		//TODO: deal with layermasks in a way that actually makes sense later
		Transform currParent = this.gameObject.transform;
		while(currParent != null)
		{
			if (collision.name == currParent.name)
			{
				Debug.Log("Stop hitting yourself");
				return;
			}
			currParent = currParent.transform.parent;
		}

		Actor actorHit = collision.GetComponent<Actor>();
		if (actorHit != null)
		{
			actorHit.takeDamage(_weaponScriptable.damage);
			Debug.Log("Hit: " + collision.name + " for " + _weaponScriptable.damage + " damage");
		}
		else
		{
			Debug.Log("Hit: " + collision.name);
		}
	}
}