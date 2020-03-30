using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HololensPETS;

namespace HololensPETSGames
{
    public class PETSFingerInputController : MonoBehaviour
    {
        public List<KeyCode> keyMappings;
        public List<float> fingerForceMinThresholds;
        public List<float> fingerForceMaxThresholds;

        private FingerForceDataProvider m_fingerForceDataProvider;

        private UDPHelper m_udpHelper;
        private AppState m_appState;

        private void Awake()
        {
            m_fingerForceDataProvider = GameObject.FindObjectOfType<FingerForceDataProvider>();

            m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();
            m_appState = GameObject.FindObjectOfType<AppState>();
        }

        private void OnEnable()
        {
            m_fingerForceDataProvider.OnFingerDataReceived += FingerDataReceivedHandler;
        }

        private void OnDisable()
        {
            m_fingerForceDataProvider.OnFingerDataReceived -= FingerDataReceivedHandler;
        }

        private void FingerDataReceivedHandler( Dictionary<Finger, double> forceValues )
        {
            foreach( Finger finger in forceValues.Keys )
            {
                int fingerIndex = (int)finger;
                float fingerForce = (float)( forceValues[finger] - m_appState.GetCurrentHandData().GetFingerBaseForce( finger ) );
                
                if( MathUtils.IsInRange( fingerForce, fingerForceMinThresholds[fingerIndex], fingerForceMaxThresholds[fingerIndex] ) )
                {
                    CustomInput.PressKey( keyMappings[fingerIndex] );

                    NetOutMessage outMessage = new NetOutMessage();

                    outMessage.WriteInt32( (int)MessageType.Command.Control );
                    outMessage.WriteInt32( (int)MessageType.ControlType.PressKey );
                    outMessage.WriteInt32( (int)keyMappings[fingerIndex] );

                    m_udpHelper.Send( outMessage, m_appState.HololensIP, Constants.NETWORK_PORT );
                }
                else
                {
                    CustomInput.ReleaseKey( keyMappings[fingerIndex] );

                    NetOutMessage outMessage = new NetOutMessage();

                    outMessage.WriteInt32( (int)MessageType.Command.Control );
                    outMessage.WriteInt32( (int)MessageType.ControlType.ReleaseKey );
                    outMessage.WriteInt32( (int)keyMappings[fingerIndex] );

                    m_udpHelper.Send( outMessage, m_appState.HololensIP, Constants.NETWORK_PORT );
                }
            }
        }
    }
}
