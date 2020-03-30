using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HololensPETS
{
    [CreateAssetMenu( menuName = "HololensPETS/Preset List" )]
    public class PresetListScriptableObject : ScriptableObject
    {
        public List<PresetScriptableObject> presets;
    }
}
