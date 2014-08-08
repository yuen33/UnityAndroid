using UnityEngine;
using System.Collections;
using System.Collections.Generic;//for List<T>, Queue<T>

public class PopulatingVer8 : MonoBehaviour {
	public GameObject BigPiece;
	public Vector3 roomcenter;
	public Vector3 roomextents;
	public Vector3 doorcenter;
	public Vector3 doorextents;
	public Vector3 windowcenter;
	public Vector3 windowextents;
	public float toDoorMIN;

	public int NumOfBigPiece=3;//TODO: read in later
	private GameObject[] BigPieces;

	private double lastScore;
	public double currentScore;
	private double globalBestScore;
	double SumsumOfDij=0f;
	double singleScore=0f;

	bool isRunning=false;
	bool rollBack=false;
	public double iterationTimes=0;
	public double criticalBeta;
	public double beta;
	public float step=10;//supposed to decrease

	public Texture2D zero;
	public Texture2D one;
	public Texture2D two;
	public Texture2D three;
	public Texture2D four;

	//getPosition() called by clone()
	Vector3 getPosition(GameObject furniture, int id){
		//check InRoomRetrival
		Vector3 randomPosition;
		Vector3 extentsDifference= roomextents- furniture.collider.bounds.extents;
		bool isSearching=false;
		do{
			randomPosition.x=roomcenter.x-extentsDifference.x+ Random.value*2*extentsDifference.x;
			randomPosition.z=roomcenter.z-extentsDifference.z+ Random.value*2*extentsDifference.z;
			randomPosition.y=furniture.collider.bounds.extents.y;

			isSearching=true;
			for(int i=0; i<InRoomRetrival.Instance.Tier1Check.Length; i++){
				/** In order to make it convinient to add objects after optimization furnishing
				 * I didn't use the predefined NumOfBigPieces
				 */
				if(!InRoomRetrival.Instance.Tier1Check[i]){
					//if (!false), which means all items have been checked
					isSearching=false;//will stop the do..while loop and return the randomPosition
					break;//break the for...loop for searching--->return randomPosition
				}else{
					//compare the distance
					Vector2 Nij;//The self-defined valid nearest distance in the same height (in x-z view)
					//sum of two extents
					Nij=new Vector2( InRoomRetrival.Instance.Tier1Data[i,1].x , InRoomRetrival.Instance.Tier1Data[i,1].z)+
						new Vector2( furniture.collider.bounds.extents.x, furniture.collider.bounds.extents.y);

					Vector2 Dij;//The actual distance between randomPosition and the read in object (in x-z view)
					//distance of two centers
					Dij=new Vector2( InRoomRetrival.Instance.Tier1Data[i,0].x , InRoomRetrival.Instance.Tier1Data[i,0].z)-
						new Vector2( furniture.collider.bounds.center.x, furniture.collider.bounds.center.z);

					//if length of Nij larger than length of Dij, "we think" they will be overlapped
					if(Nij.magnitude>Dij.magnitude){
						//overlaped, no need to check next item, go back to get a new randomPosition
						break;//break the for...loop, and keep the isSearching=true
					}//else: this item is checked; continue to go to check next item

				}//if..else

			}//for
		}while(isSearching);
		Debug.Log("Cube"+id+" position: "+randomPosition);
		return randomPosition;
	}//getPosition()

	void clone(int id){
		BigPieces[id]=(GameObject)Instantiate(BigPiece,
		                                      transform.position+getPosition(BigPiece,id)
		                                      ,transform.rotation);
		BigPieces[id].rigidbody.freezeRotation = true;
//		BigPieces[id].rigidbody.constraints = RigidbodyConstraints.FreezePositionY; // RigidbodyConstraints.FreezeRotation;
		BigPieces[id].rigidbody.rotation= Quaternion.identity;

		//Write in:
		InRoomRetrival.Instance.Tier1Check[id]=true;
		InRoomRetrival.Instance.Tier1Data[id,0]=BigPieces[id].collider.bounds.center;
		InRoomRetrival.Instance.Tier1Data[id,1]=BigPieces[id].collider.bounds.extents;

		//Add other details
		BigPieces[id].layer=9;//8:floorplan; 9: Big Pieces of furniture
		switch (id%5){
		case 0:
			BigPieces[id].renderer.material.SetTexture("_MainTex", zero);
			break;
		case 1:
			BigPieces[id].renderer.material.SetTexture("_MainTex", one);
			break;
		case 2:
			BigPieces[id].renderer.material.SetTexture("_MainTex", two);
			break;
		case 3:
			BigPieces[id].renderer.material.SetTexture("_MainTex", three);
			break;
		default:
			BigPieces[id].renderer.material.SetTexture("_MainTex", four);
			break;
		}//switch
	}
	
	void calculateScore(){
		SumsumOfDij=0;
		currentScore=0;
		for(int i=0;i<NumOfBigPiece;i++){
			Vector3 Pi=BigPieces[i].collider.bounds.center;
			singleScore=0;
			for(int j=1;j<NumOfBigPiece;j++){
				if(j!=i){
					Vector3 Pj=BigPieces[j].collider.bounds.center;
					Vector2 Dij=new Vector2(Pi.x,Pi.z)- new Vector2(Pj.x,Pj.z);
					SumsumOfDij=SumsumOfDij+Dij.magnitude;
				}//if j>i
			}//for j

//			singleScore=SumsumOfDij/(i+1);
			singleScore=-1;
			Vector2 toDoor=new Vector2(doorcenter.x,doorcenter.z) -new Vector2(Pi.x,Pi.z);
			Vector2 toWindow=new Vector2(windowcenter.x,windowcenter.z) -new Vector2(Pi.x,Pi.z);
			float toDoorDistance=toDoor.magnitude;
			toDoorMIN=doorextents.magnitude+BigPieces[i].collider.bounds.extents.magnitude;
//			if(toDoorDistance<toDoorMIN){
//				singleScore=-singleScore*(toDoorDistance-toDoorMIN);//give a big minus score
//				currentScore=currentScore+ singleScore;
//				continue;
//			}

			switch(i%5){
			case 0://next to door
				singleScore= singleScore*toDoorDistance;
				break;
			case 1://next to window
				singleScore= singleScore*toWindow.magnitude;
				break;
			case 2://next to door or window
				if(toDoorDistance> toWindow.magnitude){
					//next to door
					singleScore= singleScore*toDoorDistance;
				}else{
					//next to window
					singleScore= singleScore*toWindow.magnitude;
				}
				break;
			case 3:
				//free
				break;
			default:
				if(toDoorDistance> toWindow.magnitude){
					//if closer to door
					singleScore= -singleScore*toDoorDistance;
				}else{
					//if closer to window
					singleScore= -singleScore*toWindow.magnitude;
				}
				break;
			}//switch

			currentScore=currentScore+ singleScore;
		}//for i

		currentScore=0.2*currentScore+0.5*SumsumOfDij;
	}//calculateScore()
	
	// Use this for initialization
	void Start () {
		Random.seed=233;
		//Get information about the room
		roomcenter=gameObject.collider.bounds.center;
		roomextents=gameObject.collider.bounds.extents;
		//TODO xml read in
		GameObject door=GameObject.Find("floorplanforOBJexportV1/Porte_186");
		doorcenter=door.collider.bounds.center;
		doorextents=door.collider.bounds.extents;
		GameObject window1=GameObject.Find("floorplanforOBJexportV1/Fenetre_1_175");
		GameObject window2=GameObject.Find("floorplanforOBJexportV1/Fenetre_2_176");
		windowcenter=(window1.collider.bounds.center+window2.collider.bounds.center)*0.5F;
		windowextents=window1.collider.bounds.extents+window2.collider.bounds.extents;
		windowextents.y=window1.collider.bounds.extents.y;

		//Find furniture
		BigPiece=GameObject.Find("BigPiece");

		//put cubes in
		BigPieces=new GameObject[NumOfBigPiece];
		for(int i=0;i<NumOfBigPiece;i++){
			clone(i);
		}

		//get the score
		calculateScore();
		globalBestScore=currentScore;

		//initialise the criticalBeta
		criticalBeta=NumOfBigPiece*NumOfBigPiece*40;
//		if(criticalBeta>NumOfBigPiece*100)step=5;
//		criticalBeta=NumOfBigPiece*100;

		Debug.Log("Update() Starts...");
	}//Start()

	void move(GameObject furniture){
		Vector3 movingDirection=new Vector3(Random.value-0.5f,0.0f,Random.value-0.5f);
		movingDirection=movingDirection.normalized;

		furniture.rigidbody.MovePosition(furniture.collider.bounds.center+ step*movingDirection);
//		Vector3 newCenter=furniture.collider.bounds.center+step*movingDirection;
//		furniture.transform.position=newCenter;
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
		


	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.S)){
			isRunning=!isRunning;
		}
		if(isRunning){

			if(step<1.3){
				isRunning=false;
				rollBack=true;
			}

			iterationTimes++;

			if(iterationTimes>criticalBeta){
				beta++;
				step=(float)(step*0.998);
//				step=(float)10/Mathf.Log((float)(iterationTimes-criticalBeta));
			}else{
				beta=0.2;
			}

			lastScore=currentScore;
			Debug.Log("----------------------------------------------------------------lastScore="+lastScore);
			
			//record the lastPosition
			for(int i=0;i<NumOfBigPiece;i++){
				InRoomRetrival.Instance.Tier1LastPosition[i]=BigPieces[i].collider.bounds.center;
			}//for lastPosition

			if(Random.value<0.5){
				int[] RandomList=getRandomList(NumOfBigPiece);
				int a=RandomList[0];
				int b=RandomList[1];
				Vector3 aPosition=BigPieces[a].collider.bounds.center;
				BigPieces[a].transform.position=BigPieces[b].collider.bounds.center;
				BigPieces[b].transform.position=aPosition;
			}else{
				//move
				for(int i=0;i<NumOfBigPiece;i++){
					move(BigPieces[i]);
				}//for move
			}

			
			//calculate the score
			calculateScore();
			Debug.Log("----------------------------------------------------------------currentScore="+currentScore);



			//compare to the global best record
			if(globalBestScore<currentScore){
				globalBestScore=currentScore;
				for(int i=0;i<NumOfBigPiece;i++){
					InRoomRetrival.Instance.Tier1GlobalBest[i].x=BigPieces[i].transform.position.x;
					InRoomRetrival.Instance.Tier1GlobalBest[i].y=BigPieces[i].collider.bounds.extents.y;
					InRoomRetrival.Instance.Tier1GlobalBest[i].z=BigPieces[i].transform.position.z;
				}
			}else{
				//Metropolis-Hasting
				float lnp= Mathf.Log(Random.value);
				Debug.Log("lnp="+lnp);
				double deltaScore=beta*(currentScore-lastScore);
				Debug.Log("currentScore-lastScore="+ deltaScore);
				if(lnp>= deltaScore){
					//reset to lastPosition
					for(int i=0;i<NumOfBigPiece;i++){
						BigPieces[i].transform.position=InRoomRetrival.Instance.Tier1LastPosition[i];
					}
					iterationTimes--;
				}//if lnp
			}//if best... else MH
		}

		if(Input.GetKeyDown(KeyCode.P) || rollBack){
			isRunning=false;
			for(int i=0;i<NumOfBigPiece;i++){
				BigPieces[i].transform.position=InRoomRetrival.Instance.Tier1GlobalBest[i];
			}
		}


	}//Update()
}
