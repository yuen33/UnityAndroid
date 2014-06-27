using UnityEngine;
using System.Collections;


public class Populating : MonoBehaviour {
//	public GameObject dummy;
//	public GameObject mycube;
//	public GameObject room;
	//TODO: get the unknown prefab extents
//	public Vector3 mycubeExtents;

	public Vector3 offset;
	public Vector3 roomcenter;

	private int counter=-1;
	private GameObject currentObject;
	private bool isSearching=false;

	public static int MAX_NumOfT1=20;
	public static int MAX_InfoItems=2;
	public bool[] Tier1Check= new bool[MAX_NumOfT1];
	public Vector3[,] Tier1Data= new Vector3[MAX_NumOfT1,MAX_InfoItems];

	// Use this for initialization
	Vector3 getRandomPosition(){
		Vector3 randomPosition;
		randomPosition.x=(roomcenter.x-offset.x)+Random.value * 2* offset.x;
		randomPosition.y= PrefabsManager.Instance.BigPiece.transform.lossyScale.y;//*********TODO !!!
		//To make it over the floor, extents=000 for any prefabs not in the scene
		randomPosition.z=(roomcenter.z-offset.z)+Random.value * 2* offset.z;

		return randomPosition;
	}

	GameObject Clone(Vector3 randomPosition){
		counter++;
		Debug.Log("counter="+counter);

		GameObject clone=(GameObject)Instantiate(PrefabsManager.Instance.BigPiece, transform.position+randomPosition, transform.rotation);//TODO rotation etc.
		clone.gameObject.layer=9;//the Layer 8 is the floorplan meshes
		
		//Write into the inRoomRetrival class
		Tier1Check[counter]=true;//seems useless..in this class, as I have set a counter; ?_?
		Tier1Data[counter,0]=clone.collider.bounds.center;
//		Debug.Log("clone.collider.bounds.center"+clone.collider.bounds.center);
		Debug.Log("Write in: Tier1Data["+counter+",0]="+Tier1Data[counter,0]);
		Tier1Data[counter,1]=clone.collider.bounds.extents;
//		Debug.Log("clone.collider.bounds.extents"+clone.collider.bounds.extents);
//		Debug.Log("InRoomRetrival.Instance.Tier1Data[counter,1]"+InRoomRetrival.Instance.Tier1Data[counter,1]);

		return clone;
	}

	void Start () {
		for(int i=0; i<= MAX_NumOfT1;i++){
			Tier1Check[i]=false;
			for(int j=0; j<MAX_InfoItems;j++){
				Tier1Data[i,j]=new Vector3(0f,0f,0f);
			}
		}

//		dummy=PrefabsManager.Instance.Dummy;
//		mycube=PrefabsManager.Instance.BigPiece;
//
//		mycubeExtents=0.5f*mycube.transform.lossyScale;

		offset=gameObject.collider.bounds.extents-PrefabsManager.Instance.BigPiece.transform.lossyScale;//roomsize = 2* offset //*********TODO !!!
//		Debug.Log("room.collider.bounds.extents"+room.collider.bounds.extents);
		roomcenter=gameObject.collider.bounds.center;
//		Debug.Log("room.collider.bounds.center"+room.collider.bounds.center);

//		Vector3 randomPosition=getRandomPosition();
//
////		currentObject=Clone(randomPosition);
//		//-----------------------------------Changed for the first cube bug-------------------------------
//		counter++;
//		Debug.Log("counter="+counter);
//		
//		currentObject=(GameObject)Instantiate(dummy, transform.position+randomPosition, transform.rotation);//TODO rotation etc.
//		currentObject.gameObject.layer=9;//the Layer 8 is the floorplan meshes
//
//		/**
//		 * If I didn't write in, the second cube will have the "first cube problem" :(
//		 */
//		//Write into the inRoomRetrival class
//		InRoomRetrival.Instance.Tier1Check[counter]=true;//seems useless..in this class, as I have set a counter; ?_?
//		InRoomRetrival.Instance.Tier1Data[counter,0]=currentObject.collider.bounds.center;
//		//		Debug.Log("clone.collider.bounds.center"+clone.collider.bounds.center);
//		Debug.Log("Write in: InRoomRetrival.Instance.Tier1Data["+counter+",0]="+InRoomRetrival.Instance.Tier1Data[counter,0]);
//		InRoomRetrival.Instance.Tier1Data[counter,1]=currentObject.collider.bounds.extents;
//
//		/** Solution 1: To destroy it later
//		currentObject.name="dummy";
//		*/
//
//		//Solution 2 change render.enable to false
//		currentObject.renderer.enabled=false;
//		//-------------------------------------------------------------------------------------------------

	}

	bool Retrival(Vector3 randomPosition){
		Debug.Log("*********************************is Searching...***************************************");

		//whether it is in allocated arena
		for(int i=0;i<=counter;i++){
			Vector3 CentresDistance= Tier1Data[i,0]-randomPosition;
			Vector3 fetchedExtents= Tier1Data[i,1];

//			Debug.Log("Read in: InRoomRetrival.Instance.Tier1Check[counter]="+InRoomRetrival.Instance.Tier1Check[i]);
			Debug.Log("Read in: InRoomRetrival.Instance.Tier1Data["+i+",0]="+Tier1Data[i,0]);
//			Debug.Log("randomPosition"+randomPosition);

//			Debug.Log("CentresDistance.x"+CentresDistance.x);
//			Debug.Log("fetchedExtents.x+mycubeExtents.x"+fetchedExtents.x+mycubeExtents.x);
//
//			Debug.Log("CentresDistance.z"+CentresDistance.z);
//			Debug.Log("fetchedExtents.z+mycubeExtents.z"+fetchedExtents.z+mycubeExtents.z);

			if( Mathf.Abs(CentresDistance.x) > fetchedExtents.x+PrefabsManager.Instance.BigPiece.transform.lossyScale.x ||
			   Mathf.Abs(CentresDistance.z) > fetchedExtents.z+PrefabsManager.Instance.BigPiece.transform.lossyScale.z){
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
			for(int i=0;i<=counter;i++){
				Debug.Log("Cube"+i+" centre: "+Tier1Data[i,0]);
			}
		}

	}
}
