using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Story : MonoBehaviour {

	private static GameObject panel;
	private static GameObject start;
	private static GameObject end;

	// Use this for initialization
	void Start () {
		panel = transform.GetChild (0).gameObject;
		start = GetComponentsInChildren<ScrollRect> () [0].gameObject;
		end = GetComponentsInChildren<ScrollRect> () [1].gameObject;
		panel.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	public static void showStart(){
		panel.SetActive (true);
		start.SetActive (true);
		end.SetActive (false);
	}
	public static void showEnd (){
		panel.SetActive (true);
		start.SetActive (false);
		end.SetActive (true);
	}

}
