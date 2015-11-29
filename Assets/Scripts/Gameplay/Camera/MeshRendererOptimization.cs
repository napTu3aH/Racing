using UnityEngine;
using System.Collections;

public class MeshRendererOptimization : MonoBehaviour {
    public Transform _MeshRoot;
    public bool _Visible;

    MeshRenderer[] _Renderers;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        if (_MeshRoot)
        {
            _Renderers = _MeshRoot.GetComponentsInChildren<MeshRenderer>();
        }
    }

    void OnBecameVisible()
    {
        _Visible = true;
        Visibling();
    }

    void OnBecameInvisible()
    {
        _Visible = false;
        Visibling();
    }

    void Visibling()
    {
        if (_Renderers.Length > 0)
        {
            for (int i = 0; i < _Renderers.Length; i++)
            {
                _Renderers[i].enabled = _Visible;
            }
        }
    }
}
