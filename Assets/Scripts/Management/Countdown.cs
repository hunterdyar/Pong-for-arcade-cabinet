using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pong
{
	public class Countdown : MonoBehaviour
	{
		public static Action<int> OnCountdownStateChange;
		public static Action OnCountdownStart;
		public static Action OnCountdownGo;
		public static Action OnCountdownOver;

		private Coroutine _countdownRoutine;
		public void StartCountdown()
		{
			if (_countdownRoutine != null)
			{
				StopCoroutine(_countdownRoutine);
			}
			_countdownRoutine = StartCoroutine(DoCountdown());
		}

		private IEnumerator DoCountdown()
		{
			OnCountdownStart?.Invoke();
			//countdown
			OnCountdownStateChange?.Invoke(3);
			yield return new WaitForSeconds(1);
			OnCountdownStateChange?.Invoke(2);
			yield return new WaitForSeconds(1);
			OnCountdownStateChange?.Invoke(1);
			yield return new WaitForSeconds(1);
			OnCountdownStateChange?.Invoke(0);
		
			OnCountdownGo?.Invoke();
			yield return new WaitForSeconds(0.75f);
			OnCountdownStateChange?.Invoke(-1);
			OnCountdownOver?.Invoke();
		}
	}
}