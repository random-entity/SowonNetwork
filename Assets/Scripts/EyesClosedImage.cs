using UnityEngine;

public class EyesClosedImage : MonoBehaviour
{
    public void SetActiveInverse(bool on)
    {
        gameObject.SetActive(!on);
    }
}
