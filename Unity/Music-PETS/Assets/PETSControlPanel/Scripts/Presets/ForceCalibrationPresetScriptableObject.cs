using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HololensPETS
{
    [CreateAssetMenu( menuName = "HololensPETS/Force Calibration Preset" )]
    public class ForceCalibrationPresetScriptableObject : PresetScriptableObject
    {
        public double leftThumbMaxForce = 0.0;
        public double leftIndexFingerMaxForce = 0.0;
        public double leftMiddleFingerMaxForce = 0.0;
        public double leftRingFingerMaxForce = 0.0;
        public double leftPinkyMaxForce = 0.0;

        public double rightThumbMaxForce = 0.0;
        public double rightIndexFingerMaxForce = 0.0;
        public double rightMiddleFingerMaxForce = 0.0;
        public double rightRingFingerMaxForce = 0.0;
        public double rightPinkyMaxForce = 0.0;
        
        public double leftThumbBaseForce = 0.0;
        public double leftIndexFingerBaseForce = 0.0;
        public double leftMiddleFingerBaseForce = 0.0;
        public double leftRingFingerBaseForce = 0.0;
        public double leftPinkyBaseForce = 0.0;

        public double rightThumbBaseForce = 0.0;
        public double rightIndexFingerBaseForce = 0.0;
        public double rightMiddleFingerBaseForce = 0.0;
        public double rightRingFingerBaseForce = 0.0;
        public double rightPinkyBaseForce = 0.0;
        
    }
}
