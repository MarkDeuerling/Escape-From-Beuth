using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * This class represence a gameobject pool to save gameobjects.
 * Should be optimized, if bullet still in game and objects in list is active.
 */ 
public class ObjectPool {

	private List<GameObject> _cacheList;

	/**
	 * The constructor to initialze the list of gameobjects with count.
	 * @param count: the size of the list.
	 */
	public ObjectPool(int count = 10) {
		_cacheList = new List<GameObject>(count);

	}

	/**
	 * Create and cache object of count.
	 * @param prefab: the gameobject to be instantiate.
	 * @param count: the number of gameobject to be instantiate
	 */ 
	public void CacheObject(GameObject prefab, int count) {
		for (var i = 0; i < count; i++)
			Cache(prefab);
	}

	/**
	 * An array of all gameobject in list.
	 */ 
	public GameObject[] GetAllCacheObject() {
		return _cacheList.ToArray();
	}

	/**
	 *  Create and cache the gameobjet.
	 * @param prefab: the gameobject to be instantiate.
	 */ 
	public void Cache(GameObject prefab) {
		var obj = Object.Instantiate<GameObject>(prefab);
		obj.SetActive(false);
		_cacheList.Add(obj);
	}

	/**
	 * Deactivate any given gameobject.
	 * @param obj: the object that will be deactivated.
	 */ 
	public static void Deactivate(GameObject obj) {
		obj.SetActive(false);
	}

	/**
	 * Search for a deactivated gameobject in list and return it as active
	 * on given position and rotation.
	 * @param position: the position of the gameobject.
	 * @param rotation: the rotation of the gameobject.
	 */ 
	public GameObject Activate(Vector3 position, Quaternion rotation) {
		for (var i = 0; i < _cacheList.Count; i++) {
			var obj = _cacheList[i];

			if (!obj.activeInHierarchy) {
				obj.transform.position = position;
				obj.transform.rotation = rotation;
				obj.SetActive(true);

				return obj;
			}
		}
		return null;
	}
}
