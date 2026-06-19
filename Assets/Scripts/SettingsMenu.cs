using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public void SetQuality(int _qualityIndex)
    {
        QualitySettings.SetQualityLevel(_qualityIndex);
    }

    public void SetFullScreen(bool _isFullScreen)
    {
        Screen.fullScreen = _isFullScreen;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
