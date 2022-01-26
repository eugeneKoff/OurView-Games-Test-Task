using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RepairSlider : MonoBehaviour
{
    public static RepairSlider instance;
    
    public RectTransform sliderParentObj;
    public Image slider;
    public RectTransform parentRectTransform;

    private Camera mainCam;
    private Vector2 sliderNewPos;
    private void Start()
    {


        if (instance != null & instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;

        mainCam = Camera.main;
    }
    public void UpdateSliderValue(float amount)
    {


        slider.fillAmount = amount;
    }

    public void ToggleSlider(Vector2 position, bool isEnabled)
    {
        if (isEnabled)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, position, mainCam, out sliderNewPos);

            sliderParentObj.transform.position = position;
        }

        sliderParentObj.gameObject.SetActive(isEnabled);
    }
}
