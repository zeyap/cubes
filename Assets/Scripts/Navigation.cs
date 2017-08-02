using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour {

	public float rotateSpeed =30f;
	//private Quaternion originalRotation;
	private Vector3 originalPos;

	float radius;
	Vector3 cameraOrientation;
	float rot=0;
	Vector3 cameraPos;
	float t;

	Vector2 scrollDelta;
	Vector3 translateByScroll;

	GameObject auxiliaryCube;

	// Use this for initialization
	void Start () {
		auxiliaryCube = GameObject.Find ("auxiliaryCube");
		originalPos=Camera.main.transform.localPosition;
		cameraPos.y=Camera.main.transform.localPosition.y;

		radius = Vector3.Distance (auxiliaryCube.transform.position,originalPos);
	}

	// Update is called once per frame
	void FixedUpdate () {

		scrollDelta = Input.mouseScrollDelta;
		Camera.main.orthographicSize =Mathf.Clamp(Camera.main.orthographicSize-=scrollDelta.y,6.0f,20.0f);

		/*
		translateByScroll.x = -scrollDelta.y;
		translateByScroll.z = scrollDelta.y;
		translateByScroll.y = -scrollDelta.y;

		Camera.main.transform.localPosition+=translateByScroll;//Translate (translateByScroll);
		originalPos += translateByScroll;
		*/

		if (Input.GetMouseButton (1) == false) {
			SnapBack ();
		}

		if (Input.GetMouseButton(1)) {
			rot =   rotateSpeed * Mathf.Deg2Rad*Time.fixedDeltaTime;//Input.mousePosition.x>=0?1:(-1) *
				//Debug.Log("Dragging");
				//dead value 0.1, sensitivity 0.03, gravity 0

			cameraPos.x = radius * Mathf.Cos(-rot);
			cameraPos.z = radius * Mathf.Sin(-rot);
			Camera.main.transform.position = originalPos+cameraPos;

		}

		Camera.main.transform.LookAt(auxiliaryCube.transform);
	}

	void SnapBack() {

		Camera.main.transform.localPosition = Vector3.Slerp(Camera.main.transform.position,originalPos,8*Time.deltaTime);
	}

}
