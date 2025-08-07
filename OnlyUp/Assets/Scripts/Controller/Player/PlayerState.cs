using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player.State.Management
{
    internal interface IPlayerState
    {
        public int StateId { get; }
        internal void Enter(PlayerStateContext context);
        internal void Exit(PlayerStateContext context);
        internal void Update(PlayerStateContext context);
        internal void FixedUpdate(PlayerStateContext context);
        internal void OnCollisionEnter(PlayerStateContext context, Collision collision);
        internal void OnCollisionExit(PlayerStateContext context, Collision collision);
        internal void OnTriggetEnter(PlayerStateContext context, Collider other);
        internal void OnTriggetExit(PlayerStateContext context, Collider other);
    }
    internal class BasePlayerInputHandler
    {
        protected Dictionary<KeyCode, Action<PlayerStateContext, float>> DirectionKeyHandler = new Dictionary<KeyCode, Action<PlayerStateContext, float>>();
        protected Action<PlayerStateContext> OnEnterShiftKey;
        protected Action<PlayerStateContext> OnExitShiftKey;
        protected Action<PlayerStateContext> OnJumpKey;

        public BasePlayerInputHandler()
        {
            DirectionKeyHandler.Add(KeyCode.W, (context, speed) =>
            {
                context.Speed = speed;
                context.Direction = Vector3.up;
                context.MoveDirection = context.PC.transform.forward;
            });
            DirectionKeyHandler.Add(KeyCode.S, (context, speed) =>
            {
                context.Speed = speed;
                context.Direction = Vector3.down;
                context.MoveDirection = -context.PC.transform.forward;
            });
            DirectionKeyHandler.Add(KeyCode.A, (context, speed) =>
            {
                context.Speed = speed;
                context.Direction = Vector3.up;
                context.PC.transform.Rotate(Vector3.down * context.PC.TurnSpeed * Time.deltaTime);
                context.PC.transform.rotation = Quaternion.Euler(0, context.PC.transform.eulerAngles.y, 0);
                context.MoveDirection = context.PC.transform.forward;
            });
            DirectionKeyHandler.Add(KeyCode.D, (context, speed) =>
            {
                context.Speed = speed;
                context.Direction = Vector3.up;
                context.PC.transform.Rotate(Vector3.up * context.PC.TurnSpeed * Time.deltaTime);
                context.PC.transform.rotation = Quaternion.Euler(0, context.PC.transform.eulerAngles.y, 0);
                context.MoveDirection = context.PC.transform.forward;
            });
        }
        protected virtual void HandleInput(PlayerStateContext context, float speed, Action<PlayerStateContext> elseCallback = null)
        {
            if (OnJumpKey != null && Input.GetKey(KeyCode.Space))
            {
                OnJumpKey.Invoke(context);
                return;
            }

            if (OnExitShiftKey != null && Input.GetKey(KeyCode.LeftShift) == false)
            {
                OnExitShiftKey.Invoke(context);
                return;
            }

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) ||
                Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                if (Input.GetKey(KeyCode.A))
                    DirectionKeyHandler[KeyCode.A](context, speed);
                else if (Input.GetKey(KeyCode.D))
                    DirectionKeyHandler[KeyCode.D](context, speed);
                if (Input.GetKey(KeyCode.W))
                    DirectionKeyHandler[KeyCode.W](context, speed);
                else if (Input.GetKey(KeyCode.S))
                    DirectionKeyHandler[KeyCode.S](context, speed);

                OnEnterShiftKey?.Invoke(context);
            }
            else
            {
                elseCallback?.Invoke(context);
            }
        }
    }
    internal class PlayerStateContext
    {
        internal Animator Animator { get; set; }
        internal Rigidbody Rigidbody { get; set; }
        internal PlayerMoveController PC { get; set; }
        bool _isJumping = false;
        internal bool IsJumping
        {
            get { return _isJumping; }
            set
            {
                if (_isJumping == value)
                    return;
                _isJumping = value;
                Animator.SetBool("IsJumping", _isJumping);
            }
        }
        bool _isFalling = false;
        internal bool IsFalling
        {
            get { return _isFalling; }
            set
            {
                if (_isFalling == value)
                    return;
                _isFalling = value;
                Animator.SetBool("IsFalling", _isFalling);
            }
        }

        float _speed = 0;
        internal Vector3 MoveDirection { get; set; }
        public float Speed
        {
            get { return _speed; }
            set
            {
                if (_speed == value)
                    return;
                var prevSpeed = _speed;
                _speed = value;
                if (_speed == 0)
                    Animator.SetBool("IsMoving", false);
                else if (prevSpeed > 0 && _speed > 0)
                    Animator.SetBool("IsMoving", true);
                Animator.SetFloat("Speed", _speed);
            }
        }
        Vector3 _direction;
        internal Vector3 Direction
        {
            get
            {
                return _direction;
            }
            set
            {
                if (_direction == value)
                    return;
                _direction = value;
                Animator.SetFloat("Velocity Y", _direction.y);
            }
        }
        public PlayerStateContext(PlayerMoveController pc)
        {
            Animator = pc.GetComponent<Animator>();
            Rigidbody = pc.GetComponent<Rigidbody>();
            if (Animator == null)
                Debug.LogError("플레이어 애니메이터 없음");
            if (Rigidbody == null)
                Debug.LogError("플레이어 리지드바디 없음");
            PC = pc;
        }
    }
    internal class PlayerIdleState : IPlayerState
    {
        static PlayerIdleState _instance = new PlayerIdleState();
        public static IPlayerState Instance { get { return _instance; } }
        enum IdleState
        {
            IdleDefault,
        }
        IdleState State = IdleState.IdleDefault;
        public PlayerIdleState()
        {
        }
        public int StateId => (int)State;
        void IPlayerState.Enter(PlayerStateContext context)
        {
            context.Speed = 0f;
        }
        void IPlayerState.Exit(PlayerStateContext context)
        {
        }
        void IPlayerState.Update(PlayerStateContext context)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                context.PC.State = PlayerJumpState.Instance;
                return;
            }
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                context.PC.State = PlayerWalkState.Instance;
        }
        void IPlayerState.FixedUpdate(PlayerStateContext context)
        {
        }
        void IPlayerState.OnCollisionEnter(PlayerStateContext context, Collision collision)
        {
        }

        void IPlayerState.OnCollisionExit(PlayerStateContext context, Collision collision)
        {
        }

        void IPlayerState.OnTriggetEnter(PlayerStateContext context, Collider other)
        {
        }

        void IPlayerState.OnTriggetExit(PlayerStateContext context, Collider other)
        {

        }
    }
    internal class PlayerWalkState : BasePlayerInputHandler, IPlayerState
    {
        static PlayerWalkState _instance = new PlayerWalkState();
        public static IPlayerState Instance { get { return _instance; } }
        enum WalkState
        {
            StateDefault,
        }
        WalkState State = WalkState.StateDefault;
        public PlayerWalkState()
        {
            OnEnterShiftKey = (context) =>
            {
                context.PC.State = PlayerRunState.Instance;
            };
            OnJumpKey = (context) =>
            {
                context.PC.State = PlayerJumpState.Instance;
            };
        }
        public int StateId => (int)State;
        void IPlayerState.Enter(PlayerStateContext context)
        {
        }

        void IPlayerState.Exit(PlayerStateContext context)
        {
        }

        void IPlayerState.Update(PlayerStateContext context)
        {

        }
        void IPlayerState.FixedUpdate(PlayerStateContext context)
        {
            var nextSpeed = Mathf.Lerp(context.Speed, context.PC.WalkSpeed, Time.deltaTime * context.PC.SpeedLerpScale);
            HandleInput(context, nextSpeed, (ct) =>
            {
                var reducedSpeed = Mathf.Lerp(context.Speed, 0, Time.deltaTime * context.PC.SpeedLerpScale * 2f);
                context.Speed = reducedSpeed;

                if (context.Speed < .5f)
                    ct.PC.State = PlayerIdleState.Instance;
            });
            Move(context);
        }
        void Move(PlayerStateContext context)
        {
            context.PC.transform.position += context.MoveDirection * context.Speed * Time.deltaTime;
        }
        void IPlayerState.OnCollisionEnter(PlayerStateContext context, Collision collision)
        {
        }
        void IPlayerState.OnCollisionExit(PlayerStateContext context, Collision collision)
        {
        }

        void IPlayerState.OnTriggetEnter(PlayerStateContext context, Collider other)
        {

        }

        void IPlayerState.OnTriggetExit(PlayerStateContext context, Collider other)
        {

        }
    }
    internal class PlayerRunState : BasePlayerInputHandler, IPlayerState
    {
        static PlayerRunState _instance = new PlayerRunState();
        public static IPlayerState Instance { get { return _instance; } }
        enum RunState
        {
            StateDefault,
        }
        RunState State = RunState.StateDefault;
        public PlayerRunState()
        {
            OnEnterShiftKey = (context) =>
            {
                if (Input.GetKey(KeyCode.S))
                    return;
                if (context.Speed > context.PC.RunSpeed * .9f)
                    context.PC.State = PlayerSprintState.Instance;
            };
            OnExitShiftKey = (context) =>
            {
                var reducedSpeed = Mathf.Lerp(context.Speed, context.PC.WalkSpeed, Time.deltaTime * context.PC.SpeedLerpScale * 2f);
                context.Speed = reducedSpeed;

                if (context.Speed < context.PC.WalkSpeed * 1.1)
                    context.PC.State = PlayerWalkState.Instance;
            };
            OnJumpKey = (context) =>
            {
                context.PC.State = PlayerJumpState.Instance;
            };
        }
        public int StateId => (int)State;
        void IPlayerState.Enter(PlayerStateContext context)
        {
        }

        void IPlayerState.Exit(PlayerStateContext context)
        {
        }

        void IPlayerState.Update(PlayerStateContext context)
        {

        }
        void IPlayerState.FixedUpdate(PlayerStateContext context)
        {
            var nextSpeed = Mathf.Lerp(context.Speed, context.PC.RunSpeed, Time.deltaTime * context.PC.SpeedLerpScale);

            HandleInput(context, nextSpeed, (ct) =>
            {
                var reducedSpeed = Mathf.Lerp(context.Speed, context.PC.WalkSpeed, Time.deltaTime * context.PC.SpeedLerpScale * 2f);
                context.Speed = reducedSpeed;

                if (context.Speed < context.PC.WalkSpeed * 1.1)
                    ct.PC.State = PlayerWalkState.Instance;
            });
            Move(context);
        }
        void IPlayerState.OnCollisionEnter(PlayerStateContext context, Collision collision)
        {
        }
        void Move(PlayerStateContext context)
        {
            context.PC.transform.position += context.MoveDirection * context.Speed * Time.deltaTime;
        }
        void IPlayerState.OnCollisionExit(PlayerStateContext context, Collision collision)
        {
        }

        void IPlayerState.OnTriggetEnter(PlayerStateContext context, Collider other)
        {

        }

        void IPlayerState.OnTriggetExit(PlayerStateContext context, Collider other)
        {

        }
    }
    internal class PlayerSprintState : BasePlayerInputHandler, IPlayerState
    {
        static PlayerSprintState _instance = new PlayerSprintState();
        public static IPlayerState Instance { get { return _instance; } }
        enum SprintState
        {
            StateDefault,
        }
        SprintState State = SprintState.StateDefault;
        public PlayerSprintState()
        {
            OnExitShiftKey = (context) =>
            {
                var reducedSpeed = Mathf.Lerp(context.Speed, context.PC.RunSpeed, Time.deltaTime * context.PC.SpeedLerpScale * 2f);
                context.Speed = reducedSpeed;

                if (context.Speed < context.PC.RunSpeed * 1.1)
                    context.PC.State = PlayerRunState.Instance;
            };
            OnJumpKey = (context) =>
            {
                context.PC.State = PlayerJumpState.Instance;
            };
        }
        public int StateId => (int)State;
        void IPlayerState.Enter(PlayerStateContext context)
        {

        }

        void IPlayerState.Exit(PlayerStateContext context)
        {
        }

        void IPlayerState.Update(PlayerStateContext context)
        {
        }
        void IPlayerState.FixedUpdate(PlayerStateContext context)
        {
            var nextSpeed = Mathf.Lerp(context.Speed, context.PC.SprintSpeed, Time.deltaTime * context.PC.SpeedLerpScale);

            HandleInput(context, nextSpeed, (ct) =>
            {
                var reducedSpeed = Mathf.Lerp(context.Speed, context.PC.RunSpeed, Time.deltaTime * context.PC.SpeedLerpScale * 2f);
                context.Speed = reducedSpeed;

                if (context.Speed < context.PC.RunSpeed * 1.1)
                    ct.PC.State = PlayerRunState.Instance;
            });
            Move(context);
        }
        void Move(PlayerStateContext context)
        {
            context.PC.transform.position += context.MoveDirection * context.Speed * Time.deltaTime;
        }
        void IPlayerState.OnCollisionEnter(PlayerStateContext context, Collision collision)
        {
        }

        void IPlayerState.OnCollisionExit(PlayerStateContext context, Collision collision)
        {
        }

        void IPlayerState.OnTriggetEnter(PlayerStateContext context, Collider other)
        {
        }

        void IPlayerState.OnTriggetExit(PlayerStateContext context, Collider other)
        {

        }
    }
    internal class PlayerJumpState : BasePlayerInputHandler, IPlayerState
    {
        static PlayerJumpState _instance = new PlayerJumpState();
        public static IPlayerState Instance { get { return _instance; } }
        enum JumpState
        {
            StateDefault,
        }
        JumpState State = JumpState.StateDefault;
        public PlayerJumpState()
        {
        }
        public int StateId => (int)State;
        void IPlayerState.Enter(PlayerStateContext context)
        {
            context.Rigidbody.AddForce(Vector3.up * context.PC.JumpForce, ForceMode.Impulse);
            context.IsJumping = true;
        }
        void IPlayerState.Exit(PlayerStateContext context)
        {
            context.IsJumping = false;
            context.IsFalling = false;
        }

        void IPlayerState.Update(PlayerStateContext context)
        {
        }
        void IPlayerState.FixedUpdate(PlayerStateContext context)
        {
            var speed = Mathf.Lerp(context.Speed, context.PC.WalkSpeed, Time.deltaTime * context.PC.SpeedLerpScale);
            
            HandleInput(context, speed, (ct) =>
            {
                var reducedSpeed = Mathf.Lerp(context.Speed, 0, Time.deltaTime * context.PC.SpeedLerpScale * 2f);
                context.Speed = reducedSpeed;
            });
        }
        protected override void HandleInput(PlayerStateContext context, float speed, Action<PlayerStateContext> elseCallback = null)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) ||
                Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    speed = Mathf.Lerp(context.Speed, context.PC.SprintSpeed, Time.deltaTime * context.PC.SpeedLerpScale * 2f);

                if (Input.GetKey(KeyCode.A))
                    DirectionKeyHandler[KeyCode.A](context, speed);
                else if (Input.GetKey(KeyCode.D))
                    DirectionKeyHandler[KeyCode.D](context, speed);
                if (Input.GetKey(KeyCode.W))
                    DirectionKeyHandler[KeyCode.W](context, speed);
                else if (Input.GetKey(KeyCode.S))
                    DirectionKeyHandler[KeyCode.S](context, speed);
            }
            else
            {
                elseCallback?.Invoke(context);
            }
            Move(context);
        }
        void Move(PlayerStateContext context)
        {
            context.PC.transform.position += context.MoveDirection * context.Speed * Time.deltaTime;
        }
        void IPlayerState.OnCollisionEnter(PlayerStateContext context, Collision collision)
        {
            if (context.Speed > context.PC.RunSpeed)
                context.PC.State = PlayerSprintState.Instance;
            else if (context.Speed > context.PC.WalkSpeed)
                context.PC.State = PlayerRunState.Instance;
            else if (context.Speed > .5f)
                context.PC.State = PlayerWalkState.Instance;
            else
                context.PC.State = PlayerIdleState.Instance;
        }
        void IPlayerState.OnCollisionExit(PlayerStateContext context, Collision collision)
        {
        }

        void IPlayerState.OnTriggetEnter(PlayerStateContext context, Collider other)
        {

        }

        void IPlayerState.OnTriggetExit(PlayerStateContext context, Collider other)
        {

        }
    }
}