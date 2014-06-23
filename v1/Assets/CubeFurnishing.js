#pragma strict
var myCubeFurniture: GameObject;

function Start () {

}

function Update () {
	//Calculation

	//Generate
	if(Input.GetKey(KeyCode.Space)){
	var myCube:GameObject = Instantiate(myCubeFurniture, transform.position+Vector3(0,100,0), transform.rotation);
	
	
	Debug.Log("Furnished!",myCube);
	}
	

}