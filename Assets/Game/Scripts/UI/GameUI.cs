using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameObject windDirectionArrow;
    [SerializeField] private TMP_Text windPowerText;
    
    [SerializeField] private TMP_Text PlayerTurnText;
    private float PlayerTurnTextInitLocalPosY;
    
    [SerializeField] private TMP_Text WinnerText;
    
    public static GameUI Instance;
    
    private void Awake()
    {
        Instance = this;
        PlayerTurnTextInitLocalPosY = PlayerTurnText.transform.localPosition.y;
        
    }

    public void ShowWinner(string playerName)
    {
        StartCoroutine(WinnerEffects(playerName));
    }
    private IEnumerator WinnerEffects(string playerName)
    {
        WinnerText.enabled = true;
        WinnerText.alpha = 1;
        WinnerText.text = $"Победил {playerName}!";

        Sequence sequence = DOTween.Sequence();
        sequence.Append(WinnerText.transform.DORotate(new Vector3(0, 0, 5f), 2f).SetEase(Ease.InOutSine));
        sequence.Append(WinnerText.transform.DORotate(new Vector3(0, 0, 0), 2f).SetEase(Ease.InOutSine));
        sequence.Append(WinnerText.transform.DORotate(new Vector3(0, 0, -5f), 2f).SetEase(Ease.InOutSine));
        sequence.Append(WinnerText.transform.DORotate(new Vector3(0, 0, 0), 2f).SetEase(Ease.InOutSine));
        sequence.SetLoops(-1);
        yield return new WaitForSeconds(0f);
        
    }
    public void ShowPlayerTurn(string playerName)
    {
        StartCoroutine(PlayerTurnTextEffects(playerName));
    }

    private IEnumerator PlayerTurnTextEffects(string playerName)
    {
        PlayerTurnText.enabled = true;
        PlayerTurnText.alpha = 1;
        PlayerTurnText.text = $"Ход игрока:\n{playerName}";

        
        PlayerTurnText.transform.DOLocalMoveY(PlayerTurnTextInitLocalPosY+10f , 0.7f)
            .SetEase(Ease.OutElastic);
        yield return new WaitForSeconds(0.7f);
        PlayerTurnText.transform.DOLocalMoveY(PlayerTurnTextInitLocalPosY , 0.25f)
            .SetEase(Ease.OutElastic);
        yield return new WaitForSeconds(2);
        PlayerTurnText.DOFade(0, 0.5f);
        yield return new WaitForSeconds(0.5f);
        PlayerTurnText.enabled = false;
    }
    
    public void ChangeWindDirection(float force, int direction)
    {
        StartCoroutine(SetWindDirection(direction));

        StartCoroutine(WindTextBounce());
        windPowerText.text = $"Сила ветра: {force}";
    }

    private IEnumerator SetWindDirection(int direction)
    {
        float targetAngle = direction * -90f;
        
        windDirectionArrow.transform
            .DORotate(new Vector3(0, 0, targetAngle), 0.5f, RotateMode.FastBeyond360);

        yield return new WaitForSeconds(0);
    }

    private IEnumerator WindTextBounce()
    {
        Vector3 windPowerTextPos = windPowerText.transform.position;
        windPowerTextPos.y += 15f;
        windPowerText.transform.DOMove(windPowerTextPos, 0.5f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(0.5f);
        windPowerTextPos.y -= 15f;
        windPowerText.transform.DOMove(windPowerTextPos, 0.5f).SetEase(Ease.OutBounce);
    }
    
}
