using UnityEngine;

using HololensPETS;

namespace HololensPETSTest
{
    /**
     * UDPTestScene is the implementation of a scene
     * that was used to test the initial implementation of
     * the line graph class, as well as the communication between
     * LabWindowsCVI and Unity via UDP connection.
     */
    public class UDPTestScene : MonoBehaviour
    {
        public LineGraph lineGraph1;
        public LineGraph lineGraph2;
        public LineGraph lineGraph3;
        public LineGraph lineGraph4;
        public LineGraph lineGraph5;

        private UDPHelper udpHelper;
        
        private float t = 0.0f;

        private void Awake()
        {
            udpHelper = GameObject.FindObjectOfType<UDPHelper>();
            udpHelper.MessageReceived += UDPMessageReceivedHandler;
        }

        private void Update()
        {
            t += Time.deltaTime;
        }

        private void UDPMessageReceivedHandler( NetInMessage message )
        {
            int fingerNum = message.ReadInt32();
            double voltage = message.ReadDouble();
            switch ( fingerNum )
            {
                case 1:
                    lineGraph1.Plot(t, voltage);
                    break;
                case 2:
                    lineGraph2.Plot(t, voltage);
                    break;
                case 3:
                    lineGraph3.Plot(t, voltage);
                    break;
                case 4:
                    lineGraph4.Plot(t, voltage);
                    break;
                case 5:
                    lineGraph5.Plot(t, voltage);
                    break;
            }
        }
    }
}
