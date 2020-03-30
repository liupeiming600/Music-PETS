using HololensPETSGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using uOSC;

namespace MusicPlay
{
    public partial class OSC_Send : MonoBehaviour
    {

        public bool mute = true;
        public bool trans = false;
        public float wait_time;
        GameObject obj1, obj2, obj3, obj4, obj5, ScoOri;    //cylinders, ScoreOrigin
        CyliActwithPETS comp1, comp2, comp3, comp4, comp5;
        ScoreMove ScoMov;

        uOscClient client;

        int[] int_array = new int[] { 0, 32, 96, 127 };

        // Use this for initialization
        void Start()
        {
            obj1 = GameObject.Find("Cylinders/Cylinder_0");
            obj2 = GameObject.Find("Cylinders/Cylinder_1");
            obj3 = GameObject.Find("Cylinders/Cylinder_2");
            obj4 = GameObject.Find("Cylinders/Cylinder_3");
            obj5 = GameObject.Find("Cylinders/Cylinder_4");

            comp1 = obj1.GetComponent<CyliActwithPETS>();
            comp2 = obj2.GetComponent<CyliActwithPETS>();
            comp3 = obj3.GetComponent<CyliActwithPETS>();
            comp4 = obj4.GetComponent<CyliActwithPETS>();
            comp5 = obj5.GetComponent<CyliActwithPETS>();

            ScoOri = GameObject.Find("ScoreCanvas/ScoreOrigin");
            ScoMov = ScoOri.GetComponent<ScoreMove>();

            client = GetComponent<uOscClient>();
            //client.Send("/tempo", ScoMov.tempo);

            TrackSelect();
        }

        // Update is called once per frame
        void Update()
        {
            StartStopSend();    //Start or Stop the music
            /*
            if (comp1.pressing || comp2.pressing || comp3.pressing || comp4.pressing || comp5.pressing)
            {
                if (mute)
                {
                    //Debug.Log("sound!!");
                    //client = GetComponent<uOscClient>();
                    client.Send("/mute", 10); //muteしない

                    mute = false;
                }
            }
            else
            {
                if (!mute)
                {
                    //client = GetComponent<uOscClient>();
                    client.Send("/mute", 20); //muteする

                    mute = true;
                }
            }

            if (comp1.correct && comp2.correct && comp3.correct && comp4.correct && comp5.correct)
            {
                if (trans)
                {
                    StopCoroutine("Wait_trans");

                    //client = GetComponent<uOscClient>();
                    client.Send("/transpose", 64); //transposeしない

                    trans = false;
                }
            }
            else
            {
                if (!trans) //移調   
                {
                    trans = true;
                    StartCoroutine("Wait_trans");
                }
            }
            */
        }

        //移調までの待ち時間
        private IEnumerator Wait_trans()
        {
            yield return new WaitForSeconds(wait_time);

            int transpose = UnityEngine.Random.Range(0, int_array.Length); ;
            //var client = GetComponent<uOscClient>();
            client.Send("/transpose", int_array[transpose]); //transposeする
        }

        public void NoteOn(int key, int velocity = 100)     // noteoff -> velocity=0
        {
            client.Send("/NoteOn", key, velocity, 0);
            //Debug.Log("key = " + key + ", velocity = " + velocity);
        }
    }
}
