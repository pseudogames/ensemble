#pragma strict

public var gun : GameObject;
public var damage : float = 0.05;
public var mask : LayerMask;

function FixedUpdate () {
	if(Input.GetKeyDown(KeyCode.Escape)) {
		Application.Quit();
	}
	
	if(Input.GetButton("Fire1")) {
		gun.particleSystem.Play();
		gun.audio.volume = gun.audio.volume * 0.95 + 0.05 * 0.1; // max volume
		
		
		var hit : RaycastHit;
		var jitter : Vector3 = gun.transform.right * (Random.value - 0.5) + gun.transform.up * (Random.value - 0.5);
		var direction : Vector3 = gun.transform.forward + jitter * 0.6;
		var distance : float = 6;
        if (Physics.Raycast(gun.transform.position, direction, hit, distance, mask)) {
        	Debug.DrawRay(gun.transform.position, direction * distance, Color.red, 0.1); 
        	hit.collider.transform.localScale.y *= (1 - damage);
    	    hit.collider.renderer.material.color = Color.black;
    	    (hit.collider.GetComponent('NavMeshAgent') as NavMeshAgent).speed = 2*hit.collider.transform.localScale.y;
            if(hit.collider.transform.localScale.y < 0.25) {
	        	Destroy(hit.transform.gameObject);
            }
        }
        
	} else {
		gun.particleSystem.Stop();
		gun.audio.volume = gun.audio.volume * 0.9 + 0.1 * 0; // min volume
	}

	if(gameObject.transform.localScale.y < 1) {
		var health : float = gameObject.transform.localScale.y * 0.99 + 0.01;
		var heal : float = health - gameObject.transform.localScale.y;
		gameObject.transform.position.y += heal;
		gameObject.transform.localScale.y = health;
	}
}