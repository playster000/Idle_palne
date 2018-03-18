using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IdleProfits : MonoBehaviour {

	private static GameObject panel;
	private static Text rp;
	private static Text money;


	// Use this for initialization
	void Start () {
		panel = transform.GetChild (0).gameObject;
		rp = GetComponentsInChildren<Text> () [0];
		money = GetComponentsInChildren<Text> () [1];

		panel.gameObject.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static void showIdleProfits(double rp,double money){
		panel.SetActive (true);
		IdleProfits.rp.text = GameCore.formatSize (rp);
		IdleProfits.money.text = GameCore.formatSize (money);
	}
}
