using System;
using UnityEngine;

namespace GameAddOns.Scripts
{
    public class GrenadeLauncher : MonoBehaviour
    {
        [SerializeField] private int reward;
        [SerializeField] private int maxGrenades;
        [SerializeField] private int currentGrenades;
        [SerializeField] private float throwForce;
        [SerializeField] private Rigidbody grenade;
        [SerializeField] private Transform grenadeThrowTransform;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G) && currentGrenades > 0)
            {
                --currentGrenades;
                
                var instance = Instantiate(grenade, grenadeThrowTransform.position, grenadeThrowTransform.rotation);
                instance.AddForce(grenadeThrowTransform.forward * throwForce, ForceMode.VelocityChange);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody.TryGetComponent(out GrenadeLauncher launcher))
            {
                Destroy(gameObject);
                launcher.AddGrenade(1);
            }
        }

        public void AddGrenade(int count)
        {
            if (currentGrenades >= maxGrenades) return;
            
            currentGrenades = Mathf.Clamp(currentGrenades + count, 0, maxGrenades);
        }
    }
}