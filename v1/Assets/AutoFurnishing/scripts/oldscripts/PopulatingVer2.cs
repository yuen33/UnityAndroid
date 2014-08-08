using UnityEngine;
using System.Collections;

public class PopulatingVer2 : MonoBehaviour {
	public GameObject mycube;
	//	public GameObject room;
	//TODO: get the unknown prefab extents
	public Vector3 mycubeExtents;
	
	public Vector3 offset;
	public Vector3 roomcenter;
	
	private int counter=-1;
	private GameObject currentObject;
	private bool isSearching=false;

	public static int MAX_NumOfT1=20;
	public static int MAX_InfoItems=2;
	public bool[] Tier1Check= new bool[MAX_NumOfT1+1];
	public Vector3[,] Tier1Data= new Vector3[MAX_NumOfT1+1,MAX_InfoItems];
	
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

		Tier1Check[counter]=true;//seems useless..in this class, as I have set a counter; ?_?
		Tier1Data[counter,0]=clone.collider.bounds.center;

		Debug.Log("Write in: Tier1Data["+counter+",0]="+Tier1Data[counter,0]);
		Tier1Data[counter,1]=clone.collider.bounds.extents;
		
		return clone;
	}
	
	void Start () {
		mycube=Prefabs_seat.Instance.Seat;
		 
		mycubeExtents=0.5f*mycube.transform.lossyScale;
		offset=gameObject.collider.bounds.extents-mycubeExtents;//roomsize = 2* offset //*********TODO !!!
		//		Debug.Log("room.collider.bounds.extents"+room.collider.bounds.extents);
		roomcenter=gameObject.collider.bounds.center;
		//		Debug.Log("room.collider.bounds.center"+room.collider.bounds.center);
		
		Vector3 randomPosition=getRandomPosition();
		
		currentObject=Clone(randomPosition);
		
	}
	
	bool Retrival(Vector3 randomPosition){
		Debug.Log("*********************************is Searching...***************************************");
		
		//whether it is in allocated arena
		for(int i=0;i<=counter;i++){
			Vector3 CentresDistance= Tier1Data[i,0]-randomPosition;
			Vector3 fetchedExtents= Tier1Data[i,1];
			
			if( Mathf.Abs(CentresDistance.x) > fetchedExtents.x+mycubeExtents.x ||
			   Mathf.Abs(CentresDistance.z) > fetchedExtents.z+mycubeExtents.z){
				Debug.Log("It will be not overlapped with Cube"+i);
				continue;
			}else{
				Debug.Log(":( It will OVERLAP with Cube"+i);
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
