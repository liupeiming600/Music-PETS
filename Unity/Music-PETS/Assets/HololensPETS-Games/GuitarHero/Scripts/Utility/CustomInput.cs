using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HololensPETSGames
{
    public class CustomInput
    {
        private static HashSet<KeyCode> m_simulatedInputs;

        static CustomInput()
        {
            m_simulatedInputs = new HashSet<KeyCode>();
        }

        public static bool GetKey( KeyCode key )
        {
            return Input.GetKey( key ) || m_simulatedInputs.Contains( key );
        }

        public static void PressKey( KeyCode key )
        {
            m_simulatedInputs.Add( key );
        }

        public static void ReleaseKey( KeyCode key )
        {
            m_simulatedInputs.Remove( key );
        }
    }
}
