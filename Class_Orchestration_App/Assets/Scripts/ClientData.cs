using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientData : MonoBehaviour {

    public int userID = -1; // default -1: has not been assigned an ID by the server yet
    public int teamID = 1; // default 1: main team, everyone is in there at the beginning

    /* 
     * 
        0: guided exploration
        1: individual exploration
        2: group exploration
     * 
     */
    public int explorationType = 0;

    public bool isAdmin = false;

    public Text joinTeamID;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setTeacher()
    {
        isAdmin = true;
    }

    public void joinTeam()
    {
        int id = int.Parse(joinTeamID.text);
        teamID = id;
    }

    public void setGuided()
    {
        explorationType = 0;
    }

    public void setIndividual()
    {
        explorationType = 1;
    }

    public void setGroup()
    {
        explorationType = 2;
    }
}
