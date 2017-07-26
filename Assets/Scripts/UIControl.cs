using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour {

	public GameInfo gameInfo=GameInfo.getInstance();
	public Button button;
	public Button restartBtn;
	public Text btnText;

	public Text cubeHit;

	// Use this for initialization
	void Start () {
		button = GameObject.Find ("Button").GetComponent<Button> ();
		button.onClick.AddListener(OnClick);
		restartBtn=GameObject.Find ("restartBtn").GetComponent<Button> ();
		restartBtn.onClick.AddListener(ClickToRestart);
		btnText = GameObject.Find ("btnText").GetComponent<Text> ();
		cubeHit = GameObject.Find ("cubeHit").GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		//button & text region
		if(gameInfo.phaseNo == 0){
			gameInfo.PauseGame ();
			button.gameObject.SetActive (true);
			btnText.text = "Proceed";
			cubeHit.text = "Given several cubes, organized in 3*3 space, with a tree in one of them; now please REMEMBER the position of each cube and the tree for later manipulation & recall";
			restartBtn.gameObject.SetActive (false);
		}
		else if(gameInfo.phaseNo == 1){
			
			button.gameObject.SetActive (true);
			btnText.text = "Play";
			cubeHit.text = "Tree in one cube will travel to the nearest one with an adjoining face [LATER indicated by both being marked GREEN]";
			restartBtn.gameObject.SetActive (false);
		}
		else if(gameInfo.phaseNo == 2){
			
			if (gameInfo.moves == 1) {
				button.gameObject.SetActive (false);
			}
			else if (gameInfo.moves == 2) {
				gameInfo.target.SetActive (false);
			}

			cubeHit.text = "Now the tree is travelling between cubes";
			restartBtn.gameObject.SetActive (false);
		}
		else if (gameInfo.phaseNo == 3) {
			if (gameInfo.moves == 3) {
				gameInfo.PauseAtFirstMove ();

				button.gameObject.SetActive (true);
				btnText.text = "Retry";

				if (!gameInfo.isTargetFound) {
					gameInfo.isChooseEnabled = true;
					cubeHit.text = "NOW indicate the cube with tree by ONE CLICK on it";
				}
				restartBtn.gameObject.SetActive (true);
			}
			if (gameInfo.isTargetFound) {
				gameInfo.isChooseEnabled = false;
				cubeHit.text = "You've found the tree!";
			}
		}
	}

	void OnClick(){
		if (gameInfo.phaseNo == 0) {//Proceed
			gameInfo.phaseNo++;
		}
		else if (gameInfo.phaseNo == 1) {//play
			gameInfo.ResumeGame ();
			gameInfo.Play ();
		}
		else if (gameInfo.phaseNo == 3) {//retry
			gameInfo.Retry ();
		}
	}

	void ClickToRestart(){
		if (gameInfo.phaseNo == 3) {//restart
			gameInfo.Restart ();
		}
	}


}
