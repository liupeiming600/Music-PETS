using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HololensPETS;

namespace HololensPETSGames
{
    public class InputValueSender : MonoBehaviour
    {
        private AppState m_appState;

        private UDPHelper m_udpHelper;

        public List<float> Data = new List<float>() { 0, 0, 0, 0, 0 };

        private void Awake()
        {
            m_appState = GameObject.FindObjectOfType<AppState>();

            m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            NetOutMessage outMessage = new NetOutMessage();

            outMessage.WriteInt32((int)MessageType.Command.Data);
            //outMessage.WriteInt32((int)MessageType.ControlType.KeyVelocity);

            for (int i = 0; i < Data.Count; i++)
            {
                outMessage.WriteInt32(i);
                outMessage.WriteFloat(Data[i]);
            }

            m_udpHelper.Send(outMessage, m_appState.HololensIP, Constants.NETWORK_PORT);
        }
    }
}
