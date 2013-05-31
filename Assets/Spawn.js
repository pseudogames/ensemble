#pragma strict

public var prefab : GameObject;
public var target : GameObject;
public var start : float = 2;
public var interval : float = 10;

function Start () {
	InvokeRepeating('Spawn', start, interval);
}

function Spawn () {
	var obj : GameObject = GameObject.Instantiate(prefab, gameObject.transform.position, gameObject.transform.rotation);
	var enemy : Enemy = obj.GetComponent(Enemy) as Enemy;
	enemy.target = target;
	
}