using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class Car : MonoBehaviour
{
    public GameObject model;
    public Material lineMat;
    public Slider slider;
    public ParticleSystem gumParticles;
    public Rigidbody carRb;
    public Collider carCollider;
    public float carHealth;
    public float speed;
    private bool isTestDrive = false;
    public float damagePerObstacle = 3f;

    public int minimalNumOfCarParts = 3;
    public int totalNumOfCarParts;
    private List<CarPart> _carParts;

    public event Action CarFellApart;
    private bool isGodMode;

    public void Start()
    {
        _carParts = new List<CarPart>();
        foreach(Transform child in model.transform)
        {
            if(child.TryGetComponent(out IgnoreCarPart ignoreCarPart))
            {
                continue;
            }

            var carPart = child.gameObject.AddComponent<CarPart>();
            _carParts.Add(carPart);
            carPart.SaveDefaultTransform();
            child.gameObject.AddComponent<LineRenderer>().material = lineMat;

            child.position += new Vector3(Random.Range(-.05f, .05f), Random.Range(-.05f, .05f), Random.Range(-.05f, .05f));
            child.rotation *= Quaternion.Euler(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));

            child.gameObject.AddComponent<MeshCollider>().convex = true;
            
        }

        totalNumOfCarParts = _carParts.Count;
    }

    public void Update()
    {
        CalculateCarRepairPoints();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isGodMode = true;
            print("GOD MODE ACTIVATED");
        }
    }

    public void FixedUpdate()
    {
        if (isTestDrive)
        {
            Drive();
        }
    }
    public float CalculateCarRepairPoints()
    {
        carHealth = 0;

        foreach(var carPart in _carParts)
        {
            carHealth += carPart.currentHealth / carPart.maxHealth;

        }
        carHealth = carHealth / _carParts.Count;

        slider.value = carHealth;

        return carHealth;
    }
    

    public void Drive()
    {
        carRb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out FinishTrigger finishTrigger))
        {
            return;
        }

        foreach (var carPart in _carParts)
        {

            if (carPart.Damage(damagePerObstacle))
            {
                totalNumOfCarParts--;

                if(totalNumOfCarParts < minimalNumOfCarParts)
                {
                    

                    if (!isGodMode)
                    {
                        isTestDrive = false;
                        CarFellApart?.Invoke();

                    }
                }

            }

            CalculateCarRepairPoints();
        }
    }

    public void StartCarTestDrive()
    {
        isTestDrive = true;
        foreach (var child in _carParts)
        {
            child.PrepareForTestDrive();

        }
        carCollider.enabled = true;
        carRb.isKinematic = false;
    }

    public void StopCarTestDrive()
    {
        isTestDrive = false;
    }
}
