using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonCallbackListener : MonoBehaviour
{
    public TextMeshProUGUI text;
    Button[] buttons;
    void Start()
    {
        buttons = GetComponentsInChildren<Button>();
        var i = 0;
        foreach (var button in buttons)
        {
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            if (null != text)
            {
                text.text = $"{i:00}";
            }
            button.onClick.AddListener(() =>
            {
                this.text.text = $"Button {text.text} clicked";
            });
            i++;
        }
    }
}
