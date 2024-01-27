using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace AiActors.Scripts
{
    public class DebugMotion : MonoBehaviour
    {
        [SerializeField] private float runSpeed;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float turnSpeed;
        [SerializeField] private float accelaration;
        [SerializeField] private MotionAnimationHandler motionAnimationHandler;

        private float _turn;
        private Vector3 _input;
        private Vector3 _velocity;
        private Transform _myTransform;

        private void Awake()
        {
            _myTransform = transform;
        }

        private void Update()
        {
            CalculateAndApplyRotation();
            CalculateVelocity();

            motionAnimationHandler.Velocity = _velocity;
        }

        private void CalculateAndApplyRotation()
        {
            _turn = (Input.GetKey(KeyCode.E) ? 1 : 0) + (Input.GetKey(KeyCode.Q) ? -1 : 0);
            _myTransform.Rotate(0, _turn * turnSpeed * Time.deltaTime, 0);
        }

        private void CalculateVelocity()
        {
            _input.z = (Input.GetKey(KeyCode.W) ? 1 : 0) + (Input.GetKey(KeyCode.S) ? -1 : 0);
            _input.x = (Input.GetKey(KeyCode.D) ? 1 : 0) + (Input.GetKey(KeyCode.A) ? -1 : 0);
            _input.Normalize();

            var maxSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            var targetVelocity = maxSpeed * _input;

            _velocity = Vector3.MoveTowards(_velocity, targetVelocity, accelaration * Time.deltaTime);
        }
    }
}