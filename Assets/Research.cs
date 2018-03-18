using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Research : MonoBehaviour {

	private GameObject labBtn;
	public GameObject researchBtn;
	private static double rpPerSec = 1;
	private Text rpPerSecText;
	private static Button[] upgradesBtn = new Button[6];
	public static int[] upgradesDone = new int[6];
	int firstPrice = 300;
	public static bool[] buttonUnlocked = new bool[6];
	private bool loopingCalcUpgradeCost;

	// Use this for initialization
	void Start () {
		rpPerSec = rpPerSec / 3;
		labBtn = GameObject.FindGameObjectWithTag ("labBtn");
		//researchBtn = GameObject.FindGameObjectWithTag ("researchBtn");
		rpPerSecText = GetComponentInChildren<Text> ();
		for(int i=0;i<6;i++){
			upgradesBtn [i] = GetComponentsInChildren<Button> () [i];
			/*upgradesBtn[i].onClick.AddListener(delegate {
				Debug.Log("ss");	
			});*/
		}
		buttonUnlocked [0] = true;
		//calcUpgradeCost ();
		StartCoroutine (loopCalcUpgradeCost());
		gameObject.SetActive (false);
	}

	// Update is called once per frame
	void Update () {

	}

	public void setResearch(){
		gameObject.SetActive (true);
		if (labBtn == null)
			Start ();
		ColorBlock temp = labBtn.GetComponent<Button> ().colors;
		temp.normalColor = new Color (170 / 255f, 170 / 255f, 170 / 255f, 1);
		labBtn.GetComponent<Button> ().colors = temp;
		temp = researchBtn.GetComponent<Button> ().colors;
		temp.normalColor = new Color (1,1,1, 1);
		researchBtn.GetComponent<Button> ().colors = temp;
		loopingCalcUpgradeCost = false;
		StartCoroutine (loopCalcUpgradeCost());
	}

	public void buttonPressed(int index){
		double tempCost = ((firstPrice * Mathf.Pow (1.2f, upgradesDone [index])) * Mathf.Pow (10, index))+0;
		if (GameCore.getRpMoney(1)>=tempCost) {
			GameCore.addMoney (-tempCost);
			upgradesDone [index]++;
			calcUpgradeCost ();
		}
	}
	IEnumerator loopCalcUpgradeCost(){
		if (!loopingCalcUpgradeCost) {
			while (true) {
				yield return new WaitForSeconds (0.3f);
				calcUpgradeCost ();
				loopingCalcUpgradeCost = true;
			}
		}
	}
	void calcUpgradeCost(){
		setRpPerSec(1);
		for (int i = 0; i < 6; i++) {
			double tempCost = ((firstPrice * Mathf.Pow (1.2f, upgradesDone [i])) * Mathf.Pow (10, i))+0;
			setRpPerSec(getRpPerSec()+(upgradesDone [i] * Mathf.Pow (10, i)));
			upgradesBtn [i].GetComponentInChildren<Text> ().text = GameCore.formatSize (tempCost);
			if ((!buttonUnlocked [i] && GameCore.getRpMoney (1) >= tempCost)
				|| (upgradesBtn [i].GetComponentsInChildren<Image> ().Length > 3 && buttonUnlocked [i] && i > 0)) {

				buttonUnlocked [i] = true;
				upgradesBtn [i].GetComponentsInChildren<Image> () [2].gameObject.SetActive (false);
			}
		}
		setRpPerSec ();
	}

	void setRpPerSec(){
		rpPerSecText.text = "Research points per second:\n"+ GameCore.formatSize (getRpPerSec());
	}

	public static void setRpPerSec(double newRpPerSec){
		rpPerSec = newRpPerSec; 
		rpPerSec = rpPerSec / 3;
	}
	public static double getRpPerSec(){
		double tempRpPerSec = rpPerSec * 3;
		return tempRpPerSec; 
	}
	public static void lockButtons(){
		for(int i=1;i<6;i++){
			upgradesBtn [i].transform.GetChild (2).gameObject.SetActive (true);
		}
	}
}
