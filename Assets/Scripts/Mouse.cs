using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mouse : MonoBehaviour {

	Text cubeHit;
	private GameInfo gameInfo=GameInfo.getInstance();

	// Use this for initialization
	void Start () {
		cubeHit = GameObject.Find ("cubeHit").GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if (Physics.Raycast (ray, out hit)) {
			if (gameInfo.isChooseEnabled) {
				/*
				if (hit.collider != null) {
					cubeHit.text="Cube #" + hit.collider.name;
				}
				*/
				if (hit.collider.name==gameInfo.targetIdx.ToString()&&Input.GetMouseButton(0)) {
					//cubeHit.text+=" Correct";
					gameInfo.target.SetActive(true);
					gameInfo.isTargetFound = true;
					//gameInfo.isChooseEnabled = false;
				}
			}
		}
	}
}
