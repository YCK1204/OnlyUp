using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    static Manager _instance;
    public static Manager Instance { get { Init(); return _instance; } }

    public static UIManager UI { get { return Instance._ui; } set { Instance._ui = value; } }
    UIManager _ui; 
    static void Init()
    {
        if (_instance == null)
        {
            _instance = new GameObject().AddComponent<Manager>();
            DontDestroyOnLoad(_instance);
            _instance.name = "@Manager";
        }
    }
}
