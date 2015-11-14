using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Класс поиска child'а по имени или тегу.
/// Пример обращения: var obj = transform.SearchChildWithTag("Tag") или var obj = transform.SearchChildWithName("Name")
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Метод поиска child'а по тегу.
    /// </summary>
    public static Transform SearchChildWithTag(this Transform _target, string _tag)
    {
        var _result = SearchChild(_target, _tag, false);
        return _result;
    }

    /// <summary>
    /// Метод поиска child'а по имени.
    /// </summary>
    public static Transform SearchChildWithName(this Transform _target, string _name)
    {
        var _result = SearchChild(_target, _name, true);
        return _result;
    }

    /// <summary>
    /// Логика поиска child'а.
    /// </summary>
    static Transform SearchChild(this Transform _target, string word, bool _nameOrTag) // _nameOrTag = true (_name), false(_tag)
    {
        if (_nameOrTag)
        {
            if (_target.name == word) return _target;
        }
        else
        {
            if (_target.tag == word) return _target;
        }
        for (int i = 0; i < _target.childCount; ++i)
        {
            var _result = SearchChild(_target.GetChild(i), word, _nameOrTag);

            if (_result != null) return _result;
        }
        return null;
    }
}
