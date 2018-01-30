using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/**
 * This class represents the Player to controller movement, mouse rotation (may game pad)
 * and spawning.
 */
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class PlayerController : NetworkBehaviour {
	
	[SerializeField]
	private Movement _movement;

	[SerializeField]
	private Canvas _canvasPrefab;

	[SerializeField]
	private PlayerController.Animation _animation;

	private Canvas gui;
	private Rigidbody2D _rig;
	private Transform _selfTransfrom;
	private Rotation _rotation;
	private EnergyManager _energyManager;
	private Animator _animator;

	#region Nested Enums, Structs, Class
	/**
	 * This nested struct represents simple movement with keyboard.
	 */
	[System.Serializable]
	struct Movement {
		public float Speed;
		private const string _vertical = "Vertical";
		private const string _horizontal = "Horizontal";

		/**
		 * Move the player on input.
		 * @param: rig, the 2D rigidbody for physical movement.
		 */
		public void Move(Rigidbody2D rig) {
			var v = Input.GetAxis (_vertical);
			var h = Input.GetAxis (_horizontal);

			var velocity = Vector2.zero;
			velocity.x = h;
			velocity.y = v;

			// velocity vector is normalized for correct movement
			rig.MovePosition(rig.position + velocity.normalized * Speed * Time.fixedDeltaTime);
		}
	}
		
	/**
	 * This nested class represents mouse rotation.
	 */
	class Rotation {
		private Vector3 _oldPosition = Vector3.zero;

		/**
		 * Rotate the player to mouse position (look at mouse position).
		 * @param: transform, the transfrom of the player object.
		 */ 
		public void Rotate(Transform transform) {
			var mousePos = Input.mousePosition;
			// This can only be used with orthoraphig camera
			var relMousePos = Camera.main.ScreenToWorldPoint (mousePos);

			// cache the old value to improve performance.
			if (mousePos == _oldPosition)
				return;

			var rot = Quaternion.LookRotation (transform.position - relMousePos, Vector3.forward);
			rot.x = 0;
			rot.y = 0;

			transform.rotation = rot;

			_oldPosition = mousePos;
		}
	}

	[System.Serializable]
	struct Animation {
		public string Move;
		public string Attack;
		public string Hit;
	}
	#endregion

	// singleton of player, has to be checkt with networking
	public static PlayerController Instance { get; set; }

	void Awake() {
		_rotation = new Rotation();
		Instance = this;
	}

	void Start() {
		_rig = GetComponent<Rigidbody2D>();
		_energyManager = GetComponent<EnergyManager>();
		_selfTransfrom = transform;
		_animator = GetComponent<Animator>();

		// beginning of network related stuff
		if (!isLocalPlayer)
			return;

		InitGUI();
	}

	void Update() {
		if (!isLocalPlayer)
			return;

		UpdateLifeState();
		_rotation.Rotate(_selfTransfrom);
	}

	void FixedUpdate() {
		if (!isLocalPlayer)
			return;
		
		_movement.Move(_rig);
	}

	/**
	 * Initialize the given GUI component and pass the energyManager to its reference.
	 */
	void InitGUI() {
		gui = Instantiate(_canvasPrefab);
		var emGUI = gui.GetComponentInChildren<EnergyManagerGUI> ();
		emGUI.ManagerEnergy = GetComponent<EnergyManager>();
	}

	/**
	 * Handle the life state of the player object
	 */ 
	void UpdateLifeState() {
		if (_energyManager.CurrentValue <= 0) {
			Respawn();	
		}
	}

	/**
	 * Respawns the player object on first network start position,
	 * if the players hp is less or equal 0.
	 */
	void Respawn() {
		var pos = NetworkManager.singleton.startPositions[0];
		_rig.position = new Vector2(pos.position.x, pos.position.y);
		_energyManager.CmdResetHeal();
	}
}
