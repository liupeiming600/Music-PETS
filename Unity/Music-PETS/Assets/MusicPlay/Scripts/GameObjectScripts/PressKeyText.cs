using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MusicPlay
{
    public class PressKeyText : MonoBehaviour
    {
        private Text _text;
        GameObject obj;
        InputManager InpMng;

        public int finger_number;

        // Use this for initialization
        void Start()
        {
            _text = this.GetComponent<Text>();

            obj = GameObject.Find("InputManager");
            InpMng = obj.GetComponent<InputManager>();
        }

        // Update is called once per frame
        void Update()
        {
            if (InpMng.keys[finger_number] != this._text.text)
            {
                this._text.text = InpMng.keys[finger_number];
            }
        }
    }
}
