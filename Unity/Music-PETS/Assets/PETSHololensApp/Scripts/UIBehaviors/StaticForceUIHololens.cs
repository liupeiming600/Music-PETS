using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HololensPETS
{
    public class StaticForceUIHololens : MonoBehaviour
    {
        public List<GameObject> fingerGraphParents;

        public List<BarGraph> fingerBarGraphs;
        public List<Indicator> fingerIndicators;

        private UDPHelper m_udpHelper;

        private HololensAppState m_appState;

        private void Awake()
        {
            m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();

            m_appState = GameObject.FindObjectOfType<HololensAppState>();
        }

        private void OnEnable()
        {
            m_udpHelper.MessageReceived += UDPMessageHandler;

            for( int i = 0; i < fingerGraphParents.Count; i++ )
            {
                fingerGraphParents[i].SetActive( true );
                fingerGraphParents[i].transform.position = m_appState.FingerTransforms[i].position;
            }
        }

        private void OnDisable()
        {
            m_udpHelper.MessageReceived -= UDPMessageHandler;

            for (int i = 0; i < fingerGraphParents.Count; i++)
            {
                fingerGraphParents[i].SetActive(false);
            }
        }

        private void ResetIndicators()
        {
            for(int i = 0; i < fingerIndicators.Count; i++)
            {
                fingerIndicators[i].IsVisible = true;
                fingerIndicators[i].SetIsOn(false);
            }
        }

        private void ResetGraphValues()
        {
            for(int i = 0; i < fingerBarGraphs.Count; i++)
            {
                fingerBarGraphs[i].value = 0.0f;
                fingerBarGraphs[i].targetValue = 0.0f;
            }
        }

        private void UDPMessageHandler(NetInMessage message)
        {
            int commandInt = message.ReadInt32();
            MessageType.Command command = (MessageType.Command)commandInt;
            if(command == MessageType.Command.Control)
            {
                MessageType.ControlType controlType = (MessageType.ControlType)message.ReadInt32();
                if ((controlType == MessageType.ControlType.Start) || (controlType == MessageType.ControlType.Next))
                {
                    int activeFingerIndex = message.ReadInt32();
                    for (int i = 0; i < fingerIndicators.Count; i++)
                    {
                        fingerIndicators[i].SetIsOn(i == activeFingerIndex);
                        fingerIndicators[i].IsVisible = ( i == activeFingerIndex );
                    }

                    for (int i = 0; i < fingerBarGraphs.Count; i++)
                    {
                        int fingerIndex = message.ReadInt32();
                        double targetForce = message.ReadDouble();

                        fingerBarGraphs[fingerIndex].targetValue = targetForce;
                    }
                }
                else if (controlType == MessageType.ControlType.Stop)
                {
                    ResetIndicators();
                    ResetGraphValues();
                }
            }
            else if(command == MessageType.Command.Data)
            {
                for(int i = 0; i < fingerBarGraphs.Count; i++)
                {
                    int fingerIndex = message.ReadInt32();
                    float plotValue = message.ReadFloat();

                    fingerBarGraphs[fingerIndex].value = plotValue;
                }
            }
        }
    }
}
