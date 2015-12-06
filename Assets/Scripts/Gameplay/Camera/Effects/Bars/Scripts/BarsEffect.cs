using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/BarsEffect")]
public class BarsEffect : MonoBehaviour
{
    public static BarsEffect Instance;
    public Shader _Shader; //ImageBlendEffect.shader
    [Range(0.0f, 1.0f)]public float _Coverage = 0.1f;
    public Texture _BarTexture;

    public static float NO_COVERAGE = -0.5f;
    public static float FULL_COVERAGE = 0.0f;

    Material _material;

    void Awake()
    {
        Instance = this;
        StartCoroutine(Setting());
    }

    void OnRenderImage(RenderTexture _source, RenderTexture _destination)
    {
        _material.SetTexture("_BarTexture", _BarTexture);
        _material.SetFloat("_Coverage", Mathf.Lerp(NO_COVERAGE, FULL_COVERAGE, _Coverage));
        Graphics.Blit(_source, _destination, _material);
    }

    IEnumerator Setting()
    {
        while (!_material)
        {
            _material = new Material(_Shader);
            yield return null;
        }
        yield return null;
    }
}
