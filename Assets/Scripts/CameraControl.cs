using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private float followSpeed = 0.1f;

    [SerializeField] private Vector3 offset;

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, PlayerControl.Instance.transform.position + offset, followSpeed);
    }
}
