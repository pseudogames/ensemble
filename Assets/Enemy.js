#pragma strict

public var target : GameObject;
private var nma : NavMeshAgent;

function Start () {
	nma = gameObject.GetComponent(NavMeshAgent) as NavMeshAgent;
	InvokeRepeating('Restore', 0, 1);
}

function Restore () {
	gameObject.renderer.material.color = Color.white;
}

function FixedUpdate () {
	var distance : float = Vector3.Distance(target.transform.position, gameObject.transform.position);
	var touch : float = target.collider.bounds.extents.magnitude + gameObject.collider.bounds.extents.magnitude;
	if(distance > touch*1.05) {
		nma.destination = target.transform.position;
	} else {
		gameObject.renderer.material.color = Color.red;
		target.transform.localScale.y *= 0.95 + 0.05 * (1 - gameObject.transform.localScale.y); // the smaller the enemy, the less the damage
		if(target.transform.localScale.y < 0.25) {
			target.transform.localScale.y = 1;
			target.transform.position = Vector3.up * 7;
		}
	}
}

