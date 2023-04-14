using UnityEngine;

public class TimeScaleController : MonoBehaviour
{
    [SerializeField] float timeScale;

    void Start()
    {
        Time.timeScale = timeScale;
    }

    private void OnValidate()
    {
        Time.timeScale = timeScale;
    }

}
