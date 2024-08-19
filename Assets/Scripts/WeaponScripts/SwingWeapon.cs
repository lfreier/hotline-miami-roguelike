﻿using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class SwingWeapon : MonoBehaviour, WeaponInterface
{
	[SerializeField] public Animator anim;

	public ActionInterface secondaryAction;

	LayerMask lastTargetLayer;
	string id;

	public Controller2D controller;
	public Collider2D hitbox;
	public BoxCollider2D throwBox;

	public SpriteRenderer sprite;
	public Sprite damagedSprite;
	public SpriteRenderer trailSprite;

	public Actor actorWielder;

	public WeaponScriptable _weaponScriptable;
	public WeaponPhysics _weaponPhysics;

	private float durability;

	void Start()
	{
		durability = _weaponScriptable.durability;
		id = this.gameObject.name;
		secondaryAction = GetComponent<ActionInterface>();
		if (secondaryAction == null)
		{
			secondaryAction = GetComponentInChildren<ActionInterface>();
		}
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

		return true;
	}

	public void attackSecondary()
	{
		if (secondaryAction != null)
		{
			secondaryAction.action();
		}
	}

	public bool canBeDropped()
	{
		if (_weaponScriptable.weaponType == WeaponType.UNARMED || !_weaponScriptable.canBeDropped)
		{
			return false;
		}

		return true;
	}

	public WeaponScriptable getScriptable()
	{
		return _weaponScriptable;
	}

	public Actor getActorWielder()
	{
		return actorWielder;
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
		return Vector3.Distance(transform.position, target) <= _weaponScriptable.npcAttackRange;
	}

	public bool isActive()
	{
		return (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Idle") || _weaponPhysics.isBeingThrown());
	}

	public void physicsMove(Vector3 velocity)
	{
		controller.transform.Translate(velocity);
	}

	public void reduceDurability(float reduction)
	{
		if (durability < 0)
		{
			return;
		}

		durability -= reduction;

		if (durability <= (_weaponScriptable.durability / 2) && damagedSprite != null)
		{
			//set to damaged sprite
			sprite.sprite = damagedSprite;
		}

		if (durability <= 0)
		{
			this.actorWielder.drop();
			Debug.Log("Weapon broke: " + this.name);
			Destroy(this.transform.parent.gameObject);
		}
	}
	public void setActorToHold(Actor actor)
	{
		actorWielder = actor;
		if (secondaryAction != null)
		{
			secondaryAction.setActorToHold(actor);
		}
	}

	public void setHitbox(bool toggle)
	{
		hitbox.enabled = toggle;
	}

	public void setStartingPosition()
	{
		transform.parent.SetLocalPositionAndRotation(new Vector3(_weaponScriptable.equipPosX, _weaponScriptable.equipPosY, 0), Quaternion.Euler(0, 0, _weaponScriptable.equipRotZ));
	}

	public void slowWielder(float percentage)
	{
		if (actorWielder != null)
		{
			float slowedSpeed = actorWielder.actorData.maxSpeed * percentage;
			actorWielder.setSpeed(slowedSpeed);
		}
	}

	/* Only deal with the movement of the throw */
	public void throwWeapon(Vector3 target)
	{
		anim.Rebind();
		anim.Update(0f);
		_weaponPhysics.startThrow(target, actorWielder);
	}

	public bool toggleCollider()
	{
		if (trailSprite != null)
		{
			trailSprite.enabled = !trailSprite.enabled;
		}
		return hitbox.enabled = !hitbox.enabled;
	}

	public bool toggleSecondaryCollider()
	{
		return secondaryAction.toggleHitbox();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		float knockbackMult = 1;
		//TODO: deal with layermasks in a way that actually makes sense later
		//TODO: put this somewhere else that's not specific for the weapon?
		if (this.actorWielder != null && collision.name == actorWielder.name)
		{
			Debug.Log("Stop hitting yourself");
			return;
		}

		if (_weaponPhysics.throwingActor != null && collision.name == _weaponPhysics.throwingActor.name)
		{
			Debug.Log("Stop hitting yourself");
			return;
		}

		Actor actorHit = collision.GetComponent<Actor>();
		if (actorHit != null && actorWielder.isTargetHostile(actorHit))
		{
			actorWielder.triggerDamageEffects(actorHit);
			actorHit.takeDamage(_weaponScriptable.damage);
			reduceDurability(1);
			knockbackMult = actorHit._actorScriptable.knockbackResist;
			Debug.Log("Hit: " + collision.name + " for " + _weaponScriptable.damage + " damage");
		}
		else
		{
			Debug.Log("Hit: " + collision.name + " for no damage");
		}

		/* knockback */
		Rigidbody2D hitBody = collision.attachedRigidbody;
		if (hitBody != null)
		{
			Vector3 force = Vector3.ClampMagnitude(hitBody.transform.position - _weaponPhysics.transform.position, 1);
			force *= _weaponScriptable.knockbackDamage * 1000 * knockbackMult;
			hitBody.AddForce(force);
		}
	}
}