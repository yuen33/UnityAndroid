using UnityEngine;
using System.Collections;
using System.Text;
//using System.StringSplitOptions;
using System.IO;
//using System.Collections.Generic;//for List<T>, Queue<T>

public class Room : MonoBehaviour {
	private static Room instance = null;
	// to create a singleton which holds the variables
	public static Room Instance{
		get{
			if(instance==null){
				instance=new GameObject("Room").AddComponent<Room>();
			}
			return instance;
		}
	}
	//inputs
	public string RoomName;
	public GameObject window1;
	public GameObject window2;
	public GameObject door1;
	public GameObject door2;
	public GameObject fireplace1;
	public GameObject fireplace2;
	public GameObject stairs;
//	bool[] isfound={false,false,false,  false,false,false,  false};

	//outputs
	public Vector3[] floorCorners;
	public Vector3[,] walls;//id[startpoint,endpoint,window1,window2,door1,door2,fireplace1,fireplace2,stairs]

	protected FileInfo theSourceFile = null;
	protected StreamReader reader = null;
	protected string text = " "; // assigned to allow first line to be read below


	// Use this for initialization
	void Start () {
		/**
		 * Read floor mesh corners x-z (2D) coordinates from .txt file
		 */
		Vector3[] corners=new Vector3[10];
		theSourceFile = new FileInfo ("Assets/Autofurnishing/scripts/rooms.txt");
		reader = theSourceFile.OpenText();

		text=reader.ReadLine();
		do{
			if(text.StartsWith("R")){//Room <name>
				string[] word=text.Split(' ');
				//word[0]="Room"
				//word[1]=<room name>

				if(word[1].Equals(RoomName)){
//					print("find room");
					text=reader.ReadLine();
					int i=0;
					do{
						string[] point=text.Split(' ');
						corners[i]=new Vector3((float) double.Parse(point[0]), 
						                        gameObject.collider.bounds.center.y,
						                        (float) double.Parse(point[1]));
//						print("i="+i);
						i++;
						text=reader.ReadLine();
					}while(!text.StartsWith("R") && text!=null);
				}//if word[1].Equals

			}//if text startwith

			text=reader.ReadLine();
		}while(text != null);

		int NumOfCorners=0;
		for(int i=0; i<corners.Length;i++){
			if(corners[i]==new Vector3(0,0,0)){
				NumOfCorners=i;
				break;
			}
		}//for int
		floorCorners=new Vector3[NumOfCorners];
		for(int i=0; i<NumOfCorners;i++){
			floorCorners[i]=corners[i];
		}

		/**
		 * Get walls infomation
		 */
		walls=new Vector3[NumOfCorners,9];
		for(int i=0;i<NumOfCorners;i++){
			//[][0]wall start point
			walls[i,0]=floorCorners[i];

			//[][1]wall end point
			if(i+1<NumOfCorners){
				walls[i,1]=floorCorners[i+1];
			}else{
				walls[i,1]=floorCorners[0];
			}
		}//get all walls lines

		GetWallsDetails();

		for(int i=0;i<NumOfCorners;i++){
			print("Wall-----------------"+i);
			print(walls[i,0]);
			print(walls[i,1]);
			print(walls[i,2]);
			print(walls[i,3]);
			print(walls[i,4]);
			print(walls[i,5]);
			print(walls[i,6]);
			print(walls[i,7]);
			print(walls[i,8]);
		}
		

	}//Start()

	/**
	 * To get which walls the window1 window2 door1 door2 fireplace1 fireplace2 stairs are at
	 * it will give the gameobject position under that wall attribute
	 */
	void GetWallsDetails(){
		int NumOfCorners=floorCorners.Length;
		float[] distance=new float[NumOfCorners];
		//[][2]if window 1
		Debug.Log("search window1's wall...");
		if(window1!=null){
			
			for(int i=0;i<NumOfCorners;i++){
				Vector2 pointA=new Vector2(window1.collider.bounds.center.x,
				                           window1.collider.bounds.center.z);
				Vector2 pointB=new Vector2(walls[i,0].x, walls[i,0].z);
				Vector2 pointC=new Vector2(walls[i,1].x, walls[i,1].z);
				distance[i]=DistanceToRay2D(pointA,pointB,pointC);
			}//for int i
			
			walls[findSmallestIDAt(distance),2]=window1.collider.bounds.center;
		}//for window1
		
		//[][3]if window 2
		Debug.Log("search window2's wall...");
		if(window2!=null){
			
			for(int i=0;i<NumOfCorners;i++){
				Vector2 pointA=new Vector2(window2.collider.bounds.center.x,
				                           window2.collider.bounds.center.z);
				Vector2 pointB=new Vector2(walls[i,0].x, walls[i,0].z);
				Vector2 pointC=new Vector2(walls[i,1].x, walls[i,1].z);
				distance[i]=DistanceToRay2D(pointA,pointB,pointC);
			}//for int i
			walls[findSmallestIDAt(distance),3]=window2.collider.bounds.center;
		}//for window2
		
		//[][4]if door1
		Debug.Log("search door1's wall...");
		if(door1!=null){
			
			for(int i=0;i<NumOfCorners;i++){
				Vector2 pointA=new Vector2(door1.collider.bounds.center.x,
				                           door1.collider.bounds.center.z);
				Vector2 pointB=new Vector2(walls[i,0].x, walls[i,0].z);
				Vector2 pointC=new Vector2(walls[i,1].x, walls[i,1].z);
				distance[i]=DistanceToRay2D(pointA,pointB,pointC);
			}//for int i
			walls[findSmallestIDAt(distance),4]=door1.collider.bounds.center;
			Instantiate(GameObject.Find("DoorShelter"),
			            transform.position+door1.collider.bounds.center,
			            transform.rotation);
		}//for door1
		
		//[][5]if door2
		Debug.Log("search door2's wall...");
		if(door2!=null){
			
			for(int i=0;i<NumOfCorners;i++){
				Vector2 pointA=new Vector2(door2.collider.bounds.center.x,
				                           door2.collider.bounds.center.z);
				Vector2 pointB=new Vector2(walls[i,0].x, walls[i,0].z);
				Vector2 pointC=new Vector2(walls[i,1].x, walls[i,1].z);
				distance[i]=DistanceToRay2D(pointA,pointB,pointC);
			}//for int i
			walls[findSmallestIDAt(distance),5]=door2.collider.bounds.center;
			Instantiate(GameObject.Find("DoorShelter"),
			            transform.position+door2.collider.bounds.center,
			            transform.rotation);
			//also put one between two doors:
			Vector3 inBetween=(door1.collider.bounds.center+door2.collider.bounds.center)/2;
			Instantiate(GameObject.Find("DoorShelter"),
			            transform.position+inBetween,
			            transform.rotation);

		}//for door2
		
		//[][6]if fireplace1
		Debug.Log("search fireplace1's wall...");
		if(fireplace1!=null){
			
			for(int i=0;i<NumOfCorners;i++){
				Vector2 pointA=new Vector2(fireplace1.collider.bounds.center.x,
				                           fireplace1.collider.bounds.center.z);
				Vector2 pointB=new Vector2(walls[i,0].x, walls[i,0].z);
				Vector2 pointC=new Vector2(walls[i,1].x, walls[i,1].z);
				distance[i]=DistanceToRay2D(pointA,pointB,pointC);
			}//for int i
			walls[findSmallestIDAt(distance),6]=fireplace1.collider.bounds.center;
		}//for fireplace1
		
		//[][7]if fireplace2
		Debug.Log("search fireplace2's wall...");
		if(fireplace2!=null){
			
			for(int i=0;i<NumOfCorners;i++){
				Vector2 pointA=new Vector2(fireplace2.collider.bounds.center.x,
				                           fireplace2.collider.bounds.center.z);
				Vector2 pointB=new Vector2(walls[i,0].x, walls[i,0].z);
				Vector2 pointC=new Vector2(walls[i,1].x, walls[i,1].z);
				distance[i]=DistanceToRay2D(pointA,pointB,pointC);
			}//for int i
			walls[findSmallestIDAt(distance),7]=fireplace2.collider.bounds.center;
		}//for fireplace2
		
		//[][8]if stairs
		Debug.Log("search stairs's wall...");
		if(stairs!=null){
			
			for(int i=0;i<NumOfCorners;i++){
				Vector2 pointA=new Vector2(stairs.collider.bounds.center.x,
				                           stairs.collider.bounds.center.z);
				Vector2 pointB=new Vector2(walls[i,0].x, walls[i,0].z);
				Vector2 pointC=new Vector2(walls[i,1].x, walls[i,1].z);
				distance[i]=DistanceToRay2D(pointA,pointB,pointC);
			}//for int i
			walls[findSmallestIDAt(distance),8]=stairs.collider.bounds.center;
		}//for stairs

	}

	/**
	 * get the smallest number id of an array
	 */
	int findSmallestIDAt(float[] array){
		float smallest=9999;
		int ID=-1;
		for(int i=0;i<array.Length;i++){
			if(array[i]<smallest){
				smallest=array[i];
				ID=i;
			} 
		}
		Debug.Log("walls ID=================================="+ID);
		return ID;
	}//int findsmallestIDat

	/**
	 * get the distance from Point A to the Line of BC
	 */
	float DistanceToRay2D(Vector2 A, Vector2 B, Vector2 C){
		Ray ray=new Ray(B,C-B);
		float distance=Vector3.Cross(ray.direction,A-B).magnitude;
		Debug.Log("distance="+distance);
		return distance;
	}
	
//	// Update is called once per frame
//	void Update () {
//	
//	}

}//class
