using UnityEngine.UI;
using UnityEngine;

public class EmojiButtonManager : MonoSingleton<EmojiButtonManager>
{
    private Texture[] emojiTextures;
    [SerializeField] private Sprite[] emojiSprites;
    [SerializeField] private RectTransform addRockPanel;
    [SerializeField] private Button buttonPrefab;
    private Button[] buttons;
    [SerializeField] private Button submit;
    private Text submitText;
    [SerializeField] private InputField inputField;
    public string username = "";
    public int currentWishSelection = -1, currentGiftSelection = -1;

    private void Awake()
    {
        emojiTextures = RockManager.instance.emojiTextures;
        buttons = new Button[emojiTextures.Length];

        for (int i = 0; i < emojiTextures.Length; i++)
        {
            Button b = Instantiate(buttonPrefab);
            b.transform.SetParent(addRockPanel.transform);
            buttons[i] = b;
            b.GetComponent<EmojiButton>().index = i;
            b.image.sprite = emojiSprites[i];

            int x = i % 5;
            int y = i / 5;

            b.GetComponent<RectTransform>().position = new Vector3(
                (float)Screen.width * Mathf.Lerp(0.4f, 0.6f, (float)x / 4f),
                (float)Screen.height * Mathf.Lerp(0.6f, 0.3f, (float)y / 4f),
                -1f
            );
        }

        submitText = submit.GetComponentInChildren<Text>();
        submit.onClick.AddListener(AddRock);
    }
    public void SetUsername(string username)
    {
        this.username = username;
    }
    private void AddRock()
    {
        if (currentWishSelection == -1 || currentGiftSelection == -1)
        {
            Debug.LogWarning("at least one emoji button not selected");
            return;
        }
        else if (username == "")
        {
            Debug.LogWarning("username is blank");
            return;
        }
        else
        {
            RockManager.instance.AddRock(username, currentWishSelection, currentGiftSelection);
            addRockPanel.gameObject.SetActive(false);
        }

        inputField.Select();
        inputField.text = "";
        currentWishSelection = -1;
        currentGiftSelection = -1;
    }

    private void Update()
    {
        if (currentWishSelection == currentGiftSelection)
        {
            currentWishSelection = -1;
            currentGiftSelection = -1;
        }

        for (int i = 0; i < emojiTextures.Length; i++)
        {
            int buttonIndex = buttons[i].GetComponent<EmojiButton>().index;
            if (buttonIndex == currentWishSelection)
            {
                buttons[i].image.color = Color.gray;
            }
            else if (buttonIndex == currentGiftSelection)
            {
                buttons[i].image.color = Color.gray;
            }
            else
            {
                buttons[i].image.color = Color.white;
            }
        }

        if (currentWishSelection >= 0 && currentGiftSelection >= 0 && username != "")
        {
            submitText.text = "Submit";
        }
        else
        {
            submitText.text = "Exit";
        }
    }
}
