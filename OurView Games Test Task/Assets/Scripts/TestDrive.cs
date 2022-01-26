using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDrive : MonoBehaviour
{
    public GameObject cameraPivot;

    public GameObject testDrivePivot;
    public Canvas[] canvasesToDisableOnTestDrive;
    public Car car;
    public GameObject gameOverScreen;
    public bool _testDriveEnabled = false;

    private RotateCamera _rotCamScript;
    private Vector3 _camOffset;

    private void Start()
    {
        _rotCamScript = cameraPivot.GetComponent<RotateCamera>();
    }

    public void StartTestDrive()
    {
        StartCoroutine(TestDriveCoroutine());
    }

    public void LateUpdate()
    {
        if (_testDriveEnabled)
        {
            FollowCar();
        }
    }

    private IEnumerator TestDriveCoroutine()
    {
        _rotCamScript.enabled = false;

        while (cameraPivot.transform.position.x < testDrivePivot.transform.position.x -.5f)
        {
            cameraPivot.transform.position = Vector3.Lerp(cameraPivot.transform.position, testDrivePivot.transform.position, 2 * Time.deltaTime);
            cameraPivot.transform.rotation = Quaternion.Lerp(cameraPivot.transform.rotation, testDrivePivot.transform.rotation, 2 * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        print("lerping finished");

        _camOffset = cameraPivot.transform.position - car.transform.position ;

        foreach (var canvas in canvasesToDisableOnTestDrive)
        {
            canvas.gameObject.SetActive(false);
        }
        _testDriveEnabled = true;

        car.StartCarTestDrive();

    }

    private void FollowCar()
    {
        cameraPivot.transform.position = car.transform.position + _camOffset;
    }

    private void OnEnable()
    {
        car.CarFellApart += OnCarFellApart;
    }

    private void OnDisable()
    {
        car.CarFellApart -= OnCarFellApart;
    }

    private void OnCarFellApart()
    {
        Invoke("EnableGameOverScreen", 3f);
    }

    private void EnableGameOverScreen()
    {
        gameOverScreen.SetActive(true);
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
