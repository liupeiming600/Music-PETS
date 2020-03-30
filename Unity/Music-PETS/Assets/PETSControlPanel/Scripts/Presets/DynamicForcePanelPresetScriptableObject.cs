using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HololensPETS
{
    [CreateAssetMenu( menuName = "HololensPETS/Dynamic Force Panel Preset" )]
    public class DynamicForcePanelPresetScriptableObject : PresetScriptableObject
    {
        public bool thumbSelected;
        public bool indexFingerSelected;
        public bool middleFingerSelected;
        public bool ringFingerSelected;
        public bool pinkyFingerSelected;

        public int targetForcePercentage;

        public float staticPhaseDuration;
        public float increasingPhaseDuration;
        public float plateauPhaseDuration;
        public float decreasingPhaseDuration;
        public float staticEndPhaseDuration;
    }
}
