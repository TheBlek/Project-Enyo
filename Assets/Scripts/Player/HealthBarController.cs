using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private Damagable player;
    [SerializeField] private float fade_time;
    [SerializeField] private float time_before_fade;
    [SerializeField] private float red_bars_delay;
    [SerializeField] private float red_bars_adjustment_time;

    [SerializeField] private List<RectTransform> green_health_bars;
    [SerializeField] private List<RectTransform> red_health_bars;


    private float screen_height;
    private float time_since_damage;
    private Coroutine red_bars_coroutine;
    private Image[] children;

    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        screen_height = green_health_bars[0].rect.height;
        
        player.onDamage += OnHealthChange;
        player.onHeal += OnHealthChange;
        player.onHeal += ResetRedBars;

        children = GetComponentsInChildren<Image>();
    }

    private void Update()
    {
        time_since_damage += Time.deltaTime;
        if (time_since_damage > fade_time + time_before_fade)
            time_since_damage = fade_time + time_before_fade;

        FadeUI();
    }

    private void OnHealthChange()
    {
        time_since_damage = 0;

        RevealUI();
        AdjustGreenBarsToHP();
        AdjustRedBarsToHP();
    }

    private void AdjustGreenBarsToHP()
    {
        float share = player.GetCurrentHP() / player.GetMaxHP();
        foreach (RectTransform health_bar in green_health_bars)
        {
            health_bar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, share * screen_height);
        }
    }

    private void ResetRedBars()
    {
        float height = green_health_bars[0].rect.height;

        foreach (RectTransform health_bar in red_health_bars)
        {
            health_bar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
    }

    private void AdjustRedBarsToHP()
    {
        if (red_bars_coroutine != null)
            StopCoroutine(red_bars_coroutine);

        red_bars_coroutine = StartCoroutine(StartRedBarsAdjustment());
    }

    IEnumerator StartRedBarsAdjustment()
    {
        float delta_height = red_health_bars[0].rect.height - green_health_bars[0].rect.height;

        while (time_since_damage < red_bars_delay)
            yield return null;

        while (time_since_damage < red_bars_adjustment_time + red_bars_delay)
        {

            float share = (red_bars_adjustment_time + red_bars_delay - time_since_damage) / red_bars_adjustment_time;

            float target_height = green_health_bars[0].rect.height + delta_height * share;

            foreach (RectTransform health_bar in red_health_bars)
            {
                health_bar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, target_height);
            }

            yield return null;
        }
    }

    private void FadeUI()
    {
        if (time_since_damage <= time_before_fade)
            return;

        float share = (fade_time + time_before_fade - time_since_damage) / fade_time;

        AdjustBarsAlpha(share);
    }

    private void RevealUI()
    {
        AdjustBarsAlpha(1);
    }

    private void AdjustBarsAlpha(float value)
    {
        foreach (Image image in children)
        {
            var target_col = image.color;
            target_col.a = value;
            image.color = target_col;
        }

        if (value == 1)
            return;

        foreach (RectTransform bar in red_health_bars)
        {
            Image image = bar.GetComponent<Image>();
            var target_col = image.color;
            target_col.a = 0;
            image.color = target_col;
        }
    }
}
