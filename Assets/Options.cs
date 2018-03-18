using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour {

	private Slider soundSlider;
	private Button gear;
	private Button newGame;
	private GameObject panel;
	public Lab lab;
	public GameCore gameCore;

	// Use this for initialization
	void Start () {
		soundSlider = GetComponentInChildren<Slider> ();
		newGame = GetComponentsInChildren<Button> () [1];
		panel = newGame.transform.parent.gameObject;

		panel.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void startNewGame(){	
		//GameCore.profitsMulti += (Lab.getEnergy (0) > 100 ? (Lab.getEnergy (0) / 4800) : 0) + (Lab.getEnergy (0) > 300 ? (Lab.getEnergy (0) / 1600) : 0);
		//GameCore.setProfitsMulti(GameCore.getProfitsMulti() + (Lab.getEnergy (0) > 100 ? (Mathf.Exp((Lab.getEnergy (0) / 70.1139f))/100) : 0));
		float minEnergy = 100;
		if(((GameCore.getProfitsMulti ()*4) + Lab.getEnergy (0))>100)
			minEnergy = (GameCore.getProfitsMulti ()*4) + minEnergy;
		float tempToRemove = GameCore.getProfitsMulti ();
		if (Lab.getEnergy (0)-(tempToRemove*4)<100)
			tempToRemove = (Lab.getEnergy (0)-101)/4;
		else
			tempToRemove = GameCore.getProfitsMulti ();
		GameCore.setProfitsMulti(GameCore.getProfitsMulti() + (Lab.getEnergy (0) > minEnergy ? (Mathf.Pow(((Lab.getEnergy (0)-(tempToRemove*4))/100f), 2F)/ 5.3333f) : 0));
		GameDetails gd = new GameDetails ();
		gd.reset ();
		setActivePanel (false);
		lab.load ();
		StartCoroutine (gameCore.startGame());
		Research.lockButtons ();
	}

	public void setActivePanel(bool active){
		panel.SetActive (active);
		float minEnergy = 100;
		if(((GameCore.getProfitsMulti ()*4) + Lab.getEnergy (0))>100)
			minEnergy = (GameCore.getProfitsMulti ()*4) + minEnergy;
		float tempToRemove = GameCore.getProfitsMulti ();
		if (Lab.getEnergy (0)-(tempToRemove*4)<100)
			tempToRemove = (Lab.getEnergy (0)-101)/4;
		else
			tempToRemove = GameCore.getProfitsMulti ();
		float tempProfitsMulti = ((int)((GameCore.getProfitsMulti()+(Lab.getEnergy (0) > minEnergy ? (Mathf.Pow(((Lab.getEnergy (0)-(tempToRemove*4))/100f), 2F)/ 5.3333f) : 0)) * 100)) / 100f;
		//float tempProfitsMulti = ((int)((GameCore.getProfitsMulti()+(Lab.getEnergy (0) > 100 ? (Mathf.Exp((Lab.getEnergy (0) / 70.1139f))/100) : 0)) * 100)) / 100f;
		newGame.GetComponentInChildren<Text> ().text = "x"+(tempProfitsMulti).ToString ();
	}

	public void setVolume(float value){
		AudioListener.volume = value;
	}
}
