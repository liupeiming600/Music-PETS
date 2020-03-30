using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

namespace HololensPETS
{
    public class DynamicForcePanel : MonoBehaviour
    {
        public LineGraph lineGraph;

        public Toggle thumbToggle;
        public Toggle indexFingerToggle;
        public Toggle middleFingerToggle;
        public Toggle ringFingerToggle;
        public Toggle pinkyToggle;

        public Slider targetForceSlider;
        public InputField targetForceInput;

        public InputField staticPhaseDurationInput;
        public InputField increasingPhaseDurationInput;
        public InputField plateauPhaseDurationInput;
        public InputField decreasingPhaseDurationInput;
        public InputField staticEndPhaseDurationInput;

        public Button startButton;
        public Button stopButton;
        public Button resetGraphButton;
        private bool m_isRunning = false;
        
        private AppState m_appState;

        private UDPHelper m_udpHelper;

        private FingerForceDataProvider m_fingerForceDataProvider;

        // Boolean flag just to make sure that there won't be
        // an infinite loop when the value of the target force slider
        // changes as the result of the value of the target force input field
        // changing (and vice-versa).
        private bool m_targetForceUpdateLock = false;

        private float m_forceTrackTime = 0.0f;

        public Toggle GetFingerToggle(Finger finger)
        {
            switch (finger)
            {
                case Finger.Thumb:
                    return thumbToggle;
                case Finger.Index:
                    return indexFingerToggle;
                case Finger.Middle:
                    return middleFingerToggle;
                case Finger.Ring:
                    return ringFingerToggle;
                case Finger.Pinky:
                    return pinkyToggle;
                default:
                    return null;
            }
        }

        private void Awake()
        {
            m_appState = GameObject.FindObjectOfType<AppState>();
            m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();

            m_fingerForceDataProvider = GameObject.FindObjectOfType<FingerForceDataProvider>();
        }

        private void Start()
        {
            thumbToggle.onValueChanged.AddListener(FingerToggleValueChangedListener);
            indexFingerToggle.onValueChanged.AddListener(FingerToggleValueChangedListener);
            middleFingerToggle.onValueChanged.AddListener(FingerToggleValueChangedListener);
            ringFingerToggle.onValueChanged.AddListener(FingerToggleValueChangedListener);
            pinkyToggle.onValueChanged.AddListener(FingerToggleValueChangedListener);

            staticPhaseDurationInput.onEndEdit.AddListener(PhaseDurationInputValueChanged);
            increasingPhaseDurationInput.onEndEdit.AddListener(PhaseDurationInputValueChanged);
            plateauPhaseDurationInput.onEndEdit.AddListener(PhaseDurationInputValueChanged);
            decreasingPhaseDurationInput.onEndEdit.AddListener(PhaseDurationInputValueChanged);
            staticEndPhaseDurationInput.onEndEdit.AddListener(PhaseDurationInputValueChanged);

            startButton.onClick.AddListener( StartMeasurement );
            stopButton.onClick.AddListener( StopMeasurement );
            resetGraphButton.onClick.AddListener( ResetGraph );

            targetForceSlider.onValueChanged.AddListener(TargetForceSliderValueChanged);
            targetForceInput.onEndEdit.AddListener(TargetForceInputValueChanged);

            lineGraph.AddSeries(Constants.TARGET_FORCE_PLOT_SERIES_NAME, Color.green);
            lineGraph.AddSeries(Constants.FORCE_PLOT_SERIES_NAME, Color.red);
        }

        private void Update()
        {
            if (m_isRunning)
            {
                m_forceTrackTime += Time.deltaTime;
                float forceTrackDuration = m_appState.StaticPhaseDuration +
                    m_appState.IncreasingPhaseDuration +
                    m_appState.PlateauPhaseDuration +
                    m_appState.DecreasingPhaseDuration +
                    m_appState.StaticEndPhaseDuration;
                if (m_forceTrackTime >= forceTrackDuration)
                {
                    StopMeasurement();
                }
            }
        }

        private void OnEnable()
        {
            m_fingerForceDataProvider.OnFingerDataReceived += FingerForceDataReceivedHandler;

            m_appState.IsLeftHand.OnObservedValueChanged += IsLeftHandValueChangedListener;
        }

        private void OnDisable()
        {
            m_fingerForceDataProvider.OnFingerDataReceived -= FingerForceDataReceivedHandler;

            m_appState.IsLeftHand.OnObservedValueChanged -= IsLeftHandValueChangedListener;
        }

        private void IsLeftHandValueChangedListener( bool oldValue, bool newValue )
        {
            UpdateLineGraph();
        }

        private void FingerToggleValueChangedListener(bool newValue)
        {
            UpdateLineGraph();
        }
        
        private void UpdateLineGraph()
        {
            double totalMaxForce = 0.0;
            
            foreach ( Finger finger in Constants.FINGER_LIST )
            {
                totalMaxForce += m_appState.GetCurrentHandData().GetFingerMaxForce( finger ) - m_appState.GetCurrentHandData().GetFingerBaseForce( finger );
            }

            lineGraph.maxY = (float)totalMaxForce;
            lineGraph.ClearPlot( Constants.TARGET_FORCE_PLOT_SERIES_NAME );

            double plateauValue = GetPlateauPhaseValue();
            if (!double.IsNaN(plateauValue))
            {
                float timeStamp = 0.0f;

                lineGraph.Plot(0.0f, 0.0f, Constants.TARGET_FORCE_PLOT_SERIES_NAME);

                timeStamp += m_appState.StaticPhaseDuration;
                lineGraph.Plot(timeStamp, 0.0f, Constants.TARGET_FORCE_PLOT_SERIES_NAME);

                timeStamp += m_appState.IncreasingPhaseDuration;
                lineGraph.Plot(timeStamp, plateauValue, Constants.TARGET_FORCE_PLOT_SERIES_NAME);

                timeStamp += m_appState.PlateauPhaseDuration;
                lineGraph.Plot(timeStamp, plateauValue, Constants.TARGET_FORCE_PLOT_SERIES_NAME);

                timeStamp += m_appState.DecreasingPhaseDuration;
                lineGraph.Plot(timeStamp, 0.0f, Constants.TARGET_FORCE_PLOT_SERIES_NAME);

                timeStamp += m_appState.StaticEndPhaseDuration;
                lineGraph.Plot(timeStamp, 0.0f, Constants.TARGET_FORCE_PLOT_SERIES_NAME);

                lineGraph.maxX = timeStamp;
            }
        }

        private void TargetForceSliderValueChanged(float newValue)
        {
            // Check if this callback was called because of the change
            // in the value of the target force input field. If it is,
            // no need to change the input field text so as to avoid
            // an infinite loop (although this shouldn't really happen
            // since we use the onEndEdit event instead of onValueChanged.
            if (m_targetForceUpdateLock)
            {
                return;
            }

            if (targetForceInput != null)
            {
                targetForceInput.text = "" + newValue;
            }

            m_appState.ForceTrackMVCRatio = newValue / 100.0;

            UpdateLineGraph();
        }

        private void TargetForceInputValueChanged(string newValue)
        {
            if (targetForceSlider != null)
            {
                float val = float.Parse(newValue);
                m_targetForceUpdateLock = true;
                targetForceSlider.value = val;
                m_targetForceUpdateLock = false;

                m_appState.ForceTrackMVCRatio = val / 100.0;

                UpdateLineGraph();
            }
        }

        private void PhaseDurationInputValueChanged(string newValue)
        {
            m_appState.StaticPhaseDuration = float.Parse(staticPhaseDurationInput.text);
            m_appState.IncreasingPhaseDuration = float.Parse(increasingPhaseDurationInput.text);
            m_appState.PlateauPhaseDuration = float.Parse(plateauPhaseDurationInput.text);
            m_appState.DecreasingPhaseDuration = float.Parse(decreasingPhaseDurationInput.text);
            m_appState.StaticEndPhaseDuration = float.Parse(staticEndPhaseDurationInput.text);
            
            UpdateLineGraph();
            lineGraph.Refresh();
        }

        private double GetPlateauPhaseValue()
        {
            double ret = 0;

            if( thumbToggle.isOn )
            {
                ret += m_appState.GetCurrentHandData().GetFingerMaxForce(Finger.Thumb);
            }
            if( indexFingerToggle.isOn )
            {
                ret += m_appState.GetCurrentHandData().GetFingerMaxForce(Finger.Index);
            }
            if( middleFingerToggle.isOn )
            {
                ret += m_appState.GetCurrentHandData().GetFingerMaxForce(Finger.Middle);
            }
            if (ringFingerToggle.isOn)
            {
                ret += m_appState.GetCurrentHandData().GetFingerMaxForce(Finger.Ring);
            }
            if (pinkyToggle.isOn)
            {
                ret += m_appState.GetCurrentHandData().GetFingerMaxForce(Finger.Pinky);
            }

            if( !double.IsNaN( ret ) )
            {
                ret = ret * m_appState.ForceTrackMVCRatio;
            }

            return ret;
        }

        private void StartButtonClicked()
        {
            StartMeasurement();
        }

        private void StopButtonClicked()
        {
            StopMeasurement();
        }

        private void StartMeasurement()
        {
            m_isRunning = true;

            // Disable the start button if we're running the measurements.
            // Enable the stop button as a result.
            startButton.interactable = false;
            stopButton.interactable = true;

            m_forceTrackTime = 0.0f;

            lineGraph.ClearPlot(Constants.FORCE_PLOT_SERIES_NAME);

            NetOutMessage outMessage = new NetOutMessage();
            outMessage.WriteInt32((int)MessageType.Command.Control);
            outMessage.WriteInt32((int)MessageType.ControlType.Start);

            List<Finger> fingersToPress = new List<Finger>();
            for( int i = 0; i < 5; i++ )
            {
                Finger temp = (Finger)i;
                Toggle toggle = GetFingerToggle(temp);
                if( toggle.isOn )
                {
                    fingersToPress.Add(temp);
                }
            }

            outMessage.WriteInt32(fingersToPress.Count);
            for(int i = 0; i < fingersToPress.Count; i++)
            {
                outMessage.WriteInt32((int)fingersToPress[i]);
            }

            double totalMaxForce = 0.0;
            foreach ( Finger finger in Constants.FINGER_LIST )
            {
                totalMaxForce += m_appState.GetCurrentHandData().GetFingerMaxForce( finger ) - m_appState.GetCurrentHandData().GetFingerBaseForce( finger );
            }
            lineGraph.maxY = (float)totalMaxForce;

            outMessage.WriteDouble(totalMaxForce);
            outMessage.WriteFloat(m_appState.StaticPhaseDuration);
            outMessage.WriteFloat(m_appState.IncreasingPhaseDuration);
            outMessage.WriteFloat(m_appState.PlateauPhaseDuration);
            outMessage.WriteFloat(m_appState.DecreasingPhaseDuration);
            outMessage.WriteFloat(m_appState.StaticEndPhaseDuration);
            outMessage.WriteDouble(GetPlateauPhaseValue());

            m_udpHelper.Send(outMessage, m_appState.HololensIP, Constants.NETWORK_PORT );
        }

        private void StopMeasurement()
        {
            m_isRunning = false;

            // Enable the start button after stopping the measurement.
            // Disable the stop button as a result.
            startButton.interactable = true;
            stopButton.interactable = false;
        }

        private void ResetGraph()
        {
            lineGraph.ClearPlot( Constants.FORCE_PLOT_SERIES_NAME );
        }

        private void FingerForceDataReceivedHandler( Dictionary<Finger, double> fingerData )
        {
            if( m_isRunning )
            {
                double val = 0;
                foreach ( Finger finger in fingerData.Keys )
                {
                    double value = fingerData[finger] - m_appState.GetCurrentHandData().GetFingerBaseForce( finger );
                    if ( value < 0.0 )
                    {
                        value = 0.0;
                    }

                    Toggle fingerToggle = GetFingerToggle( finger );
                    if( fingerToggle != null && fingerToggle.isOn )
                    {
                        val += value;
                    }
                }

                lineGraph.Plot( m_forceTrackTime, val, Constants.FORCE_PLOT_SERIES_NAME );

                if( !string.IsNullOrEmpty( m_appState.HololensIP ) )
                {
                    NetOutMessage outMessage = new NetOutMessage();
                    outMessage.WriteInt32( (int)MessageType.Command.Data );
                    outMessage.WriteFloat( m_forceTrackTime );
                    outMessage.WriteFloat( (float)val );
                    m_udpHelper.Send( outMessage, m_appState.HololensIP, Constants.NETWORK_PORT );
                }
            }
        }
    }
}
