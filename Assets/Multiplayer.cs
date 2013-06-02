using UnityEngine;
using System.Collections;

public class Multiplayer : MonoBehaviour {

	public const string version = "1";
	private string playerName = "";
	public string roomName = "INFERNO";
	private enum State { SPLASH, SETUP, CONNECT, PLAYING, QUIT };
	private State state;
	private string message;
	private GameObject player;
	public GameObject cam;
	
	void Start () {
		PhotonNetwork.logLevel = PhotonLogLevel.Full;
		PhotonNetwork.autoJoinLobby = false;
		StartScreen("ENSEMBLE");
	}
	
	void StartScreen (string msg) {
		state = State.SPLASH;
		message = msg;
		PhotonNetwork.ConnectUsingSettings(version);
	}
	
	void OnGUI () {
		switch(state) {
		case State.SPLASH:
			if(Input.anyKeyDown) {
				state = State.SETUP;
			}
			break;	
		case State.SETUP:
				if(playerName.Length > 0 && Event.current.keyCode == KeyCode.Return) {
					PhotonNetwork.playerName = playerName;
					if(PhotonNetwork.countOfRooms > 0) {
						PhotonNetwork.JoinRoom(roomName);
					} else {
						PhotonNetwork.CreateRoom(roomName);
					}
					state = State.CONNECT;
					message = "Entering room " + roomName;
				} else {
					message = "Waiting for the player name:";
					GUI.SetNextControlName("playerName");
					playerName = GUI.TextField (new Rect (20, 40, 256, 20), playerName, 32);
					GUI.FocusControl("playerName");
				}
			break;
		case State.PLAYING:
			message = "room " + roomName + ", player "+ playerName + ", ping "+ PhotonNetwork.GetPing();
			if(Input.GetKeyDown(KeyCode.Escape)) {
				cam.transform.parent = null;
				PhotonNetwork.Destroy(player);
				StartPlayer ();
			}
			break;
		}
		GUI.Label(new Rect (20, 20, 256, 20), message);

	}

    private void OnCreatedRoom()
    {
		
    }
	
    private void OnJoinedRoom()
    {
        StartPlayer();
    }
	
	private void StartPlayer()
	{
		state = State.PLAYING;
		player = PhotonNetwork.Instantiate("Hunter", Vector3.up*4, Quaternion.identity, 0);
		player.AddComponent("MouseLook");
		player.AddComponent("CharacterMotor");
		player.AddComponent("FPSInputController");
		player.AddComponent("Hunter");
		cam.transform.parent = player.transform;
		cam.transform.localPosition = Vector3.up;
		cam.transform.rotation = Quaternion.identity;
		
	}

	private void OnPhotonCreateGameFailed() {
		StartScreen("Failed to create the room");
	}

	private void OnPhotonJoinGameFailed() {
		StartScreen("Failed to join the room");
	}
	
	private void OnConnectedToPhoton () {
		message += ": Ready";
	}
	
    private void OnFailedToConnectToPhoton(object parameters) {
		StartScreen("Failed to connect: " + parameters);
	}

    private void OnDisconnectedFromPhoton()
    {
		StartScreen("Disconnected");
    }	
	
}




