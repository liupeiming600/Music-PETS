using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HololensPETSGames
{
    public class SongPlayerController : MonoBehaviour
    {
        public delegate void SongStartedPlayingDelegate();
        public SongStartedPlayingDelegate OnSongStartedPlaying;

        public delegate void SongFinishedPlayingDelegate();
        public SongFinishedPlayingDelegate OnSongFinishedPlaying;

        public float timeOffset = 0.0f;

        private AudioSource m_audioSource;

        private NoteSpawnerController m_noteSpawner;

        private List<Note> m_notes = null;
        private int m_noteIndex = 0;
        private float m_timer = 0.0f;
        private bool m_isPlaying = false;

        // TODO
        private GuitarHeroGameController m_guitarHeroGameController;

        private void Awake()
        {
            m_noteSpawner = GameObject.FindObjectOfType<NoteSpawnerController>();

            m_audioSource = GetComponent<AudioSource>();

            m_guitarHeroGameController = GameObject.FindObjectOfType<GuitarHeroGameController>();
        }

        private void Update()
        {
            if( !m_isPlaying )
            {
                return;
            }

            m_timer += Time.deltaTime;

            if( !m_audioSource.isPlaying && ( m_timer >= 0.0f ) )
            {
                if( OnSongStartedPlaying != null )
                {
                    OnSongStartedPlaying();
                }

                m_audioSource.Play();
            }

            if( m_timer >= m_audioSource.clip.length )
            {
                m_isPlaying = false;

                if( OnSongFinishedPlaying != null )
                {
                    OnSongFinishedPlaying();
                }

                m_audioSource.Stop();
                m_audioSource.clip = null;

                return;
            }

            while( m_noteIndex < m_notes.Count )
            {
                if( ( m_notes[m_noteIndex].timeInSong - timeOffset ) <= m_timer )
                {
                    if( m_guitarHeroGameController.forceScalingMode )
                    {
                        m_noteSpawner.SpawnForceScalingNote( m_notes[m_noteIndex], m_timer );
                    }
                    else
                    {
                        m_noteSpawner.SpawnNote( m_notes[m_noteIndex], m_timer );
                    }
                    m_noteIndex++;
                }
                else
                {
                    break;
                }
            }
        }

        public void PlaySong( SongScriptableObject song, float initialOffset = 0.0f )
        {
            m_isPlaying = true;

            m_timer = -initialOffset;
            if( m_timer == 0.0f )
            {
                m_audioSource.Play();
                
                if( OnSongStartedPlaying != null )
                {
                    OnSongStartedPlaying();
                }
            }

            m_noteIndex = 0;

            m_notes = new List<Note>( song.notes );
            m_notes.Sort( ( a, b ) => a.timeInSong.CompareTo( b.timeInSong ) );

            m_audioSource.clip = song.audioClip;
            m_audioSource.loop = false;
        }

        public void StopCurrentSong()
        {
            m_audioSource.Stop();

            m_audioSource.clip = null;

            m_isPlaying = false;
        }
    }
}
