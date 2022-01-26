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
}
