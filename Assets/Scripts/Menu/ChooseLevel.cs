using UnityEngine;
using System.Collections;

public class ChooseLevel : MonoBehaviour
{
    [SerializeField] internal int _LevelNumber;
    [SerializeField] internal bool _ShowBackground;

    public void Load()
    {
        LoadingLevel.Instance._Indicator.Init(_LevelNumber, _ShowBackground);
    }
	
}
