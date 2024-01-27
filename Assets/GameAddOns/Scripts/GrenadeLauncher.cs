using System;
using UnityEngine;

namespace GameAddOns.Scripts
{
    public class GrenadeLauncher : MonoBehaviour
    {
        [SerializeField] private float throwForce;
        [SerializeField] private Rigidbody grenade;
        [SerializeField] private Transform grenadeThrowTransform;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                var instance = Instantiate(grenade, grenadeThrowTransform.position, grenadeThrowTransform.rotation);
                instance.AddForce(grenadeThrowTransform.forward * throwForce, ForceMode.VelocityChange);
            }
        }
    }
}