using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image _progressBar;

    public List<AsyncOperation> Proccesses;

    private void Update()
    {
        if (Proccesses == null) return;
        float value = 0f;
        foreach (AsyncOperation proccess in Proccesses)
            value += proccess.progress;
        value /= Proccesses.Count;
        _progressBar.fillAmount = value;
    }

}