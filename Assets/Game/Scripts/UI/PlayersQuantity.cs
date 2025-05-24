using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayersQuantity : MonoBehaviour
{
    public TextMeshProUGUI quantityText;
    public TMP_InputField Player1_input;
    public TMP_InputField Player2_input;
    public TMP_InputField Player3_input;
    public TMP_InputField Player4_input;
    public GameObject Player3;
    public GameObject Player4;

    public Color[] colorPalette = {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
    };

    [System.Serializable]
    public class ColorButton
    {
        public Button button;
        [HideInInspector] public int currentColorIndex;
    }

    public ColorButton[] colorButtons = new ColorButton[4];

    void Start()
    {
        for (int i = 0; i < colorButtons.Length; i++)
        {
            int index = i;
            colorButtons[i].button.onClick.AddListener(() => {
                colorButtons[index].currentColorIndex =
                    (colorButtons[index].currentColorIndex + 1) % colorPalette.Length;
                colorButtons[index].button.image.color =
                    colorPalette[colorButtons[index].currentColorIndex];
            });

            // Инициализация
            colorButtons[i].currentColorIndex = 0;
            colorButtons[i].button.image.color = colorPalette[0];
        }
    }

    // Update is called once per frame
    void Update()
        {

        }

    public void Plus_OnClick()
    {
        if (quantityText.text == "2")
        {
            quantityText.text = "3";
            Player3.SetActive(true);
        }
        else if (quantityText.text == "3")
        {
            quantityText.text = "4";
            Player4.SetActive(true);
        }
        else if (quantityText.text == "4")
        {
            quantityText.text = "4";
        }
    }

    public void Minus_OnClick()
    {
        if (quantityText.text == "2")
        {
            quantityText.text = "2";
        }
        else if (quantityText.text == "3")
        {
            quantityText.text = "2";
            Player3.SetActive(false);
        }
        else if (quantityText.text == "4")
        {
            quantityText.text = "3";
            Player4.SetActive(false);
        }
    }

    public void Start_OnClick()
    {
        List<Player> playerList = new List<Player>
        {
            new Player(Player1_input.text, colorButtons[0].button.image.color),
            new Player(Player2_input.text, colorButtons[1].button.image.color)
        };

        if (quantityText.text == "3" || quantityText.text == "4")
        {
            playerList.Add(new Player(Player3_input.text, colorButtons[2].button.image.color));

            if (quantityText.text == "4")
            {
                playerList.Add(new Player(Player4_input.text, colorButtons[3].button.image.color));
            }
        }

        Player[] players = playerList.ToArray();
        Bootstrap.Instance.GameInit(players);
        Destroy(gameObject);
    }
}
