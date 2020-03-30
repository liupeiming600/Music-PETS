using System.Collections.Generic;
using UnityEngine;

namespace HololensPETSGames
{
    [CreateAssetMenu( menuName = "GuitarHero/Song" )]
    public class SongScriptableObject : ScriptableObject
    {
        public string title;

        public AudioClip audioClip;

        public List<Note> notes;
    }
}
