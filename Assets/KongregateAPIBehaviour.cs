using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using LitJson;

public class KongregateAPIBehaviour : MonoBehaviour {
	private static KongregateAPIBehaviour instance;
	public static parseJSON parsejson;
	private static bool startDn = false;
	private static string url;

	public void Update(){
		if(startDn){
			startDn = false;
			WWW www = new WWW(url);
			StartCoroutine(DownloadJson(www));
		}
	}

	public void Start() {
		if(instance == null) {
			instance = this;
		} else if(instance != this) {
			Destroy(gameObject);
			return;
		}

		Object.DontDestroyOnLoad(gameObject);
		gameObject.name = "KongregateAPI";

		Application.ExternalEval(
			@"if(typeof(kongregateUnitySupport) != 'undefined'){
        		kongregateUnitySupport.initAPI('KongregateAPI', 'OnKongregateAPILoaded');
      		};"
    	);

	}

	public void OnKongregateAPILoaded(string userInfoString) {
		OnKongregateUserInfo(userInfoString);
		/*Application.ExternalEval(@"
      		kongregate.services.addEventListener('login', function(){
        	var unityObject = kongregateUnitySupport.getUnityObject();
        	var services = kongregate.services;
        	var params=[services.getUserId(), services.getUsername(), 
                    services.getGameAuthToken()].join('|');

        	unityObject.SendMessage('KongregateAPI', 'OnKongregateUserInfo', params);
	    	});"
	  	);*/
	}  

	public void OnKongregateUserInfo(string userInfoString) {
		var info = userInfoString.Split('|');
		var userId = System.Convert.ToInt32(info[0]);
		//NetworkManager.clientID = (int)userId;
		var username = info[1];
		//getPlayerName.playerName = username.ToString();
		//PhotonNetwork.playerName = username.ToString();
		var gameAuthToken = info[2];
		//NetworkManager.connectionText.text = "Kongregate User Info: " + username + ", userId: " + userId;
	}
	public static void sumbitScore(string name,int value){
		Application.ExternalCall("kongregate.stats.submit", name, value);
	}

	IEnumerator DownloadJson(WWW www)//string url)
	{

		//WWW www = new WWW(url);
		yield return www;
		if (www.error == null)
		{
			//NetworkManager.connectionText.text = www.text;
			Processjson(www.text);
		}
		else
		{
			//NetworkManager.connectionText.text = "ERROR: " + www.error;
			Debug.Log("ERROR: " + www.error);
		}        
	}   
	private void Processjson(string jsonString)
	{
		/*JsonData jsonvale = JsonMapper.ToObject(jsonString);
		parsejson = new parseJSON();
		//parsejson.title = jsonvale["title"].ToString();
		//parsejson.id = jsonvale["ID"].ToString();

		parsejson.username = new ArrayList ();
		parsejson.score = new ArrayList ();

		for(int i = 0; i<jsonvale["weekly_scores"].Count; i++)
		{
			parsejson.username.Add(jsonvale["weekly_scores"][i]["username"].ToString());
			parsejson.score.Add((int)jsonvale["weekly_scores"][i]["score"]);
		}    */
	}
	public static void DnJson(string urlToDn){
		if (urlToDn == null || urlToDn == "") {
			urlToDn = "http://api.kongregate.com/api/high_scores/weekly/130211.json";
		}
		url = urlToDn;
		startDn = true;
	}
}
public class parseJSON
{
	//public string title;
	//public string id;
	public ArrayList username;
	public ArrayList score;
}