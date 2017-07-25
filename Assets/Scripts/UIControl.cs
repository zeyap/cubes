using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour {

	public GameInfo gameInfo=GameInfo.getInstance();
	public Button button;
	public Text btnText;

	public Text cubeHit;

	// Use this for initialization
	void Start () {
		button = GameObject.Find ("Button").GetComponent<Button> ();
		button.onClick.AddListener(OnClick);

		btnText = GameObject.Find ("btnText").GetComponent<Text> ();
		cubeHit = GameObject.Find ("cubeHit").GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		
		//button
		if (gameInfo.periodNo == 0) {
			button.gameObject.SetActive (true);
			btnText.text="Play";
		}else if(gameInfo.periodNo == 1){
			button.gameObject.SetActive (false);
		}else if(gameInfo.periodNo >= 2){
			button.gameObject.SetActive (true);
			btnText.text="Restart";
		}

		//text region
		if (gameInfo.moves == 1) {
			if(gameInfo.periodNo == 0){
				cubeHit.text = "Tree in one cube will travel to the nearest one with an adjoining face [LATER indicated by both being marked GREEN]";
			}
			if(gameInfo.periodNo == 1){
				gameInfo.target.SetActive(false);
				cubeHit.text = "Now the tree is travelling between cubes";
			}
			if (gameInfo.periodNo >= 2) {
				if (!gameInfo.isTargetFound) {
					gameInfo.isChooseEnabled = true;
					cubeHit.text = "NOW indicate the cube with tree by ONE CLICK on it";
				} else {
					gameInfo.isChooseEnabled = false;
					cubeHit.text = "You've found the tree!";
				}
			}
		}

	}

	void OnClick(){
		if (gameInfo.periodNo == 0) {
			gameInfo.periodNo = 1;
		}
		if (gameInfo.periodNo == 2) {
			gameInfo.periodNo = 0;
		}
	}
}
