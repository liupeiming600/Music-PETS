using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

namespace HololensPETS
{
    public class StaticForcePanel : MonoBehaviour
    {
        public BarGraph thumbGraph;
        public BarGraph indexFingerGraph;
        public BarGraph middleFingerGraph;
        public BarGraph ringFingerGraph;
        public BarGraph pinkyGraph;

        public InputField thumbForceInput;
        public InputField indexFingerForceInput;
        public InputField middleFingerForceInput;
        public InputField ringFingerForceInput;
        public InputField pinkyForceInput;

        public InputField thumbMinForceInput;
        public InputField indexFingerMinForceInput;
        public InputField middleFingerMinForceInput;
        public InputField ringFingerMinForceInput;
        public InputField pinkyMinForceInput;

        public InputField thumbOrderInput;
        public InputField indexFingerOrderInput;
        public InputField middleFingerOrderInput;
        public InputField ringFingerOrderInput;
        public InputField pinkyOrderInput;

        public InputField measurementDurationInput;

        public Indicator thumbIndicator;
        public Indicator indexFingerIndicator;
        public Indicator middleFingerIndicator;
        public Indicator ringFingerIndicator;
        public Indicator pinkyIndicator;

        public Button startButton;
        public Button stopButton;
        private bool m_isRunning = false;

        private AppState m_appState;

        private FingerForceDataProvider m_fingerForceDataProvider;
        private UDPHelper m_udpHelper;

        private float m_measurementDuration;
        private float m_measurementTimer = 0.0f;

        private List<Finger> m_remainingFingers;
        private int m_currentFingerIndex = -1;

        public BarGraph GetFingerGraph( Finger finger )
        {
            switch (finger)
            {
                case Finger.Thumb:
                    return thumbGraph;
                case Finger.Index:
                    return indexFingerGraph;
                case Finger.Middle:
                    return middleFingerGraph;
                case Finger.Ring:
                    return ringFingerGraph;
                case Finger.Pinky:
                    return pinkyGraph;
                default:
                    return null;
            }
        }

        public InputField GetFingerInputField( Finger finger )
        {
            switch (finger)
            {
                case Finger.Thumb:
                    return thumbForceInput;
                case Finger.Index:
                    return indexFingerForceInput;
                case Finger.Middle:
                    return middleFingerForceInput;
                case Finger.Ring:
                    return ringFingerForceInput;
                case Finger.Pinky:
                    return pinkyForceInput;
                default:
                    return null;
            }
        }

        public InputField GetFingerMinForceInputField(Finger finger)
        {
            switch (finger)
            {
                case Finger.Thumb:
                    return thumbMinForceInput;
                case Finger.Index:
                    return indexFingerMinForceInput;
                case Finger.Middle:
                    return middleFingerMinForceInput;
                case Finger.Ring:
                    return ringFingerMinForceInput;
                case Finger.Pinky:
                    return pinkyMinForceInput;
                default:
                    return null;
            }
        }

        private void Awake()
        {
            m_appState = GameObject.FindObjectOfType<AppState>();
            m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();

            m_fingerForceDataProvider = GameObject.FindObjectOfType<FingerForceDataProvider>();

            m_remainingFingers = new List<Finger>();
        }

        private int GetFingerOrder(Finger finger)
        {
            switch(finger)
            {
                case Finger.Thumb:
                    return string.IsNullOrEmpty(thumbOrderInput.text) ? -1 : int.Parse(thumbOrderInput.text);
                case Finger.Index:
                    return string.IsNullOrEmpty(indexFingerOrderInput.text) ? -1 : int.Parse(indexFingerOrderInput.text);
                case Finger.Middle:
                    return string.IsNullOrEmpty(middleFingerOrderInput.text) ? -1 : int.Parse(middleFingerOrderInput.text);
                case Finger.Ring:
                    return string.IsNullOrEmpty(ringFingerOrderInput.text) ? -1 : int.Parse(ringFingerOrderInput.text);
                case Finger.Pinky:
                    return string.IsNullOrEmpty(pinkyOrderInput.text) ? -1 : int.Parse(pinkyOrderInput.text);
            }

            return -1;
        }

        private int GetNextFingerIndex()
        {
            int minimumOrder = int.MaxValue;
            int minimumIndex = -1;

            for( int i = 0; i < m_remainingFingers.Count; i++ )
            {
                Finger temp = m_remainingFingers[i];
                int fingerOrder = GetFingerOrder(temp);
                int currentFingerOrder = GetFingerOrder((Finger)m_currentFingerIndex);

                if( (fingerOrder != -1) && (fingerOrder >= currentFingerOrder) )
                {
                    if (fingerOrder <= minimumOrder)
                    {
                        minimumOrder = fingerOrder;
                        minimumIndex = i;
                    }
                }
            }

            if( minimumIndex != -1 )
            {
                int ret = (int)m_remainingFingers[minimumIndex];
                m_remainingFingers.RemoveAt(minimumIndex);
                return ret;
            }

            return -1;
        }

        private void Start()
        {
            InitListeners();

            startButton.interactable = true;
            stopButton.interactable = false;
        }

        private void InitListeners()
        {
            startButton.onClick.AddListener(StartButtonClicked);
            stopButton.onClick.AddListener(StopButtonClicked);

            measurementDurationInput.onEndEdit.AddListener(MeasurementDurationInputValueChangedListener);
        }

        private void ResetGraphs()
        {
            for(int i = 0; i < 5; i++)
            {
                Finger temp = (Finger)i;

                BarGraph fingerGraph = GetFingerGraph(temp);
                fingerGraph.value = fingerGraph.min;
                fingerGraph.targetValue = fingerGraph.min;
            }
        }

        private void Update()
        {
            if( m_isRunning )
            {
                m_measurementTimer += Time.deltaTime;
                if( m_measurementTimer >= m_measurementDuration )
                {
                    m_currentFingerIndex = GetNextFingerIndex();
                    if (m_currentFingerIndex != -1)
                    {
                        m_measurementTimer = 0.0f;

                        UpdateIndicators();
                        UpdateTargetForceLines();

                        NetOutMessage message = new NetOutMessage();
                        message.WriteInt32((int)MessageType.Command.Control);
                        message.WriteInt32((int)MessageType.ControlType.Start);
                        message.WriteInt32(m_currentFingerIndex);
                        for (int i = 0; i < 5; i++)
                        {
                            message.WriteInt32(i);
                            message.WriteDouble(GetFingerGraph((Finger)i).targetValue);
                        }
                        m_udpHelper.Send(message, m_appState.HololensIP, Constants.NETWORK_PORT );
                    }
                    else
                    {
                        StopMeasurement();
                    }
                }
            }
        }

        private void OnEnable()
        {
            m_fingerForceDataProvider.OnFingerDataReceived += FingerForceDataReceivedHandler;

            m_measurementDuration = float.Parse( measurementDurationInput.text );
        }

        private void OnDisable()
        {
            m_fingerForceDataProvider.OnFingerDataReceived -= FingerForceDataReceivedHandler;
        }
        
        private void StartMeasurement()
        {
            m_remainingFingers.Clear();
            for (int i = 0; i < 5; i++)
            {
                m_remainingFingers.Add((Finger)i);
            }

            m_currentFingerIndex = GetNextFingerIndex();
            if( m_currentFingerIndex < 0 )
            {
                return;
            }

            m_isRunning = true;

            // Disable the start button if we're running the measurements.
            // Enable the stop button as a result.
            startButton.interactable = false;
            stopButton.interactable = true;
            
            m_measurementTimer = 0.0f;

            UpdateIndicators();
            UpdateTargetForceLines();

            NetOutMessage message = new NetOutMessage();
            message.WriteInt32((int)MessageType.Command.Control);
            message.WriteInt32((int)MessageType.ControlType.Start);
            message.WriteInt32(m_currentFingerIndex);
            for(int i = 0; i < 5; i++)
            {
                message.WriteInt32(i);
                message.WriteDouble(GetFingerGraph((Finger)i).targetValue);
            }
            m_udpHelper.Send(message, m_appState.HololensIP, Constants.NETWORK_PORT );
        }

        private void StopMeasurement()
        {
            m_isRunning = false;

            // Enable the start button after stopping the measurement.
            // Disable the stop button as a result.
            startButton.interactable = true;
            stopButton.interactable = false;

            UpdateIndicators();
            ResetGraphs();

            NetOutMessage message = new NetOutMessage();
            message.WriteInt32((int)MessageType.Command.Control);
            message.WriteInt32((int)MessageType.ControlType.Stop);
            m_udpHelper.Send(message, m_appState.HololensIP, Constants.NETWORK_PORT );
        }

        private void UpdateIndicators()
        {
            thumbIndicator.SetIsOn((m_currentFingerIndex != -1) && (m_currentFingerIndex == (int)Finger.Thumb));
            indexFingerIndicator.SetIsOn((m_currentFingerIndex != -1) && (m_currentFingerIndex == (int)Finger.Index));
            middleFingerIndicator.SetIsOn((m_currentFingerIndex != -1) && (m_currentFingerIndex == (int)Finger.Middle));
            ringFingerIndicator.SetIsOn((m_currentFingerIndex != -1) && (m_currentFingerIndex == (int)Finger.Ring));
            pinkyIndicator.SetIsOn((m_currentFingerIndex != -1) && (m_currentFingerIndex == (int)Finger.Pinky));
        }

        private void UpdateTargetForceLines()
        {
            for( int i = 0; i < 5; i++ )
            {
                Finger temp = (Finger)i;
                if (m_currentFingerIndex != i)
                {
                    GetFingerGraph(temp).targetValue = float.Parse(GetFingerMinForceInputField(temp).text);
                }
                else
                {
                    GetFingerGraph(temp).targetValue = float.Parse(GetFingerInputField(temp).text);
                }
            }
        }

        #region Listener implementations
        private void StartButtonClicked()
        {
            StartMeasurement();
        }

        private void StopButtonClicked()
        {
            StopMeasurement();
        }
        
        private void MeasurementDurationInputValueChangedListener(string newValue)
        {
            m_measurementDuration = float.Parse(newValue);
        }

        private void FingerForceDataReceivedHandler( Dictionary<Finger, double> fingerData )
        {
            if( m_isRunning )
            {
                NetOutMessage outMessage = new NetOutMessage();
                outMessage.WriteInt32( (int)MessageType.Command.Data );

                foreach( Finger finger in fingerData.Keys )
                {
                    int fingerNumber = (int)finger;
                    double value = fingerData[finger] - m_appState.GetCurrentHandData().GetFingerBaseForce( finger );
                    if ( value < 0.0 )
                    {
                        value = 0.0;
                    }

                    BarGraph fingerGraph = GetFingerGraph( finger );
                    double maxForce = m_appState.GetCurrentHandData().GetFingerMaxForce( finger );
                    float plotValue = 0.0f;
                    if( !double.IsNaN( maxForce ) )
                    {
                        plotValue = (float)( value / maxForce * 100 );
                        fingerGraph.PlotY( plotValue );
                    }

                    outMessage.WriteInt32( fingerNumber );
                    outMessage.WriteFloat( plotValue );
                }

                m_udpHelper.Send( outMessage, m_appState.HololensIP, Constants.NETWORK_PORT );
            }
        }
        #endregion
    }
}
