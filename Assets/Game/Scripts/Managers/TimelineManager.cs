using System;
using System.Collections;
using UnityEngine;

namespace Game.Scripts.Managers
{
    public class TimelineManager : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private Coroutine _timeScaleIndependentCoroutine;

        private void Awake()
        {
            _timeScaleIndependentCoroutine = StartCoroutine(TimeScaleIndependentCoroutine());

            Time.timeScale = 0f;
        }

        private IEnumerator TimeScaleIndependentCoroutine()
        {
            while (true)
            {
                animator.Update(Time.unscaledDeltaTime);
                yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            }
        }
    }
}
