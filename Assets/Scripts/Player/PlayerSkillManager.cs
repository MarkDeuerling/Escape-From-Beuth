using UnityEngine;
using System.Collections;

public class PlayerSkillManager : SkillManager {

	[SerializeField]
	private InputManager _input;

	void Update() {
		SpawnByInput ();
	}
		
	public void SpawnByInput() {
		if (_input.onDown) {
			
			if(_input.getInputDown())
				SpawnSkill();
			if(_input.getInputUp())
				Destroy(Skill);
			
		}else if (_input.onUp) {
			
			if(_input.getInputDown())
				SpawnSkill();
			if(_input.getInputUp())
				Destroy(Skill);
			
		} else if (_input.getInputs ())
			SpawnSkill ();
	}
}
