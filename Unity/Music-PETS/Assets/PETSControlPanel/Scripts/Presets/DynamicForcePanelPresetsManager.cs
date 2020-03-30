using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace HololensPETS
{
    public class DynamicForcePanelPresetsManager : PresetsManager
    {
        public Toggle thumbToggle;
        public Toggle indexFingerToggle;
        public Toggle middleFingerToggle;
        public Toggle ringFingerToggle;
        public Toggle pinkyFingerToggle;

        public Slider targetForceSlider;

        public InputField staticPhaseDurationInputField;
        public InputField increasingPhaseDurationInputField;
        public InputField plateauPhaseDurationInputField;
        public InputField decreasingPhaseDurationInputField;
        public InputField staticEndPhaseDurationInputField;

        protected override PresetScriptableObject GetPresetScriptableObjectInstance()
        {
            return ScriptableObject.CreateInstance<DynamicForcePanelPresetScriptableObject>();
        }

        protected override void LoadPreset( PresetScriptableObject preset )
        {
            DynamicForcePanelPresetScriptableObject dynamicForcePanelPreset = preset as DynamicForcePanelPresetScriptableObject;

            thumbToggle.isOn = dynamicForcePanelPreset.thumbSelected;
            indexFingerToggle.isOn = dynamicForcePanelPreset.indexFingerSelected;
            middleFingerToggle.isOn = dynamicForcePanelPreset.middleFingerSelected;
            ringFingerToggle.isOn = dynamicForcePanelPreset.ringFingerSelected;
            pinkyFingerToggle.isOn = dynamicForcePanelPreset.pinkyFingerSelected;

            targetForceSlider.value = dynamicForcePanelPreset.targetForcePercentage;

            staticPhaseDurationInputField.text = dynamicForcePanelPreset.staticPhaseDuration.ToString();
            increasingPhaseDurationInputField.text = dynamicForcePanelPreset.increasingPhaseDuration.ToString();
            plateauPhaseDurationInputField.text = dynamicForcePanelPreset.plateauPhaseDuration.ToString();
            decreasingPhaseDurationInputField.text = dynamicForcePanelPreset.decreasingPhaseDuration.ToString();
            staticEndPhaseDurationInputField.text = dynamicForcePanelPreset.staticEndPhaseDuration.ToString();
        }

        protected override void PopulatePresetObjectForSaving( PresetScriptableObject preset )
        {
            DynamicForcePanelPresetScriptableObject dynamicForcePanelPreset = preset as DynamicForcePanelPresetScriptableObject;

            dynamicForcePanelPreset.thumbSelected = thumbToggle.isOn;
            dynamicForcePanelPreset.indexFingerSelected = indexFingerToggle.isOn;
            dynamicForcePanelPreset.middleFingerSelected = middleFingerToggle.isOn;
            dynamicForcePanelPreset.ringFingerSelected = ringFingerToggle.isOn;
            dynamicForcePanelPreset.pinkyFingerSelected = pinkyFingerToggle.isOn;

            dynamicForcePanelPreset.targetForcePercentage = (int)targetForceSlider.value;

            dynamicForcePanelPreset.staticPhaseDuration = float.Parse( staticPhaseDurationInputField.text );
            dynamicForcePanelPreset.increasingPhaseDuration = float.Parse( increasingPhaseDurationInputField.text );
            dynamicForcePanelPreset.plateauPhaseDuration = float.Parse( plateauPhaseDurationInputField.text );
            dynamicForcePanelPreset.decreasingPhaseDuration = float.Parse( decreasingPhaseDurationInputField.text );
            dynamicForcePanelPreset.staticEndPhaseDuration = float.Parse( staticEndPhaseDurationInputField.text );
        }
    }
}
