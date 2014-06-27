using UnityEngine;
using System.Collections;


public class Populating : MonoBehaviour {
	public GameObject dummy;
	public GameObject mycube;
//	public GameObject room;
	//TODO: get the unknown prefab extents
	public Vector3 mycubeExtents;

	public Vector3 offset;
	public Vector3 roomcenter;

	private int counter=-1;
	private GameObject currentObject;
	private bool isSearching=false;

	// Use this for initialization
	Vector3 getRandomPosition(){
		Vector3 randomPosition;
		randomPosition.x=(roomcenter.x-offset.x)+Random.value * 2* offset.x;
		randomPosition.y= mycubeExtents.y;//*********TODO !!!
		//To make it over the floor, extents=000 for any prefabs not in the scene
		randomPosition.z=(roomcenter.z-offset.z)+Random.value * 2* offset.z;

		return randomPosition;
	}

	GameObject Clone(Vector3 randomPosition){
		counter++;
		Debug.Log("counter="+counter);

		GameObject clone=(GameObject)Instantiate(mycube, transform.position+randomPosition, transform.rotation);//TODO rotation etc.
		clone.gameObject.layer=9;//the Layer 8 is the floorplan meshes
		
		//Write into the inRoomRetrival class
		InRoomRetrival.Instance.Tier1Check[counter]=true;//seems useless..in this class, as I have set a counter; ?_?
		InRoomRetrival.Instance.Tier1Data[counter,0]=clone.collider.bounds.center;
//		Debug.Log("clone.collider.bounds.center"+clone.collider.bounds.center);
		Debug.Log("Write in: InRoomRetrival.Instance.Tier1Data["+counter+",0]="+InRoomRetrival.Instance.Tier1Data[counter,0]);
		InRoomRetrival.Instance.Tier1Data[counter,1]=clone.collider.bounds.extents;
//		Debug.Log("clone.collider.bounds.extents"+clone.collider.bounds.extents);
//		Debug.Log("InRoomRetrival.Instance.Tier1Data[counter,1]"+InRoomRetrival.Instance.Tier1Data[counter,1]);

		return clone;
	}

	void Start () {
		mycubeExtents=0.5f*mycube.transform.lossyScale;
		offset=gameObject.collider.bounds.extents-mycubeExtents;//roomsize = 2* offset //*********TODO !!!
//		Debug.Log("room.collider.bounds.extents"+room.collider.bounds.extents);
		roomcenter=gameObject.collider.bounds.center;
//		Debug.Log("room.collider.bounds.center"+room.collider.bounds.center);

		Vector3 randomPosition=getRandomPosition();

//		currentObject=Clone(randomPosition);
		//-----------------------------------Changed for the first cube bug-------------------------------
		counter++;
		Debug.Log("counter="+counter);
		
		currentObject=(GameObject)Instantiate(dummy, transform.position+randomPosition, transform.rotation);//TODO rotation etc.
		currentObject.gameObject.layer=9;//the Layer 8 is the floorplan meshes

		/**
		 * If I didn't write in, the second cube will have the "first cube problem" :(
		 */
		//Write into the inRoomRetrival class
		InRoomRetrival.Instance.Tier1Check[counter]=true;//seems useless..in this class, as I have set a counter; ?_?
		InRoomRetrival.Instance.Tier1Data[counter,0]=currentObject.collider.bounds.center;
		//		Debug.Log("clone.collider.bounds.center"+clone.collider.bounds.center);
		Debug.Log("Write in: InRoomRetrival.Instance.Tier1Data["+counter+",0]="+InRoomRetrival.Instance.Tier1Data[counter,0]);
		InRoomRetrival.Instance.Tier1Data[counter,1]=currentObject.collider.bounds.extents;

		/** Solution 1: To destroy it later
		currentObject.name="dummy";
		*/

		//Solution 2 change render.enable to false
		currentObject.renderer.enabled=false;
		//-------------------------------------------------------------------------------------------------

	}

	bool Retrival(Vector3 randomPosition){
		Debug.Log("*********************************is Searching...***************************************");

		//whether it is in allocated arena
		for(int i=1;i<=counter;i++){
			Vector3 CentresDistance= InRoomRetrival.Instance.Tier1Data[i,0]-randomPosition;
			Vector3 fetchedExtents= InRoomRetrival.Instance.Tier1Data[i,1];

//			Debug.Log("Read in: InRoomRetrival.Instance.Tier1Check[counter]="+InRoomRetrival.Instance.Tier1Check[i]);
			Debug.Log("Read in: InRoomRetrival.Instance.Tier1Data["+i+",0]="+InRoomRetrival.Instance.Tier1Data[i,0]);
//			Debug.Log("randomPosition"+randomPosition);

//			Debug.Log("CentresDistance.x"+CentresDistance.x);
//			Debug.Log("fetchedExtents.x+mycubeExtents.x"+fetchedExtents.x+mycubeExtents.x);
//
//			Debug.Log("CentresDistance.z"+CentresDistance.z);
//			Debug.Log("fetchedExtents.z+mycubeExtents.z"+fetchedExtents.z+mycubeExtents.z);

			if( Mathf.Abs(CentresDistance.x) > fetchedExtents.x+mycubeExtents.x ||
			    Mathf.Abs(CentresDistance.z) > fetchedExtents.z+mycubeExtents.z){
				Debug.Log("It is not overlapped with Cube"+i);
				continue;
			}else{
				Debug.Log(":( OVERLAPPED with Cube"+i);
				return false;
			}
		}
		return true;
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Space) || isSearching){
			isSearching=true;
			Vector3 randomPosition=getRandomPosition();
			if( Retrival(randomPosition) ){
				currentObject=Clone(randomPosition);
				Debug.Log("Cube"+counter+" added.");
				isSearching=false;
			}else{
				Debug.Log("Overlapped...");
			}
		}

		if(Input.GetKeyDown(KeyCode.K)){
			isSearching=false;
			//Destroy(GameObject.Find("dummy"));
			for(int i=1;i<=counter;i++){
				Debug.Log("Cube"+i+" centre: "+InRoomRetrival.Instance.Tier1Data[i,0]);
			}
		}

	}
}
