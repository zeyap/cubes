//using System;
using UnityEngine;
using System.Collections;

public class Cube {

	public GameInfo gameInfo=GameInfo.getInstance();

	public int CubeNumber=3;//constant members in c# are best defined as static members of a class/struct
	public const int MaxCubeNumber=5;
	public GameObject[] cubes= new GameObject[MaxCubeNumber];
	public GameObject[] trees= new GameObject[MaxCubeNumber];
	public Vector3[] pos = new Vector3[MaxCubeNumber];
	public Vector3[] shiftedPos = new Vector3[MaxCubeNumber * 3];
	public int[] posParams= new int[9 * MaxCubeNumber]; //fixed size buffer fields may only be members of structs
	//if new every round, a stackoverflow may be caused
	public bool isPosDuplicate=true;
	public bool notWithinCube=true;

	const int cubeScale = 4;//the greater the cube sparser
	//Vector3[] move;

	public const int AdjoinCubeNum=1;

	public struct adjoin{
		public int[] idx; //=new int[AdjoinCubeNum];
		public int[] type; //=new int[AdjoinCubeNum];//share f/e/v, not adjoining: 1 2 3 4
		public int adjoinNum;
		//public adjoin * next;
	};
	public adjoin[] adjoinCube;

	private static Cube instance = new Cube ();
	private Cube(){}
	public static Cube getInstance(){
		return instance;
	}

	public void InitializePos(){

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

	public void FindAdjoiningCubes(){
		adjoinCube = new adjoin[MaxCubeNumber * 3];//for 3 move*cubeNumber

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

	public void FindCloserCube(int moves){

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


}
