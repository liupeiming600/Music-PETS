using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using HoloToolkit.Unity;

using Vuforia;

namespace HololensPETS
{
    public class HololensStartSceneBehavior : MonoBehaviour
    {
        public Text statusDisplayText;

        public List<string> FingerAnchorNames = new List<string>
        {
            "FingerAnchor1", "FingerAnchor2", "FingerAnchor3", "FingerAnchor4", "FingerAnchor5"
        };

        private HololensAppState m_appState;

        private WorldAnchorManager m_worldAnchorManager;

        private void Awake()
        {
            m_appState = GameObject.FindObjectOfType<HololensAppState>();

            m_worldAnchorManager = GameObject.FindObjectOfType<WorldAnchorManager>();
        }

        private void Start()
        {
            m_appState.CurrentState.Set( HololensAppState.State.Start, false );
        }

        private void Update()
        {
            if( m_appState.CurrentState.Value == HololensAppState.State.Start )
            {
                UpdateStatusText( "Initialized finger transforms..." );
                for( int i = 0; i < 5; i++ )
                {
                    GameObject fingerLocationAnchor = new GameObject( FingerAnchorNames[i] );
                    fingerLocationAnchor.transform.position = Vector3.zero;
                    fingerLocationAnchor.transform.rotation = Quaternion.identity;
                    fingerLocationAnchor.AddComponent<DontDestroyOnLoad>();
                    m_appState.FingerTransforms.Add( fingerLocationAnchor.transform );
                }

#if !UNITY_EDITOR && UNITY_WSA
                UpdateStatusText( "Waiting for WorldAnchorStore..." );
                m_appState.CurrentState.Value = HololensAppState.State.WaitingForAnchor;
#else
                m_appState.CurrentState.Value = HololensAppState.State.Running;

                UpdateStatusText( "Loading main scene..." );

                // Load main scene
                SceneManager.LoadScene( "HololensMainScene" );
#endif
            }
            else if( m_appState.CurrentState.Value == HololensAppState.State.WaitingForAnchor )
            {
                if( m_worldAnchorManager.AnchorStore != null )
                {
                    UpdateStatusText( "WorldAnchorStore available! Loading anchors..." );

                    // Load anchors for the finger transforms
                    for( int i = 0; i < FingerAnchorNames.Count; i++ )
                    {
                        m_worldAnchorManager.AttachAnchor( m_appState.FingerTransforms[i].gameObject );
                    }

                    m_appState.CurrentState.Value = HololensAppState.State.InitializingVuforia;
                }
            }
            else if( m_appState.CurrentState.Value == HololensAppState.State.InitializingVuforia )
            {
                VuforiaRuntime.InitState vuforiaInitState = VuforiaRuntime.Instance.InitializationState;
                if( vuforiaInitState == VuforiaRuntime.InitState.NOT_INITIALIZED )
                {
                    VuforiaRuntime.Instance.InitVuforia();

                    UpdateStatusText( "Initializing Vuforia..." );
                }
                else if( vuforiaInitState == VuforiaRuntime.InitState.INITIALIZED )
                {
                    UpdateStatusText( "Vuforia initialized!" );

                    CameraDevice.Instance.Stop();

                    m_appState.CurrentState.Value = HololensAppState.State.Running;
                }
            }
            else if( m_appState.CurrentState.Value == HololensAppState.State.Running )
            {
                UpdateStatusText( "Loading main scene..." );

                // Load main scene
                SceneManager.LoadScene( "HololensMainScene" );
            }
        }

        private void UpdateStatusText( string text, bool append = true )
        {
            if( statusDisplayText != null )
            {
                if( !append )
                {
                    statusDisplayText.text = "";
                }

                statusDisplayText.text += text + "\n";
            }
        }
    }
}
