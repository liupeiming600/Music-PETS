using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsShow : MonoBehaviour {
    int frameCount;
    float prevTime;
    float fps;

    Text FpsText;

	// Use this for initialization
	void Start () {
        FpsText = GetComponent<Text>();

        frameCount = 0;
        prevTime = 0;
	}
	
	// Update is called once per frame
	void Update () {
        frameCount++;
        float time = Time.realtimeSinceStartup - prevTime;

        if(time >= 0.5f)
        {
            fps = frameCount / time;
            FpsText.text = "FPS: " + fps;

            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
        }
	}
}
