using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using HololensPETS;

namespace HololensPETSGames
{
    public class GuitarHeroGameHololensController : MonoBehaviour
    {
        public List<RadialBarGraph> FingerForceGraphs;
        public List<GameObject> NoteLanes;
        public NoteSpawnerController noteSpawner;

        public List<GameObject> noteActivators;
        public List<GameObject> forceScalingNoteActivators;

        public AudioSource audioSource;
        public SongListController songList;

        private HololensAppState m_appState;

        private UDPHelper m_udpHelper;
        
        private void Awake()
        {
            m_appState = GameObject.FindObjectOfType<HololensAppState>();

            m_udpHelper = GameObject.FindObjectOfType<UDPHelper>();
        }

        private void OnEnable()
        {
            m_udpHelper.MessageReceived += UDPMessageReceivedHandler;

            for( int i = 0; i < NoteLanes.Count; i++ )
            {
                NoteLanes[i].transform.position = m_appState.FingerTransforms[i].position;
            }

            SetForceScalingModeValue( false );
        }

        private void OnDisable()
        {
            m_udpHelper.MessageReceived -= UDPMessageReceivedHandler;

            audioSource.Stop();

            noteSpawner.DeleteAllNotes();
        }

        private void Start()
        {
            
        }

        private void Update()
        {
        }

        private void UDPMessageReceivedHandler( NetInMessage message )
        {
            message.ResetIndex();

            MessageType.Command command = (MessageType.Command)message.ReadInt32();
            if( command == MessageType.Command.Data )
            {
                for( int i = 0; i < 5; i++ )
                {
                    int fingerIndex = message.ReadInt32();
                    float plotValue = message.ReadFloat();

                    FingerForceGraphs[fingerIndex].value = Mathf.FloorToInt( plotValue );
                    forceScalingNoteActivators[fingerIndex].GetComponent<ForceScalingNoteActivatorController>().SetForcePercentage( plotValue );
                }
            }
            else if( command == MessageType.Command.Control )
            {
                MessageType.ControlType controlType = (MessageType.ControlType)message.ReadInt32();
                if( controlType == MessageType.ControlType.PlaySong )
                {
                    int songIndex = message.ReadInt32();

                    audioSource.clip = songList.songs[songIndex].audioClip;
                    audioSource.loop = false;
                    audioSource.Play();
                }
                else if( controlType == MessageType.ControlType.StopSong )
                {
                    audioSource.Stop();

                    noteSpawner.DeleteAllNotes();
                }
                else if( controlType == MessageType.ControlType.SpawnNote )
                {
                    int spawnIndex = message.ReadInt32();
                    float duration = message.ReadFloat();
                    Vector3 locationInNoteLane = message.ReadVector3();
                    
                    noteSpawner.SpawnHoldNote( spawnIndex, locationInNoteLane, duration );
                }
                else if( controlType == MessageType.ControlType.SpawnForceScalingNote )
                {
                    int spawnIndex = message.ReadInt32();
                    float duration = message.ReadFloat();
                    Vector3 locationInNoteLane = message.ReadVector3();
                    float noteScaleX = message.ReadFloat();

                    noteSpawner.SpawnForceScalingNote( spawnIndex, locationInNoteLane, noteScaleX, duration );
                }
                else if( controlType == MessageType.ControlType.InitVisualizations )
                {
                    for( int i = 0; i < 5; i++ )
                    {
                        int fingerIndex = message.ReadInt32();
                        float minTargetValuePercentage = message.ReadFloat();
                        float maxTargetValuePercentage = message.ReadFloat();

                        FingerForceGraphs[fingerIndex].minTargetValue = Mathf.FloorToInt( minTargetValuePercentage );
                        FingerForceGraphs[fingerIndex].maxTargetValue = Mathf.FloorToInt( maxTargetValuePercentage );

                        forceScalingNoteActivators[fingerIndex].GetComponent<ForceScalingNoteActivatorController>().SetForceRangePercentage( minTargetValuePercentage, maxTargetValuePercentage );
                    }
                }
                else if( controlType == MessageType.ControlType.PressKey )
                {
                    KeyCode key = (KeyCode)message.ReadInt32();
                    CustomInput.PressKey( key );
                }
                else if( controlType == MessageType.ControlType.ReleaseKey )
                {
                    KeyCode key = (KeyCode)message.ReadInt32();
                    CustomInput.ReleaseKey( key );
                }
                else if( controlType == MessageType.ControlType.ChangeForceScalingMode )
                {
                    int isForceScalingMode = message.ReadInt32();

                    SetForceScalingModeValue( isForceScalingMode == 1 );
                }
            }
        }

        private void SetForceScalingModeValue( bool newValue )
        {
            for( int i = 0; i < FingerForceGraphs.Count; i++ )
            {
                FingerForceGraphs[i].gameObject.SetActive( !newValue );
            }
            for( int i = 0; i < noteActivators.Count; i++ )
            {
                noteActivators[i].SetActive( !newValue );
            }
            for( int i = 0; i < forceScalingNoteActivators.Count; i++ )
            {
                forceScalingNoteActivators[i].SetActive( newValue );
            }
        }
    }
}
