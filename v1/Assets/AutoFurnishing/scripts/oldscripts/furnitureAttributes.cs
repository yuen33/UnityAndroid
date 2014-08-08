using UnityEngine;
using System.Collections;

public class furnitureAttributes : MonoBehaviour {
	private static furnitureAttributes instance = null;
	// to create a singleton which holds the variables
	public static furnitureAttributes Instance{
		get{
			if(instance==null){
				instance=new GameObject("furnitureAttributes").AddComponent<furnitureAttributes>();
			}
			return instance;
		}
	}
	
	bool getFurnitureAttributes=false;
	//inputs
//	public int cost;
//	public Vector2 period;//from Year1 to Year2, eg.(1890,1990)or(1890,2014)

	public string children;//which gameobject name contains it
	public string parents;//which gameobject name contains it
	public bool BigPiece;//fireplace, TV, shelf, bed, table etc.
	public bool Seat;//sofa, chair etc.
	public bool SmallPiece;//e.g small plants (on the table), folk, pen, PC, book etc.
	public bool Paper;//e.g. poster, painting, carpet, TV-wall, mirror, sticker etc.
	
	public string pairwise;//empty means not in pair
	/** pairwise Relative Position Code
	 * like a die:
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
	 * 
	 */
	public int pairAreaCode;//should be over 1 digits e.g. 243 ==234
	public Vector2 pairDistance;//(min,max)
	public int priorityLevel;//the lower the earlier
	public bool nextToTheWall;//limited gameobject orientation potentially




	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
