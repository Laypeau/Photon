/*
Transform based character controller
Currently doesn't work 
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CapsuleCollider))]
public class CharacterController_Transform : MonoBehaviour
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
		capsuleCollider = gameObject.GetComponent<CapsuleCollider>();
		try
		{
			cameraScript = cameraFocusTransform.GetComponent<PlayerCamera>();
		}
		catch
		{
			Debug.LogWarning("Camera does not have PlayerCamera component");
		}
		
		capsuleCollider.radius = radius;
		capsuleCollider.height = height;
		capsuleCollider.center = Vector3.up * height/2;

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

		Ray sphereRay = new Ray(transform.position + (Vector3.up * (height + radius)), Vector3.down); //make not of where it starts. using down for direction doesn't allow for player rotation
		Physics.SphereCast(sphereRay, radius, out RaycastHit sphereHitAlpha, Mathf.Infinity, terrainMask, QueryTriggerInteraction.Ignore); //redo distance, add layermask
		sphereHitAlpha.collider.Raycast(new Ray(sphereHitAlpha.point + Vector3.up, sphereRay.direction), out RaycastHit lineHit, 2f);

		Vector3 rawProjected = Vector3.ProjectOnPlane(rawRotated, lineHit.normal);
		//transform.position += rawProjected * moveSpeedGround;

		/*
		Ray groundRay = new Ray(transform.position + (rawProjected * moveSpeed), Vector3.down);
		Physics.SphereCast(groundRay, radius, out RaycastHit sphereHit, Mathf.Infinity, terrainMask, QueryTriggerInteraction.Ignore); //redo distance
		if (sphereHit.point.y >= transform.position.y)
		{
			gravVel = 0f;
			
			if (sphereHit.point.y - transform.position.y >= stairStepThreshold)
			{
				transform.position += Vector3.up * (sphereHit.point.y - transform.position.y);
			}
		}
		else
		{
			if (transform.position.y - sphereHit.point.y < stairStepThreshold) //step threshold. this only works if the step is large enough and the movement is small enough
			{
				transform.position += Vector3.down * (transform.position.y - sphereHit.point.y);
				gravVel = 0f;
			}
			else
			{
				if (transform.position.y - (gravVel + gravity) < sphereHit.point.y) //if gravity moves through ground, place on ground
				{
					gravVel = -(transform.position.y - sphereHit.point.y);
				}
				else
				{
					gravVel += gravity;
				}
				transform.position += Vector3.up * gravVel;
			}
		}
		*/
		
		/*
		RaycastHit[] rayHits = Physics.CapsuleCastAll(transform.position + (Vector3.up * radius), transform.position + (Vector3.up * (height - radius)), radius, rawProjected, moveSpeed, terrainMask, QueryTriggerInteraction.Ignore);
		//order the list from closest to furthest
		foreach (RaycastHit rayHit in rayHits)
		{
			do the same as for individual capsulecast
		}
		*/

		Vector3 simulatedMove = Vector3.zero;
		if(Physics.CapsuleCast(transform.position + (Vector3.up * radius), transform.position + (Vector3.up * (height - radius)), radius - 0.0000001f, rawProjected, out RaycastHit capsuleHit, moveSpeed, terrainMask, QueryTriggerInteraction.Ignore))  //It doesnt work without subtracting a millionth of a unit because of course it doesn't. Smaller values sometimes don't work
		{			
			if (capsuleHit.point.y - transform.position.y <= stairStepThreshold)
			{
				simulatedMove += Vector3.up * (capsuleHit.point.y - transform.position.y);
			}
			else //maybe remove the else here
			{
				//calculate distance from player to wall
				//why is it not projecting and moving?
				float wallDist = Vector3.Distance(Vector3.ProjectOnPlane(transform.position, Vector3.up), Vector3.ProjectOnPlane(capsuleHit.point, Vector3.up)) - radius; //Project them both on to ground normal
				simulatedMove += rawProjected * (wallDist - 0.000001f);
				
				//calculate distance moved beyond wall and move by that amount projected along wall normal
				float beyondWall = moveSpeed - wallDist;
				//capsuleHit.collider.Raycast(new Ray(transform.position + simulatedMove, Vector3.ProjectOnPlane(capsuleHit.point, Vector3.up) - Vector3.ProjectOnPlane(transform.position + simulatedMove, Vector3.up)), out RaycastHit capLineHit, 2f); //for projecting on to face normal
				Vector3 projectedWall = Vector3.ProjectOnPlane(rawProjected, capsuleHit.normal);
				if (Physics.CapsuleCast(transform.position + simulatedMove + (Vector3.up * radius), transform.position + simulatedMove + (Vector3.up * (height-radius)), radius - 0.0000001f, projectedWall, out RaycastHit capsuleHit2, beyondWall, terrainMask, QueryTriggerInteraction.Ignore))
				{
					float wallDist2 = Vector3.Distance(Vector3.ProjectOnPlane(transform.position, Vector3.up), Vector3.ProjectOnPlane(capsuleHit2.point, Vector3.up)) - radius;
					simulatedMove += projectedWall * (wallDist2 - 0.000001f);
				}
				else
				{
					simulatedMove += projectedWall * (beyondWall - 0.000001f);
				}
			}
		}
		else
		{
			simulatedMove += rawProjected * moveSpeed;	
		}
		transform.position += simulatedMove;

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