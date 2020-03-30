using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace HololensPETS
{
    public class PCControlPanelScene : MonoBehaviour
    {
        private AppState m_appState;

        public Toggle dynamicPanelToggle;
        public Toggle staticPanelToggle;
        public Toggle forceCalibrationToggle;
        public Toggle rhythmGameToggle;
        public Toggle flappyBirdToggle;
        public Toggle musicPlayToggle;

        public Toggle leftHandToggle;
        public Toggle rightHandToggle;
        
        public string sequentialTestingSceneName = "SequentialTestingControlPanelScene";
        public string dynamicForceTrackingSceneName = "DynamicForceTrackingControlPanelScene";
        public string forceCalibrationSceneName = "ForceCalibrationScene";
        public string rhythmGameSceneName = "RhythmGameControlPanelScene";
        public string flappyBirdGameSceneName = "FlappyBirdControlPanelScene";
        public string musicplayGameSceneName = "MusicPlayControlPanelScene";

        private string m_currentSceneName = string.Empty;

        public InputField hololensIPInput;

        private UDPHelper m_udpHelper;

        private void LoadScene( string sceneName )
        {
            if( !string.IsNullOrEmpty( m_currentSceneName ) )
            {
                SceneManager.UnloadSceneAsync( m_currentSceneName );
            }

            m_currentSceneName = sceneName;
            
            SceneManager.LoadScene( sceneName, LoadSceneMode.Additive );
        }

        private void Awake()
        {
            m_appState = GameObject.FindObjectOfType<AppState>();

            m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();
        }

        private void Start()
        {
            Init(); //Toggleクリック時の実行内容を設定．ここで設定した内容は毎フレーム有効．
        }

        private void Init()
        {
            if( dynamicPanelToggle != null )
            {
                dynamicPanelToggle.onValueChanged.AddListener( DynamicPanelToggleValueChanged );
                if( dynamicPanelToggle.isOn )
                {
                    SetMode( AppMode.Dynamic );
                }
            }

            if( staticPanelToggle != null )
            {
                staticPanelToggle.onValueChanged.AddListener( StaticPanelToggleValueChanged );
                if( staticPanelToggle.isOn )
                {
                    SetMode( AppMode.Static );
                }
            }

            if( forceCalibrationToggle != null )
            {
                forceCalibrationToggle.onValueChanged.AddListener( ForceCalibrationToggleValueChanged );
                if( forceCalibrationToggle.isOn )
                {
                    SetMode( AppMode.Calibration );
                }
            }

            if( rhythmGameToggle != null )
            {
                rhythmGameToggle.onValueChanged.AddListener( RhythmGameToggleValueChanged );
                if( rhythmGameToggle.isOn )
                {
                    SetMode( AppMode.RhythmGame );
                }
            }

            if( flappyBirdToggle != null )
            {
                flappyBirdToggle.onValueChanged.AddListener( FlappyBirdToggleValueChanged );
                if( flappyBirdToggle.isOn )
                {
                    SetMode( AppMode.FlappyBird );
                }
            }

            if (musicPlayToggle != null)
            {
                musicPlayToggle.onValueChanged.AddListener(MusicPlayToggleValueChanged);
                if (musicPlayToggle.isOn)
                {
                    SetMode(AppMode.MusicPlay);
                }
            }

            if ( leftHandToggle != null )
            {
                leftHandToggle.onValueChanged.AddListener( LeftHandToggleValueChanged );
                if( leftHandToggle.isOn )
                {
                    SetIsLeftHand( true );
                }
            }

            if( rightHandToggle != null )
            {
                rightHandToggle.onValueChanged.AddListener( RightHandToggleValueChanged );
                if( rightHandToggle.isOn )
                {
                    SetIsLeftHand( false );
                }
            }

            hololensIPInput.onEndEdit.AddListener( HololensIPInputValueChanged );
        }

        private void Update()
        {
        }

        private void SetIsLeftHand( bool isLeftHand )
        {
            m_appState.IsLeftHand.Value = isLeftHand;
        }

        private void SetMode( AppMode mode )
        {
            m_appState.Mode = mode;

            switch( mode )
            {
                case AppMode.Calibration:
                    LoadScene( forceCalibrationSceneName );
                    break;

                case AppMode.Static:
                    LoadScene( sequentialTestingSceneName );
                    break;

                case AppMode.Dynamic:
                    LoadScene( dynamicForceTrackingSceneName );
                    break;

                case AppMode.RhythmGame:
                    LoadScene( rhythmGameSceneName );
                    break;

                case AppMode.FlappyBird:
                    LoadScene( flappyBirdGameSceneName );
                    break;

                case AppMode.MusicPlay:
                    LoadScene(musicplayGameSceneName);
                    break;
            }
        }

        #region Callback functions
        private void DynamicPanelToggleValueChanged( bool newValue )
        {
            if( newValue )
            {
                NetOutMessage message = new NetOutMessage();
                message.WriteInt32( (int)MessageType.Command.ModeSet );
                message.WriteInt32( (int)MessageType.Mode.Dynamic );

                m_udpHelper.Send( message, m_appState.HololensIP, Constants.NETWORK_PORT );

                SetMode( AppMode.Dynamic );
            }
        }

        private void StaticPanelToggleValueChanged( bool newValue )
        {
            if( newValue )
            {
                NetOutMessage message = new NetOutMessage();
                message.WriteInt32( (int)MessageType.Command.ModeSet );
                message.WriteInt32( (int)MessageType.Mode.Static );

                m_udpHelper.Send( message, m_appState.HololensIP, Constants.NETWORK_PORT );

                SetMode( AppMode.Static );
            }
        }

        private void ForceCalibrationToggleValueChanged( bool newValue )
        {
            if( newValue )
            {
                SetMode( AppMode.Calibration );
            }
        }

        private void RhythmGameToggleValueChanged( bool newValue )
        {
            if( newValue )
            {
                NetOutMessage message = new NetOutMessage();
                message.WriteInt32( (int)MessageType.Command.ModeSet );
                message.WriteInt32( (int)MessageType.Mode.GuitarHero );

                m_udpHelper.Send( message, m_appState.HololensIP, Constants.NETWORK_PORT );

                SetMode( AppMode.RhythmGame );
            }
        }

        private void FlappyBirdToggleValueChanged( bool newValue )
        {
            if( newValue )
            {
                NetOutMessage message = new NetOutMessage();
                message.WriteInt32( (int)MessageType.Command.ModeSet );
                message.WriteInt32( (int)MessageType.Mode.FlappyBird );

                m_udpHelper.Send( message, m_appState.HololensIP, Constants.NETWORK_PORT );

                SetMode( AppMode.FlappyBird );
            }
        }

        private void MusicPlayToggleValueChanged(bool newValue)
        {
            if (newValue)
            {
                NetOutMessage message = new NetOutMessage();
                message.WriteInt32((int)MessageType.Command.ModeSet);
                message.WriteInt32((int)MessageType.Mode.MusicPlay);

                m_udpHelper.Send(message, m_appState.HololensIP, Constants.NETWORK_PORT);
                
                SetMode(AppMode.MusicPlay);
            }
        }

        private void LeftHandToggleValueChanged( bool newValue )
        {
            SetIsLeftHand( true );
        }

        private void RightHandToggleValueChanged( bool newValue )
        {
            SetIsLeftHand( false );
        }

        private void HololensIPInputValueChanged( string newValue )
        {
            m_appState.HololensIP = newValue;
        }
        #endregion
    }
}
