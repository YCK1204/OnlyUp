using UnityEngine;

namespace Player.Controller
{
    public class PlayerMoveController : MonoBehaviour
    {
        internal PlayerStatController _stat;

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
            _stat = GetComponent<PlayerStatController>();
            if (_stat == null)
                Debug.LogError("플레이터 스탯 없음");
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