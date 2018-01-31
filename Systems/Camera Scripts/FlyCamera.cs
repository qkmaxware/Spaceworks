using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks.View {

public class FlyCamera : MonoBehaviour {

	public float cameraSensitivity = 90;
	public float climbSpeed = 4;
	public float normalMoveSpeed = 10;
	public float slowMoveFactor = 0.25f;
	public float fastMoveFactor = 3;
 
	public float rollSpeed = 50;

	public KeyCode forward = KeyCode.W;
	public KeyCode backward = KeyCode.S;
	public KeyCode right = KeyCode.D;
	public KeyCode left = KeyCode.A;
	public KeyCode up = KeyCode.Space;
	public KeyCode down = KeyCode.LeftControl;

	public KeyCode rollLeft = KeyCode.Q;
	public KeyCode rollRight = KeyCode.E;

	public KeyCode speedUp = KeyCode.F;
	public KeyCode slowDown = KeyCode.Z;

	void Update ()
	{
		float rotationX = Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
		float rotationY = Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
 
		transform.Rotate (transform.up, rotationX, Space.World);
		transform.Rotate (transform.right, -rotationY, Space.World);
 
		float vertical = (Input.GetKey (forward) ? 1 : (Input.GetKey (backward) ? -1 : 0));
		float horizontal = (Input.GetKey (right) ? 1 : (Input.GetKey (left) ? -1 : 0));

		float multiplier = (Input.GetKey (speedUp) ? fastMoveFactor : (Input.GetKey (slowDown) ? slowMoveFactor : 1));

		transform.position += transform.forward * normalMoveSpeed * multiplier * vertical * Time.deltaTime;
		transform.position += transform.right * normalMoveSpeed * multiplier * horizontal * Time.deltaTime;
 		
		float roll = (Input.GetKey (rollRight) ? 1 : (Input.GetKey (rollLeft) ? -1 : 0));
 		
		transform.Rotate (transform.forward, roll * rollSpeed * Time.deltaTime, Space.World);

		if (Input.GetKey (up)) {transform.position += transform.up * climbSpeed * multiplier * Time.deltaTime;}
		if (Input.GetKey (down)) {transform.position -= transform.up * climbSpeed * multiplier * Time.deltaTime;}
	}
}

}