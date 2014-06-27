using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {
	private static GameState instance = null;
	// to create a singleton which holds the variables
	public static GameState Instance{
		get{
			if(instance==null){
				instance=new GameObject("GameState").AddComponent<GameState>();
			}
			return instance;
		}
	}

	public bool PrefabsLoaded=false;


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
