using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BgMover : MonoBehaviour {

	public RectTransform bg1,bg2;
	private RectTransform toChange;
	private float width;
	[HideInInspector]
	public float speed;
	[HideInInspector]
	public float maxSpeed;
	[HideInInspector]
	public float acel;
	private Vector2 startPos;
	private float totalMoved=0;
	public Text speedText;
	private double[] averageMoneyPerSecSamples = new double[30];
	private int averageMoneyPerSecSamplesIndex;
	[HideInInspector]
	public AudioSource thrust;

	// Use this for initialization
	void Start () {
		//speedText = GetComponentInChildren<Text> ();
		thrust = GetComponent<AudioSource>();
		StartCoroutine (changeSpeedTxt());
		maxSpeed = 10;
		acel = 10;
		speed = 3;
		width = bg1.rect.width;
		startPos = bg2.localPosition;
		toChange = bg1;
		for(int i=0;i<averageMoneyPerSecSamples.Length;i++){
			averageMoneyPerSecSamples [i] = GameCore.averageMoneyPerSec / 10;
		}
	}
	
	// Update is called once per frame
	void Update () {
		speed += speed<maxSpeed ? acel * (Time.deltaTime/0.5f) : 0;//0.5 secs 
		if (speed > maxSpeed)
			speed = maxSpeed;
		totalMoved += ((Time.deltaTime*7)*speed);
		bg1.localPosition = new Vector2 (bg1.localPosition.x - ((Time.deltaTime*7)*speed),bg1.localPosition.y);
		bg2.localPosition = new Vector2 (bg2.localPosition.x - ((Time.deltaTime*7)*speed),bg2.localPosition.y);
		if (totalMoved - width >= -(width*0.005f)) {
			totalMoved = 0;
			StartCoroutine (changePos ());
		}
		thrust.pitch = 0.8f + (speed / 50f);
	}
	IEnumerator changePos(){
		yield return new WaitForSeconds (0.5f);
		toChange.localPosition = startPos-new Vector2(totalMoved+(width*0.001f),0);
		if(toChange.Equals(bg1))
			toChange = bg2;
		else
			toChange = bg1;
	}

	IEnumerator changeSpeedTxt(){
		while (true) {
			yield return new WaitForSeconds (0.1f);
			float tempProfitsMulti = GameCore.getProfitsMulti();
			double tempMoneyToAdd = ((((double)((1 / Mathf.Exp (0.007f * (100 - speed))) * speed * Mathf.Pow (8, GameCore.getArea() - 1)) + 1)
				* Time.deltaTime * 30) +((Lab.getEnergy(0)/16)* Mathf.Pow (8, GameCore.getArea() - 1))) * tempProfitsMulti;
			//calcAverageMoneyPerSec (tempMoneyToAdd);
			GameCore.addMoney (tempMoneyToAdd);
			GameCore.addRp ((Research.getRpPerSec()/10)*Time.deltaTime*30);
			speedText.text = (int)(speed * 30) + " km/h";
			if (SaveToFile.readyToSave) {
				double tempIdleRpToAdd = (System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond - GameCore.startMoneyTimer - 100f)
				                         * (Research.getRpPerSec () / 2000);
				double tempIdleMoneyToAdd = (System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond - GameCore.startMoneyTimer - 100f)
					* (GameCore.averageMoneyPerSec / 2000) * tempProfitsMulti;
				if((System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond - GameCore.startMoneyTimer - 100f)>4000){
					IdleProfits.showIdleProfits (tempIdleRpToAdd,tempIdleMoneyToAdd);
				}
				GameCore.addRp (tempIdleRpToAdd);
				GameCore.addMoney (tempIdleMoneyToAdd);
				//Debug.Log ((System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond - GameCore.startMoneyTimer - 100f)
				//	* (GameCore.averageMoneyPerSec / 1000));
				GameCore.averageMoneyPerSec = ((Lab.getEnergy(0))* Mathf.Pow (8, GameCore.getArea() - 1));
				GameCore.startMoneyTimer = (System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond);
			}
		}
	}

	public void calcAverageMoneyPerSec(double sampleToAdd){
		if (SaveToFile.readyToSave) {
			averageMoneyPerSecSamples [averageMoneyPerSecSamplesIndex] = sampleToAdd;
			averageMoneyPerSecSamplesIndex++;
			if (averageMoneyPerSecSamplesIndex >= averageMoneyPerSecSamples.Length)
				averageMoneyPerSecSamplesIndex = 0;
			double tempSum = 0;
			for (int i = 0; i < averageMoneyPerSecSamples.Length; i++) {
				tempSum += averageMoneyPerSecSamples [i];
			}
			GameCore.averageMoneyPerSec = tempSum / 3;
		}
	}

	public void setMaxSpeed(float value){
		maxSpeed = value;
	}

	public void setAcelSpeed(float value){
		acel = value;
	}

}
