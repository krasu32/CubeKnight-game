using UnityEngine;

public class UnlockeDoubleJump : MonoBehaviour
{
    bool used;

    void Start()
    {
        if(PlayerControl.Instance.maxAirJumps == 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if(_collision.CompareTag("Player") && !used)
        {
            used = true;
            PlayerControl.Instance.maxAirJumps = 1;

            Destroy(gameObject);
        }
    }
}
