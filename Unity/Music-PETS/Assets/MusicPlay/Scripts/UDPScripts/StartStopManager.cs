using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HololensPETS;

namespace MusicPlay
{
    public partial class OSC_Send
    {
        private AppState m_appState;

        private UDPHelper m_udpHelper;

        private ScoreMove m_scoreMove;
        private scoremanage m_scoremanage;

        public bool playing_music = false;

        private void Awake()
        {
            m_appState = GameObject.FindObjectOfType<AppState>();
            m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();
            m_scoreMove = GameObject.FindObjectOfType<ScoreMove>();
            m_scoremanage = GameObject.FindObjectOfType<scoremanage>();
        }

        void StartStopSend()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                NetOutMessage outMessage = new NetOutMessage();
                outMessage.WriteInt32((int)MessageType.Command.Control);
                
                if (playing_music)  //Stop the music
                {
                    client.Send("/start", 0);
                    m_scoremanage.SaveScore();

                    outMessage.WriteInt32((int)MessageType.ControlType.StopSong);
                }
                else   //Start the music
                {
                    client.Send("/start", 1);

                    outMessage.WriteInt32((int)MessageType.ControlType.PlaySong);
                    outMessage.WriteFloat(m_scoreMove.tempo);
                }

                m_udpHelper.Send(outMessage, m_appState.HololensIP, Constants.NETWORK_PORT);

                playing_music = !playing_music;
            }
        }

        private void OnApplicationQuit()
        {
            client.Send("/start", 0);
        }

        public void StopMusic()
        {

        }

        public void StartMusic()
        {

        }
    }
}
