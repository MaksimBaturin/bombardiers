using UnityEngine;

public class WindController : MonoBehaviour
{
    [Header("����� (-1 = �����, 0 = ���, 1 = ������)")]
    [Range(-1, 1)]
    public int windDirection = 1;

    [Header("���� �����")]
    public float windStrength = 200f;

    public static WindController Instance { get; private set; }

    // ����������� �������� �� ��������� (���� ������ �� ������)
    private static int defaultDirection = -1;
    private static float defaultStrength = 0f;

    private void Awake()
    {
        Debug.Log("[WindController] Awake ������");
        Instance = this;
    }

    public static Vector2 GetWindForce()
    {
        if (Instance != null)
        {
            return new Vector2(Instance.windDirection * Instance.windStrength, 0f);
        }
        else
        {
            return new Vector2(defaultDirection * defaultStrength, 0f);
        }
    }

    // ��������� ������ ����� ������� ��� �������
    public static void SetDefaultWind(int direction, float strength)
    {
        defaultDirection = Mathf.Clamp(direction, -1, 1);
        defaultStrength = Mathf.Max(0f, strength);
    }
    // WindController.SetDefaultWind(1, 300f);  // ����� ������, �� ������ ������, �� ����� ���

}
