//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeGame : MonoBehaviour {

	public Camera mainCamera;

	public GameObject cubePrefab;
	public GameInfo gameInfo=GameInfo.getInstance();
	public Cube cube=Cube.getInstance();

	// Use this for initialization
	void Start () {
		
		cube.InitializePos ();
		cube.FindAdjoiningCubes ();

		//Instantiate cubes
		InstantiateCubes();

		gameInfo.Init ();
	}

	// Update is called once per frame
	void Update () {

		if (gameInfo.needDestroyCubes) {
			DestroyCubes();
			gameInfo.needDestroyCubes = false;
		}
		if (gameInfo.needReInstantiate) {
			InstantiateCubes();
			gameInfo.needReInstantiate = false;
		}

		gameInfo.currTime=Time.time;

		//determine moves(1~3) & moving/still status
		if (gameInfo.currTime - gameInfo.lastUpdateTime > GameInfo.shiftDuration + GameInfo.stillDuration) {
			gameInfo.isMoving = true;
			gameInfo.lastUpdateTime = gameInfo.currTime;
			if (gameInfo.moves < 3) {
				gameInfo.moves++;
			} else {
				gameInfo.moves = 1;
				if(gameInfo.phaseNo == 2){
					if (gameInfo.travelPeriodNo == GameInfo.MaxTravelPeriodNo) {
						gameInfo.phaseNo++;
					} else {
						gameInfo.travelPeriodNo++;
						gameInfo.UndoneShifts ();
					}
				}
			}
		} else if (gameInfo.currTime - gameInfo.lastUpdateTime > GameInfo.shiftDuration && gameInfo.currTime - gameInfo.lastUpdateTime < GameInfo.shiftDuration + GameInfo.stillDuration) {
			gameInfo.isMoving = false;
		} else {
			gameInfo.isMoving = true;
		}

		//cubes move
		if (gameInfo.isMoving==true) {
			for (int i = 0; i < Cube.CubeNumber; i++) {
				cube.cubes [i].transform.SetPositionAndRotation(Vector3.Lerp(cube.shiftedPos[i*3+((gameInfo.moves-2)<0?2:(gameInfo.moves-2))],cube.shiftedPos[i*3+gameInfo.moves-1],(gameInfo.currTime-gameInfo.lastUpdateTime)/GameInfo.shiftDuration),Quaternion.identity);
			}
		}

		//Highlight adjoined cubes & Shift target tree when still
		if (gameInfo.phaseNo == 2) {
			MarkConnectionsNShiftTarget (gameInfo.moves);
		}else if (gameInfo.phaseNo == 3) {
			UndoMark ();
		}
	}

	public Material adjoinCubeMat,normalCubeMat;
	public Component[] childrenMat;

	public void MarkConnectionsNShiftTarget(int moves){
		for(int i=0;i<Cube.CubeNumber;i++){
			//-----show adjoining cubes
			if (gameInfo.isMoving) {
				ChangeMaterial (i,normalCubeMat);
			}else{//when not moving
				if (cube.adjoinCube [i * 3 + moves - 1].adjoinNum > 0) {//there exist adjoining cubes
					for (int j = 0; j < cube.adjoinCube [i * 3 + moves - 1].adjoinNum; j++) {

						if(i==gameInfo.targetIdx&&gameInfo.isShiftDone[i*3+moves-1]==false){

							//change material
							ChangeMaterial(i,adjoinCubeMat);
							ChangeMaterial(cube.adjoinCube [i * 3 + moves - 1].idx[j],adjoinCubeMat);

							//shift target
							gameInfo.targetIdx=cube.adjoinCube [i * 3 + moves - 1].idx[j];
							gameInfo.UpdateTarget(gameInfo.targetIdx);
							gameInfo.isShiftDone[i*3+moves-1]=true;
							gameInfo.isShiftDone[cube.adjoinCube [i * 3 + moves - 1].idx[j]*3+moves-1]=true;
							Debug.Log (gameInfo.targetIdx);
						}
					}
				} else {
					ChangeMaterial (i, normalCubeMat);
				}

			}
		}
	}

	public void UndoMark ()
	{
		for (int i = 0; i < Cube.CubeNumber; i++) {
			childrenMat = cube.cubes [i].GetComponentsInChildren<MeshRenderer> ();
			foreach (MeshRenderer childMat in childrenMat)
				childMat.material = normalCubeMat;
		}
	}

	public void ChangeMaterial(int idx,Material CubeMat){
		childrenMat = cube.cubes [idx].GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer childMat in childrenMat) {
			childMat.material = CubeMat;
		}
	}

	void InstantiateCubes(){
		for (int i = 0; i < Cube.CubeNumber; i++) {
			cube.cubes [i] = Instantiate(cubePrefab, cube.pos [i], Quaternion.identity);
			cube.cubes [i].name = (i).ToString ();
			cube.trees [i] = cube.cubes [i].transform.FindChild ("Tree").gameObject;
		}
	}

	void DestroyCubes(){
		GameObject obj2Destroy;
		for (int i = 0; i < Cube.CubeNumber; i++) {
			obj2Destroy = GameObject.Find ((i).ToString());
			Destroy(obj2Destroy);
		}
	}
		
}
