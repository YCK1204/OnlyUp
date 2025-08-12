using Player.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatController : MonoBehaviour
{
    [Range(1f, 200f)]
    [SerializeField]
    float _TurnSpeed;
    [SerializeField]
    float _WalkSpeed;
    [SerializeField]
    float _RunSpeed;
    [SerializeField]
    float _JumpForce;
    [SerializeField]
    float _SprintSpeed;
    [SerializeField]
    float _SpeedLerpScale;
    [SerializeField]
    Image _HP;
    [SerializeField]
    Image _Stamina;
    internal float TurnSpeed => _TurnSpeed;
    internal float WalkSpeed => _WalkSpeed;
    internal float RunSpeed => _RunSpeed;
    internal float JumpForce => _JumpForce;
    internal float SprintSpeed => _SprintSpeed;
    internal float SpeedLerpScale => _SpeedLerpScale;
    internal float HP
    {
        get { return _HP.rectTransform.localScale.x; } 
        set 
        {
            var localScale = _HP.rectTransform.localScale;
            localScale.x = Mathf.Clamp(value, 0, 1);
            _HP.rectTransform.localScale = localScale; 
        } 
    }
    internal float Stamina
    {
        get { return _Stamina.rectTransform.localScale.x; }
        set
        {
            var localScale = _Stamina.rectTransform.localScale;
            localScale.x = Mathf.Clamp(value, 0, 1);
            _Stamina.rectTransform.localScale = localScale;
        }
    }
}
