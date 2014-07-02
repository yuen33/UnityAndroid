using UnityEngine;
using System.Collections;
using System.Collections.Generic;//for List<T>, Queue<T>

public class PopulatingVer3 : MonoBehaviour {
	public GameObject BigPiece;//TODO
	public Vector3 mycubeExtents;//TODO
	public float deltaX=10;
	public float deltaZ=10;

	public Vector3 offset;
	public Vector3 roomcenter;
	
	public int NumOfBigPiece=3;
	private GameObject[] BigPieces;

	private float SumOfDij=0f;
	private Vector3 AbsDij;

	public Texture2D one;
	public Texture2D two;
	public Texture2D three;
	public Texture2D four;
	
	// Use this for initialization
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
		BigPieces=new GameObject[NumOfBigPiece];

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

			switch (i%4){
			case 0:
				clone.renderer.material.SetTexture("_MainTex", one);
				break;
			case 1:
				clone.renderer.material.SetTexture("_MainTex", two);
				break;
			case 2:
				clone.renderer.material.SetTexture("_MainTex", three);
				break;
			default:
				clone.renderer.material.SetTexture("_MainTex", four);
				break;
			}

			//Write into the inRoomRetrival class
			InRoomRetrival.Instance.Tier1Check[i]=true;//seems useless..in this class, as I have set a counter; ?_?
			InRoomRetrival.Instance.Tier1Data[i,0]=clone.collider.bounds.center;
			//		Debug.Log("clone.collider.bounds.center"+clone.collider.bounds.center);
			Debug.Log("Write in: InRoomRetrival.Instance.Tier1Data["+i+",0]="+InRoomRetrival.Instance.Tier1Data[i,0]);
			InRoomRetrival.Instance.Tier1Data[i,1]=clone.collider.bounds.extents;
			//		Debug.Log("clone.collider.bounds.extents"+clone.collider.bounds.extents);
			//		Debug.Log("InRoomRetrival.Instance.Tier1Data[i,1]"+InRoomRetrival.Instance.Tier1Data[i,1]);
		}
	}

	void calculateSumOfDji(){
		for(int i=0;i<NumOfBigPiece;i++){
			Vector3 Pi=InRoomRetrival.Instance.Tier1Data[i,0];

			for(int j=0; j<NumOfBigPiece;j++){
				Vector3 Pj=InRoomRetrival.Instance.Tier1Data[j,0];
				Vector3 Dij=Pi-Pj;
//				Debug.Log("Dij="+Dij);
				AbsDij=AbsDij+ new Vector3(Mathf.Abs(Dij.x),Mathf.Abs(Dij.y),Mathf.Abs(Dij.z));
				SumOfDij=SumOfDij+AbsDij.x+AbsDij.y+AbsDij.z;
			}
//			Debug.Log("AbsDij="+AbsDij);

			//Write in the inRoomRetrival class
			InRoomRetrival.Instance.Tier1Data[i,2]=AbsDij;
//			Debug.Log("InRoomRetrival.Instance.Tier1Data["+i+",2]="+AbsDij);
		}
		Debug.Log("SumOfDij="+SumOfDij);
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
		if(Input.GetKeyDown(KeyCode.Space)){
			BigPieces[0].transform.Translate((float)deltaX*(Random.value-0.5f), 0f,(float)deltaZ*(Random.value-0.5f));

			int[] RandomList= getRandomList(NumOfBigPiece);
			for(int i=0; i<NumOfBigPiece; i++){
				int idx=RandomList[i];

			}
		}

//		if(Input.GetKeyDown(KeyCode.R)){
//			int[] RandomList=getRandomList();
//			Debug.Log("***********RandomList:*************");
//			for(int i=0;i<NumOfBigPiece;i++){
//				Debug.Log(RandomList[i]);
//			}
//		}

		//============================Print=================================
		if(Input.GetKeyDown(KeyCode.P)){
			//Destroy(GameObject.Find("dummy"));
			for(int i=0;i<NumOfBigPiece;i++){
				Debug.Log("Cube"+i+" centre: "+InRoomRetrival.Instance.Tier1Data[i,0]+
				          "\n AbsDij="+InRoomRetrival.Instance.Tier1Data[i,2]);
			}
		}
	}
}
