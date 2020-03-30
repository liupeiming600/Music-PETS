using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace MusicPlay
{
    public class TextInput : MonoBehaviour
    {
        MIDImanager midimng;
        ScoreMove scoremove;
        uOSC.uOscClient OscCli;
        // Use this for initialization
        void Start()
        {
            //TempoInpFld = GetComponent<InputField>();
            midimng = GameObject.Find("MIDImanager").GetComponent<MIDImanager>();
            scoremove = GameObject.Find("ScoreCanvas/ScoreOrigin").GetComponent<ScoreMove>();
            OscCli = GameObject.Find("UdpObject").GetComponent<uOSC.uOscClient>();

            Initialize();
        }

        void Initialize()
        {
            SetLiveIp();
            TempoInput();
            FrequencyInput();
            for(int i=0; i<5; i++) FingerSelect(i);
            SynchronousMode();
        }

        public InputField IpInpFld;
        

        public void SetLiveIp()
        {
            string textvalue = IpInpFld.text;

            OscCli.address = textvalue;
            OscCli.ChangeAdress();
        }

        public string SongListPath;
        public Dropdown SongDrop, TrackDrop;

        public void ChooseSong()
        {
            string TrackListPath = Path.Combine(SongListPath, SongDrop.captionText.text);
            string[] tracklist = Directory.GetFiles(TrackListPath);
            TrackDrop.options = new List<Dropdown.OptionData>();
            foreach(string trackpath in tracklist)
            {
                string track = Path.GetFileName(trackpath);
                if(!track.StartsWith(".")) TrackDrop.options.Add(new Dropdown.OptionData { text = track });
            }

            TrackDrop.RefreshShownValue();
        }

        public InputField TempoInpFld, FrqInpFld;

        public void TempoInput()
        {
            string textvalue = TempoInpFld.text;
            int intvalue = int.Parse(textvalue);

            //scoremove.tempo = (float)intvalue;
            //scoremove.ChangeDy();
            //OscCli.Send("/tempo", scoremove.tempo);
        }

        public void FrequencyInput()
        {
            string textvalue = FrqInpFld.text;
            int intvalue = int.Parse(textvalue);

            midimng.freq = intvalue;
        }

        public Toggle[] FingerToggle = new Toggle[5];

        public void FingerSelect(int _FingNum)
        {
            midimng.useFinger[_FingNum] = FingerToggle[_FingNum].isOn;
        }

        public Toggle SynchronousToggle;

        public void SynchronousMode()
        {
            midimng.multifinger_mode = SynchronousToggle.isOn;
        }

        public void MakeScore()
        {
            gameObject.GetComponent<Canvas>().enabled = false;
        }
    }
}
