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


    private List<RectTransform> health_bars;
    private float screen_height;
    private float time_since_damage;

    private void Start()
    {
        Canvas.ForceUpdateCanvases();
        health_bars = GetComponentsInChildren<RectTransform>().ToList();
        health_bars.RemoveAt(0);
        
        screen_height = health_bars[0].rect.height;
        player.onDamage += OnHealthChange;
        player.onHeal += OnHealthChange;
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
        RevealUI();
        AdjustHeightToHP();
        time_since_damage = 0;
    }

    private void AdjustHeightToHP()
    {
        float share = player.GetCurrentHP() / player.GetMaxHP();
        foreach (RectTransform health_bar in health_bars)
        {
            health_bar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, share * screen_height);
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
        foreach (RectTransform health_bar in health_bars)
        {
            Image image = health_bar.GetComponent<Image>();
            var target_col = image.color;
            target_col.a = value;
            image.color = target_col;
        }
    }
}
