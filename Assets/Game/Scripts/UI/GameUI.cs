using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject windDirectionArrow;
    [SerializeField] private TMP_Text windPowerText;
    
    public static GameUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeWindDirection(float force, int direction)
    {
        StartCoroutine(SetWindDirection(direction));

        StartCoroutine(TextBounce());
        windPowerText.text = $"Сила ветра: {force}";
    }

    private IEnumerator SetWindDirection(int direction)
    {
        float targetAngle = direction * -90f;
        
        windDirectionArrow.transform
            .DORotate(new Vector3(0, 0, targetAngle), 0.5f, RotateMode.FastBeyond360);

        yield return new WaitForSeconds(0);
    }



    private IEnumerator TextBounce()
    {
        Vector3 windPowerTextPos = windPowerText.transform.position;
        windPowerTextPos.y += 15f;
        windPowerText.transform.DOMove(windPowerTextPos, 0.5f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(0.5f);
        windPowerTextPos.y -= 15f;
        windPowerText.transform.DOMove(windPowerTextPos, 0.5f).SetEase(Ease.OutBounce);
    }
    
}
