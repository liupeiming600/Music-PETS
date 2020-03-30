using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {
    public float dy;
    public List<GameObject> Origins;
    bool isPlaying = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (isPlaying)
        {
            for (int i = 0; i < Origins.Count; i++)
            {
                Origins[i].transform.localPosition += new Vector3(0, -dy * Time.deltaTime, 0);
            }
        }
	}

    public void PlayMusic(float tempo)
    {
        dy = tempo / (60.0f * 50.0f);
        isPlaying = true;
    }

    public void ResetPosition()
    {
        isPlaying = false;

        for (int i = 0; i < Origins.Count; i++)
        {
            Origins[i].transform.localPosition = new Vector3(0, 0, 0);
        }
    }

    public void ChangeDy(float value)
    {
        dy = value / (60.0f * 50.0f);
    }
}
