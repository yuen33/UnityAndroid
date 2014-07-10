using UnityEngine;
using System.Collections;

public class Moving : MonoBehaviour {
	int delta;
	Vector3 movingDirection;
	bool changingDirection=false;
	// Use this for initialization
	void Start () {
		delta=100;
		movingDirection=new Vector3(Random.value-0.5f,0.0f,Random.value-0.5f);
		movingDirection=movingDirection.normalized;
		
		gameObject.rigidbody.velocity= delta* movingDirection;
	}
	
	// Update is called once per frame
	void Update () {

		if(Random.value-0.7>0){
			changingDirection=true;
		}else{
//			gameObject.rigidbody.AddForce( delta* movingDirection);
			gameObject.rigidbody.velocity= delta* movingDirection;
		}

		if(changingDirection){
			movingDirection=new Vector3(Random.value-0.5f,0.0f,Random.value-0.5f);
			movingDirection=movingDirection.normalized;
//			gameObject.rigidbody.AddForce( delta* movingDirection);
			gameObject.rigidbody.velocity= delta* movingDirection;

			changingDirection=false;
		}//if changingDirection

	}//Update()

	void OnCollisionEnter(Collision collision) {

		if(collision.gameObject.layer==9){
//			Debug.Log("Collide!");
			gameObject.rigidbody.velocity=-delta*movingDirection;
		}
	}

}//class
