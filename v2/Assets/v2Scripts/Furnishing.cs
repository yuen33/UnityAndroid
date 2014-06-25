using UnityEngine;
using System.Collections;

public class Furnishing : MonoBehaviour {

	public bool isShown=true;//TODO
	public GameObject mycube;

	public int NumOfObjects=5;
	public GameObject room;
//	public Bounds floor;
//	public Bounds wall1;
//	public Bounds wall2;
//	public Bounds wall3;
//	public Bounds wall4;
//	public Bounds ceiling;
	public Vector3 offset;
	public Vector3 roomcenter;

	// Use this for initialization
	void Start () {
		//Random.seed=33;

		room=GameObject.Find("room_3_189");
		offset=room.collider.bounds.extents;//roomsize = 2* offset
		roomcenter=room.collider.bounds.center;
	}

	void populate(){
		Vector3 randomPosition;
		randomPosition.x=(roomcenter.x-offset.x-mycube.transform.lossyScale.x)+Random.value * 2* offset.x;
		randomPosition.y= (float) (0.5* mycube.transform.lossyScale.y);//To make it over the floor, why not be extents.y? strange
		randomPosition.z=(roomcenter.z-offset.z-mycube.transform.lossyScale.z)+Random.value * 2* offset.z;
		
		GameObject clone=(GameObject)Instantiate(mycube, transform.position+randomPosition, transform.rotation);//TODO rotation etc.
		Debug.Log("This cube position:"+clone.transform.position);


//		if(collision.rigidbody){
//			Destroy(clone);
//			Debug.Log("Collision",clone);
//		}

	}



	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)){
			populate();
			//if()
		}
	}
}
