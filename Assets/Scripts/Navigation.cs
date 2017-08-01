using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour {
	float posx,posy;
	float speedx,speedz;

	public float rotateSpeed = 200;
	private Quaternion originalRotation;

	Vector3 speed;
	Vector3 cameraOrientation;

	Vector2 scrollDelta;
	Vector3 translateByScroll;

	GameObject auxiliaryCube;

	// Use this for initialization
	void Start () {
		auxiliaryCube = GameObject.Find ("auxiliaryCube");
		originalRotation=auxiliaryCube.transform.localRotation;
	}

	// Update is called once per frame
	void Update () {

		scrollDelta = Input.mouseScrollDelta;
		translateByScroll.x = 0;
		translateByScroll.z = scrollDelta.y;
		translateByScroll.y = 0;

		Camera.main.transform.Translate (translateByScroll);

		if (Input.GetMouseButton(1) == false)
			SnapBack();

		if (Input.GetMouseButton(1)) {
			//Debug.Log("Dragging");
			float rotX = Input.GetAxis("Mouse X") * rotateSpeed * Mathf.Deg2Rad;
			float rotY = Input.GetAxis("Mouse Y") * rotateSpeed * Mathf.Deg2Rad;

			auxiliaryCube.transform.Rotate(Vector3.up, -rotX);
			auxiliaryCube.transform.Rotate(Vector3.right, rotY);
		}
	}

	void SnapBack() {

		auxiliaryCube.transform.localRotation = Quaternion.Slerp(auxiliaryCube.transform.rotation, originalRotation, 15 * Time.deltaTime);
		//this.transform.localRotation = originalRotation;
	}

}
