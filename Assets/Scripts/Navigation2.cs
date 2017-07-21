using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation2 : MonoBehaviour {
	float wx,wy;
	float posx,posy;
	float speedx,speedz;
	public Camera camera;
	Vector3 speed;
	Vector3 cameraPos;
	Vector3 cameraOrientation;
	// Use this for initialization
	void Start () {
		camera = Camera.main;
		wx = camera.pixelWidth/2;
		wy = camera.pixelHeight/2;
		cameraPos = camera.transform.position;
		cameraOrientation = camera.transform.rotation.eulerAngles;
	}

	// Update is called once per frame
	void Update () {
		posx = Input.mousePosition.x-wx;
		posy = Input.mousePosition.y-wy;
		if (Mathf.Abs (posx) > wx / 3 || Mathf.Abs (posy) > wx / 2) {
			camera.transform.SetPositionAndRotation (cameraPos, Quaternion.Euler (cameraOrientation.x - posy / wy * 10, cameraOrientation.y + posx / wx * 50, cameraOrientation.z));
			//Quaternion.Euler accepts degree param, & Mathf.Atan returns radians
		} else {
			camera.transform.SetPositionAndRotation (cameraPos, Quaternion.Euler (cameraOrientation.x, cameraOrientation.y, cameraOrientation.z));
		}

	}
}
