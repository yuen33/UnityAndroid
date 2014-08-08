﻿using UnityEngine;
using System.Collections;

public class InRoomRetrival : MonoBehaviour {
	private static InRoomRetrival instance = null;
	// to create a singleton which holds the variables
	public static InRoomRetrival Instance{
		get{
			if(instance==null){
				instance=new GameObject("InRoomRetrival").AddComponent<InRoomRetrival>();
			}
			return instance;
		}
	}

	public static int MAX_NumOfT1=20;
	public static int MAX_InfoItems=3;
	public bool[] Tier1Check= new bool[MAX_NumOfT1];
	public int[] Tier1Overlaps= new int[MAX_NumOfT1];//useless
//	public Vector3[] Tier1LastStep= new Vector3[MAX_NumOfT1];
	public double[] Tier1Cost= new double[MAX_NumOfT1];

	public Vector3[,] Tier1Data= new Vector3[MAX_NumOfT1,MAX_InfoItems];//T1 furniture record[id, 0=center 1=extents] for Start()

//	public float delta=100.0f;
	public Vector3[] Tier1LastPosition= new Vector3[MAX_NumOfT1];
	public Vector3[] Tier1GlobalBest= new Vector3[MAX_NumOfT1];
//	public GameObject BigPiece;

//	void Start(){
//		BigPiece=GameObject.Find("BigPiece");//not worked
//	}


//	// Use this for initialization
//	void Start () {
////		for(int i=0; i< MAX_NumOfT1;i++){
////			print(i);
////			Tier1Check[i]=false;
////			for(int j=0; j<MAX_InfoItems;j++){
////				Tier1Data[i,j]=new Vector3(0f,0f,0f);
////			}
////		}
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
}