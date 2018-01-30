using UnityEngine;
using System.Collections;

/**
 * This class represends the camera following of the player object.
 */ 
public class Follow : MonoBehaviour {

	private Transform startPosi;
	
	[SerializeField]
	private Transform target;
	
	[SerializeField][Range(0,1f)]
	private float trackSpeed;
	
	void Start() {
		startPosi = transform;
	}
	
	void LateUpdate() {
		if (target == null) {
			if(PlayerController.Instance == null)
				return;
			
			target = PlayerController.Instance.transform;
		} else {
			var temp = target.position;
			temp.z = startPosi.position.z;
		
			transform.position = Vector3.Lerp (transform.position, temp, trackSpeed);
		}
	}
}
