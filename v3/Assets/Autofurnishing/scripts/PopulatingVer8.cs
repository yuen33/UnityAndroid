using UnityEngine;
using System.Collections;
using System.Collections.Generic;//for List<T>, Queue<T>

public class PopulatingVer8 : MonoBehaviour {
	public GameObject BigPiece;
	public Vector3 roomcenter;
	public Vector3 roomextents;
	public int NumOfBigPiece=3;//TODO: read in later
	private GameObject[] BigPieces;

	private double lastScore;
	private double currentScore;
	private double globalBestScore;
	
	private double SumsumOfDij=0f;

	bool isRunning=false;
	bool rollBack=false;
	public double beta=1;//supposed to increase
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
		BigPieces[id].rigidbody.rotation= Quaternion.identity;
		BigPieces[id].rigidbody.freezeRotation = true;

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
		for(int i=0;i<NumOfBigPiece;i++){
			Vector3 Pi=BigPieces[i].collider.bounds.center;

			for(int j=1;j<NumOfBigPiece;j++){
				if(j!=i){
					Vector3 Pj=BigPieces[j].collider.bounds.center;
					Vector2 Dij=new Vector2(Pi.x,Pi.z)- new Vector2(Pj.x,Pj.z);
					SumsumOfDij=SumsumOfDij+Dij.magnitude;

				}//if j>i
			}//for j
		}//for i

		currentScore=beta*SumsumOfDij;

	}//calculateScore()
	
	// Use this for initialization
	void Start () {
		//Get information about the room
		roomcenter=gameObject.collider.bounds.center;
		roomextents=gameObject.collider.bounds.extents;

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

		Debug.Log("Update() Starts...");
	}//Start()

	void move(GameObject furniture){
		Vector3 movingDirection=new Vector3(Random.value-0.5f,0.0f,Random.value-0.5f);
		movingDirection=movingDirection.normalized;

		furniture.rigidbody.MovePosition(furniture.collider.bounds.center+ step*movingDirection);
//		Vector3 newCenter=furniture.collider.bounds.center+step*movingDirection;
//		furniture.transform.position=newCenter;
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.S)){
			isRunning=!isRunning;
		}
		if(isRunning){
			beta++;
			step=(float)(step*0.998);

			lastScore=currentScore;
			Debug.Log("----------------------------------------------------------------lastScore="+lastScore);
			
			//record the lastPosition
			for(int i=0;i<NumOfBigPiece;i++){
				InRoomRetrival.Instance.Tier1LastPosition[i]=BigPieces[i].collider.bounds.center;
			}//for lastPosition
			
			//move
			for(int i=0;i<NumOfBigPiece;i++){
				move(BigPieces[i]);
			}//for move
			
			//calculate the score
			calculateScore();
			Debug.Log("----------------------------------------------------------------currentScore="+currentScore);
			
			//compare to the global best record
			if(globalBestScore<currentScore){
				globalBestScore=currentScore;
			}
			
			//Metropolis-Hasting
			float lnp= Mathf.Log(Random.value);
			Debug.Log("lnp="+lnp);
			Debug.Log("currentScore-lastScore="+(currentScore-lastScore));
			if(lnp>= currentScore-lastScore){
				//rollback to lastPosition
				for(int i=0;i<NumOfBigPiece;i++){
					BigPieces[i].transform.position=InRoomRetrival.Instance.Tier1LastPosition[i];
				}
			}//if lnp
		}

		if(Input.GetKeyDown(KeyCode.Pause)){
			isRunning=false;
			for(int i=0;i<NumOfBigPiece;i++){
				BigPieces[i].transform.position=InRoomRetrival.Instance.Tier1GlobalBest[i];
			}
		}


	}//Update()
}
