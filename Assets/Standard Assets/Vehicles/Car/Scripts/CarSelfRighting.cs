using System;
using System.Collections;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    public class CarSelfRighting : MonoBehaviour
    {
        [SerializeField] float _WaitTime = 3f;
        [SerializeField] float _VelocityThreshold = 1f;
        [SerializeField] Rigidbody _Rigidbody;


        void Start()
        {
            _Rigidbody = GetComponent<Rigidbody>();
            StartCoroutine(CarSelf());
        }

        void RightCar()
        {
            transform.position += Vector3.up;
            transform.rotation = Quaternion.LookRotation(transform.forward);
        }

        IEnumerator CarSelf()
        {
            while (_Rigidbody)
            {
                if (transform.up.y < 0f)
                {
                    if (_Rigidbody.velocity.magnitude < _VelocityThreshold)
                    {
                        yield return new WaitForSeconds(_WaitTime);
                        RightCar();
                    }
                    
                }
                yield return null;
            }
            yield return null;
        }
    }
}
