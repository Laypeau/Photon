/*
Transform based character controller

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

	public float moveSpeed = 0.1f; //Clamp to < radius
	//public float moveSpeedAir = 0.05f;
	//public float maxAngle = 60f;
	public float stairStepThreshold = 0.3f; //Clamp to >= 0

	[SerializeField] private float minimumDewalling = 0.01f;
	[SerializeField] private int wallIterations = 100;
	private float gravity = -0.01f;
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
	
	void OnCollisionEnter(Collision collision)
	{
		Debug.Log("Collision enter");
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
		Vector3 simulatedMove = rawProjected * moveSpeed;
/*
		Ray groundRay = new Ray(transform.position + simulatedMove + (Vector3.up * (height - radius)), Vector3.down);
		Physics.SphereCast(groundRay, radius, out RaycastHit sphereHit, Mathf.Infinity, terrainMask, QueryTriggerInteraction.Ignore); //redo distance
		if (sphereHit.point.y >= transform.position.y)
		{
			gravVel = 0f;
			
			if (sphereHit.point.y - transform.position.y >= stairStepThreshold)
			{
				transform.position += Vector3.up * (sphereHit.point.y - transform.position.y);
				//simulatedMove += Vector3.up * (sphereHit.point.y - transform.position.y);
			}
		}
		else
		{
			if (transform.position.y - sphereHit.point.y < stairStepThreshold) //step threshold. this only works if the step is large enough and the movement is small enough
			{
				transform.position += Vector3.down * (transform.position.y - sphereHit.point.y);
				//simulatedMove += Vector3.down * (transform.position.y - sphereHit.point.y);
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
				//simulatedMove += Vector3.up * gravVel;
			}
		}
		*/

		float asdf = 0f;
		int i = 0;
		int o = 0;
		float DewallingDistance = 0f;
		do
		{
			asdf = 0f;
			foreach (Collider overlap in Physics.OverlapCapsule(transform.position + simulatedMove + (Vector3.up * radius), transform.position + simulatedMove + (Vector3.up * (height - radius)), radius, terrainMask, QueryTriggerInteraction.Ignore))
			{
				Physics.ComputePenetration(capsuleCollider, capsuleCollider.transform.position + simulatedMove, capsuleCollider.transform.rotation, overlap, overlap.transform.position, overlap.transform.rotation, out Vector3 direction, out DewallingDistance);
				simulatedMove += direction * DewallingDistance;
				if(DewallingDistance != 0f) asdf = DewallingDistance;
				o++;
			}
			i++;
			if (i >= wallIterations) break;
		} while(asdf > minimumDewalling);
		Debug.Log($"Iterations: {i}, Collisions: {o}, Distance: {asdf}");

		transform.position += simulatedMove;
	}
}