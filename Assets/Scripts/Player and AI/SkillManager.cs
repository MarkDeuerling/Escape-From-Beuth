using UnityEngine;
using System.Collections;

public class SkillManager : EntityBehaviour {

	[SerializeField]
	private GameObject _skillPrefab;

	[SerializeField]
	private float _coolDownTime;

	[SerializeField]
	private float _castTime;

	[SerializeField]
	private bool _nest;

	[SerializeField]
	private Transform _spawnPosition;

	private GameObject _skill;
	private bool _isInCoolDown;
	private bool _isInCast;

	public float CoolDownTime { get{ return _coolDownTime; } set{ _coolDownTime = value; } }
	public float CastTime { get{ return _castTime; } set{ _castTime = value; } }
	public GameObject Skill { get{ return _skill; } set{ _skill = value; } }

	public void SpawnSkill(){
		if(!_isInCoolDown)
			StartCoroutine(ActivateSkill());
	}

	IEnumerator ActivateSkill() {
		_isInCoolDown = true;
		
		if(!_isInCast)
			StartCoroutine (CastSkill ());
		
		yield return new WaitForSeconds (CoolDownTime);

		_isInCoolDown = false;
	}
	
	IEnumerator CastSkill() {
		_isInCast = true;

		yield return new WaitForSeconds(CastTime);

		Skill = Instantiate (_skillPrefab, _spawnPosition.position, _spawnPosition.rotation) as GameObject;

		if (_nest)
			Skill.transform.parent = transform;
		_isInCast = false;
	}

}
