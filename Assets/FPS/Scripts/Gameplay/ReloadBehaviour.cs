using System.Collections.Generic;
using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.FPS.Gameplay
{
    public class ReloadBehaviour : MonoBehaviour
    {

        [FormerlySerializedAs("CoolingCellsSound")] [Header("Sound")] [Tooltip("Sound played when a cell are cooling")]
        public AudioClip ReloadingSound;

        [Tooltip("Curve for ammo to volume ratio")]
        public AnimationCurve AmmoToVolumeRatioCurve;
        
        WeaponController m_Weapon;
        AudioSource m_AudioSource;
        float m_LastAmmoRatio;

        void Awake()
        {
            m_Weapon = GetComponent<WeaponController>();
            DebugUtility.HandleErrorIfNullGetComponent<WeaponController, OverheatBehavior>(m_Weapon, this, gameObject);

            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.clip = ReloadingSound;
            m_AudioSource.outputAudioMixerGroup = AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.WeaponOverheat);
        }

        void Update()
        {
            // visual smoke shooting out of the gun
            float currentAmmoRatio = m_Weapon.CurrentAmmoRatio;

            // cooling sound
            if (ReloadingSound)
            {
                if (!m_AudioSource.isPlaying
                    && currentAmmoRatio != 1
                    && m_Weapon.IsWeaponActive
                    && m_Weapon.IsReloading)
                {
                    m_AudioSource.Play();
                }
                else if (m_AudioSource.isPlaying
                         && (currentAmmoRatio == 1 || !m_Weapon.IsWeaponActive || !m_Weapon.IsReloading))
                {
                    m_AudioSource.Stop();
                    return;
                }

                m_AudioSource.volume = AmmoToVolumeRatioCurve.Evaluate(1 - currentAmmoRatio);
            }

            m_LastAmmoRatio = currentAmmoRatio;
        }
    }
}