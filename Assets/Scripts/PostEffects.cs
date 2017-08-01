using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffects : MonoBehaviour {

	public BrightnessSaturationAndContrast bscCamera;
	public Camera camera;
	public GameInfo gameInfo;
	float initialSaturation;
	float initialGreenValue;
	// Use this for initialization
	void Awake(){
		gameInfo = GameInfo.getInstance ();
		camera = Camera.main;
		bscCamera = camera.GetComponent<BrightnessSaturationAndContrast>();
		initialSaturation = bscCamera.saturation;
		initialGreenValue = bscCamera.green;
	}
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (gameInfo.phaseNo >= 3 && gameInfo.isTargetFound) {
			bscCamera.saturation = 0;
			bscCamera.green = 0.26f;
		} else {
			bscCamera.saturation = initialSaturation;
			bscCamera.green = initialGreenValue;
		}
	}
}
