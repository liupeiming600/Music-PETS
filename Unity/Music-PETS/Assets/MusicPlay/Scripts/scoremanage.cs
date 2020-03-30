using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;

public class scoremanage : MonoBehaviour {
    public int NofNote, great = 0, good = 0, missTouch = 0, totalScore = 0, early = 0, late = 0;
    float totalDist = 0, earlyDist = 0, lateDist = 0;
    public GameObject ScoreText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}
    
    public void ValueChange()
    {
        ScoreText.GetComponent<Text>().text = "Great " + great + "    Good " + good + "   Score " + totalScore;
    }

    public void Init()
    {
        great = 0;
        good = 0;
        missTouch = 0;
        totalScore = 0;
        totalDist = 0;
        early = 0;
        late = 0;
        earlyDist = 0;
        lateDist = 0;
        ValueChange();
    }

    public void Scoring(float dist, float maxDist)
    {
        float absDist = Mathf.Abs(dist);
        totalDist += absDist;
        //Debug.Log("distance = " + dist);
        float score = 1000 * (maxDist / 2 - absDist);
        totalScore += (int)score;

        if(dist < 0)
        {
            late++;
            lateDist += absDist;
        }
        else
        {
            early++;
            earlyDist += absDist;
        }
    }

    public void SaveScore()
    {
        
        string fileName = "SaveData.csv";

        using (FileStream filestream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            using (StreamReader streamReader = new StreamReader(filestream))
            using (StreamWriter streamWriter = new StreamWriter(filestream))
            {
                //総ノート数，great, good, miss, misstouch, totalscore, totalDistance, averageDistance, 
                string[] str = { "" + NofNote, "" + great, "" + good, "" + (NofNote-great-good), "" + missTouch, "" + totalScore, "" + totalDist , "" + (totalDist/(great+good)),""+early ,""+(earlyDist/early),""+late,""+(lateDist/late)};
                string str2 = string.Join(",", str);

                string preString = streamReader.ReadToEnd();
                //streamWriter.WriteLine(preString);
                //filestream.Position = 0;
                streamWriter.WriteLine(str2);
            }
        }
        
    }
}
