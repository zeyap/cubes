﻿//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cube : MonoBehaviour {

	public Camera mainCamera;

	public const int CubeNumber=3;//constant members in c# are best defined as static members of a class/struct
	public GameObject cubePrefab;
	public GameObject[] cubes;
	public Vector3[] pos;
	public Vector3[] shiftedPos;
	public int[] posParams; //fixed size buffer fields may only be members of structs
	public bool isPosDuplicate=true;
	public bool notWithinCube=true;
	int moves;//1~3
	float lastUpdateTime,currTime;
	bool isMoving;
	const float stillDuration = 5.0f;
	const float shiftDuration = 5.0f;
	const int cubeScale = 4;//the greater the cube sparser
	//Vector3[] move;
	public int currCubeIndex=5;//cube the player's in. <CubeNumber
	//int periodNo;

	public const int AdjoinCubeNum=3;

	public struct adjoin{
		public int[] idx; //=new int[AdjoinCubeNum];
		public int[] type; //=new int[AdjoinCubeNum];//share f/e/v, not adjoining: 1 2 3 4
		public int adjoinNum;
		//public adjoin * next;
	};
	adjoin[] adjoinCube;
	public Material adjoinCubeMat,normalCubeMat;
	public Component[] childrenMat;

	public struct nonAdjoin{//across 3 moves
		public float minDist;
		public int minIdx;
		public int moves;
	};
	nonAdjoin[] nonAjoinCube;

	LineRenderer[] line;
	int lineNum;
	public GameObject linePrefab;
	bool[] isLineAlreadyDrawn;

	//private ManaData manaData = ManaData.getInstance();
	// Use this for initialization
	void Start () {

		//move = new Vector3[CubeNumber];
		posParams=new int[9*CubeNumber];
		cubes = new GameObject[CubeNumber];
		pos = new Vector3[CubeNumber];
		shiftedPos = new Vector3[CubeNumber * 3];
		for (int i = 0; i < CubeNumber; i++) {
			do {
				for (int j = 9 * i; j < 9 * i + 9; j++) {
					//Debug.Log (j);
					posParams [j] = cubeScale * Mathf.CeilToInt (Random.Range (-0.5f, 0.5f));//in namespace System
					//Debug.Log(posParams [j] );
				}
				//calculate original position
				pos [i] = new Vector3 (posParams [9 * i + 0] + posParams [9 * i + 1] + posParams [9 * i + 2],
					posParams [9 * i + 3] + posParams [9 * i + 4] + posParams [9 * i + 5], 
					posParams [9 * i + 6] + posParams [9 * i + 7] + posParams [9 * i + 8]);

				//calculate shifted coordinates //3*Vector3 is allocated for every cube
				shiftedPos [i * 3].x = pos[i].x+posParams [9 * i + 0] - posParams [9 * i + 1];//+1-2
				shiftedPos [i * 3].y = pos[i].y+posParams [9 * i + 3] - posParams [9 * i + 4];
				shiftedPos [i * 3].z = pos[i].z+posParams [9 * i + 6] - posParams [9 * i + 7];

				shiftedPos [i * 3 + 1].x = shiftedPos [i * 3].x+ posParams [9 * i + 1] - posParams [9 * i + 2];//+2-3
				shiftedPos [i * 3 + 1].y = shiftedPos [i * 3].y+ posParams [9 * i + 4] - posParams [9 * i + 5];
				shiftedPos [i * 3 + 1].z = shiftedPos [i * 3].z+ posParams [9 * i + 7] - posParams [9 * i + 8];

				shiftedPos [i * 3 + 2].x = pos[i].x; //shiftedPos [i * 3 + 1].x+ posParams [9 * i + 2] - posParams [9 * i + 0];//+3-1
				shiftedPos [i * 3 + 2].y = pos[i].y; //shiftedPos [i * 3 + 1].y+ posParams [9 * i + 5] - posParams [9 * i + 3];
				shiftedPos [i * 3 + 2].z = pos[i].z; //shiftedPos [i * 3 + 1].z+ posParams [9 * i + 8] - posParams [9 * i + 6];

				//test if within a larger 3^3 cube

				for(int k=0;k<3;k++){
					if((shiftedPos[i*3+k].x==0.0f)||(shiftedPos[i*3+k].y==0.0f)||(shiftedPos[i*3+k].z==0.0f)){
						notWithinCube=true;
						//Debug.Log("WithinCube");
						break;//for
					}
					else{
						notWithinCube=false;
						//Debug.Log("NotWithinCube");
					}
				}
				
				if(notWithinCube) continue;


				//test if duplicate
				if (i > 0) {//compare with previous pos[];
					int k;
					for (k = 0; k < i; k++) {
						if ((pos [k].x == pos [i].x && pos [k].y == pos [i].y && pos [k].z == pos [i].z) ||
							(shiftedPos [k * 3].x == shiftedPos [i * 3].x && shiftedPos [k * 3].y == shiftedPos [i * 3].y && shiftedPos [k * 3].z == shiftedPos [i * 3].z) ||
							(shiftedPos [k * 3 + 1].x == shiftedPos [i * 3 + 1].x && shiftedPos [k * 3 + 1].y == shiftedPos [i * 3 + 1].y && shiftedPos [k * 3 + 1].z == shiftedPos [i * 3 + 1].z) ||
							(shiftedPos [k * 3 + 2].x == shiftedPos [i * 3 + 2].x && shiftedPos [k * 3 + 2].y == shiftedPos [i * 3 + 2].y && shiftedPos [k * 3 + 2].z == shiftedPos [i * 3 + 2].z)) {
							isPosDuplicate = true;
							break;
						}
					}
					if(k==i)isPosDuplicate = false;
				}
				else{isPosDuplicate=false;}

			} while(isPosDuplicate||notWithinCube);//while(isPosDuplicate);
		}
		for (int i = 0; i < CubeNumber; i++) {
			cubes [i] = Instantiate (cubePrefab, pos [i], Quaternion.identity);
			cubes[i].name=(i+1).ToString();
			//Debug.Log (pos [i]);
			//Debug.Log (shiftedPos [i*3]);
			//Debug.Log (shiftedPos [i*3+1]);
			//Debug.Log (shiftedPos [i*3+2]);
		}

		//cubes[i].transform.SetPositionAndRotation(pos[i],Quaternion.identity);
		lastUpdateTime = Time.time;
		isMoving = true;
		//periodNo = 0;


		adjoinCube=new adjoin[CubeNumber*3];//for 3 move*cubeNumber

		for (int i = 0; i < CubeNumber*3; i++) {//initialization
			adjoinCube[i].idx=new int[AdjoinCubeNum];
			adjoinCube[i].type=new int[AdjoinCubeNum];
			for(int j = 0; j < AdjoinCubeNum; j++){
				adjoinCube[i].idx[j] = -1;
				adjoinCube[i].type[j] = 0;
				adjoinCube[i].adjoinNum = 0;
			}
		}


		nonAjoinCube = new nonAdjoin[CubeNumber];
		for(int i=0;i<CubeNumber;i++){
			nonAjoinCube[i].minDist=cubeScale * 10;
			nonAjoinCube[i].minIdx = -1;
			nonAjoinCube [i].moves = 0;
		}


		line = new LineRenderer[CubeNumber];
		lineNum = 0;
		isLineAlreadyDrawn = new bool[CubeNumber];
		for(int i=0;i<CubeNumber;i++){
			isLineAlreadyDrawn [i] = false;
		}
		for (moves = 1; moves <= 3; moves++) {
			FindCloserCube (moves);
		}
		moves = 1;
	}


	// Update is called once per frame
	void Update () {

		//Update positiion of cubes
		currTime=Time.time;
		if (isMoving==true) {
			for (int i = 0; i < CubeNumber; i++) {
				//Debug.Log(i*3+((moves-2)<0?2:(moves-2)));
				//Debug.Log (i*3+moves-1);
				//move[i]=new Vector3(posParams [9 * i +(-1)+moves] - posParams [9 * i + moves],posParams [9 * i + 2+moves] - posParams [9 * i + 3+moves],posParams [9 * i + 5+moves] - posParams [9 * i + (6 + moves)%8]);
				//cubes [i].transform.Translate (move [i]*Time.deltaTime/shiftDuration);
				cubes [i].transform.SetPositionAndRotation(Vector3.Lerp(shiftedPos[i*3+((moves-2)<0?2:(moves-2))],shiftedPos[i*3+moves-1],(currTime-lastUpdateTime)/shiftDuration),Quaternion.identity);
				//cubes [i].transform.Translate (0.0f,0.0f,0.1f);
			}
		}
		//Debug.Log (currTime - lastUpdateTime);
		if (currTime - lastUpdateTime > shiftDuration + stillDuration) {
			isMoving = true;
			lastUpdateTime = currTime;
			//Debug.Log ("lastUpdateTime change");
			if (moves < 3) {
				moves++;
			} else {
				moves = 1;
			}
		} else if (currTime - lastUpdateTime > shiftDuration && currTime - lastUpdateTime < shiftDuration + stillDuration) {
			isMoving = false;
		}

		//shows neighboring cube pairs at end of first period

		//if(periodNo == 0){
		MarkConnections (moves);
		//if (moves == 3) {
		//	periodNo++;
		//}
		//}

	}

	void FindCloserCube(int moves){

		float tempDist;

		for(int i=0;i<CubeNumber;i++){
			for (int j = 0; j < CubeNumber; j++) {
				//calculate distance
				if(i!=j){
					tempDist=Vector3.Distance(shiftedPos[i*3+moves-1],shiftedPos[j*3+moves-1]);

					//1. connect in 2 conditions:share a face
					//2. (if no sharing)path to closest cube among 3 moves 
					if (tempDist < cubeScale * 1.05) {
						adjoinCube [i*3+moves-1].adjoinNum+=1;
						adjoinCube [i*3+moves-1].idx [adjoinCube [i*3+moves-1].adjoinNum-1] = j;
						adjoinCube [i*3+moves-1].type [adjoinCube [i*3+moves-1].adjoinNum-1] = 1;
					} else {
						//not adjoining
						if(tempDist<=nonAjoinCube[i].minDist){
							nonAjoinCube[i].minDist=tempDist;
							nonAjoinCube[i].minIdx=j;
							nonAjoinCube[i].moves = moves;
							//Debug.Log (i);
							//Debug.Log (nonAjoinCube[i].minDist);
							//Debug.Log (nonAjoinCube[i].minIdx);
						}
					}
				}

			}
		}

	}

	void MarkConnections(int moves){
		//Debug.Log ("DrawLine");

		for(int i=0;i<CubeNumber;i++){

			//-----draw Line between non-adjoining cubes

			if(isMoving==false){
				/*
					Debug.Log ("There are");
				Debug.Log(adjoinCube[i].adjoinNum);
				Debug.Log("adjoining cubes for Cube");
				Debug.Log(i);
				for(int j=0;j<AdjoinCubeNum;j++){
					Debug.Log ("adjoined CubeIndex:");
					Debug.Log (adjoinCube[i].idx[j]);
					Debug.Log ("type:");
					Debug.Log (adjoinCube[i].type[j]);
				}

				Debug.Log ("Non-adjoining nearest CubeIndex for Cube");
				Debug.Log (i);
				Debug.Log ("is");
				Debug.Log(nonAjoinCube[i].minIdx);

				Debug.Log("distance:");
				Debug.Log(minDist[i]);
				*/
			}

			if (nonAjoinCube[i].minIdx != -1) {
				if ((isLineAlreadyDrawn [i]==false)&&(moves==nonAjoinCube[i].moves)) {
					lineNum++;
					//Debug.Log (lineNum);
					int closesti=nonAjoinCube[i].minIdx;
					line [i] = Instantiate(linePrefab).GetComponent<LineRenderer>();
					line [i].SetPosition (0, shiftedPos[i*3+moves-1]+Vector3.up*cubeScale*0.5f);
					line [i].SetPosition (1, shiftedPos[closesti*3+moves-1]+Vector3.up*cubeScale*0.5f);
					//line [closesti] = line [i];
					isLineAlreadyDrawn [i] = true;
					//isLineAlreadyDrawn [closesti] = true;
				}
				else if ((isLineAlreadyDrawn [i] == true) && (moves != nonAjoinCube [i].moves)) {
					line [i].gameObject.SetActive (false);
				}
				else if((isLineAlreadyDrawn [i] == true) && (moves == nonAjoinCube [i].moves)){
					line [i].gameObject.SetActive (true);
				}
				if ((isLineAlreadyDrawn [i] == true) && isMoving) {
					line [i].gameObject.SetActive (false);
				}
			}

			//-----show adjoining cubes

			if (adjoinCube [i * 3 + moves - 1].adjoinNum > 0) {
				//Debug.Log ("adjoinCube[i].idx[k]");
				for (int j = 0; j < adjoinCube [i * 3 + moves - 1].adjoinNum; j++) {

					//Debug.Log (adjoinCube [i * 3 + moves - 1].idx [j]);
					//change material
					if (!isMoving) {
						childrenMat = cubes [i].GetComponentsInChildren<MeshRenderer>();
						foreach (MeshRenderer childMat in childrenMat)
							childMat.material = adjoinCubeMat;
						//cubes[adjoinCube [i*3+moves-1].idx [j]].SetActive(false);}
					}
				}
			} else {
				//Debug.Log ("NORMAL djoinCube[i].idx[k]");
				//Debug.Log (adjoinCube [i].idx [k]);
				childrenMat = cubes [i].GetComponentsInChildren<MeshRenderer>();
				foreach (MeshRenderer childMat in childrenMat)
					childMat.material = normalCubeMat;
				//cubes[i].SetActive(true);
			}
		}
	}
}