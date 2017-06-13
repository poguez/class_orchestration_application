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
	string base_url = "http://miri.noedominguez.com:9000";
	//string base_url = "http://localhost:9000";
	static string all_users_json;
	static User myUser = new User();

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

	void Start () {
		myclient = (new GameObject("RESTclient")).AddComponent<RestClient>();

		// Examples
		//getUsers ().ToString ();
		createUser ().ToString();
		createUser ("professor").ToString();
		//changeUserTeam(20, 14);
	}
	
	void Update () {
	}


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

	// Log the UnityWebRequest with this callback
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
	public static void GetUsersCallback(UnityWebRequest www)
	{	
		all_users_json = www.downloadHandler.text;
		Debug.Log ("All users JSON response:");
		Debug.Log ( all_users_json );
	}



}
