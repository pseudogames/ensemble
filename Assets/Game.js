#pragma strict

public var playerPrefab : GameObject;
public var spawnPrefab : GameObject;
public var spawnRadius : float = 10;

function Start () {
	var spawnArea : Vector2 = Random.insideUnitCircle * spawnRadius;
	var player : GameObject = GameObject.Instantiate(playerPrefab, Vector3.up * 7, Quaternion.identity);
	var spawn : GameObject = GameObject.Instantiate(spawnPrefab, Vector3(spawnArea.x, 1.5, spawnArea.y), Quaternion.identity);
	(spawn.GetComponent("Spawn") as Spawn).target = player;
}

function Update () {

}