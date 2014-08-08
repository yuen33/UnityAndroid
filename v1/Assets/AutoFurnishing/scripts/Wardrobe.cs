using UnityEngine;
using System.Collections;

public class Wardrobe : MonoBehaviour {
	private static Wardrobe instance = null;
	// to create a singleton which holds the variables
	public static Wardrobe Instance{
		get{
			if(instance==null){
				instance=new GameObject("Wardrobe").AddComponent<Wardrobe>();
			}
			return instance;
		}
	}
	
	/** Oriented Bounding Box face code:
	 * -like a die:
	 * 
	 *     ^ y 				1: top area
	 *     |				6: bottom area
	 *    _|__
	 * 	/__1_ /|
	 * |     |2|			2: facing area
	 * |  4  | |-------->z	5: back area
	 * |_____|/
	 *    /
	 *   / x				3: left area
	 *  v					4: right area
	 * Four boolean preference considerations:
	 * [0]			[1]				[2]				[3]
	 * alongWalls	accessibleArea	childrenArea	echoingArea
	 * 
	 */
	public bool[,] OBBPreference;
	public string alongWalls="3 4 5";
	public string accessibleArea="2";
	public string childrenArea="";
	public string echoingArea="";
	
	// Use this for initialization
	void Start () {	
		int NumOfBool=4;
		OBBPreference=new bool[7,NumOfBool];//in order to use ID as 1~6, the first element[0] is useless
		for(int i=0;i<NumOfBool;i++) inputOBBPreference(i);
		
	}
	
	void inputOBBPreference(int boolID){
		//		print(boolID);
		string[] idx;
		switch(boolID){
		case 0:
			idx=alongWalls.Split(' ');
			break;
		case 1:
			idx=accessibleArea.Split(' ');
			break;
		case 2:
			idx=childrenArea.Split(' ');
			break;
		default: //case 3
			idx=echoingArea.Split(' ');
			break;
		}
		
		if(!idx[0].Equals("")){//for non empty inputs
			for(int i=0;i<idx.Length;i++){
				int faceCode=int.Parse(idx[i]);
				//				print(faceCode);
				if(faceCode>0 && faceCode<7) OBBPreference[faceCode,boolID]=true;
			}//for int
		}//if idx[0]
	}//inputOBB...()
	
//	// Update is called once per frame
//	void Update () {
//	
//	}
}
