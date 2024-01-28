using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

namespace Game.Scripts.Managers
{
    public class TimelineManager : MonoBehaviour
    {
        [SerializeField] private GameObject hassanTheGraver;
        [SerializeField] private Animator animator;
        private Coroutine _timeScaleIndependentCoroutine;

        private void Awake()
        {
            //_timeScaleIndependentCoroutine = StartCoroutine(TimeScaleIndependentCoroutine());
            Time.timeScale = 0f;
        }

        // private IEnumerator TimeScaleIndependentCoroutine()
        // {
        //     while (true)
        //     {
        //         //animator.Update(Time.unscaledDeltaTime);
        //         yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        //     }
        // }

        public void TimelineEnded()
        {
            Time.timeScale = 1;
            hassanTheGraver.gameObject.SetActive(false);
            Destroy(gameObject);
            
        }
        
        
    }
}
