using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ClassOrchestrationApi;
using SimpleJSON;

//
// Unity Web Request
//
// https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html

public class UsersApi : MonoBehaviour {
	ClassOrchestrationApi.RestClient myclient;
	ClassOrchestrationApi.ActionWWW getUsersCallback = GetUsersCallback;
	ClassOrchestrationApi.ActionWWW createUserCallback = CreateUserCallback;
	ClassOrchestrationApi.ActionWWW createExplorationCallback = CreateExplorationCallback;
	ClassOrchestrationApi.ActionWWW createExplorationObjectCallback = CreateExplorationObjectCallback;
	string base_url = "http://miri.noedominguez.com:9000";
	//string base_url = "http://localhost:9000";
	static string all_users_json;
	static User myUser = new User();
	static Exploration myExploration = new Exploration();
	static ExplorationObject myExplorationObject = new ExplorationObject();



	/*
	 * 
	 * Classes to create objects that save state
	 * 
	 * 
	 */
	[Serializable]
	public class User
	{
		public int user_id;
		public string username;
		public bool isAdmin;
		public int teamId;

		public string toString(){
			return "user_id: " + user_id + ", " + "name: " + username + ", " + "isAdmin: " + isAdmin + ", " + "teamId: " + teamId;
		}
	}

	[Serializable]
	public class Exploration
	{
		public int explorationId;
		public int teacherId;
		public int explorationObjectId;
		public string toString(){
			return "id: "+ explorationId + ", " + "teacherId: " + teacherId + ", " + "explorationObjectId: " + explorationObjectId;
		}
	}

	[Serializable]
	public class ExplorationObject
	{
		public int explorationObjectId;
		public string objectState;
		public string modelName; // "dinosaur", "desk" or "dna"
		public string toString(){
			return "id: " + explorationObjectId + ", " + "objectState: " + objectState + "modelName: " + modelName;
		}
	}

	/*
	 * 
	 * Default Monobehavior 
	 * 
	 * 
	 */



	void Start () {
		//Assign the Rest client to an object to use ClassOrchestrationApi;
		myclient = (new GameObject("RESTclient")).AddComponent<RestClient>();

		// Examples
		//getUsers ().ToString ();
		//createUser ().ToString(); // create a student
		//createUser ("professor").ToString(); // create a professor
		//changeUserTeam(20, 14);
		//createExplorationObject ();
		//createExploration (2,19);
	}
	
	void Update () {
		
	}

	/*
	 * 
	 * API calls to the server.
	 * 
	 * 
	 */


	/*
	*	Users and Groups Api calls
	*/

	// Get all the users
	public string getUsers(){
		string get_url = base_url + "/v1/user/";
		UnityWebRequest reponse = myclient.GET(get_url, getUsersCallback);
		return reponse.downloadHandler.text;
	}

	// Create a user (userType: "professor" or "student by default")
	public string createUser(string userType = "student"){
		string isAdmin = "false";
		if (userType == "professor") {
			isAdmin = "true";
		}
		string put_url = base_url + "/v1/user/new";
		string requestBodyJsonString = "{\"name\": \"Google Cardboard User\", \"password\": \"new\", \"isAdmin\":" + isAdmin + ", \"teamId\": 1 }";
		UnityWebRequest reponse = myclient.POST(put_url, requestBodyJsonString, createUserCallback);
		return reponse.downloadHandler.text;
	}

	// Change the team of a User.
	public string changeUserTeam(int userId, int teamId){
		string put_url = base_url + "/v1/user/"+ userId ;
		string requestBodyJsonString = "{\"teamId\":" +teamId + "}";
		UnityWebRequest reponse = myclient.POST(put_url, requestBodyJsonString, createUserCallback);
		return reponse.downloadHandler.text;
	}

	/*
	* Explorations Api calls
	*/

	// Create a Exploration
	public string createExploration(int teacherId = -1, int explorationObjectId = -1){
		string teacherIdParam = teacherId.ToString();
		string explorationObjectIdParam = explorationObjectId.ToString();
		if (teacherId == -1)
		{
			teacherIdParam = "null";
		}
		if (explorationObjectId == -1)
		{
			explorationObjectIdParam = "null";
		}

		string put_url = base_url + "/v1/exploration/new";
		string requestBodyJsonString = "{\"teacherId\":" + teacherIdParam + ", " + "\"explorationObjectId\" :" +  explorationObjectIdParam + "}";
		UnityWebRequest reponse = myclient.POST(put_url, requestBodyJsonString, createExplorationCallback);
		return reponse.downloadHandler.text;
	}

	/*
	* 	ExplorationObjects Api calls
	* 
	*/

	public string createExplorationObject(string objectState = "", string modelName = "dinosaur"){
		// modelName: "dinosaur", "desk" or "dna"
		string objectStateParam = objectState;
		string modelNameParam = modelName;
		string put_url = base_url + "/v1/exploration-object/new";
		string requestBodyJsonString = "{\"objectState\": \"" + objectStateParam + "\" , " + "\"modelName\" : \"" +  modelNameParam + "\" }";
		UnityWebRequest reponse = myclient.POST(put_url, requestBodyJsonString, createExplorationObjectCallback);
		return reponse.downloadHandler.text;
	}




	/*
	 * 
	 * Callback functions to be executed when result is received from
	 * the API server.
	 *
	 * Log the UnityWebRequest with these callback
	 *
	 *
	 */


	public static void CreateUserCallback(UnityWebRequest www)
	{	
		var result = JSON.Parse(www.downloadHandler.text);
		myUser.user_id = result ["id"].AsInt;
		myUser.username = result ["name"];
		myUser.isAdmin = result ["isAdmin"];
		myUser.teamId = result ["teamId"].AsInt;
		Debug.Log ("User object print:");
		Debug.Log ( myUser.toString() );
	}

	public static void CreateExplorationCallback(UnityWebRequest www)
	{	
		var result = JSON.Parse(www.downloadHandler.text);
		myExploration.explorationId = result ["id"].AsInt;
		myExploration.teacherId = result ["teacherId"].AsInt;
		myExploration.explorationObjectId = result ["explorationObjectId"].AsInt;
		Debug.Log ("Exploration object print:");
		Debug.Log ( myExploration.toString() );
	}

	public static void CreateExplorationObjectCallback(UnityWebRequest www)
	{	
		var result = JSON.Parse(www.downloadHandler.text);
		myExplorationObject.explorationObjectId = result ["id"];
		myExplorationObject.objectState = result ["objectState"];
		myExplorationObject.modelName = result ["modelName"];
		Debug.Log ("ExplorationObject object print:");
		Debug.Log ( myExplorationObject.toString() );
	}



	public static void GetUsersCallback(UnityWebRequest www)
	{	
		all_users_json = www.downloadHandler.text;
		Debug.Log ("All users JSON response:");
		Debug.Log ( all_users_json );
	}



}
