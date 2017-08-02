using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour {
	float posx,posy;
	float speedx,speedz;

	public float rotateSpeed = 10;
	//private Quaternion originalRotation;
	private Vector3 originalPos;

	Vector3 speed;
	Vector3 cameraOrientation;

	Vector2 scrollDelta;
	Vector3 translateByScroll;

	GameObject auxiliaryCube;

	// Use this for initialization
	void Start () {
		auxiliaryCube = GameObject.Find ("auxiliaryCube");
		originalPos=Camera.main.transform.localPosition;
	}

	// Update is called once per frame
	void Update () {

		scrollDelta = Input.mouseScrollDelta;
		translateByScroll.x = -scrollDelta.y;
		translateByScroll.z = scrollDelta.y;
		translateByScroll.y = -scrollDelta.y;

		Camera.main.transform.localPosition+=translateByScroll;//Translate (translateByScroll);
		originalPos += translateByScroll;

		Camera.main.transform.LookAt(auxiliaryCube.transform);

		if (Input.GetMouseButton(1) == false)
			SnapBack();

		if (Input.GetMouseButton(1)) {
			//Debug.Log("Dragging");
			float rotX = -Input.GetAxis("Mouse X") * rotateSpeed * Mathf.Deg2Rad;
			float rotY = -Input.GetAxis("Mouse Y") * rotateSpeed * Mathf.Deg2Rad;
			//dead value 0.1, sensitivity 0.01, gravity 0

			Camera.main.transform.Translate(Vector3.right*rotX);
			Camera.main.transform.Translate(Vector3.up* rotY);
		}
	}

	void SnapBack() {

		Camera.main.transform.localPosition = Vector3.Slerp(Camera.main.transform.position,originalPos,15*Time.deltaTime);
	}

}
