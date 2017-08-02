using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour {

	private float wx;
	private static float MaxRotateAngle;
	//private Quaternion originalRotation;
	private Quaternion originalRot;

	float radius;
	Vector3 cameraOrientation;
	Quaternion rot;
	Vector3 cameraPos;

	Vector2 scrollDelta;
	Vector3 translateByScroll;

	GameObject auxiliaryCube;
	GameObject middleCube;

	GameInfo gameInfo;

	// Use this for initialization
	void Awake () {
		auxiliaryCube = GameObject.Find ("auxiliaryCube");
		middleCube = GameObject.Find ("middleCube");
		originalRot=middleCube.transform.rotation;
		cameraPos.y = 0;//Camera.main.transform.position.y;

		radius = Vector3.Distance (middleCube.transform.position,Camera.main.transform.position);
		MaxRotateAngle=90;

		wx = Camera.main.pixelRect.center.x;

		gameInfo = GameInfo.getInstance ();
	}

	void Start(){}

	// Update is called once per frame
	void Update () {

		scrollDelta = Input.mouseScrollDelta;
		Camera.main.orthographicSize =Mathf.Clamp(Camera.main.orthographicSize-=scrollDelta.y,6.0f,20.0f);

		if (Input.GetMouseButton (1) == false) {
			SnapBack ();
		}

		if (Input.GetMouseButton(1)) {
			rot = Quaternion.Euler(0,(Input.mousePosition.x-wx)/wx * MaxRotateAngle,0);//

			middleCube.transform.SetPositionAndRotation (middleCube.transform.position,Quaternion.Slerp(middleCube.transform.rotation,rot,8*Time.deltaTime));
		}
		//Camera.main.transform.LookAt(auxiliaryCube.transform);
	}

	void SnapBack() {

		middleCube.transform.SetPositionAndRotation(middleCube.transform.position,Quaternion.Slerp(middleCube.transform.rotation,originalRot,8*Time.deltaTime));
	}

}
