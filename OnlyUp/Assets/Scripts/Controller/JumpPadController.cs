using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPadController : MonoBehaviour
{
    [SerializeField]
    float BounceForce = 10f;

    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.GetComponent<Rigidbody>()?.AddForce(Vector3.up * BounceForce, ForceMode.Impulse);
    }
}
