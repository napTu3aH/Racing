using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class MeshRendererOptimization : MonoBehaviour {
    [SerializeField] internal Transform _MeshRoot;
    [SerializeField] internal bool _Visible;
    [SerializeField] internal MeshRenderer[] _Renderers;
    [SerializeField] internal CarInfo _CarInfo;

    [ContextMenu("Getting Components")]
    void Init()
    {
        _CarInfo = transform.parent.GetComponent<CarInfo>();
        if (_MeshRoot)
        {
            _Renderers = _MeshRoot.GetComponentsInChildren<MeshRenderer>();
        }
    }

    void OnBecameVisible()
    {
        _Visible = true;
        _CarInfo._Visibled = _Visible;
    }

    void OnBecameInvisible()
    {
        _Visible = false;
        _CarInfo._Visibled = _Visible;
    }
}
