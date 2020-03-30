using System.Collections.Generic;

using UnityEngine;

namespace HololensPETSGames
{
    public class NoteSpawnerController : MonoBehaviour
    {
        public List<NoteLaneController> noteLanes;

        public GameObject notePrefab;
        public GameObject holdNotePrefab;
        public GameObject forceScalingNotePrefab;

        public Scroller noteScroller;

        // TODO
        public List<float> noteScales = new List<float>
        {
            0.0f, 0.0f, 0.0f, 0.0f, 0.0f
        };

        public delegate void NoteSpawnedDelegate( Note noteSpawned, Vector3 locationInNoteLane );
        public NoteSpawnedDelegate OnNoteSpawned;

        public void DeleteAllNotes()
        {
            for( int i = 0; i < noteLanes.Count; i++ )
            {
                int numChildren = noteLanes[i].noteSpawnLocation.childCount;
                for( int j = 0; j < numChildren; j++ )
                {
                    Destroy( noteLanes[i].noteSpawnLocation.GetChild( j ).gameObject );
                }
            }
        }
        
        public void SpawnNote( Note note, float currentSongTime ) // TODO: Remove currentsongTime
        {
            if( note.noteAction == NoteAction.Hold )
            {
                int noteLaneIndex = note.noteLaneIndex;

                GameObject holdNote = Instantiate( holdNotePrefab, noteLanes[noteLaneIndex].noteSpawnLocation );

                Vector3 noteLocation = noteLanes[noteLaneIndex].GetLanePositionAtTime( note.timeInSong, currentSongTime );
                noteLocation -= noteLanes[noteLaneIndex].noteSpawnLocation.localPosition;
                holdNote.transform.localPosition = noteLocation;
                holdNote.layer = noteLanes[noteLaneIndex].gameObject.layer;

                NoteController noteController = holdNote.GetComponent<NoteController>();
                noteController.duration = note.duration;

                Vector3 holdNoteScale = holdNote.transform.localScale;
                holdNoteScale.y = note.duration;
                holdNote.transform.localScale = holdNoteScale;

                noteScroller.Add( holdNote );

                if( OnNoteSpawned != null )
                {
                    OnNoteSpawned( note, noteLocation );
                }
            }
        }

        public void SpawnForceScalingNote( Note note, float currentSongTime ) // TODO: Remove currentsongTime
        {
            if( note.noteAction == NoteAction.Hold )
            {
                int noteLaneIndex = note.noteLaneIndex;

                GameObject forceScalingNote = Instantiate( forceScalingNotePrefab, noteLanes[noteLaneIndex].noteSpawnLocation );

                Vector3 noteLocation = noteLanes[noteLaneIndex].GetLanePositionAtTime( note.timeInSong, currentSongTime );
                noteLocation -= noteLanes[noteLaneIndex].noteSpawnLocation.localPosition;
                forceScalingNote.transform.localPosition = noteLocation;
                forceScalingNote.layer = noteLanes[noteLaneIndex].gameObject.layer;

                NoteController noteController = forceScalingNote.GetComponent<NoteController>();
                noteController.duration = note.duration;

                Vector3 noteScale = forceScalingNote.transform.localScale;
                noteScale.y = note.duration;
                
                forceScalingNote.transform.localScale = noteScale;

                forceScalingNote.GetComponent<ForceScalingNoteController>().SetNoteScaleX( noteScales[noteLaneIndex] );

                noteScroller.Add( forceScalingNote );

                if( OnNoteSpawned != null )
                {
                    OnNoteSpawned( note, noteLocation );
                }
            }
        }

        public void SpawnHoldNote( int noteLaneIndex, Vector3 positionInNoteLane, float duration )
        {
            GameObject holdNote = Instantiate( holdNotePrefab, noteLanes[noteLaneIndex].noteSpawnLocation.transform );
            
            holdNote.transform.localPosition = positionInNoteLane;
            holdNote.layer = noteLanes[noteLaneIndex].gameObject.layer;

            NoteController noteController = holdNote.GetComponent<NoteController>();
            noteController.duration = duration;

            Vector3 holdNoteScale = holdNote.transform.localScale;
            holdNoteScale.y = duration;
            holdNote.transform.localScale = holdNoteScale;

            noteScroller.Add( holdNote );
        }

        public void SpawnForceScalingNote( int noteLaneIndex, Vector3 positionInNoteLane, float noteScale, float duration )
        {
            GameObject forceScalingNote = Instantiate( forceScalingNotePrefab, noteLanes[noteLaneIndex].noteSpawnLocation.transform );
            
            forceScalingNote.transform.localPosition = positionInNoteLane;
            forceScalingNote.layer = noteLanes[noteLaneIndex].gameObject.layer;

            NoteController noteController = forceScalingNote.GetComponent<NoteController>();
            noteController.duration = duration;

            Vector3 forceScalingNoteScale = forceScalingNote.transform.localScale;
            forceScalingNoteScale.y = duration;
            forceScalingNote.transform.localScale = forceScalingNoteScale;

            forceScalingNote.GetComponent<ForceScalingNoteController>().SetNoteScaleX( noteScale );

            noteScroller.Add( forceScalingNote );
        }
    }
}
