using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicPlay
{
    public partial class InputManager : MonoBehaviour
    {
        public string[] keys = new string[] { "c", "t", "y", "u", "i" };

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < 5; i++)
            {
                if (Input.GetKeyDown(keys[i]))
                {
                    pressed_fingers[i] = true;
                }
                if (Input.GetKeyUp(keys[i]))
                {
                    pressed_fingers[i] = false;
                }
            }
        }
    }
}
