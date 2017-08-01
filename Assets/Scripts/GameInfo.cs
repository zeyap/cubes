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
	//difficulty
	public int MaxTravelPeriodNo=1;
	public int CubeNumber=3;

	public float lastUpdateTime,currTime;
	public bool isMoving;
	public const float stillDuration = 1.0f;
	public const float shiftDuration = 1.0f;

	public bool[] isShiftDone=new bool[Cube.MaxCubeNumber*3];

	public bool needDestroyCubes;
	public bool needReInstantiate;

	public bool hasPhase3Begun;
	public float beginTime;
	public float reactTime;
	public int score;

	//单例模式
	private static GameInfo instance = new GameInfo ();
	private GameInfo(){}
	public static GameInfo getInstance(){
		return instance;
	}

	public void Init(){
		//initialize GameInfo records

		isTargetFound = false;
		isChooseEnabled = false;

		lastUpdateTime = Time.time;
		isMoving = true;
		moves = 1;
		phaseNo = 0;
		hasPhase3Begun = false;
		travelPeriodNo = 1;
		UndoneShifts();

		needDestroyCubes=true;
		needReInstantiate = true;
	
	}

	public void Play(){

		isTargetFound = false;
		isChooseEnabled = false;
		lastUpdateTime = Time.time;
		isMoving = true;
		moves = 1;

		phaseNo = 2;
	}

	public void Retry(){
		Init ();
		//no need to destroy old cubes
		needDestroyCubes=false;
		//no need to instantiate new cubes
		needReInstantiate=false;

	}

	public void Restart(){//Only referenced when 3 cubes already exist

		cube.CubeNumber = CubeNumber;
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
		for (int i = 0; i < cube.CubeNumber; i++) {
			cube.trees[i].SetActive(true);
		}
	}

	void SetAllTreeInvisible(){
		for (int i = 0; i < cube.CubeNumber; i++) {
			cube.trees[i].SetActive(false);
		}
	}

	public void UndoneShifts(){
		for (int i = 0; i < cube.CubeNumber; i++) {
			for (int mvs = 1; mvs <= 3; mvs++) {
				isShiftDone [i * 3 + mvs - 1] = false;
			}
		}
	}
}