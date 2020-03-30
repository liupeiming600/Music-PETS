using UnityEngine;

namespace HololensPETSGames
{
    public class NoteController : MonoBehaviour
    {
        public NoteAction action = NoteAction.None;
        
        public float duration = 1.0f;

        public int baseScore = 1;

        public virtual void Awake()
        {
            
        }

        public virtual void Start()
        {
            
        }
    }
}
