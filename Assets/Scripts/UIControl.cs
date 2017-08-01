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

	public Slider difficultySlider;
	public Text difficulty;

	public Text reactTime;

	public Slider cubeNumberSlider;
	public Text cubeNum;

	// Use this for initialization
	void Start () {
		button = GameObject.Find ("Button").GetComponent<Button> ();
		button.onClick.AddListener(OnClick);
		restartBtn=GameObject.Find ("restartBtn").GetComponent<Button> ();
		restartBtn.onClick.AddListener(ClickToRestart);
		btnText = GameObject.Find ("btnText").GetComponent<Text> ();
		cubeHit = GameObject.Find ("cubeHit").GetComponent<Text> ();

		difficultySlider = GameObject.Find ("difficultySlider").GetComponent<Slider> ();
		difficultySlider.onValueChanged.AddListener (delegate{DifficultyChangeCheck();});
		difficulty = difficultySlider.transform.FindChild ("difficulty").gameObject.GetComponent<Text> ();

		reactTime=GameObject.Find ("reactTime").GetComponent<Text> ();

		cubeNumberSlider = GameObject.Find ("cubeNumberSlider").GetComponent<Slider> ();
		cubeNumberSlider.onValueChanged.AddListener (delegate{CubeNumChangeCheck();});
		cubeNum = cubeNumberSlider.transform.FindChild ("cubeNum").gameObject.GetComponent<Text> ();

	}
	
	// Update is called once per frame
	void Update () {
		//button & text region
		if(gameInfo.phaseNo == 0){
			gameInfo.PauseGame ();
			button.gameObject.SetActive (true);
			btnText.text = "Proceed";
			cubeHit.text = "Given several cubes, organized in 3*3 space, with a tree in one of them\n\n"+"Now please REMEMBER the position of each cube and the tree for later manipulation & recall";
			restartBtn.gameObject.SetActive (false);
		}
		else if(gameInfo.phaseNo == 1){
			
			button.gameObject.SetActive (true);
			btnText.text = "Play";
			cubeHit.text = "After you press [Play], cubes will begin shifting\n\n"+"Tree in one cube will travel to the nearest one with an adjoining face, "+"which will later be indicated by both being marked [GREEN]";
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
					cubeHit.text = "Now this is the same view as shown at beginning. Indicate the cube with tree by ONE CLICK on it";
				}
			}
			if (gameInfo.isTargetFound) {
				gameInfo.isChooseEnabled = false;
				cubeHit.text = "You've found the tree!";
				restartBtn.gameObject.SetActive (true);
			}
			reactTime.text = gameInfo.reactTime.ToString ("##.000");
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

	void DifficultyChangeCheck(){
		gameInfo.MaxTravelPeriodNo=(int)((difficultySlider.value)*5+1);
		difficulty.text = (gameInfo.MaxTravelPeriodNo).ToString()+ "shifting / trial";
	}

	void CubeNumChangeCheck(){
		gameInfo.CubeNumber=(int)((cubeNumberSlider.value)*2+3);
		cubeNum.text = (gameInfo.CubeNumber).ToString()+ "cubes";
	}


}
