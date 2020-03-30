using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MusicPlay
{
    public class CyliAct : MonoBehaviour
    {

        GameObject obj, score;
        InputManager InpMng;
        scoremanage scmn;

        public char chr;

        public bool pressing = true;
        public bool correct = true;
        bool note_in = false, ignore;

        // Use this for initialization
        void Start()
        {
            obj = GameObject.Find("InputManager");
            InpMng = obj.GetComponent<InputManager>();

            score = GameObject.Find("ScoreManager");
            scmn = score.GetComponent<scoremanage>();
        }

        // Update is called once per frame
        void Update()
        {

            if (chr == 'c')
            {
                if (!pressing && InpMng.pressed_fingers[0])
                {       //表示されていなくて，キーが押されたとき
                    this.transform.localScale = new Vector3(1.5f, 0.15f, 1);
                    pressing = true;
                    if (!note_in)
                    {                   //表示されたときに譜面がなかったとき
                        StartCoroutine("Wait_note");
                    }
                }
                else if (pressing && !InpMng.pressed_fingers[0])
                { //表示されていて，キーが解除されたとき
                    this.transform.localScale = new Vector3(0, 0.15f, 1);
                    pressing = false;
                }
            }
            else if (chr == 'v')
            {
                if (!pressing && InpMng.pressed_fingers[1])
                {
                    this.transform.localScale = new Vector3(1.5f, 0.15f, 1);
                    pressing = true;
                    if (!note_in)
                    {                   //表示されたときに譜面がなかったとき
                        StartCoroutine("Wait_note");
                    }
                }
                else if (pressing && !InpMng.pressed_fingers[1])
                {
                    this.transform.localScale = new Vector3(0, 0.15f, 1);
                    pressing = false;
                }
            }
            else if (chr == 'b')
            {
                if (!pressing && InpMng.pressed_fingers[2])
                {
                    this.transform.localScale = new Vector3(1.5f, 0.15f, 1);
                    pressing = true;
                    if (!note_in)
                    {                   //表示されたときに譜面がなかったとき
                        StartCoroutine("Wait_note");
                    }
                }
                else if (pressing && !InpMng.pressed_fingers[2])
                {
                    this.transform.localScale = new Vector3(0, 0.15f, 1);
                    pressing = false;
                }
            }
            else if (chr == 'n')
            {
                if (!pressing && InpMng.pressed_fingers[3])
                {
                    this.transform.localScale = new Vector3(1.5f, 0.15f, 1);
                    pressing = true;
                    if (!note_in)
                    {                   //表示されたときに譜面がなかったとき
                        StartCoroutine("Wait_note");
                    }
                }
                else if (pressing && !InpMng.pressed_fingers[3])
                {
                    this.transform.localScale = new Vector3(0, 0.15f, 1);
                    pressing = false;
                }
            }
            else if (chr == 'm')
            {
                if (!pressing && InpMng.pressed_fingers[4])
                {
                    this.transform.localScale = new Vector3(1.5f, 0.15f, 1);
                    pressing = true;
                    if (!note_in)
                    {                   //表示されたときに譜面がなかったとき
                        StartCoroutine("Wait_note");
                    }
                }
                else if (pressing && !InpMng.pressed_fingers[4])
                {
                    this.transform.localScale = new Vector3(0, 0.15f, 1);
                    pressing = false;
                }
            }

            if (!note_in)
            {
                if (pressing && correct)
                {
                    correct = false;
                }
                else if (!pressing && !correct)
                {
                    correct = true;
                }
            }
            /*
            if(pressing && !note_in && correct){
                correct = false;
            }else if(!pressing && !correct){
                correct = true;
            }
            */
        }

        void OnTriggerStay(Collider other)
        {
            if (!note_in) note_in = true;

            if (pressing)
            {
                correct = true;
                ignore = false;

                if (chr == 'c')
                {
                    this.GetComponent<Renderer>().material.color = Color.black;
                }
                else if (chr == 'v')
                {
                    this.GetComponent<Renderer>().material.color = Color.black;
                }
                else if (chr == 'b')
                {
                    this.GetComponent<Renderer>().material.color = Color.black;
                }
                else if (chr == 'n')
                {
                    this.GetComponent<Renderer>().material.color = Color.black;
                }
                else if (chr == 'm')
                {
                    this.GetComponent<Renderer>().material.color = Color.black;
                }
            }
            else
            {
                correct = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            note_in = true;
            ignore = true;
        }

        private void OnTriggerExit(Collider other)
        {
            //other.enabled = false;
            note_in = false;
            this.GetComponent<Renderer>().material.color = Color.white;
            if (ignore)
            {
                //scmn.ignored_note += 1;
            }
        }

        private IEnumerator Wait_note()
        {
            bool come = false;
            float all_time = 0.2f, delta_time = 0.05f;

            for (int i = 0; i < all_time / delta_time; i++)
            {
                yield return new WaitForSeconds(delta_time);
                if (note_in)
                {
                    come = true;
                    ignore = false;
                }
            }

            //if (!come) scmn.miss_touch += 1;
        }
    }
}
