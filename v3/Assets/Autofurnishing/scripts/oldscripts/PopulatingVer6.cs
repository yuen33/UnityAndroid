using UnityEngine;
using System.Collections;
using System.Collections.Generic;//for List<T>, Queue<T>
//using System.Math;//for Math.exp(), because it said Mathf.Exp() invalid xxx?

public class PopulatingVer6 : MonoBehaviour {
	public GameObject BigPiece;//TODO
	public Vector3 mycubeExtents;//TODO
	public float delta=10;
	public double beta=1;
	
	public Vector3 offset;//todo
	public Vector3 roomcenter;
	
	public int NumOfBigPiece=3;
	private GameObject[] BigPieces;
	//	public KDTree kdtree;
	private Vector3[] centersArray;
	private int[] nearestPointArray;
	private double lastScore;
	private double currentScore;
	private double recordedScore;
	
	private float SumsumOfDij=0f;
	private int numOfOverlaps=0;
	private Vector3 AbsDij;
	
	public Texture2D zero;
	public Texture2D one;
	public Texture2D two;
	public Texture2D three;
	public Texture2D four;
	
	bool isRunning=false;
	bool rollBack=false;
	
	Vector3 getRandomPosition(){
		Vector3 randomPosition;
		randomPosition.x=(roomcenter.x-offset.x)+Random.value * 2* offset.x;
		randomPosition.y= mycubeExtents.y;//*********TODO !!!
		//To make it over the floor, extents=000 for any prefabs not in the scene
		randomPosition.z=(roomcenter.z-offset.z)+Random.value * 2* offset.z;
		
		return randomPosition;
	}
	
	
	// Use this for initialization
	void Start () {
		//		Random.seed=42;
		//		Debug.Log("room center="+gameObject.collider.bounds.center);
		BigPieces=new GameObject[NumOfBigPiece];
		centersArray=new Vector3[NumOfBigPiece];
		nearestPointArray=new int[NumOfBigPiece];
		
		mycubeExtents=0.5f*BigPiece.transform.lossyScale;
		offset=gameObject.collider.bounds.extents-mycubeExtents;//roomsize = 2* offset //*********TODO !!!
		//		Debug.Log("room.collider.bounds.extents"+room.collider.bounds.extents);
		roomcenter=gameObject.collider.bounds.center;
		//		Debug.Log("room.collider.bounds.center"+room.collider.bounds.center);
		
		for(int i=0; i<NumOfBigPiece;i++){
			Vector3 randomPosition=getRandomPosition();
			GameObject clone=(GameObject)Instantiate(BigPiece, transform.position+randomPosition, transform.rotation);//TODO rotation etc.
			BigPieces[i]=clone;
			clone.gameObject.layer=9;//the Layer 8 is the floorplan meshes
			centersArray[i]=clone.collider.bounds.center;
			
			switch (i%5){
			case 0:
				clone.renderer.material.SetTexture("_MainTex", zero);
				break;
			case 1:
				clone.renderer.material.SetTexture("_MainTex", one);
				break;
			case 2:
				clone.renderer.material.SetTexture("_MainTex", two);
				break;
			case 3:
				clone.renderer.material.SetTexture("_MainTex", three);
				break;
			default:
				clone.renderer.material.SetTexture("_MainTex", four);
				break;
			}//switch
		}//for
		prepareCostTerms();
		currentScore=calculateCost();
		recordedScore=currentScore;
		
	}//Start()
	
	void prepareCostTerms(){
		//Calculate the sumOfDij, the number of overlaps and find the nearest
		SumsumOfDij=0;	
		for(int i=0;i<NumOfBigPiece;i++){
			AbsDij=new Vector3(0.0f,0.0f,0.0f);//for distance vector sum: from each Pi to Pj
			Vector3 Pi=BigPieces[i].collider.bounds.center;//InRoomRetrival.Instance.Tier1Data[i,0];
			InRoomRetrival.Instance.Tier1Overlaps[i]=0;
			double distance=99999999.9;
			
			for(int j=0; j<NumOfBigPiece;j++){
				if(j!=i){
					Vector3 Pj=BigPieces[j].collider.bounds.center;//InRoomRetrival.Instance.Tier1Data[j,0];
					Vector3 Dij=Pi-Pj;
					AbsDij=AbsDij+ new Vector3(Mathf.Abs(Dij.x),Mathf.Abs(Dij.y),Mathf.Abs(Dij.z));
//					SumsumOfDij=SumsumOfDij+Mathf.Abs(Dij.x)+Mathf.Abs(Dij.z);
					SumsumOfDij=SumsumOfDij+Dij.magnitude;
					
					//find nearest
					if (Dij.magnitude < distance){
						distance=Dij.magnitude;
						nearestPointArray[i]=j;
					}
					
					//check overlaps
					float xshortest=BigPieces[i].collider.bounds.extents.x+ BigPieces[j].collider.bounds.extents.x;
					float zshortest=BigPieces[i].collider.bounds.extents.z+ BigPieces[j].collider.bounds.extents.z;
					if(Mathf.Abs(Dij.x) > xshortest ||
					   Mathf.Abs(Dij.z) > zshortest){
					}else{
						InRoomRetrival.Instance.Tier1Overlaps[i]++;
					}//if..else (check overlaps)
				}//if j!=i
			}//for j
			
			//			Debug.Log("nearestPointArray["+i+"]="+nearestPointArray[i]);
			
			//Write in the inRoomRetrival class
			InRoomRetrival.Instance.Tier1Data[i,2]=AbsDij;
			//			Debug.Log("InRoomRetrival.Instance.Tier1Data["+i+",2]="+AbsDij);
		}//for i
		//		Debug.Log("SumsumOfDij="+SumsumOfDij);
	}//prepareCostTerms()
	
	double calculateCost(){
		double overallScore;
		numOfOverlaps=0;
		Debug.Log("******************Scores********************");
		for(int i=0; i<NumOfBigPiece; i++){
			//Overlap term
			double OverlapFactor= 1.0/(InRoomRetrival.Instance.Tier1Overlaps[i]+1);
			numOfOverlaps= numOfOverlaps+ InRoomRetrival.Instance.Tier1Overlaps[i];
			
			//Distance term
			double DistanceFactor=AbsDij.x+AbsDij.z;
			
			InRoomRetrival.Instance.Tier1Cost[i]=OverlapFactor*OverlapFactor*DistanceFactor;
			//			Debug.Log("Cube"+i+" score: "+InRoomRetrival.Instance.Tier1Cost[i]);
		}//for
		//		//		overallScore=1.0/(numOfOverlaps+1) * SumsumOfDij;
		//		overallScore= 1.0/(numOfOverlaps+1)/(numOfOverlaps+1) * SumsumOfDij;
		overallScore= beta* SumsumOfDij;
		
		Debug.Log("overallScore="+overallScore);
		Debug.Log("numOfOverlaps="+numOfOverlaps);
		
		return overallScore;
	}
	
	int getWorstIdx(){
		int worstIdx=0;
		for(int i=1;i<NumOfBigPiece;i++){
			if(InRoomRetrival.Instance.Tier1Cost[i]<InRoomRetrival.Instance.Tier1Cost[i-1]){
				worstIdx=i;
			}
		}
		return worstIdx;
	}
	
	int[] getRandomList(int NumOfBigPiece){
		List<int> list=new List<int>();
		for(int i=0;i<NumOfBigPiece;i++){
			list.Add(i);
		}
		int[] RandomList= new int[NumOfBigPiece];
		for (int j=0;j<NumOfBigPiece;j++){
			//			Debug.Log("list.Count="+list.Count);
			int idx=(int) (Random.value * list.Count);
			//			Debug.Log("Remove: list["+idx+"]="+list[idx]);
			RandomList[j]=list[idx];
			//			Debug.Log("RandomList["+j+"]="+idx);
			list.RemoveAt(idx);
		}
		
		return RandomList;
	}
	
	//	int getNearestObject(int inputID){
	//		int outputID
	//		float distance=999999.9;
	//		for(int i=0;i<NumOfBigPiece;i++){
	//			float newDistance=Vector3.Distance(BigPieces[inputID].transform.position,BigPieces[i].transform.position);
	//			if(i!= inputID &&
	//			   distance>newDistance){
	//			}
	//		}
	//	}
	
	//	Vector3 getBiasedDirection(Vector3 mean){
	//
	//	}
	
	void move(GameObject cube){
		Vector3 oldCenter=cube.collider.bounds.center;
		
		Vector3 movingDirection=new Vector3(Random.value-0.5f,0.0f,Random.value-0.5f);
		movingDirection=movingDirection.normalized;
		Vector3 newCenter=oldCenter+delta*movingDirection;
		
		//Make sure newCenter is in room
		Vector3 relatedRoomExtents=gameObject.collider.bounds.extents- cube.collider.bounds.extents;
		if(newCenter.x> roomcenter.x+relatedRoomExtents.x) 
			newCenter.x= roomcenter.x+relatedRoomExtents.x;
		if(newCenter.x< roomcenter.x-relatedRoomExtents.x) 
			newCenter.x= roomcenter.x-relatedRoomExtents.x;
		if(newCenter.z> roomcenter.z+relatedRoomExtents.z) 
			newCenter.z= roomcenter.z+relatedRoomExtents.z;
		if(newCenter.z< roomcenter.z-relatedRoomExtents.z) 
			newCenter.z=roomcenter.z-relatedRoomExtents.z;
		
		cube.transform.position=newCenter;
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.S)){
			isRunning= !isRunning; 
		}
		if(isRunning && numOfOverlaps==0){
			beta++;
			delta=(float)(delta*0.998);

			lastScore=currentScore;
			Debug.Log("lastScore="+lastScore);

			recordedScore=currentScore;
			for(int i=0;i<NumOfBigPiece;i++){
				InRoomRetrival.Instance.Tier1Data[i,0]=BigPieces[i].collider.bounds.center;
				InRoomRetrival.Instance.Tier1Data[i,1]=BigPieces[i].collider.bounds.extents;
			}
			
			for(int i=0;i<NumOfBigPiece;i++){
				move(BigPieces[i]);
			}
			
			prepareCostTerms();
			currentScore=calculateCost();
			Debug.Log("currentScore"+currentScore);
			
//			if(currentScore>=recordedScore){
//				recordedScore=currentScore;
//				for(int i=0;i<NumOfBigPiece;i++){
//					InRoomRetrival.Instance.Tier1Data[i,0]=BigPieces[i].collider.bounds.center;
//					InRoomRetrival.Instance.Tier1Data[i,1]=BigPieces[i].collider.bounds.extents;
//				}
//			}
			
			float lnp= Mathf.Log(Random.value);
			Debug.Log("----------------------------------------------------------------lnp="+lnp);
			Debug.Log("currentScore-lastScore="+(currentScore-lastScore));
			if(lnp>=(currentScore-lastScore)){

				//rollback
				for(int i=0;i<NumOfBigPiece;i++){
					BigPieces[i].transform.position=InRoomRetrival.Instance.Tier1Data[i,0];
				}
			}
		}
		if(isRunning && numOfOverlaps!=0){
			currentScore=lastScore;
			
			//			int worstIdx=getWorstIdx();
			int[] randomList=getRandomList(NumOfBigPiece);
			for(int i=0;i<NumOfBigPiece;i++){
				int worstIdx=randomList[i];
				Vector3 oldCenter=BigPieces[worstIdx].collider.bounds.center;
				//				Debug.Log("Moving Cube "+worstIdx+", where is "+oldCenter);
				
				int nearestObjIdx=nearestPointArray[worstIdx];
				
				//Point A- Point B= BA (pointing to A, the direction of leaving B)
				Vector3 movingDirection=oldCenter - BigPieces[nearestObjIdx].collider.bounds.center;
				movingDirection=movingDirection.normalized;
				
				Vector3 newCenter=oldCenter+delta*movingDirection;
				
				//Make sure newCenter is in room
				Vector3 relatedRoomExtents=gameObject.collider.bounds.extents- BigPieces[worstIdx].collider.bounds.extents;
				if(newCenter.x> roomcenter.x+relatedRoomExtents.x) 
					newCenter.x= roomcenter.x+relatedRoomExtents.x;
				if(newCenter.x< roomcenter.x-relatedRoomExtents.x) 
					newCenter.x= roomcenter.x-relatedRoomExtents.x;
				if(newCenter.z> roomcenter.z+relatedRoomExtents.z) 
					newCenter.z= roomcenter.z+relatedRoomExtents.z;
				if(newCenter.z< roomcenter.z-relatedRoomExtents.z) 
					newCenter.z=roomcenter.z-relatedRoomExtents.z;
				
				BigPieces[worstIdx].transform.position=newCenter;
				//				Debug.Log("Cube "+worstIdx+" is moved to"+BigPieces[worstIdx].collider.bounds.center);
			}
			
			prepareCostTerms();
			currentScore=calculateCost();
		}//if...Space
	}//Update()
}
