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

	private Transform auxiliaryCubeTransform;

	// Use this for initialization
	void Awake(){
		auxiliaryCubeTransform=this.transform;
	}

	void Start () {
		
		cube.InitializePos ();
		cube.FindAdjoiningCubes ();

		AssignMaterials ();
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
					if (gameInfo.travelPeriodNo == gameInfo.MaxTravelPeriodNo) {
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
			for (int i = 0; i < cube.CubeNumber; i++) {
				//cube.cubes [i].transform.SetPositionAndRotation(Vector3.Lerp(cube.shiftedPos[i*3+((gameInfo.moves-2)<0?2:(gameInfo.moves-2))],cube.shiftedPos[i*3+gameInfo.moves-1],(gameInfo.currTime-gameInfo.lastUpdateTime)/GameInfo.shiftDuration),Quaternion.identity);
				cube.cubes[i].transform.localPosition=auxiliaryCubeTransform.InverseTransformPoint(Vector3.Lerp(cube.shiftedPos[i*3+((gameInfo.moves-2)<0?2:(gameInfo.moves-2))],cube.shiftedPos[i*3+gameInfo.moves-1],(gameInfo.currTime-gameInfo.lastUpdateTime)/GameInfo.shiftDuration));
			}
		}

		//Highlight adjoined cubes & Shift target tree when still
		if (gameInfo.phaseNo == 2) {
			MarkConnectionsNShiftTarget (gameInfo.moves);
		} else if (gameInfo.phaseNo == 3) {
			UndoMark ();
			if (gameInfo.hasPhase3Begun == false) {
				if (gameInfo.moves == 3) {
					gameInfo.hasPhase3Begun = true;
					gameInfo.beginTime = Time.realtimeSinceStartup;
				}
			} else {
				if (!gameInfo.isTargetFound) 
					gameInfo.reactTime = Time.realtimeSinceStartup - gameInfo.beginTime;
				//Time.time is inappropriate because frame freezes here
			}


		} else {
			gameInfo.hasPhase3Begun = false;
		}
	}

	public Material adjoinCubeMat;
	Material[] normalCubeMat=new Material[5];
	Component[] childrenMat;
	public Material mat0, mat1, mat2, mat3, mat4;

	public void AssignMaterials(){
		//assign different material
		normalCubeMat [0] = mat0;
		normalCubeMat [1] = mat1;
		normalCubeMat [2] = mat2;
		normalCubeMat [3] = mat3;
		normalCubeMat [4] = mat4;
	}

	public void MarkConnectionsNShiftTarget(int moves){
		for(int i=0;i<cube.CubeNumber;i++){
			//-----show adjoining cubes
			if (gameInfo.isMoving) {
				ChangeMaterial (i,normalCubeMat[i]);
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
						}
					}
				} else {
					ChangeMaterial (i, normalCubeMat[i]);
				}

			}
		}
	}

	public void UndoMark ()
	{
		for (int i = 0; i < cube.CubeNumber; i++) {
			ChangeMaterial (i,normalCubeMat[i]);
		}
	}

	public void ChangeMaterial(int idx,Material CubeMat){
		childrenMat = cube.cubes [idx].GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer childMat in childrenMat) {
			if(childMat.gameObject.tag!="Tree")
				childMat.material = CubeMat;
		}
	}

	public Component[] childrenTransform;
	public Shader alphablendShader;
	void InstantiateCubes(){
		//instantiate cubes
		for (int i = 0; i < cube.CubeNumber; i++) {
			cube.cubes [i] = Instantiate(cubePrefab, auxiliaryCubeTransform.InverseTransformPoint(cube.pos [i]), auxiliaryCubeTransform.rotation);
			ChangeMaterial (i,normalCubeMat[i]);
			cube.cubes [i].name = (i).ToString ();
			cube.trees [i] = cube.cubes [i].transform.FindChild ("Tree").gameObject;
			//cube.cubes [i].transform.SetParent (auxiliaryCubeTransform,false);
			cube.cubes[i].transform.parent = auxiliaryCubeTransform;
		}

		//set target
		gameInfo.targetIdx = Mathf.CeilToInt (cube.CubeNumber*Random.Range(0.01f,1.0f))-1;
		gameInfo.UpdateTarget (gameInfo.targetIdx);
		gameInfo.target.SetActive (true);
	}

	void DestroyCubes(){
		GameObject obj2Destroy;
		for (int i = 0; i < cube.CubeNumber; i++) {
			obj2Destroy = GameObject.Find ((i).ToString());
			Destroy(obj2Destroy);
		}
	}
		
}
