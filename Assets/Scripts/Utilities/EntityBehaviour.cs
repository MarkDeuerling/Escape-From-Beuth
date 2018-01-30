using UnityEngine;
using System.Collections;

/**
 * This class is an extended version of MonoBehaviour.
 */
public class EntityBehaviour : MonoBehaviour {

	[SerializeField]
	private Type _type;

	public Type type { get{ return _type; } set{ _type = value; } }
}

/**
 * This class is an extended version of NetworkBehaviour.
 */ 
public class NetworkEntityBehaviour : UnityEngine.Networking.NetworkBehaviour {

	[SerializeField]
	private Type _type;

	public Type type { get{ return _type; } set{ _type = value; } }
}
