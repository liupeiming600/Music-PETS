using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HololensPETS
{
    [CreateAssetMenu( menuName = "HololensPETS/Flappy Bird Panel Preset" )]
    public class FlappyBirdPanelPresetScriptableObject : PresetScriptableObject
    {
        public float minGapSize;
        public float maxGapSize;

        public float minDistanceBetweenPipes;
        public float maxDistanceBetweenPipes;

        public bool thumbSelected;
        public bool indexFingerSelected;
        public bool middleFingerSelected;
        public bool ringFingerSelected;
        public bool pinkySelected;
    }
}
