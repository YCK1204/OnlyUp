using System.Collections.Generic;
using UnityEngine;

namespace Player.Controller
{
    public class PlayerMoveController : MonoBehaviour
    {
        internal PlayerStatController _stat;
        [SerializeField]
        BoxCollider _boxCollider;

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
            if (_boxCollider == null)
                Debug.LogError("플레이어 박스 콜라이더 없음");
            if (_stat == null)
                Debug.LogError("플레이터 스탯 없음");
            context = new PlayerStateContext(this);
        }

        internal bool CanJump()
        {
            var bounds = _boxCollider.bounds;
            var min = bounds.min;
            var max = bounds.max;
            var center = bounds.center;
            var layer = LayerMask.GetMask("Ground");

            List<Vector3> checkPoints = new List<Vector3>
            {
                new Vector3(min.x, min.y + 0.05f, min.z),
                new Vector3(center.x, min.y + 0.05f, min.z),
                new Vector3(max.x, min.y + 0.05f, min.z),
                new Vector3(min.x, min.y + 0.05f, center.z),
                new Vector3(max.x, min.y + 0.05f, center.z),
                new Vector3(min.x, min.y + 0.05f, max.z),
                new Vector3(center.x, min.y + 0.05f, max.z),
                new Vector3(max.x, min.y + 0.05f, max.z)
            };
            // 발바닥 box collider의 8방향 체크
            foreach (var point in checkPoints)
            {
                if (Physics.Raycast(point, Vector3.down, out var hit, 0.1f, layer))
                {
                    if (hit.normal.y > 0.5f) // 바닥이 수평인 경우
                        return true; // 점프 가능
                }
            }

            return false;
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