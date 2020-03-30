using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace HololensPETS
{
    public class FlappyBirdPanelPresetsManager : PresetsManager
    {
        public InputField minGapSizeInputField;
        public InputField maxGapSizeInputField;

        public InputField minDistanceBetweenPipesInputField;
        public InputField maxDistanceBetweenPipesInputField;

        public Toggle thumbToggle;
        public Toggle indexFingerToggle;
        public Toggle middleFingerToggle;
        public Toggle ringFingerToggle;
        public Toggle pinkyToggle;

        protected override PresetScriptableObject GetPresetScriptableObjectInstance()
        {
            return ScriptableObject.CreateInstance<FlappyBirdPanelPresetScriptableObject>();
        }

        protected override void LoadPreset( PresetScriptableObject preset )
        {
            FlappyBirdPanelPresetScriptableObject flappyBirdPreset = preset as FlappyBirdPanelPresetScriptableObject;

            minGapSizeInputField.text = flappyBirdPreset.minGapSize.ToString();
            maxGapSizeInputField.text = flappyBirdPreset.maxGapSize.ToString();

            minDistanceBetweenPipesInputField.text = flappyBirdPreset.minDistanceBetweenPipes.ToString();
            maxDistanceBetweenPipesInputField.text = flappyBirdPreset.maxDistanceBetweenPipes.ToString();

            thumbToggle.isOn = flappyBirdPreset.thumbSelected;
            indexFingerToggle.isOn = flappyBirdPreset.indexFingerSelected;
            middleFingerToggle.isOn = flappyBirdPreset.middleFingerSelected;
            ringFingerToggle.isOn = flappyBirdPreset.ringFingerSelected;
            pinkyToggle.isOn = flappyBirdPreset.pinkySelected;
        }

        protected override void PopulatePresetObjectForSaving( PresetScriptableObject preset )
        {
            FlappyBirdPanelPresetScriptableObject flappyBirdPreset = preset as FlappyBirdPanelPresetScriptableObject;

            flappyBirdPreset.minGapSize = float.Parse( minGapSizeInputField.text );
            flappyBirdPreset.maxGapSize = float.Parse( maxGapSizeInputField.text );

            flappyBirdPreset.minDistanceBetweenPipes = float.Parse( minDistanceBetweenPipesInputField.text );
            flappyBirdPreset.maxDistanceBetweenPipes = float.Parse( maxDistanceBetweenPipesInputField.text );

            flappyBirdPreset.thumbSelected = thumbToggle.isOn;
            flappyBirdPreset.indexFingerSelected = indexFingerToggle.isOn;
            flappyBirdPreset.middleFingerSelected = middleFingerToggle.isOn;
            flappyBirdPreset.ringFingerSelected = ringFingerToggle.isOn;
            flappyBirdPreset.pinkySelected = pinkyToggle.isOn;
        }
    }
}
