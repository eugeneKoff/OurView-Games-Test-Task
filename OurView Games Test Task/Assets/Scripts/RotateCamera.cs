using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotateCamera : MonoBehaviour
{


    public float speed = 3.5f;

    private float rotX;
    private float rotY;

    private float X;
    private float Y;
    private Touch initTouch;


    private void Start()
    {
        rotX = transform.eulerAngles.x;
        rotY = transform.eulerAngles.y;
    }

    void Update()
    {

        //if (EventSystem.current.IsPointerOverGameObject(0))
        //{ return; }
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {

            rotX = Input.GetAxis("Mouse Y");
            rotY = Input.GetAxis("Mouse X");

            transform.Rotate(new Vector3(-rotX * speed * Time.deltaTime, rotY * speed * Time.deltaTime, 0));
            X = transform.rotation.eulerAngles.x;
            Y = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(X, Y, 0);
        }
#endif


        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                initTouch = touch;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                float deltaX = initTouch.position.x - touch.position.x;
                float deltaY = initTouch.position.y - touch.position.y;
                rotX -= touch.deltaPosition.y * Time.deltaTime * speed;
                rotY += touch.deltaPosition.x * Time.deltaTime * speed;
                

                transform.eulerAngles = new Vector3(rotX, rotY, 0);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                initTouch = new Touch();
            }

        }    
    



    }
}




