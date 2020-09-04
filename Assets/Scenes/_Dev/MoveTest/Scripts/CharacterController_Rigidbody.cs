/*
Rigidbody based character controller

*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class CharacterController_Rigidbody : MonoBehaviour
{
	#region [fields]
	public Transform cameraFocusTransform; //set in inspector
	private PlayerCamera cameraScript;
	private Rigidbody rigidBody;
	private CapsuleCollider capsuleCollider;

	public float radius = 0.5f;
	public float height = 2f;

	public float moveSpeed = 0.1f;
	//public float moveSpeedAir = 0.05f;
	//public float maxAngle = 60f;

	public float stairStepThreshold = 0.3f; //Clamp to >= 0

	private float gravity = -0.01f; //Expose
	private float gravVel;
	private LayerMask terrainMask;

	#endregion

	void Start()
	{
		rigidBody = gameObject.GetComponent<Rigidbody>();
		try
		{
			cameraScript = cameraFocusTransform.GetComponent<PlayerCamera>();
		}
		catch
		{
			Debug.LogWarning("Camera does not have PlayerCamera component");
		}
		terrainMask = LayerMask.GetMask("Terrain");

		Cursor.lockState = CursorLockMode.Locked;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1)) UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);

		if (Input.GetKeyDown(KeyCode.Escape) && Cursor.lockState == CursorLockMode.Locked)
		{
			Cursor.lockState = CursorLockMode.None;
		}
		Cursor.lockState = CursorLockMode.Locked;
	}

	void FixedUpdate()
	{
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");
		Vector3 rawRotated = Quaternion.Euler(0f, cameraFocusTransform.rotation.eulerAngles.y, 0f) * new Vector3(h, 0f, v).normalized;

		//calculate movement obstructed by walls


		//Take input
		//Project input along normal
		//Spherecast down
		//Refine spherecast with raycast down

		//Capsulecast all in direction of movement
		//for each rayhit
		//If hit point is higher than current pos
		//If point is lower than step threshold, step up
		//Else, move up to wall, break for loop
		//Move transform in direction

		//Check ground position of spherecast in relation to transform position
		//if ground higher than position
		//If ground lower than step threshold, step up
		//Else, move up to wall
		//Else (if ground lower than position)
		//If ground closer than step threshold, step down
		//Else fall by gravity


		//Fall movement
		//isGrounded bool

		//Collide with walls and capsulecast
		//Slide along walls
	}
}