using UnityEngine;
using UnityEngine.UI;

public class UI_FadeImage : MonoBehaviour
{
    void Start()
    {
        SceneFade.Instance.FadeImage = GetComponent<Image>();
    }
}
