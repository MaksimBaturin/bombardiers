using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.Scripts;

public class HealthBar : MonoBehaviour
{
    public TankModel tank; // ������ �� ����
    public Image imgObj;   // Image ��� ������� ��������
    public TextMeshProUGUI healthText; // ����� � ��������� ��������
    public float offsetY = 2f; // �������� �� ������ ��� ������
    public GameObject HealthBarPanel;

    private float maxWidth = 274f;
    private RectTransform healthBarRect;

    void Start()
    {
        // �������� RectTransform health bar
        healthBarRect = imgObj.GetComponent<RectTransform>();

        // ����������� anchor � pivot
        healthBarRect.anchorMin = new Vector2(0.5f, 0.5f);
        healthBarRect.anchorMax = new Vector2(0.5f, 0.5f);
        healthBarRect.pivot = new Vector2(0f, 0.5f);
        healthBarRect.anchoredPosition = new Vector2(-137f, 0f);
    }

    void Update()
    {
        if (tank != null)
        {
            UpdateHealthDisplay();
            // ������������� HealthBar ��� ������
            transform.position = tank.transform.position + Vector3.up * offsetY;
        }
    }

    public void Active()
    {
        HealthBarPanel.SetActive(true);
    }

    private void UpdateHealthDisplay()
    {
        if (tank == null) return;

        float healthPercentage = (tank.CurrentHealth / tank.maxHealth) * 100f;

        healthText.text = healthPercentage.ToString("0") + "%";

        float healthBarWidth = healthPercentage / 100f * maxWidth;
        imgObj.rectTransform.sizeDelta = new Vector2(healthBarWidth, 35f);
    }

    // ����������, ��� HealthBar ������ ������� �� ������
    private void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                        Camera.main.transform.rotation * Vector3.up);
    }
}