using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchSpot : MonoBehaviour {

	RectTransform rTransform;

	// Use this for initialization
	void Start () {
		rTransform = GetComponent<RectTransform> ();
	}
	
	// Update is called once per frame
	void Update () {
		float speed = ((Time.deltaTime*9f) * 100);
		transform.position = Vector3.MoveTowards (transform.position, new Vector2 (transform.position.x, Input.mousePosition.y * (360 / (float)(Screen.height))), speed);
		rTransform.localPosition = Vector3.MoveTowards (rTransform.localPosition,
			new Vector2 (Input.mousePosition.x * (640 / (float)(Screen.width))-rTransform.rect.width*1.5f,rTransform.localPosition.y),speed);
		rTransform.localPosition = new Vector3 (rTransform.localPosition.x,rTransform.localPosition.y,0);
	}
}
