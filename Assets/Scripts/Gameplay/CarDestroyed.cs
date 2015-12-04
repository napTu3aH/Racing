using UnityEngine;
using System.Collections;

public class CarDestroyed : MonoBehaviour {

    [SerializeField] internal Color _Color;
    internal MeshRenderer[] _Renderers;
	
	void Start ()
    {
        GettingRenderers();
        Destruct();
    }

    void GettingRenderers()
    {
        _Color = new Color(0.15f, 0.15f, 0.15f);
        _Renderers = transform.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer _mr in _Renderers)
        {
            if (_mr.name != "MeshRendering")
                _mr.material.color = _Color;
        }
    }

    void Destruct()
    {
        Destroy(this.gameObject, 10.0f);
    }
}
