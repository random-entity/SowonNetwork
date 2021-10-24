using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EmojiButton : MonoBehaviour, IPointerClickHandler
{
    public int index;
    private Button buttonComponent;

    private void OnEnable()
    {
        buttonComponent = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerId == -1) // left click, wish selection
        {
            EmojiButtonManager.instance.currentWishSelection = index;
            if (EmojiButtonManager.instance.currentGiftSelection == index)
            {
                EmojiButtonManager.instance.currentGiftSelection = -1;
            }
        }
        else if (eventData.pointerId == -2) // right click, gift selection
        {
            EmojiButtonManager.instance.currentGiftSelection = index;
            if (EmojiButtonManager.instance.currentWishSelection == index)
            {
                EmojiButtonManager.instance.currentWishSelection = -1;
            }
        }
    }
}
