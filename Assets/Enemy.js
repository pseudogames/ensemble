#pragma strict

public var spawn : GameObject;
private var nma : NavMeshAgent;

function Start () {
	nma = gameObject.GetComponent(NavMeshAgent) as NavMeshAgent;
	InvokeRepeating('Restore', 0, 1);
}

function Restore () {
	gameObject.renderer.material.color = Color.white;
}

function FixedUpdate () {
	var target : GameObject = (spawn.GetComponent("Spawn") as Spawn).target;
	var actuation : float = (spawn.GetComponent("Spawn") as Spawn).GetActuation();
	
	var effective : float = actuation * gameObject.transform.localScale.y;
	
	(gameObject.GetComponent('NavMeshAgent') as NavMeshAgent).speed = 6 * effective; // FIXME speed hardcoded

	var distance : float = Vector3.Distance(target.transform.position, gameObject.transform.position);
	var touch : float = target.collider.bounds.extents.magnitude + gameObject.collider.bounds.extents.magnitude;
	var wide : float = 1;
	if(distance > touch*1.05) {
		nma.destination = target.transform.position;
	} else {
		wide = 1 + effective;
		gameObject.renderer.material.color = Color.red;
		
		target.transform.localScale.y *= 0.98 + 0.02 * (1 - effective); // the smaller the enemy, the less the damage  // FIXME damage hardcoded
		if(target.transform.localScale.y < 0.25) {
			target.transform.localScale.y = 1;
			target.transform.position = Vector3.up * 7;
		}
	}
	gameObject.transform.localScale.x = gameObject.transform.localScale.z = gameObject.transform.localScale.z * 0.5 + 0.5 * wide;
}

