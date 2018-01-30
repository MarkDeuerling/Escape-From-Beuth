using UnityEngine;
using System.Collections;

/**
 * This class represents the energy booster.
 * Every player object collide on this component,
 * get an boost on its energy manager current value.
 */ 
[RequireComponent(typeof(CircleCollider2D))]
public class EnergyBooster : EntityBehaviour {

	[SerializeField]
	private int _value;

	[SerializeField]
	private bool _usePercent;

	[SerializeField]
	private int _boostOverTime;

	[SerializeField]
	private float _timeRate;

	[SerializeField]
	private float _dieTime;

	private static readonly string Player = "Player";

	void OnCollistionEnter2D(Collision2D entity) {
		OnCollision (entity.gameObject);
	}

	void OnTriggerEnter2D(Collider2D entity) {
		OnCollision (entity.gameObject);
	}

	/**
	 * If a player object collider with an object with this component,
	 * the current value of player energy manager will be boost.
	 * @param entity: the gameobject to be checkt if it is the player.
	 */ 
	void OnCollision(GameObject entity) {
		if (entity.CompareTag (Player)) {
			var energyManager = entity.GetComponent<EnergyManager> ();

			if (_usePercent)
				BoostPercent (energyManager);
			else
				Boost (energyManager);

			Destroy (gameObject);
		}
	}

	/**
	 * Boost the player based on its max value (percentige).
	 * @param energyManager: the energyMananger of the player object.
	 */ 
	void BoostPercent(EnergyManager energyManager) {
		var max = energyManager.MaxValue;
		var perc = Mathf.RoundToInt(max * .01f * _value);
		energyManager.CurrentValue += perc;
	}

	/**
	 * Bost the player additional to its current value.
	 * @param energyManger: the energyManager of  the player object.
	 */ 
	void Boost(EnergyManager energyManager) {
		energyManager.CurrentValue += _value;
	}

	// not in use.
	void BoostOverTime(GameObject entity) {
		// add flag to entity
	}
}
