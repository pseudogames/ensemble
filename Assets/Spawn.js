#pragma strict

public var prefab : GameObject;
public var target : GameObject;
//public var start : float = 2;
public var threshold : float = 0.8;
public var persistence : float = 0.99;

public var delay : float = 0.25;
private var timeout : float = 0;

private var max : float = 0;
private var avg : float = 0;
private var sum : float = 0;
private var count : float = 0;
private var actuation : float = 0;
private var trigger : boolean = false;

function Start () {
	//InvokeRepeating('Spawn', start, interval);
}

function Spawn () {
	var obj : GameObject = GameObject.Instantiate(prefab, gameObject.transform.position, gameObject.transform.rotation);
	var enemy : Enemy = obj.GetComponent(Enemy) as Enemy;
	enemy.target = target;
	
}


function OnAudioFilterRead(data:float[], channels:int) {
	for (var i = 1; i < data.Length; ++i) {
		var d : float = data[i] - data[i-1];
		sum += d * d;
	}
	count += data.Length;
}

function Update() {
	if(count) avg = sum / count;
	sum = 0;
	count = 0;
	max *= persistence;
	if(avg > max) max = avg;
	actuation = avg / max;
	if(actuation >= threshold) trigger = true;
	if(trigger && Time.time > timeout) {
		trigger = false;
		timeout = Time.time + delay;
		Spawn();
	}
	gameObject.renderer.material.color = (actuation*0.5+0.25) * Color.white;
}
