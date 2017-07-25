using System;
using UnityEngine;

public class GameInfo
{
	public int targetIdx;
	public GameObject target;
	public bool isTargetFound;
	public bool isChooseEnabled;
	public int moves;//1~3
	public int periodNo;//{Instructions,Travelling,Clicking};//0-instructions, 1-travelling, >=2-clicking

	public float lastUpdateTime,currTime;
	public bool isMoving;
	public const float stillDuration = 1.0f;
	public const float shiftDuration = 1.0f;

	public bool[] isShiftDone;

	//单例模式
	private static GameInfo instance = new GameInfo ();
	private GameInfo(){}
	public static GameInfo getInstance(){
		return instance;
	}
}