using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;
    private float _timeScaleDestination = 1f;

    private void Awake() => instance = this;
    

    private void Update()
    {
        if (Time.timeScale == _timeScaleDestination)
            return;

        Time.timeScale = Mathf.Lerp(Time.timeScale, _timeScaleDestination, 0.05f);

        if (Mathf.Abs(Time.timeScale - _timeScaleDestination) < 0.001f)
            Time.timeScale = _timeScaleDestination;
    }

    public void SetNormalTimeScale()
    {
        _timeScaleDestination = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
    public void SetSlowedDownTimeScale()
    {
        _timeScaleDestination = 0.05f;
        Time.fixedDeltaTime = 0.01f;
    }
    public void Pause() => _timeScaleDestination = 0f;
}
