using System;
using UnityEngine;
using System.Collections.Generic;
using UnityStandardAssets.Cameras;

namespace UnityStandardAssets.Vehicles.Car
{
    public class CarUserControl : MonoBehaviour
    {
        public static CarUserControl Instance;
        internal enum _TypeControl
        {
            None,
            PC,
            Type1,
            Type2,
            Type3
        }

        public CarController m_Car;
        public AutoCam _AutoCam;
        public Transform _CameraTarget;
        [SerializeField] internal _TypeControl _TypesControl;
        [SerializeField] internal List<GameObject> _ImagesTypesControl; // 0 - Type1, 1 - Type2 ... n - TypeN+1
        internal bool _shooting;
        
        float h, v, j, _ChangeSpeed = 2.5f, _max;
        bool _left, _right, _forward, _back;
        int _NumberInput = 0;

        void Awake()
        {
            Instance = this;

#if UNITY_STANDALONE || UNITY_EDITOR            
            ChangeInput();
#elif !UNITY_STANDALONE || !UNITY_EDITOR
            _NumberInput++;
            ChangeInput();
#endif
        }

        public void CarSet(CarController _controller, Transform _carTransform)
        {
            m_Car = _controller;
            _CameraTarget = _carTransform.SearchChildWithName("CameraTarget");
            SetCamera();
        }

        void SetCamera()
        {
            _AutoCam.SetTarget(_CameraTarget);
        }
        public void ChangeInput()
        {
            _NumberInput++;
            if (Enum.IsDefined(typeof(_TypeControl), _NumberInput))
            {
                _TypesControl = (_TypeControl)_NumberInput;
            }
            else
            {
#if UNITY_STANDALONE || UNITY_EDITOR
                _NumberInput = 1;
#elif !UNITY_STANDALONE || !UNITY_EDITOR
                _NumberInput = 2;
#endif
                _TypesControl = (_TypeControl)_NumberInput;
            }

            _left = false;
            _right = false;
            _forward = false;
            _back = false;

            Notification.Instance.Notificate("Input changet to: " + _TypesControl.ToString());

            foreach (GameObject _gm in _ImagesTypesControl)
            {
                if (_gm.name == _TypesControl.ToString())
                {                    
                    _gm.SetActive(true);
                    continue;
                }
                _gm.SetActive(false);
            }
        }

        float ChangeValue(float value)
        {
            if (value > 0.0f)
            {
                value -= Time.deltaTime * _ChangeSpeed;
                if (value <= 0.0f)
                {
                    value = 0.0f;
                }
            } else
            if (value < 0.0f)
            {
                value += Time.deltaTime * _ChangeSpeed;
                if (value >= 0.0f)
                {
                    value = 0.0f;
                }
            }

            return value;
        }
        float ChangeValue(float value, bool plus)
        {
            if (plus)
            {
                if (value < 1.0f)
                {
                    value += Time.deltaTime * _ChangeSpeed;
                }
                else
                {
                    value = 1.0f;
                }
            }
            else
            {
                if (value > -1.0f)
                {
                    value -= Time.deltaTime * _ChangeSpeed;
                }
                else
                {
                    value = -1.0f;
                }
            }
            return value;
        }

        void ControlValue()
        {
            if (_forward)
            {
                v = Mathf.Clamp(ChangeValue(v, true), -1, _max);
            }
            if (_back)
            {
                v = Mathf.Clamp(ChangeValue(v, false), -1, _max);
            }

            if (_TypesControl != _TypeControl.Type3)
            {
                if (_left && h != -1.0f)
                {
                    h = ChangeValue(h, false);
                }
                if (_right && h != 1.0f)
                {
                    h = ChangeValue(h, true);
                }
                if (!_left && !_right && h != 0.0f)
                {
                    h = ChangeValue(h);
                }
            }
        }

        void LateUpdate()
        {
            if (m_Car)
            {
                switch (_TypesControl)
                {
                    #region TypeControl - PC
                    case _TypeControl.PC:
                        h = Input.GetAxis("Horizontal");
                        v = Input.GetAxis("Vertical");
                        
                        if (Input.GetKey(KeyCode.Space) && !_shooting)
                        {
                            _shooting = true;
                        }
                        else
                            if (Input.GetKeyUp(KeyCode.Space) && _shooting)
                        {
                            _shooting = false;
                        }
                        break;
                    #endregion

                    #region TypeControl - One
                    case _TypeControl.Type1:
                        if (!_forward && !_back)
                        {
                            _forward = true;
                        }
                        else
                        if (_forward && _back)
                        {
                            _forward = false;
                        }
                        ControlValue();
                        break;
                    #endregion

                    #region TypeControl - Two
                    case _TypeControl.Type2:
                        if (!_forward && !_back && v != 0.0f)
                        {
                            v = ChangeValue(v);
                        }
                        ControlValue();
                        break;
                    #endregion

                    #region TypeControl - Three
                    case _TypeControl.Type3:
                        h = Input.acceleration.x;
                        ControlValue();
                        break;
                        #endregion
                }

                m_Car.Move(h, v, v, 0);
            }
        }

        public void PressLeftButton()
        {
            _max = 0.5f;
            _left = true;
        }
        public void ReleaseLeftButton()
        {
            _max = 1.0f;
            _left = false;
        }
        public void PressRightButton()
        {
            _max = 0.5f;
            _right = true;
        }
        public void ReleaseRightButton()
        {
            _max = 1.0f;
            _right = false;
        }
        public void PressForwardButton()
        {
            _forward = true;
        }
        public void ReleaseForwardButton()
        {
            _forward = false;
        }
        public void PressBackButton()
        {
            v = 0.0f;
            _back = true;
        }
        public void ReleaseBackButton()
        {
            v = 0.0f;
            _back = false;
        }

        public void ShootOnePress()
        {
            _shooting = true;
        }
        public void ShootOneRelease()
        {
            _shooting = false;
        }

    }
}
