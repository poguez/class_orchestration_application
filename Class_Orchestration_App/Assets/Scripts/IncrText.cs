using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncrText : MonoBehaviour {

    public Text text;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void increment()
    {
        int iText = int.Parse(text.text);
        if (iText < 10)
        {
            ++iText;
            text.text = iText.ToString();
        }
    }

    public void decrement()
    {
        int iText = int.Parse(text.text);
        if (iText > 1)
        {
            --iText;
            text.text = iText.ToString();
        }
    }
}
