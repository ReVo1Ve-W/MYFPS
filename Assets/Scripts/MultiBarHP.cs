using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiBarHP : MonoBehaviour
{
    [Header("数据")]
    public DefenseTarget target;
    public int barCount = 5;

    [Header("每层颜色（从上到下，第 0 个是顶层 = 最后被打的）")]
    public Color[] barColors = new Color[]
    {
        Color.red,
        new Color(1f, 0.5f, 0f),   // 橙
        Color.yellow,
        Color.green,
        Color.cyan,
    };

    [Header("血条")]
    public Image[] bars;

    [Header("文字")]
    public TextMeshProUGUI hpText;       // 底部 HP 数值
    public TextMeshProUGUI layerText;    // 顶部第几层 "x4"

    private float hpPerBar;
    private int lastActiveLayer = -1;

    void Start()
    {
        if (target == null)
            target = GetComponentInParent<DefenseTarget>();

        hpPerBar = target.maxHP / barCount;
    }

    void Update()
    {
        if (target == null) return;

        float hp = Mathf.Max(0, target.HP);

        // 计算当前是第几层（从 1 开始）
        int activeLayer = Mathf.CeilToInt(hp / hpPerBar);
        activeLayer = Mathf.Clamp(activeLayer, 1, barCount);

        for (int i = 0; i < bars.Length; i++)
        {
            if (bars[i] == null) continue;

            float barMin = i * hpPerBar;
            float barHP = Mathf.Clamp(hp - barMin, 0, hpPerBar);

            bars[i].fillAmount = barHP / hpPerBar;
            bars[i].color = barColors[Mathf.Min(i, barColors.Length - 1)];
        }

        if (hpText != null)
            hpText.text = $"HP {hp:F0} / {target.maxHP}";

        if (layerText != null && activeLayer != lastActiveLayer)
        {
            lastActiveLayer = activeLayer;
            layerText.text = $"x{activeLayer}";
        }
    }
}
