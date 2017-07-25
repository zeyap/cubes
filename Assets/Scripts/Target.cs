using System;
using UnityEngine;

public class Target
{
	public int idx;
	public GameObject target;
	public bool isFound;

	//单例模式
	private static Target instance = new Target ();
	private Target(){}
	public static Target getInstance(){
		return instance;
	}
}