using Player.State.Management;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace Player.State.Management
{
    public class PlayerMoveController : MonoBehaviour
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

        IPlayerState _curState = PlayerIdleState.Instance;
        internal IPlayerState State
        {
            get { return _curState; }
            set
            {
                if (_curState == value)
                    return;
                _curState.Exit(context);
                _curState = value;
                _curState.Enter(context);
            }
        }

        PlayerStateContext context;
        private void Start()
        {
            context = new PlayerStateContext(this);
        }
        private void Update()
        {
            _curState.Update(context);
        }
        private void OnCollisionEnter(Collision collision)
        {
            _curState.OnCollisionEnter(context, collision);
        }
        private void OnCollisionExit(Collision collision)
        {
            _curState.OnCollisionExit(context, collision);
        }
        private void OnTriggerEnter(Collider other)
        {
            _curState.OnTriggetEnter(context, other);
        }
        private void OnTriggerExit(Collider other)
        {
            _curState.OnTriggetExit(context, other);
        }
        private void FixedUpdate()
        {
            _curState.FixedUpdate(context);
        }
    }
}