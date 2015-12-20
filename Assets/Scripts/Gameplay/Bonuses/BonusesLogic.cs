using System;
using UnityEngine;
using System.Collections;

public class BonusesLogic : MonoBehaviour
{
    private static BonusesLogic _BonusesLogic;
    public static BonusesLogic Instance
    {
        get
        {
            if (_BonusesLogic != null)
            {
                return _BonusesLogic;
            }
            else
            {
                GameObject _prefab = Instantiate(Resources.Load("Gameplay/Bonuses/_Bonuses", typeof(GameObject))) as GameObject;
                _BonusesLogic = _prefab.GetComponent<BonusesLogic>();
                _BonusesLogic.transform.SetParent(GameObject.FindWithTag("InstanceLogics").transform);
                return _BonusesLogic;
            }
        }
    }
    [SerializeField] internal Vector3 _SpawnPosition;
    [SerializeField] internal Bonus[] _Bonuses;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        _BonusesLogic = this;
    }

    internal void SpawnBonus(Transform _position, Vector3 _upper)
    {
        int _randomBonus = UnityEngine.Random.Range(0, _Bonuses.Length);
        int _numberBonus = (int)_Bonuses[_randomBonus]._Bonus;
        _SpawnPosition = _upper;
        GameObject _bonus = Instantiate(_Bonuses[_randomBonus]._Prefab, _position.position + _SpawnPosition, Quaternion.identity) as GameObject;
        BonusLogic _logic = _bonus.GetComponent<BonusLogic>();

        _logic.Init(_numberBonus, _Bonuses[_randomBonus]._Factor, _Bonuses[_randomBonus]._Time, _Bonuses[_randomBonus]._Clip);
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
    [SerializeField] internal float _Time;
    [SerializeField] internal AudioClip _Clip;
}
