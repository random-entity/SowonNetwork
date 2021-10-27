using UnityEngine;

public class InverseSetActive : MonoBehaviour
{
    public void SetActiveInverse(bool on)
    {
        gameObject.SetActive(!on);
    }
}