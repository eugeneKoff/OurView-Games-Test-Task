using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RepairMode {None, Tape, Gum, Bolts}

public class RepairParts : MonoBehaviour
{
    public RepairMode repairMode = RepairMode.None;

    public RectTransform warningPanel;

    public static RepairParts instance;
    public bool isWarningPanelEnabled;

    public TouchPhase touchPhase = TouchPhase.Began;
    public CarPart carpart;
    private void Start()
    {
        instance = this;
    }

    public void SwitchRepairMode(int newMode)
    {
        switch (newMode)
        {
            case 0:
                break;
            case 1:
                repairMode = RepairMode.Tape;
                break;
            case 2:
                repairMode = RepairMode.Gum;
                break;
            case 3:
                repairMode = RepairMode.Bolts;
                break;
            default:
                break;
        }
    }

    private IEnumerator EnableWarningPanelCoroutine(float secondsToShow)
    {
        warningPanel.gameObject.SetActive(true);
        isWarningPanelEnabled = true;
        yield return new WaitForSeconds(secondsToShow);

        warningPanel.gameObject.SetActive(false);
        isWarningPanelEnabled = false;
    }


    public void ShowWarningPanel(float secondsToShow)
    {
        StartCoroutine(EnableWarningPanelCoroutine(secondsToShow));
    }

#if !UNITY_EDITOR
    private void Update()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == touchPhase)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow, 100f);
            if (Physics.Raycast(ray, out hit))
            {

                if(hit.collider.TryGetComponent(out carpart))
                {
                    carpart.isSelected = true;
                }
            }
        }
        else if(Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            carpart.isSelected = false;
        }
    }
#endif
}
