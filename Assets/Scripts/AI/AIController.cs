using UnityEngine;
using System.Collections;

/**
 * This class represence the simple AI.
 */ 
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class AIController : EntityBehaviour {
	
	[SerializeField]
	private Movement _movement;

	[SerializeField]
	private ViewField _viewField;

	[SerializeField]
	private Attack _attack;

	[SerializeField]
	private DeathManager _deathManager;

	[SerializeField]
	private AIController.Animation _animation;

	private Rigidbody2D _rig;
	private Transform _selfTransform;
	private Rotation _rotation;
	private EnergyManager _energyManager;
	private Animator _animator;

	#region Nested Enums, Structs, Classes
	/**
	 * This nested struct handles the movement of the AI.
	 * Is controlled by the player in view.
	 */ 
	[System.Serializable]
	struct Movement {
		public float Speed;

		/**
		 * The movement of the AI.
		 * @param rig: the 2D rigidbody for physical movement.
		 * @param viewField: the field of view for the target to move.
		 */ 
		public void Move(Rigidbody2D rig, ViewField viewField) {
			Vector2 targetPos;

			if (viewField.Target == null)
				return;
			
			targetPos = viewField.Target.position;
			var targetPositionRelative = targetPos - rig.position;
			rig.MovePosition(rig.position + targetPositionRelative.normalized * Speed * Time.fixedDeltaTime);
		}
	}

	/**
	 * This nested struct handles the rotation to player object.
	 */ 
	struct Rotation {

		public void RotateToTarget(Transform transform, ViewField viewField) {
			var self = transform;

			if (viewField.Target == null)
				return;
			
			var targetPos = viewField.Target.position;
			var rot = Quaternion.LookRotation (self.position - targetPos, Vector3.forward);
			rot.x = 0;
			rot.y = 0;

			transform.rotation = rot;
		}
	}

	/**
	 * This nested class represence the attack pattern of this AI.
	 */ 
	[System.Serializable]
	class Attack {
		public float Distance = 0;
		public int Damage = 0;
		public float AttackRate = 0;

		private bool _isAttacking = false;

		/**
		 * Attack the given target if it is in the range of the field of view.
		 * @param transform: the transform of the AI.
		 * @param viewField: the field of view of the AI.
		 * @param call: the a coroutin function.
		 */ 
		public void AttackTarget(Transform transform, ViewField viewField, System.Func<IEnumerator, Coroutine> call) {
			if (viewField.Target == null)
				return;

			var distanceToTarget = Vector3.Distance (transform.position, viewField.Target.position);

			if (distanceToTarget < Distance) {
				if (!_isAttacking)
					call(AttackTarget(viewField));
			}
		}

		/**
		 * Attacke the enemy on given target.
		 * @param viewField: the field of view of this AI.
		 * Returns a yield instruction.
		 */ 
		public IEnumerator AttackTarget(ViewField viewField) {
			_isAttacking = true;
			viewField.Target.GetComponent<EnergyManager>().CurrentValue -= Damage;
			// play attack animation (no loop)
			yield return new WaitForSeconds (AttackRate);
			//stop attack animation
			_isAttacking = false;
		}
	}

	/**
	 * This nested class represents the field of view of the AI.
	 */ 
	[System.Serializable]
	class ViewField {
		public Vector2 Point = Vector2.zero;
		public float Radius = 0;
		public Transform Target;

		private static readonly string Player = "Player";

		/**
		 * If a player object is in view range, than set it to its target
		 * @param transform: the transform of the AI
		 */
		public void LookForTarget(Transform transform) {
			var relPoint = transform.TransformPoint (Point);
			var targets = Physics2D.OverlapCircleAll (relPoint, Radius, 1 << LayerMask.NameToLayer(Player));

			if (targets.Length != 0)
				Target = targets[0].gameObject.transform;
			else
				Target = null;

			System.Array.Clear(targets, 0, targets.Length); // performance to test
		}
	}

	[System.Serializable]
	struct DeathManager{
		public GameObject DiePrefab;
		public float DieTime;
	}

	[System.Serializable]
	struct Animation {
		public string Move;
		public string Attack;
		public string Hit;
	}
	#endregion

	void Start() {
		_rig = GetComponent<Rigidbody2D> ();	
		_energyManager = GetComponent<EnergyManager>();
		_animator = GetComponent<Animator>();
		_selfTransform = transform;
	}

	void Update() {
		_viewField.LookForTarget(_selfTransform);
		_rotation.RotateToTarget(_selfTransform, _viewField);
		_attack.AttackTarget(_selfTransform, _viewField, StartCoroutine);
		Death();
	}

	void FixedUpdate() {
		_movement.Move (_rig, _viewField);
	}

	void OnDrawGizmos() {
		Gizmos.DrawWireSphere (transform.TransformPoint(_viewField.Point), _viewField.Radius);
	}

	/**
	 * If the hp is 0 than destroy this gameobject.
	 */
	void Death() {
		if (_energyManager.CurrentValue <= 0) {
			
			// play die animation, than do:

			if (_deathManager.DiePrefab != null) {
				var die = Instantiate(_deathManager.DiePrefab, _selfTransform.position, _selfTransform.rotation);
				Destroy(die, _deathManager.DieTime);
			}

			Destroy(gameObject);
		}
	}

}