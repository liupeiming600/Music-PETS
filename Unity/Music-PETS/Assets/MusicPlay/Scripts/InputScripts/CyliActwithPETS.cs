using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HololensPETS;
using MusicPlay;

namespace HololensPETSGames
{
    public class CyliActwithPETS : MonoBehaviour
    {
        private FingerForceDataProvider m_fingerForceDataProvider;

        private AppState m_appState;

        private UDPHelper m_udpHelper;
        private InputValueSender m_inputValueSender;
        private OSC_Send m_oscSend;
        private KeyManager m_keyManager;

        public Dictionary<Finger, int> thre_per;
        private Dictionary<char, Finger> finger_num;
        private float force_per = 0.0f, bias = 0.3f;
        bool changedColor = false;

        GameObject obj, score;
        InputManager InpMng;
        scoremanage scmn;

        public char chr;
        public int threshold = 10 ;

        public bool pressing = true;    //thresholdを超えているか，押しているか
        public bool correct = false;    //正しく押しているか

        [SerializeField]
        bool note_in = false, ignore;

        static float max_size = 1.5f;

        private void Awake()
        {
            m_fingerForceDataProvider = GameObject.FindObjectOfType<FingerForceDataProvider>();

            m_appState = GameObject.FindObjectOfType<AppState>();

            m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();

            m_inputValueSender = GameObject.FindObjectOfType<InputValueSender>();

            m_oscSend = GameObject.FindObjectOfType<OSC_Send>();

            m_keyManager = GameObject.FindObjectOfType<KeyManager>();
        }

        // Use this for initialization
        void Start()
        {
            obj = GameObject.Find("InputManager");
            InpMng = obj.GetComponent<InputManager>();

            score = GameObject.Find("ScoreManager");
            scmn = score.GetComponent<scoremanage>();

            thre_per = new Dictionary<Finger, int>
            {
                { Finger.Thumb, threshold },
                { Finger.Index, threshold },
                { Finger.Middle, threshold },
                { Finger.Ring, threshold },
                { Finger.Pinky, threshold }
            };

            finger_num = new Dictionary<char, Finger>()
            {
                {'c', Finger.Thumb},
                {'v', Finger.Index},
                {'b', Finger.Middle},
                {'n', Finger.Ring},
                {'m', Finger.Pinky}
            };
        }

        // Update is called once per frame
        void Update()
        {
            this.transform.localScale = new Vector3(max_size * force_per, 0.15f, 1);

            if (!note_in)   //noteが来てなくて
            {
                if (pressing && correct)    //押していたら
                {
                    correct = false;
                }
                else if (!pressing && !correct) //押していなかったら
                {
                    correct = true;
                }
            }
        }

        void OnTriggerStay(Collider other)  //noteが来ている間
        {
            if (!note_in) note_in = true;

            if (pressing && !changedColor)   //押していたら
            {
                changedColor = true;
                correct = true;
                ignore = false;

                //HoloLensのCylinderの色を変える
                NetOutMessage outMessage = new NetOutMessage();
                outMessage.WriteInt32((int)MessageType.Command.Control);
                outMessage.WriteInt32((int)MessageType.ControlType.ChangeColor);
                outMessage.WriteInt32((int)finger_num[chr]);

                if (chr == 'c')
                {
                    this.GetComponent<Renderer>().material.color = Color.red;
                    outMessage.WriteVector3(new Vector3(1, 0, 0));
                }
                else if (chr == 'v')
                {
                    this.GetComponent<Renderer>().material.color = Color.yellow;
                    outMessage.WriteVector3(new Vector3(1, 1, 0));
                }
                else if (chr == 'b')
                {
                    this.GetComponent<Renderer>().material.color = Color.green;
                    outMessage.WriteVector3(new Vector3(0, 1, 0));
                }
                else if (chr == 'n')
                {
                    this.GetComponent<Renderer>().material.color = Color.cyan;
                    outMessage.WriteVector3(new Vector3(0, 1, 1));
                }
                else if (chr == 'm')
                {
                    this.GetComponent<Renderer>().material.color = Color.blue;
                    outMessage.WriteVector3(new Vector3(0, 0, 1));
                }
                m_udpHelper.Send(outMessage, m_appState.HololensIP, Constants.NETWORK_PORT);
            }
            else if(!pressing && changedColor)
            {
                this.GetComponent<Renderer>().material.color = Color.white;
                correct = false;
                changedColor = false;
            }
        }

        private void OnTriggerEnter(Collider other) //noteが来たら
        {
            note_in = true;
            ignore = true;
        }

        private void OnTriggerExit(Collider other)　//noteが出たら
        {
            //other.enabled = false;
            changedColor = false;
            note_in = false;

            this.GetComponent<Renderer>().material.color = Color.white;

            NetOutMessage outMessage = new NetOutMessage();
            outMessage.WriteInt32((int)MessageType.Command.Control);
            outMessage.WriteInt32((int)MessageType.ControlType.ChangeColor);
            outMessage.WriteInt32((int)finger_num[chr]);
            outMessage.WriteVector3(new Vector3(255,255,255));
            m_udpHelper.Send(outMessage, m_appState.HololensIP, Constants.NETWORK_PORT);

            if (ignore)
            {
                //scmn.ignored_note += 1;
            }
        }

        private IEnumerator Wait_note() //押して少し後にnoteが来るか判定
        {
            bool come = false;
            float all_time = 0.2f, delta_time = 0.05f;

            for (int i = 0; i < all_time / delta_time; i++)
            {
                yield return new WaitForSeconds(delta_time);
                if (note_in)
                {
                    come = true;
                    ignore = false;
                }
            }

            //if (!come) scmn.miss_touch += 1;
        }

        //forceData取得
        private void OnEnable()
        {
            m_fingerForceDataProvider.OnFingerDataReceived += FingerForceDataReceivedHandler;
        }

        private void OnDisable()
        {
            m_fingerForceDataProvider.OnFingerDataReceived -= FingerForceDataReceivedHandler;
        }

        private void FingerForceDataReceivedHandler(Dictionary<Finger, double> forceData)
        {
            double force;

            force = forceData[finger_num[chr]] - m_appState.GetCurrentHandData().GetFingerBaseForce(finger_num[chr]);
            force_per = (float)(force / (m_appState.GetCurrentHandData().GetFingerMaxForce(finger_num[chr])));  //x%
            force_per = (float)((force_per * 100)/ (thre_per[finger_num[chr]]));   //x/10

            //押した！！
            if(1.0f <= force_per && !pressing)
            {
                pressing = true;
                
                //Send NoteOn message to Live
                m_keyManager.NoteOn((int)finger_num[chr]);
                

                //Scoring

                float boxHeight = 1.5f;
                Collider[] cols = Physics.OverlapBox(new Vector3(transform.position.x,transform.position.y,transform.position.z), new Vector3(transform.localScale.x/2, boxHeight/2, transform.localScale.z));
                
                Collider Closest = null;
                float min_dist = Mathf.Infinity;
                foreach(Collider col in cols)
                {
                    if(col.transform.tag == "note")
                    {
                        float distance = col.transform.position.y - (transform.position.y);
                        if(Mathf.Abs(min_dist) > Mathf.Abs(distance))
                        {
                            min_dist = distance;
                            Closest = col;
                        }
                    }
                }
                if (Closest == null)
                {
                    bool missTouch = false;
                    Collider[] cols2 = Physics.OverlapBox(new Vector3(0, transform.position.y, transform.position.z), new Vector3(10, boxHeight, transform.localScale.z));
                    Collider Closest2 = null;
                    float min_dist2 = Mathf.Infinity;

                    foreach (Collider col in cols2)
                    {
                        if (col.transform.tag == "note" || col.transform.tag == "PassedNote")
                        {
                            float distance2 = Mathf.Abs(col.transform.position.y - (transform.position.y));
                            if (min_dist2 > distance2)
                            {
                                min_dist2 = distance2;
                                Closest2 = col;
                            }
                        }
                    }

                    if(Closest2 != null)
                    {
                        if (Closest2.transform.position.x != ((int)finger_num[chr] - 2 * 2.5f))
                        {
                            missTouch = true;
                        }
                    }

                    if (missTouch) scmn.missTouch++;
                }
                else
                {
                    Closest.transform.tag = "PassedNote";
                    scmn.Scoring(min_dist, boxHeight);

                    if (min_dist < boxHeight / 6.0f)
                    {
                        scmn.great++;
                    }
                    else
                    {
                        scmn.good++;
                    }
                }
                scmn.ValueChange();
            }
            else if(0.9f > force_per && pressing)
            {
                pressing = false;                
                m_keyManager.Rerease((int)finger_num[chr]);
            }

            force_per = (Mathf.Clamp(force_per , bias, 1.00f) - bias) * 1 / (1 - bias);

            m_inputValueSender.Data[(int)finger_num[chr]] = force_per;  //Send to HoloLens
        }
    }
}
