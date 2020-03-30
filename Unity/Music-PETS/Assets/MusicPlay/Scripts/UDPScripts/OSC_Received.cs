using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MusicPlay;
using uOSC;
using HololensPETS;

public class OSC_Received : MonoBehaviour {

    ScoreMove m_scoreMove;

    private AppState m_appState;

    private UDPHelper m_udpHelper;

    public float SongTempo = 120;
    float preTempo = 100;

	// Use this for initialization
	void Start () {
        m_scoreMove = GameObject.FindObjectOfType<ScoreMove>();

        m_appState = GameObject.FindObjectOfType<AppState>();

        m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();

        var server = GetComponent<uOscServer>();
        server.onDataReceived.AddListener(OnDataReceived);
	}
	
    void OnDataReceived(Message message)
    {
        if(message.address == "/tempo")
        {
            foreach (var value in message.values)
            {
                SongTempo = (float)value;
                if (SongTempo != preTempo) SetTempo();
                preTempo = SongTempo;
            }
        }
    }

    public void SetTempo()
    {
        m_scoreMove.ChangeDy(SongTempo);

        //Send to HoloLens
        NetOutMessage outMessage = new NetOutMessage();
        outMessage.WriteInt32((int)MessageType.Command.Control);
        outMessage.WriteInt32((int)MessageType.ControlType.ChangeTempo);

        outMessage.WriteFloat(SongTempo);

        m_udpHelper.Send(outMessage, m_appState.HololensIP, Constants.NETWORK_PORT);
    }
}
