using System;
using System.Collections;
using Unity.FPS.Game;
using UnityEngine;

namespace GameAddOns.Scripts
{
    public class RevolverCylinderHandler : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        [SerializeField] private Transform axis;
        [SerializeField] private Transform cylinder;
        [SerializeField] private WeaponController weaponController;

        private Quaternion _initial;
        private Quaternion _current;
        private Quaternion _to;

        private void Awake()
        {
            _initial = _current = _to = cylinder.localRotation;
        }

        private void OnEnable()
        {
            cylinder.rotation = _to = _current = _initial;
            weaponController.OnShootProcessed += RotateCylinder;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                RotateCylinder();
            }
            
            _current = Quaternion.RotateTowards(_current, _to, rotationSpeed * Time.deltaTime);
            cylinder.transform.localRotation = _current;
        }

        private void OnDisable()
        {
            weaponController.OnShootProcessed -= RotateCylinder;
        }

        private void RotateCylinder()
        {
            _to = Quaternion.Euler(axis.parent.InverseTransformVector(axis.forward) * -90) * _to;
        }
    }
}