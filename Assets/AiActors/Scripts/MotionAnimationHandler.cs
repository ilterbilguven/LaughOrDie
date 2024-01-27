using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AiActors.Scripts
{
    public class MotionAnimationHandler : MonoBehaviour
    {
        [SerializeField] private float runSpeed;
        [SerializeField] private float walkSpeed;
        [SerializeField] private Animator animator;

        public Vector3 Velocity { get; set; }

        private void Update()
        {
            var groundVelocity = Vector3.ProjectOnPlane(Velocity, Vector3.up);
            var forwardVelocity = Vector3.Project(Velocity, transform.forward);
            var rightVelocity = Vector3.Project(Velocity, transform.right);

            var (forward, right) = CalculateForwardAndRight(forwardVelocity, rightVelocity);
            
            var walkRunT = Mathf.Clamp01((groundVelocity.magnitude - walkSpeed) / (runSpeed - walkSpeed));
            var currentMaxSpeed = Mathf.Lerp(walkSpeed, runSpeed, walkRunT);

            animator.SetFloat(AnimParams.Speed, walkRunT);
            animator.SetFloat(AnimParams.Right, right * groundVelocity.magnitude / currentMaxSpeed);
            animator.SetFloat(AnimParams.Forward, forward * groundVelocity.magnitude / currentMaxSpeed);
        }

        private (float f, float r) CalculateForwardAndRight(Vector3 forward, Vector3 right)
        {
            var mRight = right.magnitude;
            var mForward = forward.magnitude;
            var sRight = Mathf.Sign(Vector3.Dot(transform.right, right));
            var sForward = Mathf.Sign(Vector3.Dot(transform.forward, forward));
            
            if (mForward > mRight)
            {
                return (sForward, sForward * sRight * mRight / mForward);
            }

            if (mRight > mForward)
            {
                return (sForward * mForward / mRight, sForward * sRight);
            }

            return (sForward, sForward * sRight);
        }
    }
}