using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    public GameObject WinScreen;


    public void OnTriggerEnter(Collider other)
    {
        print("hui");

        Invoke("EnableWinScreen", 1f);
        Car car;
        if(!other.TryGetComponent(out car))
        {
            car = other.GetComponentInParent<Car>();
        }
        car.StopCarTestDrive();


    }

    private void EnableWinScreen()
    {
        WinScreen.SetActive(true);

    }
}
