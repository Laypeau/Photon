using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//make clicking on a box copy its text

public class Chatx3 : MonoBehaviour 
{
	[SerializeField] private TMP_Text text1;
	[SerializeField] private TMP_Text text2;
	[SerializeField] private TMP_Text text3;
	[SerializeField] private TMP_InputField inputField;

	public string[] messageArray= new string[3];

	void Start()
	{
		if (text1 == null || text2 == null || text3 == null || inputField == null)
		{
			throw new UnityException("Object references not set");
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			
		}
	}

	private void RefreshText()
	{
		text1.text = messageArray[2];
		text2.text = messageArray[1];
		text3.text = messageArray[0];
		inputField.text = "";
	}
}
