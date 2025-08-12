using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Controller
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

                Vector3 direction = context.Direction;
                direction.y = Mathf.Lerp(context.Direction.y, 1, context.SpeedLerpScale * Time.deltaTime);
                context.Direction = direction;
                context.MoveDirection = context.Transform.forward;
            });
            DirectionKeyHandler.Add(KeyCode.S, (context, speed) =>
            {
                context.Speed = speed;
                Vector3 direction = context.Direction;
                float y = Mathf.Lerp(context.Direction.y, -1, context.SpeedLerpScale * Time.deltaTime);
                direction.y = y;
                context.Direction = direction;
                context.MoveDirection = -context.Transform.forward;
            });
            DirectionKeyHandler.Add(KeyCode.A, (context, speed) =>
            {
                context.Speed = speed;
                Vector3 direction = context.Direction;
                direction.y = Mathf.Lerp(context.Direction.y, 1, context.SpeedLerpScale * Time.deltaTime);
                context.Direction = direction;
                context.Transform.Rotate(Vector3.down * context.TurnSpeed * Time.deltaTime);
                context.Transform.rotation = Quaternion.Euler(0, context.Transform.eulerAngles.y, 0);
                context.MoveDirection = context.Transform.forward;
            });
            DirectionKeyHandler.Add(KeyCode.D, (context, speed) =>
            {
                context.Speed = speed;
                Vector3 direction = context.Direction;
                direction.y = Mathf.Lerp(context.Direction.y, 1, context.SpeedLerpScale * Time.deltaTime);
                context.Direction = direction;
                context.Transform.Rotate(Vector3.up * context.TurnSpeed * Time.deltaTime);
                context.Transform.rotation = Quaternion.Euler(0, context.Transform.eulerAngles.y, 0);
                context.MoveDirection = context.Transform.forward;
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

                if (Input.GetKey(KeyCode.LeftShift))
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
        PlayerMoveController PC { get; set; }
        internal bool CanJump => PC.CanJump();
        internal float TurnSpeed => PC._stat.TurnSpeed;
        internal float WalkSpeed => PC._stat.WalkSpeed;
        internal float RunSpeed => PC._stat.RunSpeed;
        internal float JumpForce => PC._stat.JumpForce;
        internal float SprintSpeed => PC._stat.SprintSpeed;
        internal float SpeedLerpScale => PC._stat.SpeedLerpScale;
        internal IPlayerState State { get { return PC.State; } set { PC.State = value; } }
        internal Transform Transform { get { return PC.transform; } }
        internal Vector3 Position { get { return PC.transform.position; } set { PC.transform.position = value; } }
        internal Quaternion Rotation { get { return PC.transform.rotation; } set { PC.transform.rotation = value; } }
        internal Vector3 LocalScale { get { return PC.transform.localScale; } set { PC.transform.localScale = value; } }
        internal float HP
        {
            get { return PC._stat.HP.rectTransform.localScale.x; }
            set
            {
                var localScale = PC._stat.HP.rectTransform.localScale;
                localScale.x = Mathf.Clamp(value, 0, 1f);
                PC._stat.HP.rectTransform.localScale = localScale;
            }
        }
        internal float Stamina
        {
            get { return PC._stat.Stamina.rectTransform.localScale.x; }
            set
            {
                var localScale = PC._stat.Stamina.rectTransform.localScale;
                localScale.x = Mathf.Clamp(value, 0, 1f);
                PC._stat.Stamina.rectTransform.localScale = localScale;
            }
        }
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
        internal bool IsGrounded => PC.IsGrounded();
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
        internal float JumpCheckInterval = 0.1f;

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
        float _lastJumpCheckTime = 0;
        void IPlayerState.Enter(PlayerStateContext context)
        {
            context.Speed = 0f;
        }
        void IPlayerState.Exit(PlayerStateContext context)
        {
        }
        void IPlayerState.Update(PlayerStateContext context)
        {
            if (Input.GetKey(KeyCode.Space) && context.CanJump)
            {
                context.Stamina -= .1f;
                context.Rigidbody.AddForce(Vector3.up * context.JumpForce, ForceMode.Impulse);
                context.State = PlayerJumpState.Instance;
                return;
            }
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                context.State = PlayerWalkState.Instance;
            if (Time.time - _lastJumpCheckTime > context.JumpCheckInterval)
            {
                _lastJumpCheckTime = Time.time;
                if (!context.IsGrounded)
                    context.State = PlayerJumpState.Instance;
            }
            var stamina = context.Stamina;
            context.Stamina += .001f;
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
                context.State = PlayerRunState.Instance;
            };
            OnJumpKey = (context) =>
            {
                if (context.CanJump)
                {
                    context.Stamina -= .1f;
                    context.Rigidbody.AddForce(Vector3.up * context.JumpForce, ForceMode.Impulse);
                    context.State = PlayerJumpState.Instance;
                }
            };
        }
        public int StateId => (int)State;
        void IPlayerState.Enter(PlayerStateContext context)
        {
        }

        void IPlayerState.Exit(PlayerStateContext context)
        {
        }

        float _lastJumpCheckTime = 0;
        void IPlayerState.Update(PlayerStateContext context)
        {
            if (Time.time - _lastJumpCheckTime > context.JumpCheckInterval)
            {
                _lastJumpCheckTime = Time.time;
                if (!context.IsGrounded)
                    context.State = PlayerJumpState.Instance;
            }
            context.Stamina += .001f;
        }
        void IPlayerState.FixedUpdate(PlayerStateContext context)
        {
            var nextSpeed = Mathf.Lerp(context.Speed, context.WalkSpeed, Time.deltaTime * context.SpeedLerpScale);
            HandleInput(context, nextSpeed, (ct) =>
            {
                var reducedSpeed = Mathf.Lerp(context.Speed, 0, Time.deltaTime * context.SpeedLerpScale * 2f);
                context.Speed = reducedSpeed;

                if (context.Speed < .5f)
                    ct.State = PlayerIdleState.Instance;
            });
            Move(context);
        }
        void Move(PlayerStateContext context)
        {
            context.Position += context.MoveDirection * context.Speed * Time.deltaTime;
        }
        void IPlayerState.OnCollisionEnter(PlayerStateContext context, Collision collision)
        {
        }
        void IPlayerState.OnCollisionExit(PlayerStateContext context, Collision collision)
        {
            if (!context.CanJump)
                context.State = PlayerJumpState.Instance;
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
                if (context.Speed > context.RunSpeed * .9f)
                    context.State = PlayerSprintState.Instance;
            };
            OnExitShiftKey = (context) =>
            {
                var reducedSpeed = Mathf.Lerp(context.Speed, context.WalkSpeed, Time.deltaTime * context.SpeedLerpScale * 2f);
                context.Speed = reducedSpeed;

                if (context.Speed < context.WalkSpeed * 1.1)
                    context.State = PlayerWalkState.Instance;
            };
            OnJumpKey = (context) =>
            {
                if (context.CanJump)
                {
                    context.Stamina -= .1f;
                    context.Rigidbody.AddForce(Vector3.up * context.JumpForce, ForceMode.Impulse);
                    context.State = PlayerJumpState.Instance;
                }
            };
        }
        public int StateId => (int)State;
        void IPlayerState.Enter(PlayerStateContext context)
        {
        }

        void IPlayerState.Exit(PlayerStateContext context)
        {
        }
        float _lastJumpCheckTime = 0;
        void IPlayerState.Update(PlayerStateContext context)
        {
            context.Stamina -= .001f;
            if (context.Stamina <= 0)
                context.State = PlayerWalkState.Instance;

            if (Time.time - _lastJumpCheckTime > context.JumpCheckInterval)
            {
                _lastJumpCheckTime = Time.time;
                if (!context.IsGrounded)
                    context.State = PlayerJumpState.Instance;
            }
        }
        void IPlayerState.FixedUpdate(PlayerStateContext context)
        {
            var nextSpeed = Mathf.Lerp(context.Speed, context.RunSpeed, Time.deltaTime * context.SpeedLerpScale);

            HandleInput(context, nextSpeed, (ct) =>
            {
                var reducedSpeed = Mathf.Lerp(context.Speed, context.WalkSpeed, Time.deltaTime * context.SpeedLerpScale * 2f);
                context.Speed = reducedSpeed;

                if (context.Speed < context.WalkSpeed * 1.1)
                    ct.State = PlayerWalkState.Instance;
            });
            Move(context);
        }
        void IPlayerState.OnCollisionEnter(PlayerStateContext context, Collision collision)
        {
        }
        void Move(PlayerStateContext context)
        {
            context.Position += context.MoveDirection * context.Speed * Time.deltaTime;
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
                var reducedSpeed = Mathf.Lerp(context.Speed, context.RunSpeed, Time.deltaTime * context.SpeedLerpScale * 2f);
                context.Speed = reducedSpeed;

                if (context.Speed < context.RunSpeed * 1.1)
                    context.State = PlayerRunState.Instance;
            };
            OnJumpKey = (context) =>
            {
                if (context.CanJump)
                {
                    context.Stamina -= .1f;
                    context.Rigidbody.AddForce(Vector3.up * context.JumpForce, ForceMode.Impulse);
                    context.State = PlayerJumpState.Instance;
                }
            };
        }
        public int StateId => (int)State;
        void IPlayerState.Enter(PlayerStateContext context)
        {
        }

        void IPlayerState.Exit(PlayerStateContext context)
        {
        }
        float _lastJumpCheckTime = 0;
        void IPlayerState.Update(PlayerStateContext context)
        {
            context.Stamina -= .002f;
            if (context.Stamina <= 0)
                context.State = PlayerRunState.Instance;
            if (Time.time - _lastJumpCheckTime > context.JumpCheckInterval)
            {
                _lastJumpCheckTime = Time.time;
                if (!context.IsGrounded)
                    context.State = PlayerJumpState.Instance;
            }
        }
        void IPlayerState.FixedUpdate(PlayerStateContext context)
        {
            float nextSpeed;
            if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
                nextSpeed = Mathf.Lerp(context.Speed, context.RunSpeed, Time.deltaTime * context.SpeedLerpScale);
            else
                nextSpeed = Mathf.Lerp(context.Speed, context.SprintSpeed, Time.deltaTime * context.SpeedLerpScale);

            HandleInput(context, nextSpeed, (ct) =>
            {
                var reducedSpeed = Mathf.Lerp(context.Speed, context.RunSpeed, Time.deltaTime * context.SpeedLerpScale * 2f);
                context.Speed = reducedSpeed;

                if (context.Speed < context.RunSpeed * 1.1)
                    ct.State = PlayerRunState.Instance;
            });
            Move(context);
        }
        void Move(PlayerStateContext context)
        {
            context.Position += context.MoveDirection * context.Speed * Time.deltaTime;
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
            var speed = Mathf.Lerp(context.Speed, context.WalkSpeed, Time.deltaTime * context.SpeedLerpScale);

            HandleInput(context, speed, (ct) =>
            {
                var reducedSpeed = Mathf.Lerp(context.Speed, 0, Time.deltaTime * context.SpeedLerpScale * 2f);
                context.Speed = reducedSpeed;
            });
        }
        protected override void HandleInput(PlayerStateContext context, float speed, Action<PlayerStateContext> elseCallback = null)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) ||
                Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                    speed = Mathf.Lerp(context.Speed, context.SprintSpeed, Time.deltaTime * context.SpeedLerpScale * 2f);

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
            context.Position += context.MoveDirection * context.Speed * Time.deltaTime;
        }
        void IPlayerState.OnCollisionEnter(PlayerStateContext context, Collision collision)
        {
            if (context.IsGrounded)
            {
                if (context.Speed > context.RunSpeed)
                    context.State = PlayerSprintState.Instance;
                else if (context.Speed > context.WalkSpeed)
                    context.State = PlayerRunState.Instance;
                else if (context.Speed > .5f)
                    context.State = PlayerWalkState.Instance;
                else
                    context.State = PlayerIdleState.Instance;
            }
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