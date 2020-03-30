using System.Collections.Generic;

using UnityEngine;

namespace HololensPETS
{
    public class FingerForceDataProvider : MonoBehaviour
    {
        public delegate void OnFingerDataReceivedDelegate( Dictionary<Finger, double> forceValues );
        public OnFingerDataReceivedDelegate OnFingerDataReceived;

        private UDPHelper m_udpHelper;

        private void Awake()
        {
            m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();
        }

        private void OnEnable()
        {
            if(m_udpHelper != null)
            {
                m_udpHelper.MessageReceived += UDPMessageReceivedHandler;
            }
        }
        
        private void OnDisable()
        {
            if(m_udpHelper != null)
            {
                m_udpHelper.MessageReceived -= UDPMessageReceivedHandler;
            }
        }

        private void UDPMessageReceivedHandler(NetInMessage message)
        {
            MessageType.Command command = (MessageType.Command)message.ReadInt32();
            if( command != MessageType.Command.FingerData )
            {
                return;
            }

            Dictionary<Finger, double> data = new Dictionary<Finger, double>();
            for( int i = 0; i < 5; i++ )
            {
                int fingerIndex = message.ReadInt32();
                Finger finger = (Finger)fingerIndex;

                double force = message.ReadDouble();

                data.Add(finger, force);
            }

            if( OnFingerDataReceived != null )  //処理が入れられていたらdataをinputとしてその処理を実行する
            {
                OnFingerDataReceived( data );
            }
        }
    }
}
