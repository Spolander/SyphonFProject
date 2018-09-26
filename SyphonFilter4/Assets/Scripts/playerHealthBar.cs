using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class playerHealthBar : MonoBehaviour {

    public static playerHealthBar m_playerHealthBar;

    float startWidth;

    float maximumHealth;
    Image m_image;
    RectTransform rect;

    Color startingColor;
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        m_image = GetComponent<Image>();
        startingColor = m_image.color;
        m_playerHealthBar = this;
        startWidth = rect.sizeDelta.x;
    }


    public void UpdateHealthBar(float maximumHealth, float currentHealth)
    {

        float width = (currentHealth / maximumHealth) * startWidth;
        Color c = Color.Lerp(Color.red, startingColor, currentHealth / maximumHealth);
        m_image.color = c;

        rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);


    }

	// Use this for initialization
	
}
