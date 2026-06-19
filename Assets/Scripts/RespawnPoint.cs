using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OggerEnter2D(Collider2D _other)
    {
        GameManager.Instance.platformingRespawnPoint = transform.position;
    }
}
