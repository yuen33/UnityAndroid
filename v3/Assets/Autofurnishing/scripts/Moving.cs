using UnityEngine;
using System.Collections;

public class Moving : MonoBehaviour {
	int delta=100;
	Vector3 movingDirection;
	bool changingDirection=false;
	// Use this for initialization
	void Start () {
		delta=100;
	}
	
	// Update is called once per frame
	void Update () {
		if(Random.value-0.995>0){
			changingDirection=true;
		}else{
			gameObject.rigidbody.velocity= delta* movingDirection;
		}

		if(changingDirection){
			movingDirection=new Vector3(Random.value-0.5f,0.0f,Random.value-0.5f);
			movingDirection=movingDirection.normalized;
			
			gameObject.rigidbody.velocity= delta* movingDirection;

			changingDirection=false;
		}
	}
}
