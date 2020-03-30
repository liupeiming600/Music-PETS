using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HololensPETSGames
{
    public class ForceScalingNoteController : NoteController
    {
        public GameObject noteSpriteParent;

        public void SetNoteScaleX( float noteXScale )
        {
            Vector3 scale = noteSpriteParent.transform.localScale;
            scale.x = noteXScale;
            noteSpriteParent.transform.localScale = scale;
        }
    }
}
