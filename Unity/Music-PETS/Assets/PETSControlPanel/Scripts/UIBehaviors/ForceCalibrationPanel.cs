using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace HololensPETS
{
    public class ForceCalibrationPanel : MonoBehaviour
    {
        //MaxForceText
        public Text leftThumbMaxForceText;
        public Text leftIndexFingerMaxForceText;
        public Text leftMiddleFingerMaxForceText;
        public Text leftRingFingerMaxForceText;
        public Text leftPinkyMaxForceText;

        public Text rightThumbMaxForceText;
        public Text rightIndexFingerMaxForceText;
        public Text rightMiddleFingerMaxForceText;
        public Text rightRingFingerMaxForceText;
        public Text rightPinkyMaxForceText;

        //CalibrationButton
        public Button leftThumbCalibrationButton;
        public Button leftIndexFingerCalibrationButton;
        public Button leftMiddleFingerCalibrationButton;
        public Button leftRingFingerCalibrationButton;
        public Button leftPinkyCalibrationButton;

        public Button rightThumbCalibrationButton;
        public Button rightIndexFingerCalibrationButton;
        public Button rightMiddleFingerCalibrationButton;
        public Button rightRingFingerCalibrationButton;
        public Button rightPinkyCalibrationButton;

        public Button calibratePETSButton;


        public Text instructionText;
        
        private AppState m_appState;

        private float m_calibrationDuration = 4.0f; // In seconds
        private float m_calibrationTimer = 0.0f;
        private bool m_isCalibrating = false;
        private bool m_isCalibratingPETS = false;
        private double weight = 1000;

        private bool m_isCalibratingLeftHand = true;
        private Finger m_calibratingFinger = Finger.Thumb;

        private FingerForceDataProvider m_fingerForceDataProvider;

        private PETSCalibrationTask m_petsCalibrationTask;

        private void Awake()
        {
            m_fingerForceDataProvider = GameObject.FindObjectOfType<FingerForceDataProvider>();

            m_appState = GameObject.FindObjectOfType<AppState>();

            m_petsCalibrationTask = new PETSCalibrationTask();
        }

        private void Start()
        {
            leftThumbCalibrationButton.onClick.AddListener( LeftThumbButtonClicked );
            leftIndexFingerCalibrationButton.onClick.AddListener( LeftIndexFingerButtonClicked );
            leftMiddleFingerCalibrationButton.onClick.AddListener( LeftMiddleFingerButtonClicked );
            leftRingFingerCalibrationButton.onClick.AddListener( LeftRingFingerButtonClicked );
            leftPinkyCalibrationButton.onClick.AddListener( LeftPinkyButtonClicked );

            rightThumbCalibrationButton.onClick.AddListener( RightThumbButtonClicked );
            rightIndexFingerCalibrationButton.onClick.AddListener( RightIndexFingerButtonClicked );
            rightMiddleFingerCalibrationButton.onClick.AddListener( RightMiddleFingerButtonClicked );
            rightRingFingerCalibrationButton.onClick.AddListener( RightRingFingerButtonClicked );
            rightPinkyCalibrationButton.onClick.AddListener( RightPinkyButtonClicked );

            calibratePETSButton.onClick.AddListener( CalibratePETS );
        }

        private void Update()
        {
            double leftThumbMax = m_appState.LeftHandData.GetFingerMaxForce( Finger.Thumb );
            double leftIndexFingerMax = m_appState.LeftHandData.GetFingerMaxForce( Finger.Index );
            double leftMiddleFingerMax = m_appState.LeftHandData.GetFingerMaxForce( Finger.Middle );
            double leftRingFingerMax = m_appState.LeftHandData.GetFingerMaxForce( Finger.Ring );
            double leftPinkyMax = m_appState.LeftHandData.GetFingerMaxForce( Finger.Pinky );

            leftThumbMaxForceText.text = MaxForceToString( leftThumbMax * weight);
            leftIndexFingerMaxForceText.text = MaxForceToString( leftIndexFingerMax * weight);
            leftMiddleFingerMaxForceText.text = MaxForceToString( leftMiddleFingerMax * weight);
            leftRingFingerMaxForceText.text = MaxForceToString( leftRingFingerMax * weight);
            leftPinkyMaxForceText.text = MaxForceToString( leftPinkyMax * weight);

            double rightThumbMax = m_appState.RightHandData.GetFingerMaxForce( Finger.Thumb );
            double rightIndexFingerMax = m_appState.RightHandData.GetFingerMaxForce( Finger.Index );
            double rightMiddleFingerMax = m_appState.RightHandData.GetFingerMaxForce( Finger.Middle );
            double rightRingFingerMax = m_appState.RightHandData.GetFingerMaxForce( Finger.Ring );
            double rightPinkyMax = m_appState.RightHandData.GetFingerMaxForce( Finger.Pinky );

            rightThumbMaxForceText.text = MaxForceToString( rightThumbMax * weight);
            rightIndexFingerMaxForceText.text = MaxForceToString( rightIndexFingerMax * weight);
            rightMiddleFingerMaxForceText.text = MaxForceToString( rightMiddleFingerMax * weight);
            rightRingFingerMaxForceText.text = MaxForceToString( rightRingFingerMax * weight);
            rightPinkyMaxForceText.text = MaxForceToString( rightPinkyMax * weight);

            if( m_isCalibrating )
            {
                m_calibrationTimer -= Time.deltaTime;
                if( m_calibrationTimer <= 0.0f )
                {
                    m_isCalibrating = false;

                    leftThumbCalibrationButton.interactable = true;
                    leftIndexFingerCalibrationButton.interactable = true;
                    leftMiddleFingerCalibrationButton.interactable = true;
                    leftRingFingerCalibrationButton.interactable = true;
                    leftPinkyCalibrationButton.interactable = true;

                    rightThumbCalibrationButton.interactable = true;
                    rightIndexFingerCalibrationButton.interactable = true;
                    rightMiddleFingerCalibrationButton.interactable = true;
                    rightRingFingerCalibrationButton.interactable = true;
                    rightPinkyCalibrationButton.interactable = true;

                    instructionText.text = "Force calibration for " + ( m_isCalibratingLeftHand ? "left " : "right " ) + m_calibratingFinger.ToString().ToLower() + " finger done!";
                }
            }
        }

        private string MaxForceToString( double value )
        {
            return double.IsNaN( value ) ? "Uncalibrated" : value.ToString( "F3" );
        }

        private void OnEnable()
        {
            m_fingerForceDataProvider.OnFingerDataReceived += FingerForceDataReceivedHandler;
        }

        private void OnDisable()
        {
            m_fingerForceDataProvider.OnFingerDataReceived -= FingerForceDataReceivedHandler;
        }

        private void LeftThumbButtonClicked()
        {
            StartCalibration( Finger.Thumb, true );
        }

        private void LeftIndexFingerButtonClicked()
        {
            StartCalibration( Finger.Index, true );
        }

        private void LeftMiddleFingerButtonClicked()
        {
            StartCalibration( Finger.Middle, true );
        }

        private void LeftRingFingerButtonClicked()
        {
            StartCalibration( Finger.Ring, true );
        }

        private void LeftPinkyButtonClicked()
        {
            StartCalibration( Finger.Pinky, true );
        }

        private void RightThumbButtonClicked()
        {
            StartCalibration( Finger.Thumb, false );
        }

        private void RightIndexFingerButtonClicked()
        {
            StartCalibration( Finger.Index, false );
        }

        private void RightMiddleFingerButtonClicked()
        {
            StartCalibration( Finger.Middle, false );
        }

        private void RightRingFingerButtonClicked()
        {
            StartCalibration( Finger.Ring, false );
        }

        private void RightPinkyButtonClicked()
        {
            StartCalibration( Finger.Pinky, false );
        }

        private void StartCalibration( Finger finger, bool isLeftHand )
        {
            if( m_isCalibrating )
            {
                return;
            }
            m_isCalibrating = true;
            m_isCalibratingLeftHand = isLeftHand;
            m_calibratingFinger = finger;
            m_calibrationTimer = m_calibrationDuration;

            leftThumbCalibrationButton.interactable = false;
            leftIndexFingerCalibrationButton.interactable = false;
            leftMiddleFingerCalibrationButton.interactable = false;
            leftRingFingerCalibrationButton.interactable = false;
            leftPinkyCalibrationButton.interactable = false;

            rightThumbCalibrationButton.interactable = false;
            rightIndexFingerCalibrationButton.interactable = false;
            rightMiddleFingerCalibrationButton.interactable = false;
            rightRingFingerCalibrationButton.interactable = false;
            rightPinkyCalibrationButton.interactable = false;

            instructionText.text = "Press with your " + ( m_isCalibratingLeftHand ? "left " : "right " ) + m_calibratingFinger.ToString().ToLower() + " finger as hard as possible.";
        }

        private void FingerForceDataReceivedHandler( Dictionary<Finger, double> fingerData )
        {
            if( m_isCalibrating || m_isCalibratingPETS )
            {
                foreach( Finger finger in fingerData.Keys )
                {
                    double value = fingerData[finger];

                    if ( m_isCalibrating )
                    {
                        if ( finger == m_calibratingFinger )
                        {
                            HandData handData = m_isCalibratingLeftHand ? m_appState.LeftHandData : m_appState.RightHandData;
                            value -= handData.GetFingerBaseForce( finger );

                            double currentMax = handData.GetFingerMaxForce( finger );
                            handData.SetFingerMaxForce( finger, double.IsNaN( currentMax ) ? value : MathUtils.Max( value, currentMax ) );
                        }
                    }
                    else if( m_isCalibratingPETS )
                    {
                        m_petsCalibrationTask.FeedSample( finger, value );
                    }
                }

                if( m_isCalibratingPETS )
                {
                    if ( m_petsCalibrationTask.IsDone() )
                    {
                        m_isCalibratingPETS = false;
                        OnPETSCalibrationFinished();

                        Dictionary<Finger, double> calibrationResult = m_petsCalibrationTask.GetCalibrationResult();
                        if ( calibrationResult != null )
                        {
                            foreach ( Finger finger in calibrationResult.Keys )
                            {
                                m_appState.LeftHandData.SetFingerBaseForce( finger, calibrationResult[finger] );
                                m_appState.RightHandData.SetFingerBaseForce( finger, calibrationResult[finger] );
                            }
                        }
                        else
                        {
                            Debug.LogError( "Failed to calibrate PETS!" );
                        }
                    }
                }
            }
        }

        public void CalibratePETS()
        {
            m_petsCalibrationTask.Start( 100 );
            m_isCalibratingPETS = true;

            OnPETSCalibrationStarted();
        }

        private void OnPETSCalibrationStarted()
        {
            calibratePETSButton.interactable = false;

            leftThumbCalibrationButton.interactable = false;
            leftIndexFingerCalibrationButton.interactable = false;
            leftMiddleFingerCalibrationButton.interactable = false;
            leftRingFingerCalibrationButton.interactable = false;
            leftPinkyCalibrationButton.interactable = false;

            rightThumbCalibrationButton.interactable = false;
            rightIndexFingerCalibrationButton.interactable = false;
            rightMiddleFingerCalibrationButton.interactable = false;
            rightRingFingerCalibrationButton.interactable = false;
            rightPinkyCalibrationButton.interactable = false;
        }

        private void OnPETSCalibrationFinished()
        {
            calibratePETSButton.interactable = true;

            leftThumbCalibrationButton.interactable = true;
            leftIndexFingerCalibrationButton.interactable = true;
            leftMiddleFingerCalibrationButton.interactable = true;
            leftRingFingerCalibrationButton.interactable = true;
            leftPinkyCalibrationButton.interactable = true;

            rightThumbCalibrationButton.interactable = true;
            rightIndexFingerCalibrationButton.interactable = true;
            rightMiddleFingerCalibrationButton.interactable = true;
            rightRingFingerCalibrationButton.interactable = true;
            rightPinkyCalibrationButton.interactable = true;
        }
    }
}
