using Player.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerController Player;
    private void Start()
    {
        if (Player == null)
            Debug.LogError("게임 매니저 플레이어 설정 안돼있음");
        if (Manager.Game == null)
        {
            Manager.Game = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
