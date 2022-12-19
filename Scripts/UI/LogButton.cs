using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LogButton : MonoBehaviour
{
    private Selectable m_selectable;
 
    // Use this for initialization
    void Start()
    {
        m_selectable = GetComponent<Selectable>();
    }

    public void OnSelect(BaseEventData _data) {
        //Manually handle
        if (!m_selectable.interactable && _data is AxisEventData axisEvent) {
            var goBackSelect = m_selectable.FindSelectable(axisEvent.moveVector *= -1);
            StartCoroutine(DelaySelect(goBackSelect));
            return;
        }
        
    }

    // Delay the select until the end of the frame.
    // If we do not, the current object will be selected instead.
    private IEnumerator DelaySelect(Selectable select)
    {
        yield return new WaitForEndOfFrame();
    
        if (select != null || !select.gameObject.activeInHierarchy)
        select.Select();
        else
        Debug.LogWarning("Please make sure your explicit navigation is configured correctly.");
    }
}
