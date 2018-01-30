using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class EnemySpawn : NetworkBehaviour {

	[SerializeField]
	private ViewField _fieldOfView;

	[SerializeField]
	private GameObject _enemyPrefab;

	[SerializeField]
	private int _spawnCount;

	[SerializeField]
	private float _spawnRate; // if is 0 burst spawn

	[SerializeField]
	private float _reactivationTime;

	private bool _isActivationRunning;
	/**
	 * This inner class represents the field of view of the enemy spawner.
	 */ 
	[System.Serializable]
	class ViewField{
		public Vector2 Position = Vector2.zero;
		public float Radius = 0;

		public int SpawnCount { get; set; }
		public float SpawnRate { get; set; }
		public Transform SelfTransform { get ; set; }
		public GameObject EnemyPrefab { get; set; }
		public bool Used { get; set; }

		private static readonly string Player = "Player";

		/**
		 * If a player is in range, enemy will start spawning.
		 * @param call: pass in your StartCoroutin of CustomStartCoroutin function.
		 */ 
		public void SpawnInRange(System.Func<IEnumerator, Coroutine> call) {
			var local = SelfTransform.TransformPoint(Position);
			var colliders = Physics2D.OverlapCircleAll(local, Radius);

			for (var i = 0; i < colliders.Length; i++) {
				var obj = colliders[i];

				if (obj.gameObject.CompareTag(Player)) {
					call(Spawn());
					Used = true;
					break;
				}
			}
		}

		/**
		 * Spawns the enemy of spawncount on server and send to client.
		 */
	    IEnumerator Spawn() {
			for (int i = 0; i < SpawnCount; i++) {
				var enemy = Instantiate (EnemyPrefab, SelfTransform.position, SelfTransform.rotation) as GameObject;
				NetworkServer.Spawn(enemy);

				yield return new WaitForSeconds(SpawnRate);
			}
		}
	}

	public override void OnStartServer() {
		base.OnStartServer();
		_fieldOfView.Used = false;
		InitFOV();
	}

	void Update() {
		if (!isServer)
			return;
		if (!_fieldOfView.Used)
			InitFOV();
	}

	void OnDrawGizmos() {
		Gizmos.DrawWireSphere (transform.TransformPoint(_fieldOfView.Position), _fieldOfView.Radius);
	}

	/**
	 * Initialize the field of view for AI scipted object (enemys).
	 */ 
	void InitFOV() {
		_fieldOfView.EnemyPrefab = _enemyPrefab;
		_fieldOfView.SpawnCount = _spawnCount;
		_fieldOfView.SpawnRate = _spawnRate;
		_fieldOfView.SelfTransform = transform;
		_fieldOfView.SpawnInRange(StartCoroutine);
	}
}
