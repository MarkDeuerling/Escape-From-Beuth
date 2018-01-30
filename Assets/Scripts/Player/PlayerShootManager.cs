using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/**
 * This class can only be used in 2D behaviours
 */
public class PlayerShootManager : NetworkBehaviour {

	[SerializeField]
	private GameObject _reloadPrefab;

	[SerializeField]
	private float _reloadDieTime;

	[SerializeField]
	private GameObject _sleevePrefab;

	[SerializeField]
	private float _sleeveDieTime;

	[SerializeField]
	private Bullet[] _bullets;

	[SerializeField]
	private float _fireRate;

	[SerializeField]
	private ShootType _shootType;

	[SerializeField]
	private int _magazineSize;

	[SerializeField]
	private int _currentSize;

	[SerializeField]
	private InputManager _input;

	[SerializeField]
	private float _reloadTime;

	private bool _isOnReload = false;
	private bool _isFireReady = true;
	private Transform _selfTransform;
	private ObjectPool _pool; // pool
	// private AudioSource audio;

	/**
	 * This class spawns the bullet with an angle, damage, speed(not relative to player),
	 * speed and position.
	 * Returns the instantiated bullet gameobject.
	 */
	[System.Serializable]
	struct Bullet {
		public GameObject ProjectilePrefab;
		public GameObject HitPrefab;
		public float Angle;
		public int Damage;
		public float Speed;
		public Vector3 Position;

		/**
		 * Spawns the projectile on given position and rotation.
		 * Has also an angle.
		 * @param: position, the position to spawn the prejctile.
		 * @param: rotation, the roation on the spawned projectile.
		 * @return: the spawned projectile.
		 */
		public GameObject Spawn(Transform position, Quaternion rotation, ObjectPool pool) { // pool
			rotation *= Quaternion.Euler (0, 0, Angle);

			var relPos = position.TransformPoint(Position);

			var bullet  = pool.Activate(relPos, rotation); // pool

			if (bullet == null)
				return null;
			
			var bc = bullet.GetComponent<BulletController>(); // cannot be used 'as' ??
			bc.Speed = Speed;
			bc.Damage = Damage;
			bc.HitPrefab = HitPrefab;

			return bullet.gameObject;
		}
	}

	[System.Serializable]
	enum ShootType {
		Halfautomatic,
		Fullautomatic
	}

	public int MagazineSize { get{ return _magazineSize; } set{ _magazineSize = value; } }
	public int CurrentSize { get{ return _currentSize; } set{ _currentSize = value; } }
	public float ReloadTime { get{ return _reloadTime; } set{ _reloadTime = value; } }

	void Start() {
		CurrentSize = MagazineSize;
		_selfTransform = transform;

		_pool = new ObjectPool(); 	// pool
		CacheObject(); 				// pool
		CmdCache(); 				// pool
		// audio = GetComponent<AudioSourche>();
	}

	/**
	 * Cache bullets with size of magazine.
	 */ 
	void CacheObject() {
		for (var i = 0; i < _bullets.Length; i++)
			_pool.CacheObject(_bullets[i].ProjectilePrefab, MagazineSize);
	}

	/**
	 * From all cached object, create them on server.
	 */ 
	[Command]
	void CmdCache() {
		var obj = _pool.GetAllCacheObject();

		for (var i = 0; i < obj.Length; i++)
			NetworkServer.Spawn(obj[i]);
	}

	void Update() {
		if (!isLocalPlayer)
			return;

		if (_input.getInputs())
			GunType ();
	}

	/**
	 * Switch between gun types by given enum.
	 */
 	void GunType() {
		if (!Loaded())
			return;

		switch (_shootType) {
		case ShootType.Halfautomatic:
			Halfautomatic();
			break;

		case ShootType.Fullautomatic:
			Fullautomatic();
			break;
		}
	}
	
	void Halfautomatic() {
		if(_input.getInputDown()) {
			if(_isFireReady)
				StartCoroutine(FireBullet());
		}
	}
	void Fullautomatic() {
		if (_input.getInputs()) {
			if(_isFireReady)
				StartCoroutine(FireBullet());
		}
	}

	/**
	 * Controlls how fast bullets will be spawnd
	 */
	IEnumerator FireBullet(){
		_isFireReady = false;
		CurrentSize--;
		CmdInstantiateBullet();
		// audio.play();

		yield return new WaitForSeconds (_fireRate);

		_isFireReady = true;
	}

	/**
	 * Instantiate all bullets in list.
	 */ 
	void InstantiateBullet() { // will called 3 time if size is 3 but only 1 bullet will be activate
		if (_bullets.Length == 1) {
			PrewarmInstantiate(0);
		} else {
			for (var i = 0; i < _bullets.Length; i++) {
				PrewarmInstantiate(i);
			}
		}
	}

	/**
	 * Instantiate the bullet  and sleeve on given index.
	 * @param: the index of the list.
	 */ 
	void PrewarmInstantiate(int index) {
		_bullets[index].Spawn(_selfTransform, _selfTransform.rotation, _pool);

		if (_sleevePrefab != null) {
			var sleeve = Instantiate(_sleevePrefab, transform.position, transform.rotation) as GameObject;
			Destroy(sleeve, _sleeveDieTime);
		}
	}

	/**
	 * Call on Server to all clients to activate bullet and instantiate sleeve.
	 */ 
	[Command]
	void CmdInstantiateBullet() {
		RpcInstantiateBullet();
	}

	/**
	 * Call from server on all clients to instantiate bullet and sleeve.
	 */ 
	[ClientRpc]
	void RpcInstantiateBullet() {
		InstantiateBullet ();
	}

	/**
	 * Keep an eye on current size and reload flag.
	 * Return true if current size is greater than 0.
	 * Return false otherwise.
	 */
	bool Loaded() {
		if (CurrentSize <= 0) {
			if(_isOnReload)
				return false;
			else {
				StartCoroutine(Reload());
				return false;
			}
		}
		return true;
	}

	/**
	 * Wait until reloading is done.
	 * Instantiate reload prefab.
	 */
	IEnumerator Reload() {
		_isOnReload = true;

		if (_reloadPrefab != null) {
			var reloadMagazine = Instantiate(_reloadPrefab, transform.position, transform.rotation) as GameObject;
			Destroy(reloadMagazine, _reloadDieTime);
		}

		yield return new WaitForSeconds(ReloadTime);

		CurrentSize = MagazineSize;
		_isOnReload = false;
	}

	// Draw the bullets position in scene (not visiable in game)
	void OnDrawGizmos() {
		for (var i = 0; i < _bullets.Length; i++) {
			Gizmos.DrawWireSphere (transform.TransformPoint(_bullets[i].Position), 0.1f);
		}
	}
}