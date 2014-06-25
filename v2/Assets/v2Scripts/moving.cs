using UnityEngine;
using System.Collections;

public class moving : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log("extents"+GameObject.Find("mycube").collider.bounds.extents);
	
	}

	// Update is called once per frame
	void Update () {
//		if(Input.GetKeyDown(KeyCode.M)){
//			gameObject.transform.Translate((float)10*(Random.value-0.5f), 0f, (float)10*(Random.value-0.5f));
//			Debug.Log("currentPosition="+gameObject.transform.position);
//		}
	}
}
