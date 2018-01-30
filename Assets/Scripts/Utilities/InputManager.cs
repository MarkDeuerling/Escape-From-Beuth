using UnityEngine;
using System.Collections;

/**
 * This class handles the input of key and mouse.
 */ 
[System.Serializable]
public class InputManager {

	public string input;
	public bool mouseLeft;
	public bool mouseRight;
	public bool onDown;
	public bool onUp;

	/**
	 * Returns the mouse or key button.
	 */ 
	public bool getInputs(){
		return getMouseButton () || getKey ();
	}

	/**
	 * Returns the pressed mouse or key button.
	 */ 
	public bool getInputDown() {
		return getMouseButtonDown () || getKeyDown ();
	}

	/**
	 * Returns the pulled mouse or key button.
	 */ 
	public bool getInputUp() {
		return getMouseButtonUp () || getKeyUp ();
	}

	/**
	 * Returns input of mouse button left or right or key button.
	 */ 
	public bool getInput() {
		if (mouseLeft) 
			return mouseInput (0);
		else if (mouseRight)
			return mouseInput (1);
		else
			return keyInput ();
	}

	/**
	 * Returns the pressed mouse button left or right.
	 */ 
	public bool getMouseButtonDown(){
		if(mouseLeft)
			return Input.GetMouseButtonDown (0);
		if (mouseRight)
			return Input.GetMouseButtonDown (1);
		return false;
	}

	/**
	 * Returns the pulled mouse button left or right.
	 */ 
	public bool getMouseButtonUp() {
		if (mouseLeft)
			return Input.GetMouseButtonUp (0);
		if (mouseRight)
			return Input.GetMouseButtonUp (1);
		return false;
	}

	/**
	 * Returns the pressed or pulled mouse butten left or right.
	 */ 
	public bool getMouseButton() {
		if (mouseLeft)
			return Input.GetMouseButton (0);
		if (mouseRight)
			return Input.GetMouseButton (1);
		return false;
	}

	/**
	 * Returns pressed key.
	 */ 
	public bool getKeyDown(){
		if(input.Length == 0) return false;
		return Input.GetKeyDown (input);
	}

	/**
	 * Returns pulled key.
	 */ 
	public bool getKeyUp(){
		if(input.Length == 0) return false;
		return Input.GetKeyUp (input);
	}

	/**
	 * Returns pressed or pulled key.
	 */ 
	public bool getKey(){
		if(input.Length == 0) return false;
		return Input.GetKey (input);
	}

	/**
	 * Returns pressed or pulled mouse on given input.
	 * @param num: 0 for left or right for right.
	 */ 
	bool mouseInput(int num){
		if(onDown)
			return Input.GetMouseButtonDown (num);
		if(onUp)
			return Input.GetMouseButtonUp (num);

		return Input.GetMouseButton (num);
	}

	/**
	 * Returns key pressed or pulled.
	 */ 
	bool keyInput(){
		if (onDown)
			return Input.GetKeyDown (input);
		if (onUp)
			return Input.GetKeyUp (input);

		return Input.GetKey (input);
	}
}
