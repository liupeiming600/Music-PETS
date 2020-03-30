using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using HololensPETSGames;

namespace HololensPETS
{
    public class PCGuitarHeroControlPanelScene : MonoBehaviour
    {
        public Dropdown songDropdown;

        public List<RadialBarGraph> fingerForceGraphs;
        public List<InputField> fingerTargetForceInputs;

        public Toggle forceScalingModeToggle;
        public List<GameObject> noteActivatorControllers;
        public List<GameObject> forceScalingNoteActivators;

        public InputField forceLeewayInput;
        public InputField songInitialDelayInput;

        public Button startButton;
        public Button stopButton;

        public NoteSpawnerController noteSpawner;

        private GuitarHeroGameController m_guitarHeroGameController;

        private PETSFingerInputController m_fingerInputController;

        private FingerForceDataProvider m_fingerForceDataProvider;

        private UDPHelper m_udpHelper;

        private SongListController m_songList;

        private AppState m_appState;

        public void StartRun()
        {
            SongScriptableObject selectedSong = GetSelectedSong();
            float songInitialDelay = GetSongInitialDelayValue();
            m_guitarHeroGameController.songPlayer.PlaySong( selectedSong, songInitialDelay );

            startButton.interactable = false;
            stopButton.interactable = true;
        }

        public void StopRun()
        {
            m_guitarHeroGameController.songPlayer.StopCurrentSong();

            noteSpawner.DeleteAllNotes();

            NetOutMessage outMessage = new NetOutMessage();
            outMessage.WriteInt32( (int)MessageType.Command.Control );
            outMessage.WriteInt32( (int)MessageType.ControlType.StopSong );

            m_udpHelper.Send( outMessage, m_appState.HololensIP, Constants.NETWORK_PORT );

            startButton.interactable = true;
            stopButton.interactable = false;
        }

        public void ReturnToMainMenu()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene( "PCControlPanelScene" );
        }

        private void Awake()
        {
            m_guitarHeroGameController = GameObject.FindObjectOfType<GuitarHeroGameController>();

            m_fingerInputController = GameObject.FindObjectOfType<PETSFingerInputController>();

            m_songList = GameObject.FindObjectOfType<SongListController>();

            m_appState = GameObject.FindObjectOfType<AppState>();

            m_fingerForceDataProvider = GameObject.FindObjectOfType<FingerForceDataProvider>();

            m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();
        }

        private void Start()
        {
            InitFingerForceInputHandlers();
            InitForceLeewayInputHandler();
        }

        private void OnEnable()
        {
            m_fingerForceDataProvider.OnFingerDataReceived += FingerForceDataReceivedHandler;
            noteSpawner.OnNoteSpawned += NoteSpawnedHandler;

            m_guitarHeroGameController.songPlayer.OnSongStartedPlaying += SongStartedPlayingHandler;
            m_guitarHeroGameController.songPlayer.OnSongFinishedPlaying += SongStoppedPlayingHandler;

            forceScalingModeToggle.onValueChanged.AddListener( SetForceScalingModeValue );

            UpdateSongListDropdown();
            UpdateForceIndicators();

            SetForceScalingModeValue( forceScalingModeToggle.isOn );

            startButton.interactable = true;
            stopButton.interactable = false;
        }

        private void OnDisable()
        {
            m_fingerForceDataProvider.OnFingerDataReceived -= FingerForceDataReceivedHandler;
            noteSpawner.OnNoteSpawned -= NoteSpawnedHandler;

            forceScalingModeToggle.onValueChanged.RemoveListener( SetForceScalingModeValue );
        }

        private void Update()
        {
        }

        private void UpdateSongListDropdown()
        {
            songDropdown.ClearOptions();

            List<string> songOptions = new List<string>();

            List<SongScriptableObject> songs = m_songList.songs;
            for( int i = 0; i < songs.Count; i++ )
            {
                songOptions.Add( songs[i].title );
            }

            songDropdown.AddOptions( songOptions );
        }

        private void UpdateForceIndicator( int index )
        {
            Finger finger = (Finger)index;

            float forceLeeway = GetForceLeewayValue();
            double maxFingerForce = m_appState.GetCurrentHandData().GetFingerMaxForce( finger ) - m_appState.GetCurrentHandData().GetFingerBaseForce( finger );
            float forcePercentage = GetFingerForcePercentage( index );

            float targetValue = (float)maxFingerForce * forcePercentage;

            float forceThresholdMin = Mathf.Clamp( (float)maxFingerForce * ( forcePercentage - forceLeeway / 2.0f ), (float)m_appState.GetCurrentHandData().GetFingerBaseForce( finger ), (float)maxFingerForce );
            float forceThresholdMax = Mathf.Clamp( (float)maxFingerForce * ( forcePercentage + forceLeeway / 2.0f ), (float)m_appState.GetCurrentHandData().GetFingerBaseForce( finger ), (float)maxFingerForce );
            m_fingerInputController.fingerForceMinThresholds[index] = forceThresholdMin;
            m_fingerInputController.fingerForceMaxThresholds[index] = forceThresholdMax;

            fingerForceGraphs[index].minTargetValue = Mathf.FloorToInt( (float)( forceThresholdMin / maxFingerForce ) * 100.0f );
            fingerForceGraphs[index].maxTargetValue = Mathf.FloorToInt( (float)( forceThresholdMax / maxFingerForce ) * 100.0f );

            forceScalingNoteActivators[index].GetComponent<ForceScalingNoteActivatorController>().SetForceRangePercentage( (float)( forceThresholdMin / maxFingerForce ) * 100.0f, (float)( forceThresholdMax / maxFingerForce ) * 100.0f );

            noteSpawner.noteScales[index] = forcePercentage;

            NetOutMessage outMessage = new NetOutMessage();

            outMessage.WriteInt32( (int)MessageType.Command.Control );
            outMessage.WriteInt32( (int)MessageType.ControlType.InitVisualizations );

            for( int i = 0; i < 5; i++ )
            {
                outMessage.WriteInt32( i );

                outMessage.WriteFloat( fingerForceGraphs[i].minTargetValue );
                outMessage.WriteFloat( fingerForceGraphs[i].maxTargetValue );
            }

            m_udpHelper.Send( outMessage, m_appState.HololensIP, Constants.NETWORK_PORT );
        }

        private void UpdateForceIndicators()
        {
            for( int i = 0; i < fingerForceGraphs.Count; i++ )
            {
                UpdateForceIndicator( i );
            }
        }

        private SongScriptableObject GetSelectedSong()
        {
            int songIndex = songDropdown.value;

            return m_songList.songs[songIndex];
        }

        private float GetFingerForcePercentage( int fingerIndex )
        {
            int ret = 0;
            if( !int.TryParse( fingerTargetForceInputs[fingerIndex].text, out ret ) )
            {
                ret = 0;
            }

            return ( ret / 100.0f );
        }

        private float GetForceLeewayValue()
        {
            int ret = 0;
            if( !int.TryParse( forceLeewayInput.text, out ret ) )
            {
                ret = 0;
            }

            return ( ret / 100.0f );
        }

        private float GetSongInitialDelayValue()
        {
            float ret = 0.0f;
            if( !float.TryParse( songInitialDelayInput.text, out ret ) )
            {
                ret = 0.0f;
            }

            return ret;
        }

        private void FingerForceDataReceivedHandler( Dictionary<Finger, double> fingerData )
        {
            NetOutMessage outMessage = new NetOutMessage();
            outMessage.WriteInt32( (int)MessageType.Command.Data );

            foreach( Finger finger in fingerData.Keys )
            {
                outMessage.WriteInt32( (int)finger );

                double adjustedForceValue = fingerData[finger] - m_appState.GetCurrentHandData().GetFingerBaseForce( finger );
                double adjustedMaxValue = m_appState.GetCurrentHandData().GetFingerMaxForce( finger ) - m_appState.GetCurrentHandData().GetFingerBaseForce( finger );
                float percentage = (float)( adjustedForceValue / adjustedMaxValue * 100.0f );

                fingerForceGraphs[(int)finger].value = Mathf.FloorToInt( percentage );
                
                outMessage.WriteFloat( percentage );

                int fingerIndex = (int)finger;
                forceScalingNoteActivators[fingerIndex].GetComponent<ForceScalingNoteActivatorController>().SetForcePercentage( percentage );
            }

            m_udpHelper.Send( outMessage, m_appState.HololensIP, Constants.NETWORK_PORT );
        }

        private void NoteSpawnedHandler( Note noteSpawned, Vector3 locationInNoteLane )
        {
            NetOutMessage outMessage = new NetOutMessage();
            outMessage.WriteInt32( (int)MessageType.Command.Control );
            if( forceScalingModeToggle.isOn )
            {
                outMessage.WriteInt32( (int)MessageType.ControlType.SpawnForceScalingNote );
            }
            else
            {
                outMessage.WriteInt32( (int)MessageType.ControlType.SpawnNote );
            }
            
            outMessage.WriteInt32( noteSpawned.noteLaneIndex );
            outMessage.WriteFloat( noteSpawned.duration );
            outMessage.WriteVector3( locationInNoteLane );

            if( forceScalingModeToggle.isOn )
            {
                outMessage.WriteFloat( noteSpawner.noteScales[noteSpawned.noteLaneIndex] );
            }

            m_udpHelper.Send( outMessage, m_appState.HololensIP, Constants.NETWORK_PORT );
        }

        private void InitFingerForceInputHandlers()
        {
            for( int i = 0; i < fingerForceGraphs.Count; i++ )
            {
                fingerTargetForceInputs[i].onEndEdit.AddListener( ( s ) => { UpdateForceIndicators(); } );
            }
        }

        private void InitForceLeewayInputHandler()
        {
            forceLeewayInput.onEndEdit.AddListener( ( s ) => { UpdateForceIndicators(); } );
        }

        private void SongStartedPlayingHandler()
        {
            NetOutMessage outMessage = new NetOutMessage();
            outMessage.WriteInt32( (int)MessageType.Command.Control );
            outMessage.WriteInt32( (int)MessageType.ControlType.PlaySong );
            outMessage.WriteInt32( songDropdown.value );

            m_udpHelper.Send( outMessage, m_appState.HololensIP, Constants.NETWORK_PORT );
        }

        private void SongStoppedPlayingHandler()
        {
            NetOutMessage outMessage = new NetOutMessage();
            outMessage.WriteInt32( (int)MessageType.Command.Control );
            outMessage.WriteInt32( (int)MessageType.ControlType.StopSong );

            m_udpHelper.Send( outMessage, m_appState.HololensIP, Constants.NETWORK_PORT );
        }

        private void SetForceScalingModeValue( bool newValue )
        {
            for( int i = 0; i < fingerForceGraphs.Count; i++ )
            {
                fingerForceGraphs[i].gameObject.SetActive( !newValue );
            }

            for( int i = 0; i < noteActivatorControllers.Count; i++ )
            {
                noteActivatorControllers[i].SetActive( !newValue );
            }

            for( int i = 0; i < forceScalingNoteActivators.Count; i++ )
            {
                forceScalingNoteActivators[i].SetActive( newValue );
            }

            m_guitarHeroGameController.forceScalingMode = newValue;

            NetOutMessage outMessage = new NetOutMessage();
            outMessage.WriteInt32( (int)MessageType.Command.Control );
            outMessage.WriteInt32( (int)MessageType.ControlType.ChangeForceScalingMode );
            outMessage.WriteInt32( forceScalingModeToggle.isOn ? 1 : 0 );

            m_udpHelper.Send( outMessage, m_appState.HololensIP, Constants.NETWORK_PORT );
        }
    }
}
