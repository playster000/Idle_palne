using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class SaveInFile : MonoBehaviour{
	
}

[Serializable]
public class GameDetails {

	double money,rp;
	int area;
	bool[] areasUnlocked;
	float energy,sensorEnergy,maxSpeedEnergy,accelEnergy,sideSpeedEnergy;
	double rpPerSec;
	int[] upgradesDone;
	bool[] buttonUnlocked;
	long startMoneyTimer;
	double averageMoneyPerSec;
	bool storyStarted;
	bool storyEnded;
	float profitsMulti;
	public double saves;
	public int fileNum;

	public GameDetails(){
		storyStarted = GameCore.storyStarted;
		storyEnded = GameCore.storyEnded;
		money = GameCore.getRpMoney(1);
		rp = GameCore.getRpMoney(0);
		area = GameCore.getArea ();
		areasUnlocked = GameCore.areasUnlocked;
		energy = Lab.getEnergy(0);
		sensorEnergy = Lab.getEnergy(1);
		maxSpeedEnergy = Lab.getEnergy(2);
		accelEnergy = Lab.getEnergy(3);
		sideSpeedEnergy= Lab.getEnergy(4);
		rpPerSec = Research.getRpPerSec();
		upgradesDone = Research.upgradesDone;
		buttonUnlocked = Research.buttonUnlocked;
		startMoneyTimer = GameCore.startMoneyTimer;
		averageMoneyPerSec = GameCore.averageMoneyPerSec;
		profitsMulti = GameCore.getProfitsMulti();
		saves = SaveToFile.saves;
		fileNum = SaveToFile.fileNum;
	
	}

	public void load(){
		GameCore.addMoney (money-GameCore.getRpMoney(1));
		GameCore.addRp (rp-GameCore.getRpMoney(0));
		GameCore.setArea (area);
		GameCore.areasUnlocked = areasUnlocked;
		Lab.setEnergy (0,energy);
		Lab.setEnergy (1,sensorEnergy);
		Lab.setEnergy (2,maxSpeedEnergy);
		Lab.setEnergy (3,accelEnergy);
		Lab.setEnergy (4,sideSpeedEnergy);
		Research.setRpPerSec (rpPerSec);
		Research.upgradesDone = upgradesDone;
		Research.buttonUnlocked = buttonUnlocked;
		GameCore.startMoneyTimer = startMoneyTimer;
		GameCore.averageMoneyPerSec = averageMoneyPerSec;
		GameCore.storyStarted = storyStarted;
		GameCore.storyEnded = storyEnded;
		GameCore.setProfitsMulti(profitsMulti);
		SaveToFile.saves = saves;
		SaveToFile.fileNum = fileNum;
	}
	public void reset(){
		SaveToFile.saves = 0;
		GameCore.addMoney (-GameCore.getRpMoney(1));
		GameCore.addRp (-GameCore.getRpMoney(0));
		GameCore.setArea (1);
		GameCore.areasUnlocked = new bool[areasUnlocked.Length];
		GameCore.areasUnlocked [0] = true;
		Lab.setEnergy (0,40);
		Lab.setEnergy (1,10);
		Lab.setEnergy (2,10);
		Lab.setEnergy (3,10);
		Lab.setEnergy (4,10);
		Research.setRpPerSec (1);
		Research.upgradesDone = new int[upgradesDone.Length];
		Research.buttonUnlocked = new bool[buttonUnlocked.Length];
		Research.buttonUnlocked [0] = true;
		GameCore.startMoneyTimer = (System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond);
		GameCore.averageMoneyPerSec = 20;
		GameCore.storyStarted = false;
		GameCore.storyEnded = false;

		float tempProfitsMulti = PlayerPrefs.GetFloat("profitsMulti");
		PlayerPrefs.DeleteAll ();
		PlayerPrefs.SetFloat("profitsMulti",tempProfitsMulti);
	}
}



public class SaveToFile {

	public static int fileNum = 0;
	public static double saves = 0;
	public static string dataPath =Application.persistentDataPath + "/GameDetails"+fileNum.ToString()+".dat";//string.Format("{0}/GameDetails.dat",Application.persistentDataPath);
	static GameDetails gameDetails = null;
	static bool fileCheckRunning;
	public static bool saving;
	public static bool readyToSave = false;
	public static bool usePlayerPrefs = false;

	public static IEnumerator save(bool forceSave){
		if (!usePlayerPrefs) {
			if ((readyToSave && !saving) || forceSave) {
				saving = true;
				yield return new WaitForSeconds (60);
				saves++;
				GameDetails gameDetails = new GameDetails ();
				BinaryFormatter binaryFormater = new BinaryFormatter ();
				FileStream fileStream;

				if (File.Exists (dataPath)) {
					File.WriteAllText (dataPath, String.Empty);
					fileStream = File.Open (dataPath, FileMode.Open);
				} else {
					fileStream = File.Create (dataPath);
				}
				//Debug.Log ("saving");

				binaryFormater.Serialize (fileStream, gameDetails);
				fileStream.Close ();
				if (File.Exists (dataPath)) {
					binaryFormater = new BinaryFormatter ();
					fileStream = File.Open (dataPath, FileMode.Open);

					saves = 0;
					gameDetails = null;
					gameDetails = (GameDetails)binaryFormater.Deserialize (fileStream);
					fileStream.Close ();
					saves = gameDetails.saves;
				}
				saving = false;
				forceSave = false;
				savePrefs ();
			}
		} else {
			yield return new WaitForSeconds (2);
			savePrefs ();
		}
	}
	public static GameDetails load(){

		if (!usePlayerPrefs) {
			if (File.Exists (dataPath)) {
				BinaryFormatter binaryFormater = new BinaryFormatter ();
				FileStream fileStream = File.Open (dataPath, FileMode.Open);

				gameDetails = null;
				gameDetails = (GameDetails)binaryFormater.Deserialize (fileStream);
				fileStream.Close ();
				gameDetails.load ();
				loadPrefs ();
				readyToSave = true;
			} else {
				loadPrefs ();
				readyToSave = true;
				save (false);
			}
			return gameDetails;
		} else {
			loadPrefs ();
			return null;
		}
	}
	public static IEnumerator fileCheck(){

		if (!usePlayerPrefs) {
			if (!fileCheckRunning && !readyToSave) {
				fileCheckRunning = true;
				yield return new WaitForSeconds (0.5f);
				while (File.Exists (dataPath) && gameDetails == null && !readyToSave) {//file corrupted
					fileNum++;
					dataPath = Application.persistentDataPath + "/GameDetails" + fileNum.ToString () + ".dat";
					//Debug.Log (fileNum + " " + (gameDetails == null));
					load ();
					yield return new WaitForSeconds (0.5f);
					if (fileNum != gameDetails.fileNum) {
						fileNum = gameDetails.fileNum;
						dataPath = Application.persistentDataPath + "/GameDetails" + fileNum.ToString () + ".dat";
						load ();
					}
				}
				fileCheckRunning = false;
			}
		}
	}

	public static void savePrefs(){

		PlayerPrefs.SetInt("areasUnlocked.Length",GameCore.areasUnlocked.Length);
		for(int i=0;i<GameCore.areasUnlocked.Length;i++){
			PlayerPrefs.SetString("areasUnlocked"+i,GameCore.areasUnlocked[i].ToString());
		}
		PlayerPrefs.SetInt("upgradesDone.Length",Research.upgradesDone.Length);
		for(int i=0;i<Research.upgradesDone.Length;i++){
			PlayerPrefs.SetString("upgradesDone"+i,Research.upgradesDone[i].ToString());
		}
		PlayerPrefs.SetInt("buttonUnlocked.Length",Research.buttonUnlocked.Length);
		for(int i=0;i<Research.buttonUnlocked.Length;i++){
			PlayerPrefs.SetString("buttonUnlocked"+i,Research.buttonUnlocked[i].ToString());
		}

		PlayerPrefs.SetString("storyStarted",GameCore.storyStarted.ToString());
		PlayerPrefs.SetString("storyEnded",GameCore.storyEnded.ToString());
		PlayerPrefs.SetString("money",GameCore.getRpMoney(1).ToString());
		PlayerPrefs.SetString("rp",GameCore.getRpMoney(0).ToString());
		PlayerPrefs.SetString("area",GameCore.getArea().ToString());
		PlayerPrefs.SetFloat("energy",Lab.getEnergy(0));
		PlayerPrefs.SetFloat("sensorEnergy",Lab.getEnergy(1));
		PlayerPrefs.SetFloat("maxSpeedEnergy",Lab.getEnergy(2));
		PlayerPrefs.SetFloat("accelEnergy",Lab.getEnergy(3));
		PlayerPrefs.SetFloat("sideSpeedEnergy",Lab.getEnergy(4));
		PlayerPrefs.SetString("area",GameCore.getArea().ToString());
		PlayerPrefs.SetString("rpPerSec",Research.getRpPerSec().ToString());
		PlayerPrefs.SetString("startMoneyTimer",GameCore.startMoneyTimer.ToString());
		PlayerPrefs.SetString("averageMoneyPerSec",GameCore.averageMoneyPerSec.ToString());
		PlayerPrefs.SetFloat("profitsMulti",GameCore.getProfitsMulti());
		PlayerPrefs.Save();
	}
	public static void loadPrefs(){
		if (PlayerPrefs.HasKey ("money")) {
			bool[] tempareasUnlocked = new bool[PlayerPrefs.GetInt ("areasUnlocked.Length")];
			for (int i = 0; i < tempareasUnlocked.Length; i++) {
				tempareasUnlocked [i] = Boolean.Parse (PlayerPrefs.GetString ("areasUnlocked" + i));
			}
			GameCore.areasUnlocked = tempareasUnlocked;
			int[] tempupgradesDone = new int[PlayerPrefs.GetInt ("upgradesDone.Length")];
			for (int i = 0; i < tempupgradesDone.Length; i++) {
				tempupgradesDone [i] = Int32.Parse (PlayerPrefs.GetString ("upgradesDone" + i));
			}
			Research.upgradesDone = tempupgradesDone;
			bool[] tempbuttonUnlocked = new bool[PlayerPrefs.GetInt ("buttonUnlocked.Length")];
			for (int i = 0; i < tempbuttonUnlocked.Length; i++) {
				tempbuttonUnlocked [i] = Boolean.Parse (PlayerPrefs.GetString ("buttonUnlocked" + i));
			}
			Research.buttonUnlocked = tempbuttonUnlocked;

			GameCore.addMoney ((Double.Parse (PlayerPrefs.GetString ("money"))) - GameCore.getRpMoney (1));
			GameCore.addRp ((Double.Parse (PlayerPrefs.GetString ("rp"))) - GameCore.getRpMoney (0));
			GameCore.setArea ((Int32.Parse (PlayerPrefs.GetString ("area"))));
			Lab.setEnergy (0, PlayerPrefs.GetFloat ("energy"));
			Lab.setEnergy (1, PlayerPrefs.GetFloat ("sensorEnergy"));
			Lab.setEnergy (2, PlayerPrefs.GetFloat ("maxSpeedEnergy"));
			Lab.setEnergy (3, PlayerPrefs.GetFloat ("accelEnergy"));
			Lab.setEnergy (4, PlayerPrefs.GetFloat ("sideSpeedEnergy"));
			Research.setRpPerSec ((Double.Parse (PlayerPrefs.GetString ("rpPerSec"))));
			GameCore.startMoneyTimer = (long.Parse (PlayerPrefs.GetString ("startMoneyTimer")));
			GameCore.averageMoneyPerSec = (Double.Parse (PlayerPrefs.GetString ("averageMoneyPerSec")));
			GameCore.storyStarted = (Boolean.Parse (PlayerPrefs.GetString ("storyStarted")));
			GameCore.storyEnded = (Boolean.Parse (PlayerPrefs.GetString ("storyEnded")));
			GameCore.setProfitsMulti(PlayerPrefs.GetFloat ("profitsMulti"));
			SaveToFile.saves = saves;
			SaveToFile.fileNum = fileNum;
		}
	}

}

