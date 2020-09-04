using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//this goes on the camera focus gameobject
public class PlayerCamera : MonoBehaviour
{
	public Transform followingTransform;
	[HideInInspector] public Transform cameraTransform;

	public Vector3 cameraFocusOffset;
	public Vector3 cameraOffset = new Vector3(0.5f, 0.5f, 2f);
	[Range(0f, 1f)] public float cameraLerp = 1f;

	[Range(-10f, 10f)] public float sensitivityX = 5f;
	[Range(-10f, 10f)] public float sensitivityY = -5f;

	private float rotX = 0f; //Up negative, down positive
	private float rotY = 0f; //Left negative, Right positive

	public bool shoulderCam = true;

	private float trauma;
	private float traumaDelta;

	void Start()
	{
		if (transform.GetChild(0).GetComponent<Camera>() == null) throw new UnityException("Camera not set as child 0 of camera focus");
		else cameraTransform = transform.GetChild(0).transform;

		if (followingTransform == null) throw new UnityException("Following transform not set");

		transform.position = followingTransform.position;
		cameraTransform.localPosition = cameraOffset;
	}

	void LateUpdate()
	{
		rotX += Input.GetAxis("Mouse Y") * sensitivityY;
		rotX = Mathf.Clamp(rotX, -90f, 90f);
		rotY = rotY % 360f + Input.GetAxis("Mouse X") * sensitivityX;
		transform.rotation = Quaternion.Euler(rotX, rotY, 0f);

		transform.position = Vector3.Lerp(transform.position, followingTransform.position + cameraFocusOffset, cameraLerp);

		DoCameraShake();
	}

	public void AddShakeTrauma(float _TraumaAmount)
	{
		traumaDelta += _TraumaAmount;
	}

	private void DoCameraShake()
	{
		trauma = Mathf.Clamp(trauma + traumaDelta, 0f, 100f);
		traumaDelta = 0f;

		//ranges from -0.5 to 0.5
		float _ShakeX = (Mathf.Pow(trauma, 2f) / 10000) * (Mathf.PerlinNoise(69f, Time.time * 5f) - 0.5f);
		float _ShakeY = (Mathf.Pow(trauma, 2f) / 10000) * (Mathf.PerlinNoise(420f, Time.time * 5f) - 0.5f);

		cameraTransform.localPosition = cameraOffset + new Vector3(_ShakeX * 1f, _ShakeY * 1f, 0f); //expose shake amount

		trauma = Mathf.Clamp(trauma - (10f * Time.timeScale * Time.deltaTime), 0, Mathf.Infinity); //expose trama decrement
	}
}
