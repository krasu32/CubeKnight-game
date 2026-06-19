using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private string transitionTo;

    [SerializeField] private Transform startPoint;

    [SerializeField] private Vector2 exitDirection;

    [SerializeField] private float exitTime;

    private void Start()
    {
        if(transitionTo == GameManager.Instance.transitionedFromScene)
        {
            PlayerControl.Instance.transform.position = startPoint.position;

            StartCoroutine(PlayerControl.Instance.WalkIntoNewScene(exitDirection, exitTime));
        }
        
        StartCoroutine(UiManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if(_other.CompareTag("Player"))
        {
            GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;

            PlayerControl.Instance.pState.cutscene = true;

            StartCoroutine(UiManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, transitionTo));
        }
    }
}
