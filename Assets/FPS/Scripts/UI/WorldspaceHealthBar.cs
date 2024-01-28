using System.Collections;
using DG.Tweening;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.UI
{
    public class WorldspaceHealthBar : MonoBehaviour
    {
        [Tooltip("Health component to track")] public Health Health;

        [Tooltip("Image component displaying health left")]
        public Image HealthBarImage;

        [Tooltip("The floating healthbar pivot transform")]
        public Transform HealthBarPivot;

        [Tooltip("Whether the health bar is visible when at full health or not")]
        public bool HideFullHealthBar = true;

        private bool _healthBarDisabling;

        void Update()
        {
            if (_healthBarDisabling) return;
            
            // update health bar value
            HealthBarImage.fillAmount = Health.CurrentHealth / Health.MaxHealth;

            // rotate health bar to face the camera/player
            HealthBarPivot.LookAt(Camera.main.transform.position);

            // hide health bar if needed
            if (HideFullHealthBar)
                HealthBarPivot.gameObject.SetActive(HealthBarImage.fillAmount != 1);
            else if (HealthBarImage.fillAmount <= 0)
            {
                _healthBarDisabling = true;
                HealthBarImage.DOFade(0, 0.15f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    HealthBarImage.gameObject.SetActive(false);
                });
            }
        }
    }
}