using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HololensPETS
{
    public class ForceCalibrationPresetsManager : PresetsManager
    {
        private AppState m_appState;

        protected override void Awake()
        {
            base.Awake();

            m_appState = GameObject.FindObjectOfType<AppState>();
        }

        protected override PresetScriptableObject GetPresetScriptableObjectInstance()
        {
            return ScriptableObject.CreateInstance<ForceCalibrationPresetScriptableObject>();
        }

        protected override void LoadPreset( PresetScriptableObject preset )
        {
            ForceCalibrationPresetScriptableObject forceCalibrationPreset = preset as ForceCalibrationPresetScriptableObject;

            HandData leftHandData = new HandData();
            leftHandData.SetFingerMaxForce( Finger.Thumb, forceCalibrationPreset.leftThumbMaxForce );
            leftHandData.SetFingerMaxForce( Finger.Index, forceCalibrationPreset.leftIndexFingerMaxForce );
            leftHandData.SetFingerMaxForce( Finger.Middle, forceCalibrationPreset.leftMiddleFingerMaxForce );
            leftHandData.SetFingerMaxForce( Finger.Ring, forceCalibrationPreset.leftRingFingerMaxForce );
            leftHandData.SetFingerMaxForce( Finger.Pinky, forceCalibrationPreset.leftPinkyMaxForce );
            
            leftHandData.SetFingerBaseForce(Finger.Thumb, m_appState.LeftHandData.GetFingerBaseForce(Finger.Thumb));
            leftHandData.SetFingerBaseForce(Finger.Index, m_appState.LeftHandData.GetFingerBaseForce(Finger.Index));
            leftHandData.SetFingerBaseForce(Finger.Middle, m_appState.LeftHandData.GetFingerBaseForce(Finger.Middle));
            leftHandData.SetFingerBaseForce(Finger.Ring, m_appState.LeftHandData.GetFingerBaseForce(Finger.Ring));
            leftHandData.SetFingerBaseForce(Finger.Pinky, m_appState.LeftHandData.GetFingerBaseForce(Finger.Pinky));
            
            HandData rightHandData = new HandData();
            rightHandData.SetFingerMaxForce( Finger.Thumb, forceCalibrationPreset.rightThumbMaxForce );
            rightHandData.SetFingerMaxForce( Finger.Index, forceCalibrationPreset.rightIndexFingerMaxForce );
            rightHandData.SetFingerMaxForce( Finger.Middle, forceCalibrationPreset.rightMiddleFingerMaxForce );
            rightHandData.SetFingerMaxForce( Finger.Ring, forceCalibrationPreset.rightRingFingerMaxForce );
            rightHandData.SetFingerMaxForce( Finger.Pinky, forceCalibrationPreset.rightPinkyMaxForce );
            
            rightHandData.SetFingerBaseForce(Finger.Thumb, m_appState.RightHandData.GetFingerBaseForce(Finger.Thumb));
            rightHandData.SetFingerBaseForce(Finger.Index, m_appState.RightHandData.GetFingerBaseForce(Finger.Index));
            rightHandData.SetFingerBaseForce(Finger.Middle, m_appState.RightHandData.GetFingerBaseForce(Finger.Middle));
            rightHandData.SetFingerBaseForce(Finger.Ring, m_appState.RightHandData.GetFingerBaseForce(Finger.Ring));
            rightHandData.SetFingerBaseForce(Finger.Pinky, m_appState.RightHandData.GetFingerBaseForce(Finger.Pinky));
            
            m_appState.SetHandData( leftHandData, rightHandData );
        }

        protected override void PopulatePresetObjectForSaving( PresetScriptableObject preset )
        {
            ForceCalibrationPresetScriptableObject forceCalibrationPreset = preset as ForceCalibrationPresetScriptableObject;

            forceCalibrationPreset.leftThumbMaxForce = m_appState.LeftHandData.GetFingerMaxForce( Finger.Thumb );
            forceCalibrationPreset.leftIndexFingerMaxForce = m_appState.LeftHandData.GetFingerMaxForce( Finger.Index );
            forceCalibrationPreset.leftMiddleFingerMaxForce = m_appState.LeftHandData.GetFingerMaxForce( Finger.Middle );
            forceCalibrationPreset.leftRingFingerMaxForce = m_appState.LeftHandData.GetFingerMaxForce( Finger.Ring );
            forceCalibrationPreset.leftPinkyMaxForce = m_appState.LeftHandData.GetFingerMaxForce( Finger.Pinky );
            /*
            forceCalibrationPreset.leftThumbBaseForce = m_appState.LeftHandData.GetFingerBaseForce(Finger.Thumb);
            forceCalibrationPreset.leftIndexFingerBaseForce = m_appState.LeftHandData.GetFingerBaseForce(Finger.Index);
            forceCalibrationPreset.leftMiddleFingerBaseForce = m_appState.LeftHandData.GetFingerBaseForce(Finger.Middle);
            forceCalibrationPreset.leftRingFingerBaseForce = m_appState.LeftHandData.GetFingerBaseForce(Finger.Ring);
            forceCalibrationPreset.leftPinkyBaseForce = m_appState.LeftHandData.GetFingerBaseForce(Finger.Pinky);
            */
            forceCalibrationPreset.rightThumbMaxForce = m_appState.RightHandData.GetFingerMaxForce( Finger.Thumb );
            forceCalibrationPreset.rightIndexFingerMaxForce = m_appState.RightHandData.GetFingerMaxForce( Finger.Index );
            forceCalibrationPreset.rightMiddleFingerMaxForce = m_appState.RightHandData.GetFingerMaxForce( Finger.Middle );
            forceCalibrationPreset.rightRingFingerMaxForce = m_appState.RightHandData.GetFingerMaxForce( Finger.Ring );
            forceCalibrationPreset.rightPinkyMaxForce = m_appState.RightHandData.GetFingerMaxForce( Finger.Pinky );
            /*
            forceCalibrationPreset.rightThumbBaseForce = m_appState.RightHandData.GetFingerBaseForce(Finger.Thumb);
            forceCalibrationPreset.rightIndexFingerBaseForce = m_appState.RightHandData.GetFingerBaseForce(Finger.Index);
            forceCalibrationPreset.rightMiddleFingerBaseForce = m_appState.RightHandData.GetFingerBaseForce(Finger.Middle);
            forceCalibrationPreset.rightRingFingerBaseForce = m_appState.RightHandData.GetFingerBaseForce(Finger.Ring);
            forceCalibrationPreset.rightPinkyBaseForce = m_appState.RightHandData.GetFingerBaseForce(Finger.Pinky);
            */
        }
    }
}
