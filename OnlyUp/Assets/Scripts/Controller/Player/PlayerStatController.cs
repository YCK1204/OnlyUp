using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatController : MonoBehaviour
{
    [Range(1f, 200f)]
    [SerializeField]
    internal float TurnSpeed;
    [SerializeField]
    internal float WalkSpeed;
    [SerializeField]
    internal float RunSpeed;
    [SerializeField]
    internal float JumpForce;
    [SerializeField]
    internal float SprintSpeed;
    [SerializeField]
    internal float SpeedLerpScale;
}
