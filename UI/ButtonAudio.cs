using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] AudioSource Audio;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Button Button=GetComponent<Button>();
        if(Button.interactable)
        {
            Audio.Play();
        }
    }
}
