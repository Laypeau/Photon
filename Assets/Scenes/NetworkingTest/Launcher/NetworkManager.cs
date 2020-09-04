using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	public static NetworkManager instance;

	public string loadScene = "Chatx3";
	[SerializeField] private string gameVersion = "Chatx3_1";
	[SerializeField] private TMP_InputField usernameField;
	[SerializeField] private TMP_InputField roomField;
	[SerializeField] private Button joinButton;
	[SerializeField] private TMP_Text statusText;

	void Awake()
	{
		if (instance == null || instance != this)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}

	void Start()
	{
		PlayerPrefs.DeleteAll();
		joinButton.interactable = false;
		PhotonNetwork.AutomaticallySyncScene = true;
		statusText.text = "Connecting to network...";

		PhotonNetwork.GameVersion = gameVersion;
		PhotonNetwork.ConnectUsingSettings();
	}

	public void JoinRoom()
	{
		if (PhotonNetwork.IsConnected)
		{
			if (!string.IsNullOrWhiteSpace(usernameField.text) || !string.IsNullOrWhiteSpace(roomField.text))
			{
				statusText.text = $"Joining {roomField.text}...";
				PhotonNetwork.LocalPlayer.NickName = usernameField.text;

				RoomOptions roomOptions = new RoomOptions();
				TypedLobby typedLobby = new TypedLobby(roomField.text, LobbyType.Default);
				PhotonNetwork.JoinOrCreateRoom(roomField.text, roomOptions, typedLobby);

				usernameField.interactable = false;
				roomField.interactable = false;
				joinButton.GetComponentInChildren<TMP_Text>().text = "Joining...";
			}
		}
	}

	#region Photon callbacks
	public override void OnConnected()
	{
		base.OnConnected();

		statusText.text = "Connected to Photon!";
		joinButton.interactable = true;
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		statusText.text = "Disconnected from Photon :(";
	}

	public override void OnCreatedRoom()
	{

	}

	public override void OnJoinedRoom()
	{
		statusText.text = $"Joined Room '{PhotonNetwork.CurrentRoom.Name}'";

		if (PhotonNetwork.IsMasterClient) //maybe put on room created? Check if it only calls back for the master
		{
			PhotonNetwork.LoadLevel(loadScene);
		}
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		statusText.text = "Join room failed :(";
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
	}

	public override void OnPlayerLeftRoom(Player player)
	{
		base.OnPlayerLeftRoom(player);

	}
	#endregion
}

