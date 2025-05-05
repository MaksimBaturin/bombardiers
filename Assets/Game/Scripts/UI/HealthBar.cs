using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    public Health Health;
    public Image imgObj;
    public Sprite spriteImage;
    public TextMeshProUGUI healthText;
    public float healthPercentage = 100; //������� ����� ����� ������ � Health
    private float maxWidth = 274f;
    private RectTransform healthBarRect;
    private RectTransform parentRect;

    void Start()
    {
        // �������� RectTransform health bar � ��� ��������
        healthBarRect = imgObj.GetComponent<RectTransform>();
        parentRect = imgObj.transform.parent.GetComponent<RectTransform>();

        // ������������� anchor � ����� ����� ��������
        healthBarRect.anchorMin = new Vector2(0.5f, 0.5f);
        healthBarRect.anchorMax = new Vector2(0.5f, 0.5f);

        // ������������� pivot � ����� ����� (0 - �����, 0.5 - �� ������ �� ���������)
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
        healthPercentage -= 0.01f;//������� ����� ����� ������ � Health
        //float healthPercentage = Health.GetHealthPercentage(); //���������������� ����� ����� ������ � Health

        healthText.text = healthPercentage.ToString("0") + "%";

        float healthBarWidth = healthPercentage / 100f * maxWidth;
        imgObj.rectTransform.sizeDelta = new Vector2(healthBarWidth, 35f);

    }
}
