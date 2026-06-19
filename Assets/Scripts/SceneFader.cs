using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [SerializeField] public float fadeTime;

    private Image fadeOutUiImage;

    public enum FadeDirection
    {
        In,
        Out
    }
    void Awake()
    {
        fadeOutUiImage = GetComponent<Image>();
    }

    public void CallFadeAndLoadScene(string _sceneToLoad)
    {
        StartCoroutine(FadeAndLoadScene(FadeDirection.In, _sceneToLoad));
    }

    public IEnumerator Fade(FadeDirection _fadeDirection)
    {
        float _alpha = _fadeDirection == FadeDirection.Out ? 1 : 0;
        float _fadeEndValue = _fadeDirection == FadeDirection.Out ? 0 : 1;

        if(_fadeDirection == FadeDirection.Out)
        {
            while(_alpha >= _fadeEndValue)
            {
                SetColorImage(ref _alpha, _fadeDirection);

                yield return null;
            }

            fadeOutUiImage.enabled = false;
        }
        else
        {
            fadeOutUiImage.enabled = true;

            while(_alpha <= _fadeEndValue)
            {
                SetColorImage(ref _alpha, _fadeDirection);

                yield return null;
            }

        }
    }

    public IEnumerator FadeAndLoadScene(FadeDirection _fadeDirection, string _sceneToLoad)
    {
        fadeOutUiImage.enabled = true;

        yield return Fade(_fadeDirection);

        SceneManager.LoadScene(_sceneToLoad);
    }

    void SetColorImage(ref float _alpha, FadeDirection _fadeDirection)
    {
        fadeOutUiImage.color = new Color(fadeOutUiImage.color.r, fadeOutUiImage.color.g, fadeOutUiImage.color.b, _alpha);

        _alpha += Time.deltaTime * (1 / fadeTime) * (_fadeDirection == FadeDirection.Out ? -1 : 1);
    }
}
