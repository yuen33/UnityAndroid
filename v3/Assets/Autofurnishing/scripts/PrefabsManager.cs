using UnityEngine;
using System.Collections;

public class PrefabsManager : MonoBehaviour {
	private static PrefabsManager instance = null;

	// to create a singleton which holds the variables
	public static PrefabsManager Instance{
		get{
			if(instance==null){
				instance=new GameObject("PrefabsManager").AddComponent<PrefabsManager>();
			}
			return instance;
		}
	}
	public GameObject Dummy;
	public GameObject BigPiece;
	public GameObject Seat;
	public GameObject SmallPiece;
	public GameObject Plane;


	// Use this for initialization
	void Start () {
		GameState.Instance.PrefabsLoaded=true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
