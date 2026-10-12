using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SystemMessage : MonoBehaviour
{
public static SystemMessage Create(string message)
    {
        Transform sysMessageTransform = Instantiate(GameAssets.i.pfSysMessage, GameManager.i.GetSysMessagePoint());
        SystemMessage sysMessagePopup = sysMessageTransform.GetComponent<SystemMessage>();
        sysMessagePopup.Setup(message);
        return sysMessagePopup;
    }
    
    private const float DISAPPEAR_TIMER_MAX = 2f;
    private const float DISAPPEAR_TIMER_MAX_SHOP = .5f;
    
    private TextMeshProUGUI sysMsgText;

    private float disappearTimer;
    private Color textColor;
    private Vector3 moveVector;

    private void Awake()
    {
        sysMsgText = transform.GetComponent<TextMeshProUGUI>();
    }

    public void Setup(string msg)
    {
        if(GameManager.i.GetIsShop()) 
        {
            sysMsgText.fontSize = 42;
            textColor = Color.white;
            sysMsgText.text = msg;
            sysMsgText.color = textColor; 
              
            disappearTimer = DISAPPEAR_TIMER_MAX_SHOP;
        }
        else
        {
            textColor = Color.white;
            sysMsgText.text = msg;
            sysMsgText.color = textColor;     
            
            disappearTimer = DISAPPEAR_TIMER_MAX;
        }
    }
    private void Update()
    {
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            // Start disappearing
            float disappearSpeed = 1.5f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            sysMsgText.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
