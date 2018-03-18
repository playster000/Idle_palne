using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lab : MonoBehaviour {

	private GameObject labBtn;
	public GameObject researchBtn;
	private static float energy = 40,sensorEnergy = 10,maxSpeedEnergy = 10,accelEnergy = 10,sideSpeedEnergy = 10;
	private Slider[] sliders = new Slider[4];
	public BgMover bgMover;
	public Fighter fighter;
	public Obstacle obstacle;
	public Sensor sensor;
	private Text energyAllocatedText,upgradeCostTxt;
	private Button upgrade;
	double upgradeCost = 10;

	// Use this for initialization
	void Start () {
		energy = energy / 3;
		sensorEnergy = sensorEnergy / 3;
		maxSpeedEnergy = maxSpeedEnergy / 3;
		accelEnergy = accelEnergy / 3;
		sideSpeedEnergy = sideSpeedEnergy / 3;
		labBtn = GameObject.FindGameObjectWithTag ("labBtn");
		//researchBtn = GameObject.FindGameObjectWithTag ("researchBtn");
		upgrade = GetComponentInChildren<Button> ();
		upgradeCostTxt = upgrade.gameObject.GetComponentInChildren<Text> ();
		for(int i=0;i<sliders.Length;i++){
			sliders [i] = GetComponentsInChildren<Slider> ()[i];
		}
		energyAllocatedText = GetComponentInChildren<Text> ();
		InvokeRepeating ("load",0,0.1f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void load(){
		if (SaveToFile.readyToSave) {
			setUpgradeCost ();
			sliders [0].value = getEnergy(1);
			sliders [1].value = getEnergy(2);
			sliders [2].value = getEnergy(3);
			sliders [3].value = getEnergy(4);
			for(int i=0;i<4;i++){
				setSlider (i);
			}
			CancelInvoke ("load");
		}
	}

	public void setLab(){
		ColorBlock temp = researchBtn.GetComponent<Button> ().colors;
		temp.normalColor = new Color (170 / 255f, 170 / 255f, 170 / 255f, 1);
		researchBtn.GetComponent<Button> ().colors = temp;
		temp = labBtn.GetComponent<Button> ().colors;
		temp.normalColor = new Color (1,1,1, 1);
		labBtn.GetComponent<Button> ().colors = temp;
	}

	public void setSlider(int i){
		float tempEnergyAvailable = getEnergy(0) - (getEnergy(1)+getEnergy(2)+getEnergy(3)+getEnergy(4));
		string tempName = "Energy allocated: ";

		if(i == 0){
			if (sliders [i].value - getEnergy(i+1) <= tempEnergyAvailable) {
				sensor.setSensor (sliders [i].value);
				setEnergy (1,sliders [i].value);
			} else {
				sensor.setSensor (tempEnergyAvailable+getEnergy(1));
				sliders [i].value = (tempEnergyAvailable + getEnergy(1));
				setEnergy (1,sliders [i].value);
			}
			tempName = "Sensor Energy allocated: ";	
		}else if(i == 1){
			if (sliders [i].value - getEnergy(i+1) <= tempEnergyAvailable) {
				bgMover.setMaxSpeed (sliders [i].value);
				setEnergy (i+1,sliders [i].value);
			} else {
				bgMover.setMaxSpeed (tempEnergyAvailable+getEnergy(i+1));
				sliders [i].value = (tempEnergyAvailable + getEnergy(i+1));
				setEnergy (i+1,sliders [i].value);
			}
			tempName = "Max Velocity Energy allocated: ";	
		}else if(i == 2){
			if (sliders [i].value - getEnergy(i+1) <= tempEnergyAvailable) {
				bgMover.setAcelSpeed (sliders [i].value);
				setEnergy (i+1,sliders [i].value);
			} else {
				bgMover.setAcelSpeed (tempEnergyAvailable+getEnergy(i+1));
				sliders [i].value = (tempEnergyAvailable + getEnergy(i+1));
				setEnergy (i+1,sliders [i].value);
			}
			tempName = "Acceleration Energy allocated: ";	
		}else if(i == 3){
			if (sliders [i].value - getEnergy(i+1) <= tempEnergyAvailable) {
				fighter.setSideSpeed (sliders [i].value);
				setEnergy (i+1,sliders [i].value);
			} else {
				fighter.setSideSpeed (tempEnergyAvailable+getEnergy(i+1));
				sliders [i].value = (tempEnergyAvailable + getEnergy(i+1));
				setEnergy (i+1,sliders [i].value);
			}
			tempName = "Maneuver Velocity Energy allocated: ";	
		}
		if(sliders[0].value ==100 && sliders[1].value ==100 
			&& sliders[2].value ==100 && sliders[3].value ==100 && !GameCore.storyEnded){
			GameCore.storyEnded = true;
			Story.showEnd ();
		}
		setEnergyAllocatedText (getEnergy(0) - (getEnergy(1)+getEnergy(2)+getEnergy(3)+getEnergy(4)));
		if(i>=0 && i<sliders.Length)
		sliders [i].GetComponentInChildren<Text> ().text =  tempName + sliders [i].value.ToString();
	}

	public void setEnergyAllocatedText(float energyAlocated){
		energyAllocatedText.text = "Energy to allocate: " + ((int)energyAlocated).ToString ();
	}

	void setUpgradeCost(){
		int level = (int)(getEnergy(0) / 10) - 4;
		upgradeCost = (double)((Mathf.Exp (3 + (0.5f * level)) * 3) - 40);
		upgradeCostTxt.text = GameCore.formatSize (upgradeCost);
	}
	public void upgradePressed(){
		if(upgradeCost<=GameCore.getRpMoney(0)){
			setEnergy (0,getEnergy(0)+10);
			setSlider (-1);
			GameCore.addRp (-upgradeCost);
			setUpgradeCost ();
			if(GameCore.getProfitsMulti()>1)
				KongregateAPIBehaviour.sumbitScore ("maxProfitMult", (int)GameCore.getProfitsMulti());
		}
	}

	public static float getEnergy(int index){
		float toReturn = 0;
		if(index == 0){
			toReturn =  energy * 3;
		}if(index == 1){
			toReturn =  sensorEnergy* 3;
		}if(index == 2){
			toReturn =  maxSpeedEnergy* 3;
		}if(index == 3){
			toReturn =  accelEnergy* 3;
		}if(index == 4){
			toReturn =  sideSpeedEnergy* 3;
		}
		return toReturn;
	}
	public static void setEnergy(int index,float value){
		if(index == 0){
			energy = value;
			energy = energy / 3;
		}if(index == 1){
			sensorEnergy = value;
			sensorEnergy = sensorEnergy / 3;
		}if(index == 2){
			maxSpeedEnergy = value;
			maxSpeedEnergy = maxSpeedEnergy / 3;
		}if(index == 3){
			accelEnergy = value;
			accelEnergy = accelEnergy / 3;
		}if(index == 4){
			sideSpeedEnergy = value;
			sideSpeedEnergy = sideSpeedEnergy / 3;
		}
	}

}
