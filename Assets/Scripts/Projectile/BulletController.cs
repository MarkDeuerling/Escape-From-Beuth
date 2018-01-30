using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/**
 * This class represence the bullet.
 * Handles the movement and collision.
 */ 
[RequireComponent(typeof(Rigidbody2D))]
public class BulletController : NetworkBehaviour {

	[SerializeField]
	private GameObject _hitPrefab;

	[SerializeField]
	private float _speed;

	[SerializeField]
	private int _damage;

	private Rigidbody2D _rig;
	private Transform _selfTransform;
	private readonly string Player = "Player";
	private readonly string Enemy = "Enemy";

	public float Speed { get{ return _speed; } set{ _speed = value; } }
	public int Damage { get{ return _damage; } set{ _damage = value; } }
	public GameObject HitPrefab { get{ return _hitPrefab; } set{ _hitPrefab = value; } }

	void Start() {
		Init();
	}

	void OnCollisionEnter2D(Collision2D entity) {
		var enemy = entity.gameObject;

		if (enemy.CompareTag(Enemy) || enemy.CompareTag(Player))
			enemy.GetComponent<EnergyManager>().TakeDamage(Damage);

		if (_hitPrefab != null)
			Instantiate(_hitPrefab, transform.position, transform.rotation);
		
		// Destroy(gameObject);
		gameObject.SetActive(false); // pool
	}

	void OnEnable() {
		Init();
	}

	/**
	 * Initializer of this class.
	 * Can be used in Start() and/or OnEnable() function for example.
	 */ 
	void Init() {
		_rig = GetComponent<Rigidbody2D> ();
		_selfTransform = transform;
		_rig.velocity = _selfTransform.up * Speed;
	}

}
