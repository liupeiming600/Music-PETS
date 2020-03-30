using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HololensPETS;
using MusicPlay;

public class MIDImanager : MonoBehaviour {
    private AppState m_appState;

    private UDPHelper m_udpHelper;
    private ScoreMove m_scoreMove;
    private KeyManager m_keyManager;
    private scoremanage m_scoreManage;

    public int tick;            //4分音符の長さ
    public GameObject note_obj, scoreorigin, BarObj;
    public static float noteWidth=1, fingerInterval=2.5f;
    byte[] midi_data;
    private List<int> fingers = new List<int>();    //演奏する指
    public bool[] useFinger = new bool[5];
    public bool multifinger_mode, rythmical_mode, forcechanging_mode;
    public int freq=64;

    MusicPlay.TextInput txtinp;
    public Dropdown SongDrop, TrackDrop;

    private void Awake()
    {
        m_appState = GameObject.FindObjectOfType<AppState>();
        m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();
        m_scoreMove = GameObject.FindObjectOfType<ScoreMove>();
        m_keyManager = GameObject.FindObjectOfType<KeyManager>();
        m_scoreManage = GameObject.FindObjectOfType<scoremanage>();
    }

    private void Start()
    {
        
        txtinp = GameObject.Find("SettingCanvas").GetComponent<MusicPlay.TextInput>();
        scoreorigin = GameObject.Find("ScoreCanvas/ScoreOrigin");
    }

    // Use this for initialization
    public void MakeGameScore () {
        UnityEngine.Random.InitState(1);
        DestroyScore(scoreorigin);
        m_scoreManage.Init();
        fingers = ChoiceFinger(fingers, useFinger, m_appState.IsLeftHand.Value);

        string midi_path = Path.Combine(txtinp.SongListPath, SongDrop.captionText.text, TrackDrop.captionText.text);
        //Debug.Log(midi_path);
        midi_data = Load_midi(midi_path);  //midiファイルをbyte列に読み込む
        tick = Get_tick(midi_data);        //4分音符の分解能を取得
        Read_midi(midi_data, note_obj, tick, freq, fingers, multifinger_mode, m_appState, m_udpHelper, m_scoreMove, m_keyManager, m_scoreManage, BarObj);   //midiファイルから譜面を作成
	}

    public static void DestroyScore(GameObject obj)
    {
        foreach (Transform child in obj.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public static List<int> ChoiceFinger(List<int> fingers, bool[] usefinger, bool isLeft){
        fingers = new List<int>();
        for (int i = 0; i < usefinger.Length;i++){
            if (usefinger[i])
            {
                if(isLeft) fingers.Add(4-i);
                else fingers.Add(i);   //(Right)sum=0,index=1,middle=2,ring=3,little=4,(Left)sum=4,index=3,middle=2,ring=1,little=0
            }
        }
        return fingers;
    }

    public static byte[] Load_midi(string filename)
    {
        System.IO.FileStream fs = new System.IO.FileStream(@filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        int filesize = (int)fs.Length;
        byte[] data = new byte[filesize];
        fs.Read(data, 0, filesize);
        fs.Dispose();

        return data;
    }

    public static int Get_tick(byte[] byte_array){
        int header_len = ToInt32(byte_array, 4, 4);     //headerの長さを取得
        return ToInt32(byte_array,12,header_len - 4);   //headerからTickの情報を取得
    }

    public static int ToInt32(byte[] data, int index, int length)
    {
        int value = 0;
        for (int i = 0; i < length; i++)
        {
            value += data[index + length - 1 - i] * (int)Math.Pow(16, i * 2);
        }
        return value;
    }

    public static void Read_midi(byte[] midi_byte,GameObject obj, int tick, int freq, List<int> fingers, bool multifinger_mode, AppState m_appState, UDPHelper m_udpHelper, ScoreMove m_scoreMove, KeyManager m_keyManager, scoremanage m_scoremanage,  GameObject BarObj)
    {
        Stack<Vector3> notes = new Stack<Vector3>();
        notes.Push(new Vector3(0, -100, 0.01f));

        int[] noteOn_time = new int[128];   //各キーのnote-onの時間
        int tick_time = 0;                 //delta_time累積値
        bool in_MTrk = false;
        float min_noteLength = 4 / (float)freq;
        float scale = 50.0f;
        SortedSet<int> keySet = new SortedSet<int>();
        List<KeyManager.KeyInfo> ListKeyInfo = new List<KeyManager.KeyInfo>();


        List<Color> FingerColor = new List<Color>()
        {
            new Color(1, 0.5f, 0.5f, 1),
            new Color(1, 1, 0.5f, 1),
            new Color(0.5f, 1, 0.5f, 1),
            new Color(0.5f, 1, 1, 1),
            new Color(0.5f, 0.5f, 1, 1)
        };

        GameObject origin, pre_note= Instantiate(obj);
        origin = GameObject.Find("ScoreOrigin");
        pre_note.transform.SetParent(origin.transform);
        pre_note.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, -100f, 0.0f);

        m_keyManager.KeyInfoInit();

        for (int i = 4; i < midi_byte.Length; i++)
        {
            //トラックチャンク初め(4D 54 72 6B)と終わり(00 FF 2F 00)
            if (midi_byte[i] == 77 && midi_byte[i + 1] == 84 && midi_byte[i + 2] == 114 && midi_byte[i + 3] == 107) {
                in_MTrk = true;
                i += 8; //MTrkとlengthを飛ばす
            }
            else if(midi_byte[i] == 0 && midi_byte[i + 1] == 255 && midi_byte[i + 2] == 47 && midi_byte[i + 3] == 0) {
                in_MTrk = false;
                break;
            }

            if (in_MTrk)
            {
                int[] tuple_time_size = Variable2int(midi_byte, i);  //(delta_time, data size of delta_time)
                tick_time += tuple_time_size[0];
                i += tuple_time_size[1];

                if (midi_byte[i] == 144)   //90: note on
                {
                    int key = midi_byte[i + 1];
                    noteOn_time[key] = tick_time;    //noteの先頭の位置

                    i += 2;

                }
                else if (midi_byte[i] == 128) //80: note off
                {
                    int key = midi_byte[i + 1];
                    keySet.Add(key);

                    KeyManager.KeyInfo m_keyInfo;
                    m_keyInfo.key = key;
                    m_keyInfo.OnTime = noteOn_time[key] / (float)tick;
                    m_keyInfo.OffTime = tick_time / (float)tick;

                    m_keyInfo.tag = 0;

                    ListKeyInfo.Add(m_keyInfo);

                    i += 2;
                }else if (midi_byte[i] == 192){ //C0
                    i += 1;
                }else if (midi_byte[i] == 176 || midi_byte[i] == 224)  //B0, E0
                {
                    i += 2;
                }
                else if(midi_byte[i] == 255){  //FF
                    i += midi_byte[i + 2] + 2;
                }
            }
        }

        //Assign notes to five buttons
        int[] keyMap = new int[128];
        //Debug.Log("fingers: " + fingers.Count);
        int assignNum=fingers[0], ind=0;
        foreach(var key in keySet)
        {
            keyMap[key] = assignNum;
            ind = (ind + 1) % fingers.Count;
            assignNum =fingers[ind];
        }
        foreach(var _keyinfo in ListKeyInfo)
        {
            // Spawn the note object
            float note_height = _keyinfo.OffTime - _keyinfo.OnTime;
            Vector3 pos = new Vector3((assignNum - 2) * fingerInterval, _keyinfo.OnTime, 0);
            Vector2 siz = new Vector2(noteWidth, Mathf.Max(note_height,(float)(1.0/8.0)));
            GameObject note = Instantiate(obj);
            note.transform.SetParent(origin.transform);
            note.GetComponent<RectTransform>().sizeDelta = siz;
            note.GetComponent<RawImage>().color = FingerColor[assignNum];
            float pre_y = pre_note.GetComponent<RectTransform>().localPosition.y;
            float pre_size = pre_note.GetComponent<RectTransform>().sizeDelta.y;
            Vector3 SendVec;    //(fin_num, y_position, Height)
            if (pos.y - pre_y < min_noteLength || (assignNum == keyMap[_keyinfo.key] && pos.y - (pre_y+pre_size) < 1.0/8.0))
            {    //結合
                note_height = pos.y + siz.y - pre_y;

                note.GetComponent<RectTransform>().localPosition = new Vector3(pos.x, pre_y, 0);
                note.GetComponent<RectTransform>().sizeDelta = new Vector3(noteWidth, note_height);

                Destroy(pre_note);
                notes.Pop();

                SendVec = new Vector3(assignNum, pre_y / scale, note_height / scale);
                notes.Push(SendVec);
            }
            else
            {
                assignNum = keyMap[_keyinfo.key];
                note.GetComponent<RectTransform>().localPosition = pos = new Vector3((assignNum - 2) * fingerInterval, _keyinfo.OnTime, 0); ;
                note.GetComponent<RawImage>().color = FingerColor[assignNum];

                SendVec = new Vector3(assignNum, pos.y / scale, note_height / scale);
                notes.Push(SendVec);
            }
            /*
            if (multifinger_mode)
            {
                Clone(note, fingers);
            }
            */
            m_keyManager.KeyInfoList[assignNum].Add(_keyinfo);
            note.GetComponent<BoxCollider>().center = new Vector3(0, note_height / 2.0f, 0);
            note.GetComponent<BoxCollider>().size = new Vector3(0.5f, note_height, 0.2f);

            pre_note = note;
        }


        for(int i=0; i<m_keyManager.KeyInfoList.Count; i++)
        {
            m_keyManager.KeyInfoList[i].Sort((a, b) => FloatSort(a.OnTime,b.OnTime));
        }
        m_keyManager.KeyInit();

        int LastTime = tick_time / tick;
        if (tick_time % tick != 0) LastTime++;
        m_scoreMove.MusicLength = LastTime;
        for(int i=0; i<LastTime; i++)
        {
            GameObject barInstance = Instantiate(BarObj);
            barInstance.transform.SetParent(origin.transform);
            barInstance.GetComponent<RectTransform>().localPosition = new Vector3(0, i, 0);
            barInstance.transform.SetAsFirstSibling();
        }

        NetOutMessage outMessage = new NetOutMessage();
        outMessage.WriteInt32((int)MessageType.Command.Control);
        outMessage.WriteInt32((int)MessageType.ControlType.SpawnNote);
            
        int NumberofNotes = notes.Count;
        m_scoremanage.NofNote = notes.Count - 1;
        outMessage.WriteInt32(NumberofNotes);
        //Debug.Log("Number of notes is " + (NumberofNotes - 1));

        while (notes.Count > 0)
        {
            outMessage.WriteVector3(notes.Peek());
            notes.Pop();
        }

        outMessage.WriteInt32(LastTime);

        m_udpHelper.Send(outMessage, m_appState.HololensIP, Constants.NETWORK_PORT);
    }

    public static int[] Variable2int(byte[] byte_array, int index){
        int[] value = new int[2];
        value[0] = value[1] = 0;

        while(byte_array[index + value[1]]>128){
            value[0] *= 128;
            value[0] += (byte_array[index + value[1]] - 128) * 128;
            value[1] += 1;
        }
        value[0] += byte_array[index + value[1]];
        value[1] += 1;
        return value;
    }

    //今後変更予定
    static void Clone(GameObject obj, List<int> fingers)
    {
        GameObject copy_note = Instantiate(obj);
        copy_note.transform.SetParent(obj.transform.root);
        Vector3 pos = obj.GetComponent<RectTransform>().localPosition;
        int _newFin = ((int)(pos.x / fingerInterval) + 2 + 1)%5;
        pos.x = (_newFin - 2) * fingerInterval;
        copy_note.GetComponent<RectTransform>().localPosition = pos;
    }

    public static int FloatSort(float a, float b)
    {
        if (a > b) return 1;
        if (a < b) return -1;
        return 0;
    }
}
