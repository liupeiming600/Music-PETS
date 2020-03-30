using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.IO;
using MusicPlay;
using UnityEngine.UI;

public class KeyManager : MonoBehaviour {
    public struct KeyInfo
    {
        public int key;
        public float OnTime;
        public float OffTime;
        public int tag;
    }

    public struct AutoPlay
    {
        public bool pressed;
        public int NofAutoNote;
        public int subIndex;
        public List<int> LastOffNoteIndex;  //最後にNoteOffになるNoteのIndex
        public List<int> OnIndex;
    }


    public List<KeyInfo> thumbStore = new List<KeyInfo>();
    public List<KeyInfo> indexStore = new List<KeyInfo>();
    public List<KeyInfo> middleStore = new List<KeyInfo>();
    public List<KeyInfo> ringStore = new List<KeyInfo>();
    public List<KeyInfo> littleStore = new List<KeyInfo>();

    public List<List<KeyInfo>> KeyInfoList;
    List<int> index = new List<int>() { 0,0,0,0,0 };
    //List<int> preIndex = new List<int>() { 0, 0, 0, 0, 0 };

    public AutoPlay thumbAuto, indexAuto, middleAuto, ringAuto, littleAuto;
    public List<AutoPlay> AutoManager;

    //ボタンに割り当てられているキー
    public List<int> thumbKey = new List<int>() { 60 };
    public List<int> indexKey = new List<int>() { 62 };
    public List<int> middleKey = new List<int>() { 64 };
    public List<int> ringKey = new List<int>() { 65 };
    public List<int> littleKey = new List<int>() { 67 };

    public List<List<int>> KeyList;

    //Onになっているキー
    public List<int> thumbOn = new List<int>();
    public List<int> indexOn = new List<int>();
    public List<int> middleOn = new List<int>();
    public List<int> ringOn = new List<int>();
    public List<int> littleOn = new List<int>();

    List<List<int>> OnList;
    List<int> AllOnList = new List<int>();

    public List<bool> pressing = new List<bool>() { false, false, false, false, false}, firstFlag = new List<bool>() { true, true, true, true, true };


    OSC_Send m_oscSend;
    MIDImanager m_midiManager;
    public GameObject m_scoreOrigin;
    float PlayTime;

    public List<Text> KeyText, OnKeyText;

    // Use this for initialization
    void Start () {
        KeyInfoList = new List<List<KeyInfo>>() { thumbStore , indexStore, middleStore, ringStore, littleStore};
        AutoManager = new List<AutoPlay>() { thumbAuto, indexAuto, middleAuto, ringAuto, littleAuto };
        KeyList = new List<List<int>>() { thumbKey, indexKey, middleKey, ringKey, littleKey};
        OnList = new List<List<int>>() { thumbOn, indexOn, middleOn, ringOn, littleOn };

        m_oscSend = GameObject.FindObjectOfType<OSC_Send>();
        m_midiManager = GameObject.FindObjectOfType<MIDImanager>();
    }
	
	// Update is called once per frame
	void Update () {
        //再生中なら
        if (m_oscSend.playing_music)
        {
            PlayTime = Mathf.Abs(-3.5f - m_scoreOrigin.transform.localPosition.y);

            for (int i = 0; i < KeyInfoList.Count; i++) //すべての指について
            {
                if (KeyInfoList[i].Count > 0)
                {
                    if (index[i] + 1 < KeyInfoList[i].Count)
                    {
                        int check_index = AutoManager[i].LastOffNoteIndex[AutoManager[i].LastOffNoteIndex.Count - 1]; //check_index: 最後に終わるノートのインデックス


                        if (PlayTime >= KeyInfoList[i][check_index].OffTime)    //最後に終わるノートのNoteOffを過ぎていたら
                        {

                            if (index[i] + AutoManager[i].NofAutoNote < KeyInfoList[i].Count)
                            {
                                index[i] += AutoManager[i].NofAutoNote;
                                KeyList[i] = new List<int>();
                                KeyList[i].Add(KeyInfoList[i][index[i]].key);
                                CheckAutoPlay(i);
                            }
                        }
                    }

                    AutoPlay AP = AutoManager[i];
                    List<int> _offIndex = new List<int>();
                    //終わったnoteをnoteOff
                    foreach (int j in AutoManager[i].OnIndex)   //自動演奏中のすべてのnoteについて
                    {
                        //jがLastOffNoteIndexでなくて
                        //演奏時間がnoteOffTime以上なら
                        if (PlayTime >= KeyInfoList[i][j].OffTime && ((!AutoManager[i].LastOffNoteIndex.Contains(j))))
                        {
                            m_oscSend.NoteOn(KeyInfoList[i][j].key, 0);
                            KeyList[i].Remove(KeyInfoList[i][j].key);
                            OnList[i].Remove(KeyInfoList[i][j].key);
                            AllOnList.Remove(KeyInfoList[i][j].key);
                            _offIndex.Add(j);
                        }
                    }
                    foreach (int val in _offIndex) AP.OnIndex.Remove(val);    //上でforeachを使っているため
                    AutoManager[i] = AP;

                    //押していてnoteOnが来たらnoteOn
                    int _nextIndex = index[i] + AutoManager[i].subIndex + 1;
                    if (_nextIndex < KeyInfoList[i].Count)
                    {
                        if (PlayTime >= KeyInfoList[i][_nextIndex].OnTime)
                        {
                            AP = AutoManager[i];
                            AP.subIndex++;
                            AP.OnIndex.Add(_nextIndex);
                            AutoManager[i] = AP;

                            if (pressing[i] && AP.pressed && KeyInfoList[i][_nextIndex - 1].tag == 1)
                            {
                                m_oscSend.NoteOn(KeyInfoList[i][_nextIndex].key, 100);
                                //KeyList[i].Add(KeyInfoList[i][_nextIndex].key);
                                OnList[i].Add(KeyInfoList[i][_nextIndex].key);
                                AllOnList.Add(KeyInfoList[i][_nextIndex].key);
                            }
                            KeyList[i].Add(KeyInfoList[i][_nextIndex].key);
                        }
                    }

                    TextUpdate(KeyText[i], KeyList[i]);
                    TextUpdate(OnKeyText[i], OnList[i]);
                }
            }
        }
	}

    public void CheckAutoPlay(int finger_num)
    {
        AutoPlay AP = AutoManager[finger_num];
        List<int> _offIndex = new List<int>();
        foreach (int j in AutoManager[finger_num].OnIndex)   //自動演奏中のすべてのnoteについて
        {
            if (!AutoManager[finger_num].LastOffNoteIndex.Contains(j))  //最後のノートでなければ，NoteOff
            {
                m_oscSend.NoteOn(KeyInfoList[finger_num][j].key, 0);
                KeyList[finger_num].Remove(KeyInfoList[finger_num][j].key);
                OnList[finger_num].Remove(KeyInfoList[finger_num][j].key);
                AllOnList.Remove(KeyInfoList[finger_num][j].key);
                _offIndex.Add(j);
            }
        }
        foreach (int val in _offIndex) AP.OnIndex.Remove(val);

        AP.pressed = false;
        AP.OnIndex = new List<int>();

        AP.NofAutoNote = CheckAutoNote(finger_num, index[finger_num]) + 1;
        AP.subIndex = 0;
        AP.LastOffNoteIndex = LastNote(finger_num, index[finger_num], AP.NofAutoNote);
        AP.OnIndex.Add(index[finger_num]);
        AutoManager[finger_num] = AP;
    }

    public void Rerease(int finger_num)
    {
        pressing[finger_num] = false;
        //Send NoteOff message to Live
        NoteOff(finger_num);
    }

    public void TextUpdate(Text txt, List<int> Keys)
    {
        txt.text = "";
        for(int i = 0; i < Keys.Count; i++)
        {
            txt.text += Keys[i] + "\n";
        }
    }

    public void KeyInfoInit()
    {
        for (int i = 0; i < KeyInfoList.Count; i++)
        {
            KeyInfoList[i] = new List<KeyInfo>();   //All Clear
        }
    }

    public void KeyInit()
    {
        index = new List<int>() { 0, 0, 0, 0, 0 };
        AllOnList = new List<int>();

        for (int i = 0; i < KeyList.Count; i++)
        {
            if (KeyInfoList[i].Count > 0)   //noteが存在するなら
            {
                KeyList[i] = new List<int>() { KeyInfoList[i][index[i]].key };
                TextUpdate(KeyText[i], KeyList[i]);
                TextUpdate(OnKeyText[i], OnList[i]);

                AutoPlay AP = AutoManager[i];
                AP.subIndex = 0;
                AP.OnIndex = new List<int>();
                AutoManager[i] = AP;
                CheckAutoPlay(i);
            }
        }
        MakeTagforNotes();
        
        for(int i = 0; i<KeyInfoList.Count; i++)
        {
            WriteCsv(KeyInfoList[i], i);
        }
        
    }

    public void NoteOn(int finger, int velocity = 100)
    {
        pressing[finger] = true;
        AutoPlay AP = AutoManager[finger];
        AP.pressed = true;
        AutoManager[finger] = AP;

        for (int i = 0; i < KeyList[finger].Count; i++)
        {
            m_oscSend.NoteOn(KeyList[finger][i], velocity);
        }

        OnList[finger].AddRange(KeyList[finger]);
        AllOnList.AddRange(KeyList[finger]);
        TextUpdate(OnKeyText[finger], OnList[finger]);
    }

    public void NoteOff(int finger)
    {
        foreach (int value in OnList[finger])
        {
            AllOnList.Remove(value);
            
            if (0 == AllOnList.Count(n => n == value))
            {
                m_oscSend.NoteOn(value, 0);
            }
        }

        OnList[finger] = new List<int>();
        TextUpdate(OnKeyText[finger], OnList[finger]);
    }

    //最後にnoteOffになるnoteを見つける
    public List<int> LastNote(int finger, int index, int n)
    {
        float maxOffTime = -1;
        List<int> maxOffIndex = new List<int>() { index };
        for(int i=1; i<n; i++)
        {
            if (maxOffTime < KeyInfoList[finger][index + i].OffTime)
            {
                maxOffTime = KeyInfoList[finger][index + i].OffTime;
                maxOffIndex = new List<int>() { index + i };
            }
            else if(maxOffTime == KeyInfoList[finger][index + i].OffTime)
            {
                maxOffIndex.Add(index + i);
            }
        }

        return maxOffIndex;
    }

    //自動演奏のnoteの数を調べる
    public int CheckAutoNote(int finger, int index)
    {
        if (index + 1 != KeyInfoList[finger].Count)
        {
            if (1 == KeyInfoList[finger][index].tag)
            {
                return CheckAutoNote(finger, index + 1) + 1;
            }
        }
        return 0;
    }

    public void MakeTagforNotes()
    {
        for(int i=0; i<KeyInfoList.Count; i++)
        {
            for(int j=0; j<KeyInfoList[i].Count; j++)
            {
                KeyInfo KI = KeyInfoList[i][j];
                if (j != KeyInfoList[i].Count - 1) KI.tag = GiveTag(KeyInfoList[i], j);
                else KI.tag = 0;
                KeyInfoList[i][j] = KI;
            }
        }
    }

    public int GiveTag(List<KeyInfo> ListofKeyInfo, int index)
    {
        int isAutoPlay = 0;

        if(index < ListofKeyInfo.Count)
        {
            if ((ListofKeyInfo[index + 1].OnTime - ListofKeyInfo[index].OnTime) <= (4 / (float)m_midiManager.freq)) isAutoPlay = 1;
        }
        
        return isAutoPlay;
    }
    
    public void WriteCsv(List<KeyInfo> ListKI, int finger)
    {
        
        StreamWriter sw;
        FileInfo fi;

        fi = new FileInfo("Finger" + finger + "KeyInfo.csv");
        sw = fi.AppendText();
        for (int i = 0; i < ListKI.Count; i++)
        {
            string[] str = {"" + i, "" + ListKI[i].key, "" + ListKI[i].OnTime, "" + ListKI[i].OffTime, "" + ListKI[i].tag};
            string str2 = string.Join(",", str);

            sw.WriteLine(str2);
        }
        sw.Flush();
        sw.Close();
        
    }
    
}
