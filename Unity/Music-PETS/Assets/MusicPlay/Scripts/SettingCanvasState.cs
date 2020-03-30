using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingCanvasState : MonoBehaviour {
    Canvas settingcanvas, keyDebugCanvas;

	// Use this for initialization
	void Start () {
        settingcanvas = GameObject.Find("SettingCanvas").GetComponent<Canvas>();
        keyDebugCanvas = GameObject.Find("KeyDebugCanvas").GetComponent<Canvas>();
        //settingcanvas.enabled = false;
        keyDebugCanvas.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.S))
        {
            settingcanvas.enabled = !settingcanvas.enabled;
        }

        if (Input.GetKeyDown(KeyCode.D) && (Input.GetKey(KeyCode.LeftShift)||(Input.GetKey(KeyCode.RightShift))))
        {
            keyDebugCanvas.enabled = !keyDebugCanvas.enabled;
        }
    }
}
