using UnityEngine;
using System.Collections;

public class Moving : MonoBehaviour {
	int delta;
	Vector3 movingDirection;
	bool changingDirection=false;
	bool pause=false;

	// Use this for initialization
	void Start () {
		rigidbody.rotation= Quaternion.identity;
		rigidbody.freezeRotation = true;

		delta=500;
		movingDirection=new Vector3(Random.value-0.5f,0.0f,Random.value-0.5f);
		movingDirection=movingDirection.normalized;
		
		gameObject.rigidbody.velocity= delta* movingDirection;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.P)){
			pause=!pause;
		}

		if(pause){
			gameObject.rigidbody.velocity=new Vector3(0,0,0);
		}else{
			//		changingDirection=!changingDirection;
			if(Random.value-0.9>0){
				changingDirection=true;
			}
			
			if(changingDirection){
				movingDirection=new Vector3(Random.value-0.5f,0.0f,Random.value-0.5f);
				movingDirection=movingDirection.normalized;
				gameObject.rigidbody.velocity= delta* movingDirection;
				
				changingDirection=false;
			}else{
				gameObject.rigidbody.velocity= delta* movingDirection;
			}//if Random.value...else
		}//if pause... else


	}//Update()

//	void OnCollisionEnter(Collision collision) {
//
//		if(collision.gameObject.layer==9){
////			Debug.Log("Collide!");
//			gameObject.rigidbody.velocity=-10*delta*movingDirection;
//		}
//	}

}//class
