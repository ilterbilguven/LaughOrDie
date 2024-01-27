using System;
using Unity.FPS.Game;
using UnityEngine;

namespace GameAddOns.Scripts
{
    public class BulletsToReloadCounter : MonoBehaviour
    {
        [SerializeField] private WeaponController controller;

        private int _bulletsToReload;

        private void LateUpdate()
        {
            if (controller.IsReloading) return;
            
            var currentAmmo = controller.GetCurrentAmmo();
            _bulletsToReload = controller.ClipSize - currentAmmo;

            controller.WeaponAnimator.SetInteger("BulletsToReload", _bulletsToReload);
        }

        private void DecrementRequiredBulletsToReload()
        {
            controller.WeaponAnimator.SetInteger("BulletsToReload", --_bulletsToReload);
        }
    }
}