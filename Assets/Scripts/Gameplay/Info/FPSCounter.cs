﻿using UnityEngine;
using System.Collections;

public class FPSCounter : MonoBehaviour {

    const float fpsMeasurePeriod = 0.5f;
    private int m_FpsAccumulator = 0;
    private float m_FpsNextPeriod = 0;
    private int m_CurrentFps;
    const string display = "{0} FPS";
    private UILabel m_Text;

    void Awake()
    {
        m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
        m_Text = GetComponent<UILabel>();
        
    }

    void OnEnable()
    {
        StartCoroutine(FPS());
    }

    IEnumerator FPS()
    {
        while (true)
        {
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > m_FpsNextPeriod)
            {
                m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
                m_FpsAccumulator = 0;
                m_FpsNextPeriod += fpsMeasurePeriod;
                m_Text.text = string.Format(display, m_CurrentFps);
            }
            yield return null;
        }        
    }
}
