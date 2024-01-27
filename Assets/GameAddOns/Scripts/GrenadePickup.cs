using System;
using UnityEngine;

namespace GameAddOns.Scripts
{
    public class GrenadePickup : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out GrenadeLauncher grenadeLauncher))
            {
                grenadeLauncher.AddGrenade(1);
            }
        }
    }
}