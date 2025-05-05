using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Health Health;
    public Image imgObj;
    public Sprite spriteImage;
    public TextMeshProUGUI healthText;
    public float healthPercentage = 100; //удалить когда будет объект с Health
    private float maxWidth = 274f;
    private RectTransform healthBarRect;
    private RectTransform parentRect;

    void Start()
    {
        // Получаем RectTransform health bar и его родителя
        healthBarRect = imgObj.GetComponent<RectTransform>();
        parentRect = imgObj.transform.parent.GetComponent<RectTransform>();

        // Устанавливаем anchor в левый центр родителя
        healthBarRect.anchorMin = new Vector2(0.5f, 0.5f);
        healthBarRect.anchorMax = new Vector2(0.5f, 0.5f);

        // Устанавливаем pivot в левый центр (0 - слева, 0.5 - по центру по вертикали)
        healthBarRect.pivot = new Vector2(0f, 0.5f);

        healthBarRect.anchoredPosition = new Vector2(-137f, 0f);

        UpdateHealthDisplay();
    }

    void Update()
    {
        UpdateHealthDisplay();
    }

    private void UpdateHealthDisplay()
    {
        healthPercentage -= 0.01f;//удалить когда будет объект с Health
        //float healthPercentage = Health.GetHealthPercentage(); //раскоментировать когда будет объект с Health

        healthText.text = healthPercentage.ToString("0") + "%";

        float healthBarWidth = healthPercentage / 100f * maxWidth;
        imgObj.rectTransform.sizeDelta = new Vector2(healthBarWidth, 35f);

    }
}
