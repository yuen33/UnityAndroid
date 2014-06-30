using UnityEngine;
using System.Collections;

public class Prefabs_seat : MonoBehaviour {
	private static Prefabs_seat instance = null;
	// to create a singleton which holds the variables
	public static Prefabs_seat Instance{
		get{
			if(instance==null){
				instance=new GameObject("Prefabs_seat").AddComponent<Prefabs_seat>();
			}
			return instance;
		}
	}

	public GameObject BigPiece;
	public GameObject Seat;
	public GameObject SmallPiece;
	public GameObject Plane;

//	// Use this for initialization
//	void Start () {
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
}
