using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MusicPlay
{
    public class ScoreMove : MonoBehaviour
    {
        GameObject Obj, udp, ProgressBar;
        MIDImanager midimanager;
        OSC_Send _osc;
        RectTransform Progress;
        Vector2 BarRect;
        int tick;
        float StartPos = -3.5f;

        public float tempo, dy;
        public int MusicLength = 100;

        // Use this for initialization
        void Start()
        {
            Obj = GameObject.Find("MIDImanager");
            midimanager = Obj.GetComponent<MIDImanager>();
            tick = midimanager.tick;
            udp = GameObject.Find("UdpObject");
            _osc = udp.GetComponent<OSC_Send>();
            ProgressBar = GameObject.Find("Canvas/ProgressBar");
            Progress = ProgressBar.GetComponent<RectTransform>();
            BarRect = Progress.sizeDelta;
            Progress.sizeDelta = new Vector2( 0, BarRect.y);

            dy = tempo / 60.0f;
        }

        // Update is called once per frame
        void Update()
        {
            if (_osc.playing_music)
            {
                transform.position += new Vector3(0, -dy * Time.deltaTime, 0);
            }
            else
            {
                if(transform.position.y != -3.5f) transform.localPosition = new Vector3(0, StartPos, 0);
            }

            float width = Mathf.Min((StartPos - transform.localPosition.y) *10 / MusicLength, 10.0f);
            Progress.sizeDelta = new Vector2( width, BarRect.y);
        }

        public void SetToOrigin()
        {
            transform.localPosition = new Vector3(0, StartPos, 0);
        }

        public void ChangeDy(float value)
        {
            tempo = value;
            //Debug.Log("Tempo: " + tempo);
            dy = tempo / 60.0f;
        }
    }
}
