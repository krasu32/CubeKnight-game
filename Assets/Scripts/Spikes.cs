using System.Collections;
using UnityEngine;

public class Spikes : MonoBehaviour
{
        private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            StartCoroutine(RespawnPoint());
        }
    }

    IEnumerator RespawnPoint()
    {
        PlayerControl.Instance.pState.cutscene = true;
        PlayerControl.Instance.pState.invincible = true;
        PlayerControl.Instance.rb.linearVelocity = Vector2.zero;
        Time.timeScale = 0;
        StartCoroutine(UiManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.In));
        PlayerControl.Instance.TakeDamage(1);
        yield return new WaitForSecondsRealtime(1f);
        PlayerControl.Instance.transform.position = GameManager.Instance.platformingRespawnPoint;
        StartCoroutine(UiManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
        yield return new WaitForSecondsRealtime(UiManager.Instance.sceneFader.fadeTime);
        PlayerControl.Instance.pState.cutscene = false;
        PlayerControl.Instance.pState.invincible = false;
        Time.timeScale = 1;
    }
}
