using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	public GameObject cam;
	public float spawnRadius = 10;

	private string version = "1";
	private string roomName = "INFERNO";

	void Start () {
		PhotonNetwork.logLevel = PhotonLogLevel.Full;
		PhotonNetwork.playerName = "Player "+Time.time;
		PhotonNetwork.autoJoinLobby = false;
		PhotonNetwork.ConnectUsingSettings(version);
	}
	
	void OnConnectedToMaster () {
		PhotonNetwork.CreateRoom(roomName);
	}
	void OnPhotonCreateRoomFailed () {
		PhotonNetwork.JoinRoom(roomName);
	}

	void OnJoinedRoom () {
		Vector2 spawnArea = Random.insideUnitCircle * spawnRadius;
		
		GameObject player = PhotonNetwork.Instantiate("Player", Vector3.up * 7, Quaternion.identity, 0);
		player.AddComponent("MouseLook");
		player.AddComponent("CharacterMotor");
		player.AddComponent("FPSInputController");
		
		// only local players instantiates a camera and audio listener
		//(player.GetComponent("Player") as Player).cam = cam; 
		cam.transform.parent = player.transform;
		cam.transform.rotation = player.transform.rotation;
		cam.transform.position = player.transform.position + Vector3.up;
		
		GameObject spawn = PhotonNetwork.Instantiate("Spawn", new Vector3(spawnArea.x, 1.5f, spawnArea.y), Quaternion.identity, 0);
		(spawn.GetComponent("Spawn") as Spawn).target = player; // only local spawn attaches to it's player
	}
	
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
	}
}
