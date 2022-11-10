using System;
using System.Collections;
using System.Collections.Generic;
using Pong;
using TMPro;
using UnityEngine;

namespace Pong.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class UICountdownText : MonoBehaviour
    {
        private TMP_Text _text;
        private Coroutine countRoutine;
        // Start is called before the first frame update
        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        void OnEnable()
        {
            Countdown.OnCountdownStateChange += OnCountdownStateChange;
        }

        private void OnDisable()
        {
            Countdown.OnCountdownStateChange -= OnCountdownStateChange;
        }

        private void OnCountdownStateChange(int countdownState)
        {
            switch (countdownState)
            {
                case 3:
                    DoCountRoutine("3");
                    break;
                case 2:
                    DoCountRoutine("2");
                    break;
                case 1:
                    DoCountRoutine("1");
                    break;
                case 0:
                    DoCountRoutine("GO!");
                    break;
                default:
                    _text.text = "";
                    _text.enabled = false;
                    break;
            }
        }

        private void DoCountRoutine(string s)
        {
            if (countRoutine != null)
            {
                StopCoroutine(countRoutine);
            }
            _text.enabled = true;
            _text.text = s;
            StartCoroutine(CountRoutine());
        }

        private IEnumerator CountRoutine()
        {
            float t = 0;
            transform.localScale = Vector3.one * 0.8f;
            while (t < 1)
            {
                t += Time.deltaTime;
                transform.localScale = Vector3.one * Mathf.Lerp(0.8f, 1.2f, t);
                _text.alpha = Mathf.Lerp(1, 0.2f, t);
                yield return null;
            }
        }
    }
}