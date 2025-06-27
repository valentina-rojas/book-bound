using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]

public class OptionButton : MonoBehaviour
{
    
    private TextMeshProUGUI m_text = null;
    private Button m_button = null;
    private Image m_image = null;
    private Color m_originalColor = Color.black;

    public Option Option {get; set; }
    
    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_image = GetComponent<Image>();
        m_text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        m_originalColor = m_image.color;
    }
    
    public void Construct(Option option, Action<OptionButton> callback)
    {
        m_text.text = option.text;

        m_button.onClick.RemoveAllListeners();
        m_button.enabled = true;
        m_image.color = m_originalColor;
        Option = option;

        m_button.onClick.AddListener(delegate
        {
            callback(this);
        });

    }

    public void SetColor(Color c)
    {
        m_button.enabled = false;
        m_image.color = c;
    }
}
