using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour {

	public float rotateSpeed = 10;
	//private Quaternion originalRotation;
	private Vector3 originalPos;

	float zoomByScroll;

	GameObject auxiliaryCube;

	// Use this for initialization
	void Awake () {
		auxiliaryCube = GameObject.Find ("auxiliaryCube");
		originalPos=Camera.main.transform.localPosition;
	}
	void Start(){
	}

	// Update is called once per frame
	void Update () {
		zoomByScroll=Input.mouseScrollDelta.y;
		Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize -= zoomByScroll, 6.0f, 20.0f);
		//Camera.main.orthographicSize-=zoomByScroll;
		if (Input.GetMouseButton (1) == false) {
			SnapBack ();
		}
		else if (Input.GetMouseButton(1)) {
			//Debug.Log("Dragging");
			float rotX = -Input.GetAxis("Mouse X") * rotateSpeed * Mathf.Deg2Rad;
			//float rotY = -Input.GetAxis("Mouse Y") * rotateSpeed * Mathf.Deg2Rad;
			//dead value 0.1, sensitivity 0.3, gravity 0

			Camera.main.transform.Translate(Vector3.right*rotX);
			//Camera.main.transform.Translate(Vector3.up* rotY);
		}

		Camera.main.transform.LookAt(auxiliaryCube.transform);
	}

	void SnapBack() {

		Camera.main.transform.localPosition = Vector3.Slerp(Camera.main.transform.position,originalPos,15*Time.deltaTime);
	}

}
