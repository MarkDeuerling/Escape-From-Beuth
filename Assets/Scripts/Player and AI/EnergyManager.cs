using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/**
 * This class represence the Energy like HP or MP in network behaviour.
 */
public class EnergyManager : NetworkBehaviour {

	[SerializeField]
	private int _maxValue;

	[SyncVar]
	[SerializeField]
	private int _currentValue;

	[SerializeField]
	private int _recoverValue;

	[SerializeField]
	private int _recoverRate;

	private bool _isRecoverStepDone = true;

	public int MaxValue { get{ return _maxValue; } set{ _maxValue = value; } }
	public int CurrentValue { get{ return _currentValue; } set{ _currentValue = value; } }
	public int RecoverValue { get{ return _recoverValue; } set{ _recoverValue = value; } }
	public int RecoverRate { get{ return _recoverRate; } set{ _recoverRate = value; } }

	/**
	 * This function should be called on the server.
	 * Apply damage to the holder of this component.
	 * @param damage: the damage to apply.
	 */ 
	public void TakeDamage(int damage) {
		if (!isServer)
			return;

		CurrentValue -= damage;
	}

	/**
	 * This function is send to the server.
	 * Reset the current heal value to the max value.
	 */ 
	[Command]
	public void CmdResetHeal() {
		CurrentValue = MaxValue;
	}

	void Start() {
		CurrentValue = MaxValue;
	}

	void Update() {
		if (RecoverRate == 0 || CurrentValue == MaxValue)
			return;
		
		if (_isRecoverStepDone)
			StartCoroutine(Recover());

		Smooth ();
	}

	/**
	 * Recover the current value by time.
	 * Return a coroutine.
	 */ 
	IEnumerator Recover() {
		_isRecoverStepDone = false;

		yield return new WaitForSeconds (RecoverRate);

		CurrentValue += RecoverValue;
		_isRecoverStepDone = true;
	}

	/**
	 * Smooth the current value to 0 or max value.
	 * Value like -1 or current value greater than max value is not present.
	 */ 
	void Smooth() {
		if (CurrentValue > MaxValue)
			CurrentValue = MaxValue;
		if (CurrentValue < 0) 
			CurrentValue = 0;
	}
}