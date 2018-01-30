using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

/**
 * This class represents the visible hp of the player object.
 * This class is not optimize!
 */ 
public class EnergyManagerGUI : NetworkBehaviour {	

	private Image _img;

	public EnergyManager ManagerEnergy { get; set; }

	void Start() {
		_img = GetComponent<Image> ();
	}

	void Update() {
		CalculatePercent ();
	}

	/**
	 * Calculate the player hp to visualize.
	 */ 
	void CalculatePercent(){
		if (ManagerEnergy != null) {
			var perc = (float)ManagerEnergy.CurrentValue / ManagerEnergy.MaxValue;
			_img.fillAmount = perc;
		}
	}
}
