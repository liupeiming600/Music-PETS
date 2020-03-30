using UnityEngine;
using UnityEngine.UI;

namespace HololensPETS
{
    public class DynamicForceUIHololens : MonoBehaviour
    {
        public LineGraph lineGraph;

        public Indicator thumbIndicator;
        public Indicator indexFingerIndicator;
        public Indicator middleFingerIndicator;
        public Indicator ringFingerIndicator;
        public Indicator pinkyIndicator;

        private UDPHelper m_udpHelper;

        private HololensAppState m_appState;
        
        private bool m_isRunning = false;
        private float m_time = 0.0f;

        private void Awake()
        {
            m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();

            m_appState = GameObject.FindObjectOfType<HololensAppState>();
        }

        private void Start()
        {
            lineGraph.AddSeries(Constants.TARGET_FORCE_PLOT_SERIES_NAME, Color.green);
            lineGraph.AddSeries(Constants.FORCE_PLOT_SERIES_NAME, Color.red);
        }

        private void Update()
        {
            if ( m_isRunning )
            {
                m_time += Time.deltaTime;
            }
        }

        private void OnEnable()
        {
            m_udpHelper.MessageReceived += UDPMessageReceived;

            for( int i = 0; i < 5; i++ )
            {
                GetFingerIndicator( (Finger)i ).transform.SetParent( m_appState.FingerTransforms[i] );
                GetFingerIndicator( (Finger)i ).transform.localPosition = Vector3.zero;
            }

            transform.position = m_appState.FingerTransforms[2].position + Vector3.up * 0.07f;
            transform.localScale = new Vector3( 0.0002f, 0.0002f, 0.0002f );

            ClearIndicators();
            HideIndicators();
        }

        private void OnDisable()
        {
            m_udpHelper.MessageReceived -= UDPMessageReceived;

            ClearIndicators();
            HideIndicators();
        }

        private Indicator GetFingerIndicator(Finger finger)
        {
            switch(finger)
            {
                case Finger.Thumb:
                    return thumbIndicator;
                case Finger.Index:
                    return indexFingerIndicator;
                case Finger.Middle:
                    return middleFingerIndicator;
                case Finger.Ring:
                    return ringFingerIndicator;
                case Finger.Pinky:
                    return pinkyIndicator;
                default:
                    return null;
            }
        }

        private void ClearIndicators()
        {
            for(int i = 0; i < 5; i++)
            {
                GetFingerIndicator((Finger)i).SetIsOn(false);
            }
        }

        private void HideIndicators()
        {
            for (int i = 0; i < 5; i++)
            {
                GetFingerIndicator((Finger)i).IsVisible = false;
            }
        }

        private void UDPMessageReceived(NetInMessage message)
        {
            MessageType.Command commandType = (MessageType.Command)message.ReadInt32();
            if(commandType == MessageType.Command.Data)
            {
                if( m_isRunning )
                {
                    float timeX = message.ReadFloat();
                    float valueToPlot = message.ReadFloat();
                    lineGraph.Plot(timeX, valueToPlot, Constants.FORCE_PLOT_SERIES_NAME);
                }
            }
            else if(commandType == MessageType.Command.Control)
            {
                MessageType.ControlType controlType = (MessageType.ControlType)message.ReadInt32();
                if (controlType == MessageType.ControlType.Start)
                {
                    m_isRunning = true;
                    m_time = 0.0f;

                    ClearIndicators();
                    HideIndicators();
                    
                    int numFingersToPress = message.ReadInt32();
                    for( int i = 0; i < numFingersToPress; i++ )
                    {
                        int fingerIndex = message.ReadInt32();
                        Indicator indicator = GetFingerIndicator((Finger)fingerIndex);
                        if(indicator != null)
                        {
                            indicator.IsVisible = true;
                            indicator.SetIsOn(true);
                        }
                    }

                    double maximumPlotValue = message.ReadDouble();
                    float staticPhaseDuration = message.ReadFloat();
                    float increasingPhaseDuration = message.ReadFloat();
                    float plateauPhaseDuration = message.ReadFloat();
                    float decreasingPhaseDuration = message.ReadFloat();
                    float staticEndPhaseDuration = message.ReadFloat();

                    double plateauPhaseValue = message.ReadDouble();

                    float measurementDuration = staticPhaseDuration +
                        increasingPhaseDuration +
                        plateauPhaseDuration +
                        decreasingPhaseDuration +
                        staticEndPhaseDuration;

                    lineGraph.maxX = measurementDuration;
                    lineGraph.maxY = (float)maximumPlotValue;
                    lineGraph.Refresh();

                    lineGraph.ClearPlot(Constants.TARGET_FORCE_PLOT_SERIES_NAME);
                    lineGraph.ClearPlot(Constants.FORCE_PLOT_SERIES_NAME);

                    float timeStamp = 0.0f;
                    lineGraph.Plot(0.0f, 0.0f, Constants.TARGET_FORCE_PLOT_SERIES_NAME);

                    timeStamp += staticPhaseDuration;
                    lineGraph.Plot(timeStamp, 0.0f, Constants.TARGET_FORCE_PLOT_SERIES_NAME);

                    timeStamp += increasingPhaseDuration;
                    lineGraph.Plot(timeStamp, plateauPhaseValue, Constants.TARGET_FORCE_PLOT_SERIES_NAME);

                    timeStamp += plateauPhaseDuration;
                    lineGraph.Plot(timeStamp, plateauPhaseValue, Constants.TARGET_FORCE_PLOT_SERIES_NAME);

                    timeStamp += decreasingPhaseDuration;
                    lineGraph.Plot(timeStamp, 0.0f, Constants.TARGET_FORCE_PLOT_SERIES_NAME);

                    timeStamp += staticEndPhaseDuration;
                    lineGraph.Plot(timeStamp, 0.0f, Constants.TARGET_FORCE_PLOT_SERIES_NAME);

                    m_time = 0.0f;
                }
                else if (controlType == MessageType.ControlType.Stop)
                {
                    m_isRunning = false;

                    ClearIndicators();
                    HideIndicators();
                }
            }
        }
    }
}
