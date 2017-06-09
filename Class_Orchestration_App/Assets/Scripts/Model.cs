using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour {

    public GameObject[] objects = new GameObject[5];
    public GameObject[] previewModels = new GameObject[5];

    // TODO: retrieve ID from database
    public int modelID;

    private GameObject model;
    private GameObject preview;

    private bool scale = false;
    private bool rotate = false;
    private bool translate = false;

    public float rotSpeed = 100.0f;
    public float transSpeed = 100.0f;

	// Use this for initialization
	void Start () {
        model = objects[modelID];
        //model.SetActive(true);
        preview = previewModels[modelID];
        //model.transform.localPosition = new Vector3(0.0f, 0.0f, 10.0f);
	}

    // TODO: handle input
    public void leftArrow()
    {
        if (modelID == 0)
            modelID = 5;
        modelID = (modelID - 1) % 5;
        previewObject();
    }
    public void rightArrow()
    {
        modelID = (modelID + 1) % 5;
        previewObject();
    }

    public void previewObject()
    {
        preview.SetActive(false);
        preview = previewModels[modelID];
        preview.SetActive(true);
        //preview.transform.localPosition = new Vector3(0.0f, 0.0f, 10.0f);
    }

    // Update is called once per frame
    void Update() {
        // TODO: if model selection is enabled
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            modelID = 0;
            resetObject();
        }
            
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            modelID = 1;
            resetObject();
        }
            
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            modelID = 2;
            resetObject();
        }
            
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            modelID = 3;
            resetObject();
        }
            
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            modelID = 4;
            resetObject();
        }
            
        // TODO: if interaction is enabled
        if (Input.GetKeyDown(KeyCode.S) || Input.GetButtonDown("Fire3"))
            scale = true;

        if (Input.GetKeyUp(KeyCode.S) || Input.GetButtonUp("Fire3"))
            scale = false;

        if (scale)
        {
            model.transform.localScale = model.transform.localScale + new Vector3(1.0f, 1.0f, 1.0f) * Input.GetAxis("Mouse X") * Time.deltaTime;
            model.transform.localScale = model.transform.localScale + new Vector3(1.0f, 1.0f, 1.0f) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Fire1"))
            rotate = true;

        if (Input.GetKeyUp(KeyCode.R) || Input.GetButtonUp("Fire1"))
            rotate = false;

        if (rotate)
        {
            model.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), -1 * Input.GetAxis("Mouse X"), 0) * Time.deltaTime * rotSpeed);
            model.transform.Rotate(new Vector3(Input.GetAxis("Vertical"), -1 * Input.GetAxis("Horizontal"), 0) * Time.deltaTime * rotSpeed);
        }

        if (Input.GetKeyDown(KeyCode.T) || Input.GetButtonDown("Fire2")) {
            translate = true;
        }

        if (Input.GetKeyUp(KeyCode.T) || Input.GetButtonUp("Fire2"))
            translate = false;

        if(translate)
        {
            model.transform.Translate((Vector3.up * Input.GetAxis("Mouse Y") + Vector3.right * Input.GetAxis("Mouse X")) * Time.deltaTime * transSpeed);
            model.transform.Translate((Vector3.up * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal")) * Time.deltaTime * transSpeed);
        }

        // TODO: synchronize transformation with database

        // TODO: retrieve transformation update from database
        // scaling, rotation, translation
    }

    public void resetObject()
    {
        preview.SetActive(false);
        model.SetActive(false);
        //model.transform.localPosition = new Vector3(0.0f, 0.1f, 0.0f);
        model = objects[modelID];
        //model.transform.localPosition = new Vector3(0.0f, 0.1f, 0.0f);
        model.SetActive(true);
    }
}
