using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HololensPETS;

namespace HololensPETSGames
{
    public class MusicPlayGameHololensController : MonoBehaviour
    {
        public List<GameObject> NoteLanes;
        public float CyliSize;
        public List<GameObject> Cylinders;
        public List<GameObject> Origins = new List<GameObject>(5);
        public GameObject note_obj, bar_obj;
        public Text MidiDebug;

        private HololensAppState m_appState;

        private UDPHelper m_udpHelper;

        float tempo = 120, scale = 50.0f;


        List<Color> FingerColor = new List<Color>()
        {
            new Color(1, 0.4f, 0.4f, 0.8f),
            new Color(1, 1, 0.4f, 0.8f),
            new Color(0.4f, 1, 0.4f, 0.8f),
            new Color(0.4f, 1, 1, 0.8f),
            new Color(0.4f, 0.4f, 1, 0.8f)
        } ;

        private void Awake()
        {
            m_appState = GameObject.FindObjectOfType<HololensAppState>();

            m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();
        }

        private void OnEnable()
        {
            m_udpHelper.MessageReceived += UDPMessageReceivedHandler;

            float minHight = Mathf.Infinity;
            for (int i = 0; i < NoteLanes.Count; i++)
            {
                if (minHight > m_appState.FingerTransforms[i].position.y)
                {
                    minHight = m_appState.FingerTransforms[i].position.y;
                }
            }
            minHight -= 0.01f;
            for (int i = 0; i < NoteLanes.Count; i++)
            {
                NoteLanes[i].transform.position = new Vector3(m_appState.FingerTransforms[i].position.x, minHight, m_appState.FingerTransforms[i].position.z);
            }

            float xx = NoteLanes[2].transform.position.x - NoteLanes[1].transform.position.x;
            float zz = NoteLanes[2].transform.position.z - NoteLanes[1].transform.position.z;
            float x = NoteLanes[0].transform.position.x * xx + NoteLanes[1].transform.position.x * zz * zz / xx + zz * (NoteLanes[0].transform.position.z - NoteLanes[1].transform.position.z);
            x = x / (xx + zz * zz / xx);
            float z = zz * (x - NoteLanes[1].transform.position.x) / xx + NoteLanes[1].transform.position.z;

            NoteLanes[0].transform.position = new Vector3(x, minHight, z);
        }

        private void OnDisable()
        {
            m_udpHelper.MessageReceived -= UDPMessageReceivedHandler;
        }

        private void UDPMessageReceivedHandler(NetInMessage message)
        {
            int commandInt = message.ReadInt32();
            MessageType.Command command = (MessageType.Command)commandInt;
            if (command == MessageType.Command.Data)
            {
                for (int i = 0; i < 5; i++)
                {
                    int fin_num = message.ReadInt32();
                    float velocity = message.ReadFloat();
                    Cylinders[fin_num].transform.localScale = new Vector3(CyliSize * velocity, 0.003f, CyliSize * velocity);
                }
            }else if(command == MessageType.Command.Control)
            {
                //MidiDebug.text = "Command: Control";
                MessageType.ControlType controlType = (MessageType.ControlType)message.ReadInt32();
                if (controlType == MessageType.ControlType.SpawnNote)
                {
                    int NoteCount = message.ReadInt32();
                    DestroyScore(Origins);
                    //Debug.Log("notecount = " + NoteCount);
                    //MidiDebug.text = "Control Type: SpawnNote";

                    for (int i=0; i<NoteCount-1; i++)
                    {
                        //Debug.Log(i);
                        Vector3 NoteInfo = message.ReadVector3();

                        GameObject note = Instantiate(note_obj);
                        note.transform.SetParent(Origins[(int)NoteInfo.x].transform);

                        note.GetComponent<RectTransform>().localPosition = new Vector3(0, NoteInfo.y, 0);
                        note.GetComponent<RectTransform>().sizeDelta = new Vector2(0.02f, NoteInfo.z);
                        note.GetComponent<RectTransform>().localRotation = Quaternion.Euler(Vector3.zero);
                        note.GetComponent<RawImage>().color = FingerColor[(int)NoteInfo.x];
                    }
                    //MidiDebug.text = "count = " + NoteCount;
                    message.ReadVector3();
                    
                    int LastTime = message.ReadInt32();
                    
                    for (int i=0; i<LastTime; i++)
                    {
                            for (int j = 0; j < 5; j++)
                            {
                                GameObject bar = Instantiate(bar_obj);
                                bar.transform.SetParent(Origins[j].transform);

                                bar.GetComponent<RectTransform>().localPosition = new Vector3(0, i / scale, 0);
                                //bar.GetComponent<RectTransform>().sizeDelta = new Vector2(0.03f, 1 / scale);
                                bar.GetComponent<RectTransform>().localRotation = Quaternion.Euler(Vector3.zero);
                                bar.transform.SetAsFirstSibling();
                        }
                    }
                    
                }
                else if (controlType == MessageType.ControlType.DestroyNote)
                {
                    
                }
                else if (controlType == MessageType.ControlType.PlaySong)
                {
                    tempo = message.ReadFloat();
                    GetComponent<Move>().PlayMusic(tempo);
                }
                else if (controlType == MessageType.ControlType.StopSong)
                {
                    GetComponent<Move>().ResetPosition();
                }
                else if (controlType == MessageType.ControlType.ChangeColor)
                {
                    int fin_num = message.ReadInt32();
                    Vector3 RGB = message.ReadVector3();

                    Cylinders[fin_num].GetComponent<Renderer>().material.color = new Color(RGB.x, RGB.y, RGB.z, 1.0f);

                }
                else if (controlType == MessageType.ControlType.ChangeTempo)
                {
                    tempo = message.ReadFloat();
                    MidiDebug.text = "tempo = " + tempo;
                    GetComponent<Move>().ChangeDy(tempo);
                }
            }
        }

        public static void DestroyScore(List<GameObject> obj)
        {
            for (int i = 0; i < obj.Count; i++)
            {
                foreach (Transform child in obj[i].transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }

    }
}
