using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;


namespace ClassOrchestrationApi
{
	public delegate void Action(string text);
	public delegate void ActionWWW(UnityWebRequest www);

	class RestClient : MonoBehaviour
	{
		public UnityWebRequest GET(string url, ActionWWW onComplete ) {
			UnityWebRequest www = UnityWebRequest.Get(url);
			StartCoroutine (WaitForRequest (www, onComplete));
			return www;
		}

		public UnityWebRequest POST(string url, string requestBodyJsonString, ActionWWW onComplete){
			UnityWebRequest www;
			www = UnityWebRequest.Put(url, requestBodyJsonString);
			www.SetRequestHeader("accept", "application/json; charset=UTF-8");
			www.SetRequestHeader("cache-control", "no-cache");
			www.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
			StartCoroutine (WaitForRequest (www, onComplete));
			return www;
		}

		IEnumerator WaitForRequest(UnityWebRequest www, ActionWWW onComplete )
		{
			yield return www.Send ();
			if (www.error == null) {
				onComplete(www);
			} else {
				Debug.Log (www.error);
			}
		}
	}
}