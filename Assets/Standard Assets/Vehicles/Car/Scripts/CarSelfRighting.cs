using System;
using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public class CarSelfRighting : MonoBehaviour
    {
        // Automatically put the car the right way up, if it has come to rest upside-down.
        [SerializeField] float _WaitTime = 3f;           // time to wait before self righting
        [SerializeField] float _VelocityThreshold = 1f;  // the velocity below which the car is considered stationary for self-righting
        [SerializeField] Rigidbody _Rigidbody;


        void Start()
        {
            _Rigidbody = GetComponent<Rigidbody>();
            StartCoroutine(CarSelf());
        }

        // put the car back the right way up:
        void RightCar()
        {
            // set the correct orientation for the car, and lift it off the ground a little
            transform.position += Vector3.up;
            transform.rotation = Quaternion.LookRotation(transform.forward);
        }

        IEnumerator CarSelf()
        {
            while (_Rigidbody)
            {
                if (transform.up.y < 0f && _Rigidbody.velocity.magnitude < _VelocityThreshold)
                {
                    yield return new WaitForSeconds(_WaitTime);
                    RightCar();
                }
                yield return null;
            }
            yield return null;
        }
    }
}
