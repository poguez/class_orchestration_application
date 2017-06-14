using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Model : MonoBehaviour {


    const int numModels = 3;

    public GameObject[] objects = new GameObject[numModels];
    public GameObject[] previewModels = new GameObject[numModels];

    public EventSystem eventSystem;

    // TODO: retrieve ID from database
    public int modelID;

    public GameObject model;
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
            modelID = numModels;
        modelID = (modelID - 1) % numModels;
        previewObject();
    }
    public void rightArrow()
    {
        modelID = (modelID + 1) % numModels;
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
        // TODO: if interaction is enabled
        if (Input.GetKeyDown(KeyCode.S) || Input.GetButtonDown("Fire3"))
        {
            scale = true;
            eventSystem.sendNavigationEvents = false;
        }

        if (Input.GetKeyUp(KeyCode.S) || Input.GetButtonUp("Fire3"))
        {
            scale = false;
            eventSystem.sendNavigationEvents = true;
        }

        if (scale)
        {
            model.transform.localScale = model.transform.localScale + new Vector3(1.0f, 1.0f, 1.0f) * Input.GetAxis("Mouse X") * Time.deltaTime;
            model.transform.localScale = model.transform.localScale + new Vector3(1.0f, 1.0f, 1.0f) * Input.GetAxis("Horizontal") * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Fire1"))
        {
            rotate = true;
            eventSystem.sendNavigationEvents = false;
        }

        if (Input.GetKeyUp(KeyCode.R) || Input.GetButtonUp("Fire1"))
        {
            rotate = false;
            eventSystem.sendNavigationEvents = true;
        }

        if (rotate)
        {
            model.transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), -1 * Input.GetAxis("Mouse X"), 0) * Time.deltaTime * rotSpeed);
            model.transform.Rotate(new Vector3(Input.GetAxis("Vertical"), -1 * Input.GetAxis("Horizontal"), 0) * Time.deltaTime * rotSpeed);
        }

        if (Input.GetKeyDown(KeyCode.T) || Input.GetButtonDown("Fire2")) {
            translate = true;
            eventSystem.sendNavigationEvents = false;
        }

        if (Input.GetKeyUp(KeyCode.T) || Input.GetButtonUp("Fire2"))
        {
            translate = false;
            eventSystem.sendNavigationEvents = true;
        }

        if(translate)
        {
            model.transform.Translate((Vector3.up * Input.GetAxis("Mouse Y") + Vector3.right * Input.GetAxis("Mouse X")) * Time.deltaTime * transSpeed);
            model.transform.Translate((Vector3.up * Input.GetAxis("Vertical") + Vector3.right * Input.GetAxis("Horizontal")) * Time.deltaTime * transSpeed);
        }
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
