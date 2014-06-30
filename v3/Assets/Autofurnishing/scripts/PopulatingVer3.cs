using UnityEngine;
using System.Collections;

public class PopulatingVer3 : MonoBehaviour {
	public GameObject BigPiece;//TODO
	public Vector3 mycubeExtents;//TODO

	public Vector3 offset;
	public Vector3 roomcenter;
	
	public int NumOfBigPiece=3;
	private GameObject currentObject;

	public float SumOfDij=0f;
	
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
		mycubeExtents=0.5f*BigPiece.transform.lossyScale;
		offset=gameObject.collider.bounds.extents-mycubeExtents;//roomsize = 2* offset //*********TODO !!!
		//		Debug.Log("room.collider.bounds.extents"+room.collider.bounds.extents);
		roomcenter=gameObject.collider.bounds.center;
		//		Debug.Log("room.collider.bounds.center"+room.collider.bounds.center);

		for(int i=0; i<NumOfBigPiece;i++){
			Vector3 randomPosition=getRandomPosition();
			GameObject clone=(GameObject)Instantiate(BigPiece, transform.position+randomPosition, transform.rotation);//TODO rotation etc.
			clone.gameObject.layer=9;//the Layer 8 is the floorplan meshes

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
			for(int j=0;j<NumOfBigPiece;j++){
				Vector3 Pj=InRoomRetrival.Instance.Tier1Data[j,0];
				Vector3 Dij=Pi-Pj;
				SumOfDij=SumOfDij+Mathf.Abs(Dij.x)+Mathf.Abs(Dij.y)+Mathf.Abs(Dij.z);
			}
		}
		Debug.Log("SumOfDij="+SumOfDij);
	}


	// Update is called once per frame
	void Update () {
		//Print
		if(Input.GetKeyDown(KeyCode.P)){
			//Destroy(GameObject.Find("dummy"));
			for(int i=0;i<=NumOfBigPiece;i++){
				Debug.Log("Cube"+i+" centre: "+InRoomRetrival.Instance.Tier1Data[i,0]);
			}
		}

		if(Input.GetKeyDown(KeyCode.Space)){
			calculateSumOfDji();
		}

	
	}
}
