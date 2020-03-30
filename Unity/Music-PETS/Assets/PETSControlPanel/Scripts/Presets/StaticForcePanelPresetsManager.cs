using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace HololensPETS
{
    public class StaticForcePanelPresetsManager : PresetsManager
    {
        public InputField thumbTargetForceInputField;
        public InputField indexFingerTargetForceInputField;
        public InputField middleFingerTargetForceInputField;
        public InputField ringFingerTargetForceInputField;
        public InputField pinkyTargetForceInputField;

        public InputField thumbMinForceInputField;
        public InputField indexFingerMinForceInputField;
        public InputField middleFingerMinForceInputField;
        public InputField ringFingerMinForceInputField;
        public InputField pinkyMinForceInputField;

        public InputField thumbOrderInputField;
        public InputField indexFingerOrderInputField;
        public InputField middleFingerOrderInputField;
        public InputField ringFingerOrderInputField;
        public InputField pinkyOrderInputField;

        public InputField measurementDurationInputField;

        protected override PresetScriptableObject GetPresetScriptableObjectInstance()
        {
            return ScriptableObject.CreateInstance<StaticForcePanelPresetScriptableObject>();
        }

        protected override void LoadPreset( PresetScriptableObject preset )
        {
            StaticForcePanelPresetScriptableObject staticForcePanelPreset = preset as StaticForcePanelPresetScriptableObject;

            thumbTargetForceInputField.text = staticForcePanelPreset.thumbTargetForcePercent.ToString();
            indexFingerTargetForceInputField.text = staticForcePanelPreset.indexFingerTargetForcePercent.ToString();
            middleFingerTargetForceInputField.text = staticForcePanelPreset.middleFingerTargetForcePercent.ToString();
            ringFingerTargetForceInputField.text = staticForcePanelPreset.ringFingerTargetForcePercent.ToString();
            pinkyTargetForceInputField.text = staticForcePanelPreset.pinkyTargetForcePercent.ToString();

            thumbMinForceInputField.text = staticForcePanelPreset.thumbMinimumForcePercent.ToString();
            indexFingerMinForceInputField.text = staticForcePanelPreset.indexFingerMinimumForcePercent.ToString();
            middleFingerMinForceInputField.text = staticForcePanelPreset.middleFingerMinimumForcePercent.ToString();
            ringFingerMinForceInputField.text = staticForcePanelPreset.ringFingerMinimumForcePercent.ToString();
            pinkyMinForceInputField.text = staticForcePanelPreset.pinkyMinimumForcePercent.ToString();

            thumbOrderInputField.text = staticForcePanelPreset.thumbOrder.ToString();
            indexFingerOrderInputField.text = staticForcePanelPreset.indexFingerOrder.ToString();
            middleFingerOrderInputField.text = staticForcePanelPreset.middleFingerOrder.ToString();
            ringFingerOrderInputField.text = staticForcePanelPreset.ringFingerOrder.ToString();
            pinkyOrderInputField.text = staticForcePanelPreset.pinkyOrder.ToString();

            measurementDurationInputField.text = staticForcePanelPreset.measurementDuration.ToString();
        }

        protected override void PopulatePresetObjectForSaving( PresetScriptableObject preset )
        {
            StaticForcePanelPresetScriptableObject staticForcePanelPreset = preset as StaticForcePanelPresetScriptableObject;

            staticForcePanelPreset.thumbTargetForcePercent = int.Parse( thumbTargetForceInputField.text );
            staticForcePanelPreset.indexFingerTargetForcePercent = int.Parse( indexFingerTargetForceInputField.text );
            staticForcePanelPreset.middleFingerTargetForcePercent = int.Parse( middleFingerTargetForceInputField.text );
            staticForcePanelPreset.ringFingerTargetForcePercent = int.Parse( ringFingerTargetForceInputField.text );
            staticForcePanelPreset.pinkyTargetForcePercent = int.Parse( pinkyTargetForceInputField.text );

            staticForcePanelPreset.thumbMinimumForcePercent = int.Parse( thumbMinForceInputField.text );
            staticForcePanelPreset.indexFingerMinimumForcePercent = int.Parse( indexFingerMinForceInputField.text );
            staticForcePanelPreset.middleFingerMinimumForcePercent = int.Parse( middleFingerMinForceInputField.text );
            staticForcePanelPreset.ringFingerMinimumForcePercent = int.Parse( ringFingerMinForceInputField.text );
            staticForcePanelPreset.pinkyMinimumForcePercent = int.Parse( pinkyMinForceInputField.text );

            staticForcePanelPreset.thumbOrder = int.Parse( thumbOrderInputField.text );
            staticForcePanelPreset.indexFingerOrder = int.Parse( indexFingerOrderInputField.text );
            staticForcePanelPreset.middleFingerOrder = int.Parse( middleFingerOrderInputField.text );
            staticForcePanelPreset.ringFingerOrder = int.Parse( ringFingerOrderInputField.text );
            staticForcePanelPreset.pinkyOrder = int.Parse( pinkyOrderInputField.text );

            staticForcePanelPreset.measurementDuration = float.Parse( measurementDurationInputField.text );
        }
    }
}
