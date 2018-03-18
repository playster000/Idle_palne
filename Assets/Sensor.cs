using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sensor : MonoBehaviour {

	[HideInInspector]
	public float sensor;
	float bgWidth=195;
	RectTransform rTran;
	Vector2 startPos;

	// Use this for initialization
	void Start () {
		sensor = 10;
		rTran = GetComponent<RectTransform> ();
		startPos = rTran.localPosition;
		rTran.localPosition = new Vector2 (rTran.localPosition.x+bgWidth*(sensor/100f),rTran.localPosition.y);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setSensor(float value){
		sensor = value;
		rTran.localPosition = new Vector2 (startPos.x+bgWidth*(sensor/100f),startPos.y);
	}

}
