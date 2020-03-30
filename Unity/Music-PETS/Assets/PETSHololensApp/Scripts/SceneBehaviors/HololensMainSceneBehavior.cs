using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

using Vuforia;

namespace HololensPETS
{
    public class HololensMainSceneBehavior : MonoBehaviour, IInputClickHandler, IHoldHandler
    {
        public Text debugText;

        public ImageTargetBehaviour marker;
        public GameObject coordinatesObject;
        
        public GameObject dynamicForceUI;
        public GameObject staticForceUI;
        public GameObject guitarHeroObj;
        public GameObject flappyBirdObj;
        public GameObject musicplayObj;

        private UDPHelper m_udpHelper;

        private bool m_doCalibrate = false;

        private HololensAppState m_appState;

        public GameObject fingerMarkerPrefab;
        private List<GameObject> m_fingerMarkers = new List<GameObject>();
        
        private int m_fingerBeingCalibrated = 0;

        private WorldAnchorManager m_worldAnchorManager;

        private void Awake()
        {
            m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();
            
            m_appState = GameObject.FindObjectOfType<HololensAppState>();
            
            m_worldAnchorManager = GameObject.FindObjectOfType<WorldAnchorManager>();
        }

        private void Start()
        {
            m_udpHelper.MessageReceived += HandleUDPMessageReceived;

            InputManager.Instance.AddGlobalListener(gameObject);

            dynamicForceUI.SetActive(false);
            staticForceUI.SetActive(false);
            guitarHeroObj.SetActive( false );
            flappyBirdObj.SetActive( false );
            musicplayObj.SetActive(false);


            coordinatesObject.SetActive( false );

            for( int i = 0; i < m_appState.FingerTransforms.Count; i++ )
            {
                GameObject fingerMarker = Instantiate( fingerMarkerPrefab );
                fingerMarker.transform.SetParent( m_appState.FingerTransforms[i].transform, false );
                fingerMarker.SetActive( false );
                m_fingerMarkers.Add( fingerMarker );
            }
        }

        private void Update()
        {
            if( m_appState.CurrentState.Value != HololensAppState.State.Calibrating )
            {
                if( CameraDevice.Instance.IsActive() )
                {
                    CameraDevice.Instance.Stop();
                }
            }

            if( m_appState.CurrentState.Value == HololensAppState.State.Running )
            {
                if( m_doCalibrate )
                {
                    for( int i = 0; i < m_appState.FingerTransforms.Count; i++ )
                    {
                        m_worldAnchorManager.RemoveAnchor(m_appState.FingerTransforms[i].gameObject);
                        m_appState.FingerTransforms[i].position = Vector3.zero;
                        m_appState.FingerTransforms[i].rotation = Quaternion.identity;
                    }
                    
                    m_appState.CurrentState.Value = HololensAppState.State.Calibrating;

                    m_doCalibrate = false;

                    m_fingerBeingCalibrated = 0;
                    
                    coordinatesObject.SetActive( true );
                    CameraDevice.Instance.Start();
                }
            }

            if( debugText != null )
            {
                string message = "State: " + m_appState.CurrentState.Value.ToString() + "\n";
                if( m_appState.CurrentState.Value == HololensAppState.State.Calibrating )
                {
                    message += "Finger being calibrated: " + m_fingerBeingCalibrated + "\n";
                }

                debugText.text = message;
            }
        }

        private void HandleUDPMessageReceived( NetInMessage message )
        {
            message.ResetIndex();

            MessageType.Command command = (MessageType.Command)message.ReadInt32();
            if(command == MessageType.Command.ModeSet)
            {
                MessageType.Mode mode = (MessageType.Mode)message.ReadInt32();

                dynamicForceUI.SetActive( mode == MessageType.Mode.Dynamic );
                staticForceUI.SetActive( mode == MessageType.Mode.Static );
                guitarHeroObj.SetActive( mode == MessageType.Mode.GuitarHero );
                flappyBirdObj.SetActive( mode == MessageType.Mode.FlappyBird );
                musicplayObj.SetActive(mode == MessageType.Mode.MusicPlay);
            }
        }

        public void OnInputClicked( InputClickedEventData eventData )
        {
            if ( m_appState.CurrentState.Value == HololensAppState.State.Calibrating)
            {
                if( marker.CurrentStatus == TrackableBehaviour.Status.TRACKED )
                {
                    if (m_fingerBeingCalibrated < m_appState.FingerTransforms.Count)
                    {
                        GameObject fingerMarker = m_fingerMarkers[m_fingerBeingCalibrated];
                        fingerMarker.SetActive(true);

                        Transform fingerPosition = m_appState.FingerTransforms[m_fingerBeingCalibrated];

                        Vector3 newPosition = marker.transform.position;
                        newPosition.y -= 0.03f;
                        fingerPosition.position = newPosition;

                        m_worldAnchorManager.AttachAnchor(fingerPosition.gameObject);
                        
                        m_fingerBeingCalibrated++;

                        if( m_fingerBeingCalibrated == m_appState.FingerTransforms.Count )
                        {
                            coordinatesObject.SetActive( false );
                            CameraDevice.Instance.Stop();

                            m_appState.CurrentState.Value = HololensAppState.State.ConfirmCalibration;
                        }
                    }
                }
            }
            else if ( m_appState.CurrentState.Value == HololensAppState.State.ConfirmCalibration)
            {
                for (int i = 0; i < m_fingerMarkers.Count; i++)
                {
                    m_fingerMarkers[i].SetActive(false);
                }

                m_appState.CurrentState.Value = HololensAppState.State.Running;
            }
        }

        public void OnHoldStarted(HoldEventData eventData)
        {
            m_doCalibrate = true;
        }

        public void OnHoldCompleted(HoldEventData eventData)
        {
        }

        public void OnHoldCanceled(HoldEventData eventData)
        {
        }
    }
}
