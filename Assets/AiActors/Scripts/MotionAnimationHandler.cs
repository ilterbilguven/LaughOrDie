using UnityEngine;

namespace AiActors.Scripts
{
    public class MotionAnimationHandler : MonoBehaviour
    {
        [SerializeField] private float runSpeed;
        [SerializeField] private float walkSpeed;
        [SerializeField] private Animator animator;

        public Vector3 Velocity { get; set; }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var forwardVelocity = Vector3.Project(Velocity, transform.forward);
            var rightVelocity = Vector3.Project(Velocity, transform.right);

            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.DrawLine(transform.position, transform.position + new Vector3(0, 0, Velocity.z));
            
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.DrawLine(transform.position, transform.position + new Vector3(Velocity.x, 0, 0));

            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawLine(transform.position, transform.position + transform.right * 10, 1);
            
            UnityEditor.Handles.color = Color.blue;
            UnityEditor.Handles.DrawLine(transform.position, transform.position + transform.forward * 10, 1);

            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawLine(transform.position, transform.position + forwardVelocity, 1);

            UnityEditor.Handles.color = Color.magenta;
            UnityEditor.Handles.DrawLine(transform.position, transform.position + rightVelocity, 1);
        }
        #endif

        private void Update()
        {
            var groundVelocity = Vector3.ProjectOnPlane(Velocity, Vector3.up);
            var forwardVelocity = Vector3.Project(Velocity, transform.forward);
            var rightVelocity = Vector3.Project(Velocity, transform.right);

            var (forward, right) = CalculateForwardAndRight(forwardVelocity, rightVelocity);
            
            var walkRunT = Mathf.Clamp01((groundVelocity.magnitude - walkSpeed) / (runSpeed - walkSpeed));
            var currentMaxSpeed = Mathf.Lerp(walkSpeed, runSpeed, walkRunT);

            animator.SetFloat(AnimParams.Speed, walkRunT);
            animator.SetFloat(AnimParams.Right, right / currentMaxSpeed);
            animator.SetFloat(AnimParams.Forward, forward / currentMaxSpeed);
        }

        private (float f, float r) CalculateForwardAndRight(Vector3 forward, Vector3 right)
        {
            var mRight = right.magnitude;
            var mForward = forward.magnitude;
            var projectedRight = Vector3.Project(transform.right, right);
            var projectedForward = Vector3.Project(transform.forward, forward);
            var sRight = Mathf.Sign(Vector3.Dot(projectedRight, right));
            var sForward = Mathf.Sign(Vector3.Dot(projectedForward, forward));
            
            if (mForward > mRight)
            {
                return (sForward * mForward, sRight * mRight / mForward);

                // if (mForward > 0)
                // return (mForward, sRight * mRight / mForward);

                // return (sForward, -sRight * mRight / mForward);
            }

            if (mRight > mForward)
            {
                return (sForward * mForward / mRight, sRight * mRight);
                // if (mForward > 0)
                // return (sForward * mForward / mRight, sRight);

                // return (sForward * mForward / mRight, -sRight);
            }

            return (sForward * mForward, sRight * mRight);
            // return (sForward, sForward * sRight);
        }
    }
}