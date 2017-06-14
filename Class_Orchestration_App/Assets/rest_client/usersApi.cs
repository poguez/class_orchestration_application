using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using ClassOrchestrationApi;
using SimpleJSON;


//
// Unity Web Request
//
// https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html

public class UsersApi : MonoBehaviour {
    ClassOrchestrationApi.RestClient myclient;
    ClassOrchestrationApi.ActionWWW getUsersCallback = GetUsersCallback;
    ClassOrchestrationApi.ActionWWW getExplorationCallback = GetExplorationCallback;
    ClassOrchestrationApi.ActionWWW createUserCallback = CreateUserCallback;
    ClassOrchestrationApi.ActionWWW createExplorationCallback = CreateExplorationCallback;
    ClassOrchestrationApi.ActionWWW createExplorationObjectCallback = CreateExplorationObjectCallback;
    ClassOrchestrationApi.ActionWWW createEventCallback = CreateEventCallback;
    ClassOrchestrationApi.ActionWWW getExplorationObjectStateCallback = GetExplorationObjectStateCallback;
    ClassOrchestrationApi.ActionWWW getEventCallback = GetEventCallback;
    
    string base_url = "http://miri.noedominguez.com:9000";
    //string base_url = "http://localhost:9000";
    static string all_users_json;
    static User myUser = new User();
    static Exploration myExploration = new Exploration();
    static ExplorationObject myExplorationObject = new ExplorationObject();
    static ExplorationEventManager myExplorationEventManager = new ExplorationEventManager();
    //static var myExplorationEvent;

    static Vector3 myPosition;
    static Quaternion myRotation;
    static Vector3 myScale;
    static int myModelType;

    bool isStudent = false;
    bool exploration = false;
    bool teacherExplorationObject = false;
    int oldObjectId = -1;
    int currentObjectId;

    public Text teamID;
    public GameObject model;

    public GameObject explorationModeInfo;

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

        public string toString() {
            return "user_id: " + user_id + ", " + "name: " + username + ", " + "isAdmin: " +
                isAdmin + ", " + "teamId: " + teamId;
        }
    }

    [Serializable]
    public class Exploration
    {
        public int explorationId;
        public int teacherId;
        public int explorationObjectId;
        public int explorationMode = 0; // 0: guided, 1: individual, 2: group
        public string toString() {
            return "id: " + explorationId + ", " + "teacherId: " + teacherId + ", " +
                "explorationObjectId: " + explorationObjectId;
        }
    }

    [Serializable]
    public class ExplorationObject
    {
        public int explorationObjectId = -1;
        public string objectState;
        public string modelName; // "dinosaur", "desk" or "dna"
        public string toString() {
            return "id: \"" + explorationObjectId + "\", \"" + "objectState\": \"" + objectState +
                "\" \"modelName\": \"" + modelName + "\"";
        }
    }

    public class ExplorationEventManager
    {
        public int newestEventId;
        public int lastProcessedEventId = -1;
        public Queue<UsersApi.ExplorationEvent> awaitingProcessEventsQueue = new Queue<UsersApi.ExplorationEvent>();
        public void insertToQueue(UsersApi.ExplorationEvent eventForQueue) {
            myExplorationEventManager.awaitingProcessEventsQueue.Enqueue(eventForQueue);
        }

    }
    public class ExplorationEvent
    {
        public int explorationEventId;
        public string name;
        public string description;
        public string toString() {
            return "id: \"" + explorationEventId + "\", " + "name: \"" + name +
                "\" description: \"" + description + "\"";
        }
    }



    /*
	 * 
	 * Default Monobehavior 
	 * 
	 * 
	 */



    void Start() {
        //Assign the Rest client to an object to use ClassOrchestrationApi;
        myclient = (new GameObject("RESTclient")).AddComponent<RestClient>();

        // Examples
        //getUsers ().ToString ();
        //createUser (); // create a student
        //createUser ("professor").ToString(); // create a professor
        //changeUserTeam(20, 14);
        //createExplorationObject ();
        //createExploration (2,19);
        //createExplorationEvent ("changeExplorationObject", "This is a dummy description");
    }

    void Update() {
        currentObjectId = myExplorationObject.explorationObjectId;

        if (myUser.isAdmin && !exploration)
        {
            createExploration(myUser.user_id);
            exploration = true;
            model.GetComponent<Model>().student = false;
        } else if (isStudent && !exploration)
        {
            // get newest entry for exploration
            getExplorations();
            exploration = true;
        }

        if (myUser.isAdmin && myExplorationObject.explorationObjectId != -1 && !teacherExplorationObject)
        {
            teacherExplorationObject = true;
            createExplorationObject();
            currentObjectId = myExplorationObject.explorationObjectId;
        }

        if (exploration && teacherExplorationObject && (oldObjectId != currentObjectId))
        {
            oldObjectId = currentObjectId;
            updateExplorationObject(myExploration.explorationId, myExplorationObject.explorationObjectId);
            createExplorationEvent("setExplorationObject", myExplorationObject.explorationObjectId.ToString());
        }

        // teacher updates teacher's object

        string modelName = "";
        if (model.GetComponent<Model>().modelID == 0)
        {
            modelName = "desk";
        } else if (model.GetComponent<Model>().modelID == 1)
        {
            modelName = "dinosaur";
        } else
        {
            modelName = "dna";
        }

        if (myUser.isAdmin && teacherExplorationObject &&
            (myExplorationObject.modelName.Equals(modelName)))
        {
            string parameters = "";
            parameters = model.GetComponent<Model>().model.transform.position.ToString("F4");
            parameters += ";";
            parameters += model.GetComponent<Model>().model.transform.rotation.ToString("F4");
            parameters += ";";
            parameters += model.GetComponent<Model>().model.transform.localScale.ToString("F4");

            updateObject(myExplorationObject.explorationObjectId, parameters);
        }

        // group members update group object

        // guided exploration
        if (myExploration.explorationMode == 0 && !myUser.isAdmin && exploration)
        {
            model.GetComponent<Model>().setModel(myModelType);

            getExplorationObjectState(myExploration.explorationObjectId);
            model.GetComponent<Model>().setPosition(myPosition);
            model.GetComponent<Model>().setRotation(myRotation);
            model.GetComponent<Model>().setScale(myScale);
        }


        if (myExplorationEventManager.awaitingProcessEventsQueue.Count > 0)
        {
            ExplorationEvent ev = myExplorationEventManager.awaitingProcessEventsQueue.Dequeue();
            if (ev.name.Equals("setExplorationObject"))
            {

            } else
            {

            }
        }
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

    // get events
    public void getEvents()
    {
        string put_url = base_url + "/v1/exploration-event/";
        UnityWebRequest reponse = myclient.GET(put_url, GetEventCallback);
    }

    // get object state
    public string getExplorationObjectState(int objectId)
    {
        string objectState = "";

        string put_url = base_url + "/v1/exploration-object/" + objectId;
        UnityWebRequest reponse = myclient.GET(put_url, GetExplorationObjectStateCallback);

        return objectState;
    }

    // Set exploration mode
    public void setExplorationMode(string mode)
    {
        explorationModeInfo.GetComponentInChildren<Text>().text = mode;
        // guided
        if (mode.Equals("Guided")) {
            model.GetComponent<Model>().gameMode = 0;
            explorationModeInfo.GetComponent<Image>().color =
                new Color32(0, 100, 255, 100);
            updateExplorationMode(myExploration.explorationId, 0);
        }
        // individual
        else if (mode.Equals("Indiv."))
        {
            model.GetComponent<Model>().gameMode = 1;
            explorationModeInfo.GetComponent<Image>().color =
                new Color32(255, 0, 100, 100);
            updateExplorationMode(myExploration.explorationId, 1);
        }
        // group
        else
        {
            model.GetComponent<Model>().gameMode = 2;
            explorationModeInfo.GetComponent<Image>().color =
                new Color32(100, 255, 0, 100);
            updateExplorationMode(myExploration.explorationId, 2);
        }
    }

    public void updateObject(int objectId, string objectState)
    {
        string objectStateParam = objectState;
        string put_url = base_url + "/v1/exploration-object/" + objectId;
        string requestBodyJsonString = "{\"objectState\": \"" + objectStateParam + "\" }";
        UnityWebRequest reponse = myclient.POST(put_url, requestBodyJsonString, createExplorationObjectCallback);
        return;// reponse.downloadHandler.text;
    }

	// Get all the users
	public string getUsers(){
		string get_url = base_url + "/v1/user/";
		UnityWebRequest reponse = myclient.GET(get_url, getUsersCallback);
		return reponse.downloadHandler.text;
	}

    // Get all the explorations
    public string getExplorations()
    {
        string get_url = base_url + "/v1/exploration/";
        UnityWebRequest reponse = myclient.GET(get_url, getExplorationCallback);
        return reponse.downloadHandler.text;
    }


    // Create a user (userType: "professor" or "student by default")
    public void createUser(string userType = "student"){
		string isAdmin = "false";
		if (userType == "professor") {
			isAdmin = "true";
        }
        else
        {
            isStudent = true;
        }
		string put_url = base_url + "/v1/user/new";
		string requestBodyJsonString = 
			"{\"name\": \"Google Cardboard User\", \"password\": \"new\", \"isAdmin\":" + 
			isAdmin + ", \"teamId\": 1 }";
        UnityWebRequest reponse = myclient.POST(put_url, requestBodyJsonString, createUserCallback);

		return;// reponse.downloadHandler.text;
	}

	// Change the team of a User.
	// STUDENT
	public void changeUserTeam(){
		string put_url = base_url + "/v1/user/"+ myUser.user_id ;
		string requestBodyJsonString = "{\"teamId\":" + teamID.text + "}";
		UnityWebRequest reponse = myclient.POST(put_url, requestBodyJsonString, createUserCallback);
		return;// reponse.downloadHandler.text;
	}

	/*
	* Explorations Api calls
	*/

	// Create a Exploration
	// TEACHER
	public void createExploration(int teacherId, int explorationObjectId = -1){
        int teacher = teacherId;
        string teacherIdParam = teacher.ToString();
		string explorationObjectIdParam = explorationObjectId.ToString();
		if (teacher == -1)
		{
			teacherIdParam = "null";
		}
		if (explorationObjectId == -1)
		{
			explorationObjectIdParam = "null";
		}

		string put_url = base_url + "/v1/exploration/new";
		string requestBodyJsonString = "{\"teacherId\":" + teacherIdParam + ", " + 
			"\"explorationObjectId\" :" +  explorationObjectIdParam + "}";
        UnityWebRequest reponse = myclient.POST(put_url, requestBodyJsonString, createExplorationCallback);
        return;// reponse.downloadHandler.text;
	}

    public void updateExplorationMode(int explorationId, int explorationMode)
    {
        string put_url = base_url + "/v1/exploration/" + explorationId;
        string requestBodyJsonString = "{\"explorationMode\":" + explorationMode + "}";
        UnityWebRequest response = myclient.POST(put_url, requestBodyJsonString, createExplorationCallback);
    }

    public string updateExplorationObject(int explorationId, int explorationObjectId)
    {
        string put_url = base_url + "/v1/exploration/" + explorationId;
        string requestBodyJsonString = "{\"explorationObjectId\" :" + explorationObjectId + "}";
        UnityWebRequest reponse = myclient.POST(put_url, requestBodyJsonString, createExplorationCallback);
        return reponse.downloadHandler.text;
    }

    /*
	* 	ExplorationObjects Api calls
	* 
	*/

    // TEACHER
    public void createExplorationObject(string objectState = ""){
		int id = model.GetComponent<Model>().modelID;

		// modelName: "dinosaur", "desk" or "dna"
		string modelName;
		if (id == 0)
		{
			modelName = "desk";
		}
		else if (id == 1)
		{
			modelName = "dinosaur";
		}
		else
		{
			modelName = "dna";

		}

        if (myExploration.explorationObjectId != myExplorationObject.explorationObjectId)
        {
            Debug.Log("Create object");
            string objectStateParam = objectState;
            string modelNameParam = modelName;
            string put_url = base_url + "/v1/exploration-object/new";
            string requestBodyJsonString = "{\"objectState\": \"" + objectStateParam + "\" , " +
                "\"modelName\" : \"" + modelNameParam + "\" }";
            UnityWebRequest reponse = myclient.POST(put_url, requestBodyJsonString, createExplorationObjectCallback);
            return;// reponse.downloadHandler.text;
        } else
        {
            Debug.Log("Update object");
            string objectStateParam = objectState;
            string modelNameParam = modelName;
            string put_url = base_url + "/v1/exploration-object/" + myExplorationObject.explorationObjectId;
            string requestBodyJsonString = "{\"objectState\": \"" + objectStateParam + "\" , " +
                "\"modelName\" : \"" + modelNameParam + "\" }";
            UnityWebRequest reponse = myclient.POST(put_url, requestBodyJsonString, createExplorationObjectCallback);
            return;// reponse.downloadHandler.text;
        }
	}

	/*
	* Explorations Api calls
	*/

	// Create a Exploration Event (name, description)
	public void createExplorationEvent(string name, string description){
		string put_url = base_url + "/v1/exploration-event/new";
		string requestBodyJsonString = 
			"{\"name\": \"" + name + "\", " + "\"description\": \"" + description+"\" }";
		UnityWebRequest reponse = myclient.POST(put_url, requestBodyJsonString, createEventCallback);
		return;// reponse.downloadHandler.text;
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
		myExplorationObject.explorationObjectId = result ["id"].AsInt;
		myExplorationObject.objectState = result ["objectState"];
		myExplorationObject.modelName = result ["modelName"];
		Debug.Log ("ExplorationObject object print:");
		Debug.Log ( myExplorationObject.toString() );
	}


	public static void CreateEventCallback(UnityWebRequest www)
	{	
		var result = JSON.Parse(www.downloadHandler.text);
		UsersApi.ExplorationEvent thisEvent = new ExplorationEvent ();
		thisEvent.explorationEventId = result ["id"];
		thisEvent.name = result ["name"];
		thisEvent.description = result ["description"];
		//Add the evento to the Exploration Event Manager
		myExplorationEventManager.newestEventId = thisEvent.explorationEventId;
		myExplorationEventManager.insertToQueue (thisEvent);
		//Log the Enqueued Event
		Debug.Log ("Exploration Event Enqueued print:");
		Debug.Log ( thisEvent.toString() );
	}

	public static void GetUsersCallback(UnityWebRequest www)
	{	
		all_users_json = www.downloadHandler.text;
		Debug.Log ("All users JSON response:");
		Debug.Log ( all_users_json );
	}

    public static void GetExplorationCallback(UnityWebRequest www)
    {
        string all_explorations_json = www.downloadHandler.text;
        Debug.Log("All users JSON response:");
        Debug.Log(all_explorations_json);
        var result = JSON.Parse(all_explorations_json);

        myExploration.explorationId = result[result.Count - 1]["id"].AsInt;
        myExploration.teacherId = result[result.Count - 1]["teacherId"].AsInt;
        myExploration.explorationObjectId = result[result.Count - 1]["explorationObjectId"].AsInt;
        myExploration.explorationMode = result[result.Count - 1]["explorationMode"].AsInt;
    }

    public static void GetExplorationObjectStateCallback(UnityWebRequest www)
    {
        string object_state_json = www.downloadHandler.text;
        Debug.Log("Object state: ");
        Debug.Log(object_state_json);
        var result = JSON.Parse(object_state_json);
        string objectState = result["objectState"];
        objectState = objectState.Replace("(", "");
        objectState = objectState.Replace(")", "");
        string[] transforms = objectState.Split(';');
        string[] pos = transforms[0].Split(',');
        Vector3 position;
        position = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
        myPosition = position;

        string[] rot = transforms[1].Split(',');
        Quaternion rotation;
        rotation = new Quaternion(
            float.Parse(rot[0]), float.Parse(rot[1]), float.Parse(rot[2]), float.Parse(rot[3]));
        myRotation = rotation;

        string[] sca = transforms[2].Split(',');
        Vector3 scale;
        scale = new Vector3(float.Parse(sca[0]), float.Parse(sca[1]), float.Parse(sca[2]));
        myScale = scale;

        if (result["modelName"].Equals("desk"))
        {
            myModelType = 0;
        } else if (result["modelName"].Equals("dinosaur"))
        {
            myModelType = 1;
        } else
        {
            myModelType = 2;
        }
    }

    public static void GetEventCallback(UnityWebRequest www)
    {
        string events_json = www.downloadHandler.text;
        Debug.Log("Object state: ");
        Debug.Log(events_json);
        var result = JSON.Parse(events_json);
        int last = result[result.Count - 1]["id"].AsInt;
        myExplorationEventManager.newestEventId = last;

        if (last > myExplorationEventManager.lastProcessedEventId)
        {
            // put in queue
            for (int i = 0; i < last - myExplorationEventManager.lastProcessedEventId; i++)
            {
                ExplorationEvent ev = new ExplorationEvent();
                ev.explorationEventId = result[result.Count - 1 - i]["id"].AsInt;
                ev.name = result[result.Count - 1 - i]["name"];
                ev.description = result[result.Count - 1 - i]["description"];
                myExplorationEventManager.insertToQueue(ev);
            }
        }
    }
}