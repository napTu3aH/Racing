using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraSetter : MonoBehaviour
{
    Camera _Camera;
    public float ShadowDistance;
    public int ShadowCascades;

    public float DetailDistance = 500;

    void Start()
    {
        _Camera = GetComponent<Camera>();
        ChangeCamera.Instance._Cameras.Add(_Camera);
        if (_Camera != Camera.main)
        {
            _Camera.enabled = false;
        }
        
    }

    void OnPreRender()
    {
        QualitySettings.shadowDistance = ShadowDistance;
        QualitySettings.shadowCascades = ShadowCascades;
    }

    public static List<Terrain> ActiveTerrains;

    void OnPreCull()
    {
        if (ActiveTerrains != null)
        {
            for (int i = 0; i < ActiveTerrains.Count; i++)
            {
                ActiveTerrains[i].detailObjectDistance = DetailDistance;
            }
        }
    }
}

