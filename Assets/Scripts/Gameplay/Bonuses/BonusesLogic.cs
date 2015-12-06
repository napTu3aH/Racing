using System;
using UnityEngine;
using System.Collections;

public class BonusesLogic : MonoBehaviour
{
    public static BonusesLogic Instance;
    [SerializeField] internal Bonus[] _Bonuses;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        Instance = this;
    }

    internal void SpawnBonus(Transform _position)
    {
        int _randomBonus = UnityEngine.Random.Range(0, _Bonuses.Length);
        int _numberBonus = (int)_Bonuses[_randomBonus]._Bonus;
        GameObject _bonus = Instantiate(_Bonuses[_randomBonus]._Prefab, _position.position, Quaternion.identity) as GameObject;
        BonusLogic _logic = _bonus.GetComponent<BonusLogic>();

        _logic.Init(_numberBonus, _Bonuses[_randomBonus]._Factor, _Bonuses[_randomBonus]._Clip);
    }
}

[Serializable]
public class Bonus
{
    public string _Name;
    internal enum BonusEffect
    {
        Damage,
        Speed,
        Armor,
        Repair
    }

    [SerializeField] internal BonusEffect _Bonus;
    [SerializeField] internal GameObject _Prefab;
    [SerializeField] internal float _Factor;
    [SerializeField] internal AudioClip _Clip;
}
