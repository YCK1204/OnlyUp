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
            Debug.LogError("���� �Ŵ��� �÷��̾� ���� �ȵ�����");
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
