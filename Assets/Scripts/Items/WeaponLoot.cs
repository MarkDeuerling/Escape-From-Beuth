using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerShootManager))]
[RequireComponent(typeof(CircleCollider2D))]
public class WeaponLoot : EntityBehaviour {

	[SerializeField]
	private GameObject _weaponPrefab;

	[SerializeField]
	private bool _usePrefab;

	private GameObject _player; 

	void Start() {
		GetComponent<PlayerShootManager>().enabled = false;
	}

	void OnCollisionEnter2D(Collision2D entity) {
		_player = entity.gameObject;

		DestroyPlayerWeapon();

		if (_usePrefab)
			ReplaceWeaponGameobjectToPlayer();
		else
			ReplaceWeaponSpriteToPlayer();
		
		ReplaceShootManagerToPlayer();
	}

	/**
	 * Destroy the sprite of the player weapon.
	 */ 
	void DestroyPlayerWeapon() {
		Destroy(_player.transform.GetChild(0).gameObject);
	}

	/**
	 * Replace the weapon sprite from player with the sprite on this gameobject.
	 * The Gameobject with the weapon sprite have to be nested on player.
	 */ 
	void ReplaceWeaponSpriteToPlayer() {
		var spriteRenderer = GetComponent<SpriteRenderer>();
		bool hasCopyed = UnityEditorInternal.ComponentUtility.CopyComponent(spriteRenderer); // has to be removed for build

		if (hasCopyed) {
			var weaponObject = new GameObject();
			weaponObject.transform.position = _player.transform.position;
			weaponObject.transform.rotation = _player.transform.rotation;

			var weaponSprite = weaponObject.AddComponent<SpriteRenderer>();

			var hasPaste = UnityEditorInternal.ComponentUtility.PasteComponentValues(weaponSprite);

			if (hasPaste)
				weaponObject.transform.SetParent(_player.transform);
		}
	}

	/**
	 * Replace the weapon gameobject on player with on given prefab.
	 */ 
	void ReplaceWeaponGameobjectToPlayer() {
		var weapon = Instantiate(_weaponPrefab, _player.transform.position, _player.transform.rotation) as Transform;
		weapon.SetParent(_player.transform);
	}

	/**
	 * Replace the value of the shoot manager of the player.
	 * After calling this method, this gameobject will be destoryed.
	 * Call this function at least.
	 */ 
	void ReplaceShootManagerToPlayer() {
		// copy PlayerShootManager of this gameobject
		var comp = GetComponent<PlayerShootManager>();
		bool hasCopyed = UnityEditorInternal.ComponentUtility.CopyComponent(comp);

		if (hasCopyed) {
			// paste the copyed component to player
			var playerShootManager = _player.GetComponent<PlayerShootManager>();
			hasCopyed = UnityEditorInternal.ComponentUtility.PasteComponentValues(playerShootManager);

			playerShootManager.GetComponent<PlayerShootManager>().enabled = true;

			if (hasCopyed) 
				Destroy(gameObject);
		}
	}
}