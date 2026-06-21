using UnityEngine;

public class RespawnPoint : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D _other)
    {
        GameManager.Instance.platformingRespawnPoint = transform.position;
    }
}
