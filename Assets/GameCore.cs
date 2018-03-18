using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCore : MonoBehaviour {

	[HideInInspector]
	private static double money = 0, rp = 0;
	public static Text rpTxt,moneyTxt;
	private static int area = 1;
	public static bool[] areasUnlocked = new bool[5];
	private GameObject unlockAreaObj;
	private int wrongTimerCounter = 0;
	public Sprite[] areasSprites;
	public Sprite[] obstaclesSprites;
	private Image[] areaBgs = new Image[2];
	private int valueArea = 0;
	private Image obstacleImg;
	private Dropdown areaDropdown;
	private float timer;
	private long dateTimer;
	public static long startMoneyTimer;
	public static double averageMoneyPerSec = 50;
	public static bool storyStarted;
	public static bool storyEnded;
	private static float profitsMulti = 1;
	AudioSource audioSource;
	public AudioClip[] clips;
	public static bool speedCheating;
	public static double sumbitResearchCount;
	public static double sumbitResearchCountP;

	// Use this for initialization
	void Start () {
		rp = rp / 3;
		area = area << 3;
		money = money / 3;
		profitsMulti = profitsMulti / 3;

		InvokeRepeating ("savePrefs",3,1);
		AudioListener.volume = 0.5f;
		audioSource = GetComponent<AudioSource> ();
		Application.targetFrameRate = 30;
		InvokeRepeating("checkSpeedHack",0,3f);
		startMoneyTimer = (System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond);
		StartCoroutine (startGame());
		timer = Time.realtimeSinceStartup;
		dateTimer = (System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond);
		rpTxt = transform.GetChild (2).GetComponentsInChildren<Text> ()[0];
		moneyTxt = transform.GetChild (2).GetComponentsInChildren<Text> ()[1];
		unlockAreaObj = GameObject.FindGameObjectWithTag ("unlockArea");
		unlockAreaObj.SetActive (false);
		areaBgs [0] = GameObject.FindGameObjectWithTag ("areaBg1").GetComponent<Image> ();
		areaBgs [1] = GameObject.FindGameObjectWithTag ("areaBg2").GetComponent<Image> ();
		obstacleImg = GameObject.FindGameObjectWithTag ("obstacle").GetComponent<Image> ();
		areaDropdown = GetComponentInChildren<Dropdown> ();
		areasUnlocked [0] = true;
		addRp (0);
		addMoney (0);
		StartCoroutine (SaveToFile.fileCheck());
		GameDetails gd =  SaveToFile.load ();
		if (gd != null)gd.load ();
		//InvokeRepeating ("save",1,1);
		StartCoroutine(checkMusic());
	}

	// Update is called once per frame
	void Update () {
		StartCoroutine (SaveToFile.fileCheck());
		StartCoroutine (SaveToFile.save(false));
		//Debug.Log (area * 3);
	}

	public void newSaveInFile(){
		SaveToFile.fileNum++;
		SaveToFile.dataPath = Application.persistentDataPath + "/GameDetails" + SaveToFile.fileNum.ToString () + ".dat";
		StartCoroutine (SaveToFile.save(true));
	}
	void savePrefs(){
		if(SaveToFile.readyToSave)
			SaveToFile.savePrefs ();
		KongregateAPIBehaviour.sumbitScore ("moneyReached", (int)(getRpMoney(1)/1000000));
	}

	IEnumerator checkMusic(){
		while (true) {
			yield return new WaitForSeconds (2);
			if (!audioSource.isPlaying) {
				audioSource.clip = clips [Random.Range (0, clips.Length)];
				float audioLength = 0;
				if(audioSource.clip!=null)
					audioLength = audioSource.clip.length;
				audioSource.Play ();
				yield return new WaitForSeconds (audioLength+2);
			}
		}
	}

	public IEnumerator startGame(){
		while(!SaveToFile.readyToSave){
			yield return new WaitForSeconds (0.1f);
		}
		areaChanged (getArea()-1);
		areaDropdown.value = getArea () - 1;
		if(!storyStarted){
			storyStarted = true;
			Story.showStart ();
		}
		if (getProfitsMulti() < 1)
			setProfitsMulti(1); 
	}

	void checkSpeedHack(){
		long unixTimePassed = (((System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond) - dateTimer));
		float unityTimePassed = ((Time.realtimeSinceStartup) - timer)*1000;

		if (Mathf.Abs (unixTimePassed - unityTimePassed) > 400/3f) {
			wrongTimerCounter++;
			if (wrongTimerCounter > 1) {
				//exit
				sumbitResearchCount = 0;
				sumbitResearchCountP = 0;
				addRp(-getRpMoney(0));
				addMoney(-getRpMoney(1));
			}
		} else {
			wrongTimerCounter = 0;
			if(sumbitResearchCount>=1000000){
				KongregateAPIBehaviour.sumbitScore ("totalResearch", (int)(sumbitResearchCount/1000000));
				sumbitResearchCount = 0;
			}
			if(sumbitResearchCountP>=1000000000000){
				KongregateAPIBehaviour.sumbitScore ("totalResearchP", (int)(sumbitResearchCountP/1000000000000));
				sumbitResearchCountP -= (int)(sumbitResearchCountP/1000000000000)*1000000000000;
				if(sumbitResearchCountP<0)
					sumbitResearchCountP = 0;
					
			}
		}

		timer = Time.realtimeSinceStartup;
		dateTimer = (System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond);

	}

	public static void addRp(double toAdd){
		rp = rp * 3;
		if(toAdd>0 && SaveToFile.readyToSave){
			sumbitResearchCount += toAdd;
			sumbitResearchCountP += toAdd;
		}
		rp += toAdd;
		rpTxt.text = formatSize(rp);
		rp = rp / 3;
	}
	public static void addMoney(double toAdd){
		money = money * 3;
		money += toAdd;
		moneyTxt.text = formatSize(money);
		money = money / 3;
	}


	public static string formatSize(double num){
		string temp= ((int)num).ToString();
		if(num > 99999){//K
			temp = ((int)(num/1000)).ToString()+"K";
		}
		if(num > 99999999){//M
			temp = ((int)(num/1000000)).ToString()+"M";
		}
		if(num > 99999999999){//G
			temp = ((int)(num/1000000000)).ToString()+"G";
		}
		if(num > 99999999999999){//T
			temp = ((int)(num/1000000000000)).ToString()+"T";
		}
		if(num > 99999999999999999){//P
			temp = ((int)(num/1000000000000000)).ToString()+"P";
		}
		return temp;
	}
	public static double getRpMoney(int rpOrMoney){
		if(rpOrMoney==0){
			double tempRp = rp * 3;
			return tempRp;
		}
		if(rpOrMoney==1){
			double tempMoney = money * 3;
			return tempMoney;
		}
		return 0;
	}

	public void areaChanged(int value){
		if (areasUnlocked [value]) {
			unlockThisArea (value);
		} else {
			areaDropdown.GetComponent<Image> ().sprite = areasSprites [value];
			unlockAreaObj.SetActive (true);
			unlockAreaObj.GetComponentsInChildren<Text> () [1].text = "Unlock Area " + (value+1);
			double tempCost = 0;
			for(int i=((value-1)*12);i<(value*12);i++){
				tempCost += (Mathf.Exp (3 + (0.29f * i)) * 3) - 40;
			}
			unlockAreaObj.GetComponentsInChildren<Text> () [0].text = formatSize(tempCost);
			valueArea = value;
		}
	}
	public void unlockAreaPressed(){
		int value = valueArea;
		double tempCost = 0;
		for(int i=((value-1)*12);i<(value*12);i++){
			tempCost += (Mathf.Exp (3 + (0.29f * i)) * 3) - 40;
		}
		if(tempCost<=getRpMoney(0)){
			addRp (-tempCost);
			unlockThisArea (value);
		}
	}
	private void unlockThisArea(int value){
		areasUnlocked [value] = true;
		areaBgs [0].sprite = areasSprites [value];
		areaBgs [1].sprite = areasSprites [value];
		obstacleImg.sprite = obstaclesSprites [value];
		areaDropdown.GetComponent<Image> ().sprite = areasSprites [value];
		unlockAreaObj.SetActive (false);
		setArea(value + 1);
	}
	public void dropDownOptionsSetBg(){
		Toggle[] tempToggle =  areaDropdown.GetComponentsInChildren<Toggle> ();

		for(int i=0;i<tempToggle.Length;i++){
			tempToggle[i].GetComponentInChildren<Image>().sprite = areasSprites[i];
		}
	}

	public static int getArea(){
		int tempArea = area >> 3;
		return tempArea;
	}
	public static void setArea(int newArea){
		area = newArea; 
		area = area << 3;
	}
	public void changeVolume(float value){
		AudioListener.volume = value;
	}
	public static float getProfitsMulti(){
		return (profitsMulti * 3);
	}
	public static void setProfitsMulti(float newValue){
		profitsMulti = newValue;
		profitsMulti = profitsMulti / 3;
	}
}
