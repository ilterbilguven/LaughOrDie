using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Helpers
{
    public class RagdollHelper : MonoBehaviour
    {
        [SerializeField] private List<Rigidbody> ragdollRigidbodies;
        [SerializeField] private List<Collider> ragdollColliders;
        [SerializeField] private Animator animator;
        
        public void EnableRagdoll()
        {
            animator.enabled = false;
            foreach (var rb in ragdollRigidbodies)
            {
                rb.isKinematic = false;
            }
            foreach (var col in ragdollColliders)
            {
                col.enabled = true;
            }
        }
        
        public void DisableRagdoll()
        {
            animator.enabled = true;
            foreach (var rb in ragdollRigidbodies)
            {
                rb.isKinematic = true;
            }
            foreach (var col in ragdollColliders)
            {
                col.enabled = false;
            }
        }
    }
}
