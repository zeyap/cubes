//using System;
using System.Collections;
using UnityEngine;

public class GameInfo
{

	public Cube cube=Cube.getInstance();

	public int targetIdx;
	public GameObject target;
	public bool isTargetFound;
	public bool isChooseEnabled;
	public int moves;//1~3
	public int phaseNo;//{Instructions,Travelling,Clicking};//0-"proceed",1-"play", 2-travelling, >=3-clicking
	public int travelPeriodNo;
	public int MaxTravelPeriodNo=1;
	public float lastUpdateTime,currTime;
	public bool isMoving;
	public const float stillDuration = 1.0f;
	public const float shiftDuration = 1.0f;

	public bool[] isShiftDone=new bool[Cube.CubeNumber*3];

	public bool needDestroyCubes;
	public bool needReInstantiate;

	//单例模式
	private static GameInfo instance = new GameInfo ();
	private GameInfo(){}
	public static GameInfo getInstance(){
		return instance;
	}

	public void Init(){
		//initialize GameInfo records

		targetIdx = Mathf.CeilToInt (Cube.CubeNumber*Random.Range(0.01f,1.0f))-1;

		UpdateTarget (targetIdx);
		target.SetActive (true);

		isTargetFound = false;
		isChooseEnabled = false;

		lastUpdateTime = Time.time;
		isMoving = true;
		moves = 1;
		phaseNo = 0;
		travelPeriodNo = 1;
		UndoneShifts();

		needDestroyCubes=false;
		needReInstantiate = false;
	
	}

	public void Play(){
		UpdateTarget (targetIdx);

		target.SetActive (true);
		isTargetFound = false;
		isChooseEnabled = false;
		lastUpdateTime = Time.time;
		isMoving = true;
		moves = 1;

		phaseNo = 2;
	}

	public void Retry(){
		Init ();
	}

	public void Restart(){//Only referenced when 3 cubes already exist
		
		//destroy old cubes
		needDestroyCubes=false;
		//Instantiate new cubes
		needReInstantiate=false;

		cube.InitializePos ();
		cube.FindAdjoiningCubes ();
		Init ();
	}

	public void PauseGame(){
		Time.timeScale = 0;
	}
	public void PauseAtFirstMove(){
		moves = 1;
		PauseGame ();
	}
	public void ResumeGame(){
		Time.timeScale = 1;
	}

	public void UpdateTarget(int targetIdx){
		SetAllTreeVisible ();
		target = cube.trees [targetIdx];
		SetAllTreeInvisible ();
	}

	void SetAllTreeVisible(){
		for (int i = 0; i < Cube.CubeNumber; i++) {
			cube.trees[i].SetActive(true);
		}
	}

	void SetAllTreeInvisible(){
		for (int i = 0; i < Cube.CubeNumber; i++) {
			cube.trees[i].SetActive(false);
		}
	}

	public void UndoneShifts(){
		for (int i = 0; i < Cube.CubeNumber; i++) {
			for (int mvs = 1; mvs <= 3; mvs++) {
				isShiftDone [i * 3 + mvs - 1] = false;
			}
		}
	}
}