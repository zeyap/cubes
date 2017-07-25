//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

	const int cubeScale = 4;//the greater the cube sparser
	//Vector3[] move;
	public int currCubeIndex=5;//cube the player's in. <CubeNumber

	public const int AdjoinCubeNum=1;

	public struct adjoin{
		public int[] idx; //=new int[AdjoinCubeNum];
		public int[] type; //=new int[AdjoinCubeNum];//share f/e/v, not adjoining: 1 2 3 4
		public int adjoinNum;
		//public adjoin * next;
	};
	adjoin[] adjoinCube;
	public Material adjoinCubeMat,normalCubeMat;
	public Component[] childrenMat;

	public GameInfo gameInfo=GameInfo.getInstance();

	// Use this for initialization
	void Start () {

		InitializePos ();

		for (int i = 0; i < CubeNumber; i++) {
			cubes [i] = Instantiate (cubePrefab, pos [i], Quaternion.identity);
			cubes [i].name = (i).ToString ();
		}

		//initialize GameInfo records
		gameInfo.targetIdx = Mathf.CeilToInt (CubeNumber*Random.Range(0.01f,1.0f))-1;
		Debug.Log (gameInfo.targetIdx);
		gameInfo.target = cubes [gameInfo.targetIdx].transform.FindChild("Tree").gameObject;
		gameInfo.target.SetActive (true);
		gameInfo.isTargetFound = false;
		gameInfo.isChooseEnabled = false;

		gameInfo.lastUpdateTime = Time.time;
		gameInfo.isMoving = true;
		gameInfo.moves = 1;
		gameInfo.periodNo = 0;
		gameInfo.isShiftDone=new bool[CubeNumber*3];
		for (int i = 0; i < CubeNumber; i++) {
			for (int moves = 1; moves <= 3; moves++) {
				gameInfo.isShiftDone [i * 3 + moves - 1] = false;
			}
		}

		FindAdjoiningCubes ();
	}

	// Update is called once per frame
	void Update () {
		//Update positiion of cubes
		gameInfo.currTime=Time.time;
		if (gameInfo.isMoving==true) {
			for (int i = 0; i < CubeNumber; i++) {
				
				cubes [i].transform.SetPositionAndRotation(Vector3.Lerp(shiftedPos[i*3+((gameInfo.moves-2)<0?2:(gameInfo.moves-2))],shiftedPos[i*3+gameInfo.moves-1],(gameInfo.currTime-gameInfo.lastUpdateTime)/GameInfo.shiftDuration),Quaternion.identity);
			}
		}
		if (gameInfo.currTime - gameInfo.lastUpdateTime > GameInfo.shiftDuration + GameInfo.stillDuration) {
			gameInfo.isMoving = true;
			gameInfo.lastUpdateTime = gameInfo.currTime;
			if (gameInfo.moves < 3) {
				gameInfo.moves++;
			} else {
				gameInfo.moves = 1;
				if(gameInfo.periodNo == 1){
					gameInfo.periodNo++;
				}
			}
		} else if (gameInfo.currTime - gameInfo.lastUpdateTime > GameInfo.shiftDuration && gameInfo.currTime - gameInfo.lastUpdateTime < GameInfo.shiftDuration + GameInfo.stillDuration) {
			gameInfo.isMoving = false;
		} else {
			gameInfo.isMoving = true;
		}

		if (gameInfo.periodNo == 1) {
			MarkConnectionsNShiftTarget (gameInfo.moves);
		}if (gameInfo.periodNo == 2) {
			RecoverMaterial ();
		}

		//tree is visible in the first period
	}

	void InitializePos(){
		posParams = new int[9 * CubeNumber];
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
				shiftedPos [i * 3].x = pos [i].x + posParams [9 * i + 0] - posParams [9 * i + 1];//+1-2
				shiftedPos [i * 3].y = pos [i].y + posParams [9 * i + 3] - posParams [9 * i + 4];
				shiftedPos [i * 3].z = pos [i].z + posParams [9 * i + 6] - posParams [9 * i + 7];

				shiftedPos [i * 3 + 1].x = shiftedPos [i * 3].x + posParams [9 * i + 1] - posParams [9 * i + 2];//+2-3
				shiftedPos [i * 3 + 1].y = shiftedPos [i * 3].y + posParams [9 * i + 4] - posParams [9 * i + 5];
				shiftedPos [i * 3 + 1].z = shiftedPos [i * 3].z + posParams [9 * i + 7] - posParams [9 * i + 8];

				shiftedPos [i * 3 + 2].x = pos [i].x; //shiftedPos [i * 3 + 1].x+ posParams [9 * i + 2] - posParams [9 * i + 0];//+3-1
				shiftedPos [i * 3 + 2].y = pos [i].y; //shiftedPos [i * 3 + 1].y+ posParams [9 * i + 5] - posParams [9 * i + 3];
				shiftedPos [i * 3 + 2].z = pos [i].z; //shiftedPos [i * 3 + 1].z+ posParams [9 * i + 8] - posParams [9 * i + 6];

				//test if within a larger 3^3 cube

				for (int k = 0; k < 3; k++) {
					if ((shiftedPos [i * 3 + k].x == 0.0f) || (shiftedPos [i * 3 + k].y == 0.0f) || (shiftedPos [i * 3 + k].z == 0.0f)) {
						notWithinCube = true;
						//Debug.Log("WithinCube");
						break;//for
					} else {
						notWithinCube = false;
						//Debug.Log("NotWithinCube");
					}
				}

				if (notWithinCube)
					continue;

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
					if (k == i)
						isPosDuplicate = false;
				} else {
					isPosDuplicate = false;
				}

			} while(isPosDuplicate || notWithinCube);//while(isPosDuplicate);
		}
		
	}
		
	void FindAdjoiningCubes(){
		adjoinCube = new adjoin[CubeNumber * 3];//for 3 move*cubeNumber

		for (int i = 0; i < CubeNumber * 3; i++) {//initialization
			adjoinCube [i].idx = new int[AdjoinCubeNum];
			adjoinCube [i].type = new int[AdjoinCubeNum];
			for (int j = 0; j < AdjoinCubeNum; j++) {
				adjoinCube [i].idx [j] = -1;
				adjoinCube [i].type [j] = 0;
				adjoinCube [i].adjoinNum = 0;
			}
		}

		for (int moves = 1; moves <= 3; moves++) {
			FindCloserCube (moves);
		}
	}

	void FindCloserCube(int moves){

		float tempDist;

		for(int i=0;i<CubeNumber;i++){
			for (int j = 0; j < CubeNumber; j++) {
				//calculate distance
				if(i!=j){
					tempDist=Vector3.Distance(shiftedPos[i*3+moves-1],shiftedPos[j*3+moves-1]);

					if (tempDist < cubeScale * 1.05&&adjoinCube [i*3+moves-1].adjoinNum<AdjoinCubeNum) {
						adjoinCube [i*3+moves-1].adjoinNum+=1;
						adjoinCube [i*3+moves-1].idx [adjoinCube [i*3+moves-1].adjoinNum-1] = j;
						adjoinCube [i*3+moves-1].type [adjoinCube [i*3+moves-1].adjoinNum-1] = 1;
					}
				}
			}
		}
	}

	void MarkConnectionsNShiftTarget(int moves){
		for(int i=0;i<CubeNumber;i++){
			//-----show adjoining cubes
			if (gameInfo.isMoving) {
				childrenMat = cubes [i].GetComponentsInChildren<MeshRenderer>();
				foreach (MeshRenderer childMat in childrenMat)
					childMat.material = normalCubeMat;
			}else{//when not moving
				if (adjoinCube [i * 3 + moves - 1].adjoinNum > 0) {//there exist adjoining cubes
					for (int j = 0; j < adjoinCube [i * 3 + moves - 1].adjoinNum; j++) {
						//change material
						childrenMat = cubes [i].GetComponentsInChildren<MeshRenderer>();
						foreach (MeshRenderer childMat in childrenMat) {
							childMat.material = adjoinCubeMat;
						}

						//shift target
						if(i==gameInfo.targetIdx&&gameInfo.isShiftDone[i*3+moves-1]==false){
							gameInfo.targetIdx=adjoinCube [i * 3 + moves - 1].idx[j];
							gameInfo.target = cubes [gameInfo.targetIdx].transform.FindChild("Tree").gameObject;
							gameInfo.isShiftDone[i*3+moves-1]=true;
							gameInfo.isShiftDone[adjoinCube [i * 3 + moves - 1].idx[j]*3+moves-1]=true;
							Debug.Log (gameInfo.targetIdx);
						}
					}
				} else {
					childrenMat = cubes [i].GetComponentsInChildren<MeshRenderer>();
					foreach (MeshRenderer childMat in childrenMat)
						childMat.material = normalCubeMat;
				}
				
			}
		}
	}
	void RecoverMaterial ()
	{
		for (int i = 0; i < CubeNumber; i++) {
			childrenMat = cubes [i].GetComponentsInChildren<MeshRenderer> ();
			foreach (MeshRenderer childMat in childrenMat)
				childMat.material = normalCubeMat;
		}
	}
}
