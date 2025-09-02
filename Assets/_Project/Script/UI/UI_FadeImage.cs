using UnityEngine;
using UnityEngine.UI;

public class UI_FadeImage : MonoBehaviour
{
    void Start()
    {
        ScreenFader.Instance.FadeImage = GetComponent<Image>();
    }
}
