using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HololensPETS
{
    [CreateAssetMenu( menuName = "HololensPETS/Static Force Panel Preset" )]
    public class StaticForcePanelPresetScriptableObject : PresetScriptableObject
    {
        public int thumbTargetForcePercent;
        public int indexFingerTargetForcePercent;
        public int middleFingerTargetForcePercent;
        public int ringFingerTargetForcePercent;
        public int pinkyTargetForcePercent;

        public int thumbMinimumForcePercent;
        public int indexFingerMinimumForcePercent;
        public int middleFingerMinimumForcePercent;
        public int ringFingerMinimumForcePercent;
        public int pinkyMinimumForcePercent;

        public int thumbOrder;
        public int indexFingerOrder;
        public int middleFingerOrder;
        public int ringFingerOrder;
        public int pinkyOrder;

        public float measurementDuration;
    }
}
