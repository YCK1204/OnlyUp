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
                Debug.LogError("�÷��̾� �ڽ� �ݶ��̴� ����");
            if (_stat == null)
                Debug.LogError("�÷����� ���� ����");
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
            // �߹ٴ� box collider�� 8���� üũ
            foreach (var point in checkPoints)
            {
                if (Physics.Raycast(point, Vector3.down, out var hit, 0.1f, layer))
                {
                    if (hit.normal.y > 0.5f) // �ٴ��� ������ ���
                        return true; // ���� ����
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