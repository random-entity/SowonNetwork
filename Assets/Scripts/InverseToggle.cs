using UnityEngine;
using UnityEngine.UI;

public class InverseToggle : MonoBehaviour
{
    public void OffWhenOtherOn(bool otherOn)
    {
        if (otherOn)
        {
            GetComponent<Toggle>().isOn = false;
        }
    }
}