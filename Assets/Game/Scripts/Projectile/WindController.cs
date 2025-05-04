using UnityEngine;

public class WindController : MonoBehaviour
{
    [Header("Ветер (-1 = влево, 0 = нет, 1 = вправо)")]
    [Range(-1, 1)]
    public int windDirection = 1;

    [Header("Сила ветра")]
    public float windStrength = 200f;

    public static WindController Instance { get; private set; }

    // Статические значения по умолчанию (если объект не создан)
    private static int defaultDirection = -1;
    private static float defaultStrength = 0f;

    private void Awake()
    {
        Debug.Log("[WindController] Awake вызван");
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

    // Позволяет задать ветер вручную без объекта
    public static void SetDefaultWind(int direction, float strength)
    {
        defaultDirection = Mathf.Clamp(direction, -1, 1);
        defaultStrength = Mathf.Max(0f, strength);
    }
    // WindController.SetDefaultWind(1, 300f);  // Ветер вправо, но триста дофига, не делай так

}
