using UnityEngine;
using System.Collections;

public class Arrangement : MonoBehaviour {
	public GameObject mycube;
	public GameObject room;
	//	public Bounds floor;
	//	public Bounds wall1;
	//	public Bounds wall2;
	//	public Bounds wall3;
	//	public Bounds wall4;
	//	public Bounds ceiling;
	public Vector3 offset;
	public Vector3 roomcenter;

	public int NumOfObjects=5;
	private uint counter=0;
	private string[] Tier1Names=new string[]{"arroundWall","pair1","pair2","freePrior","freeOthers"};
	private GameObject currentObject;
	private bool collided=false;

	GameObject populate(){
		Vector3 randomPosition;
		randomPosition.x=(roomcenter.x-offset.x)+Random.value * 2* offset.x;
		randomPosition.y= (float) (0.5* mycube.transform.lossyScale.y);//To make it over the floor, why not be extents.y? strange
		randomPosition.z=(roomcenter.z-offset.z)+Random.value * 2* offset.z;
		
		GameObject clone=(GameObject)Instantiate(mycube, transform.position+randomPosition, transform.rotation);//TODO rotation etc.

//		GameObject clone=(GameObject) Instantiate(mycube,roomcenter,transform.rotation);
		clone.gameObject.layer=8;//The first tier furniture can be regarded as the same tier as the floor etc.

		clone.name=Tier1Names[counter%NumOfObjects];
		switch(counter%NumOfObjects){
		case 0:
			clone.renderer.material.color=Color.white;
			break;
		case 1:
			clone.renderer.material.color=Color.green;
			break;
		case 2:
			clone.renderer.material.color=Color.green;
			break;
		case 3:
			clone.renderer.material.color=Color.yellow;
			break;
		default:
			clone.renderer.material.color=Color.red;
			break;
		}

		Debug.Log("clone.collider.bounds.extents"+clone.collider.bounds.extents);
		return clone;

//		Debug.Log("This cube position:"+clone.transform.position);
	}

	// Use this for initialization
	void Start () {
		//room=GameObject.Find("room_3_189");
		//room=GameObject.Find("room_1_185");
		offset=room.collider.bounds.extents-0.5f*mycube.transform.lossyScale;//roomsize = 2* offset
		Debug.Log("room.collider.bounds.extents"+room.collider.bounds.extents);
		roomcenter=room.collider.bounds.center;
		Debug.Log("room.collider.bounds.center"+room.collider.bounds.center);

		currentObject= populate();
	
	}
	
	// Update is called once per frame
	void Update () {
//		switch(counter){
//		case 1:
//			//alongWalls();
//			break;
//		case 2:
//			break;
//		case 3:
//			break;
//		case 4:
//			break;
//		default:
//			break;
//		}
		if(currentObject)

		if(Input.GetKeyDown(KeyCode.Space)){
			counter++;
			currentObject=populate();
		}

	}
}
