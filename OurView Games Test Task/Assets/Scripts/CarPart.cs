
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CarPart : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public float currentHealth;
    public float maxHealth = 10f;
    public float linePointsSetDelay = .1f;


    public bool isSelected = false;

    public Vector3 _defaultPosition;
    private Quaternion _defaultRotation;

    private MeshCollider _meshCollider;

    private LineRenderer _lineRenderer;
    private float _elapsedTime;
    private Camera _mainCam;

    private ParticlePool _particlePool;
    private Rigidbody _rb;
    private float _prevHealthRatio;
    
    public void Start()
    {
        currentHealth = 0;
        _meshCollider = GetComponent<MeshCollider>();

        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.widthMultiplier = .25f;
        _lineRenderer.positionCount = 0;
        _lineRenderer.numCornerVertices = 1;
        _elapsedTime = linePointsSetDelay;
        _mainCam = Camera.main;

        _particlePool = GetComponentInParent<ParticlePool>();
        
    }

    public void Update()
    {
        if (isSelected)
        {
            if(RepairParts.instance.repairMode == RepairMode.None)
            {
                if (RepairParts.instance.isWarningPanelEnabled)
                {
                    return;
                }
                RepairParts.instance.ShowWarningPanel(3f);
                return;
            }
            _elapsedTime -= Time.deltaTime;
            if(_elapsedTime < 0)
            {
                switch (RepairParts.instance.repairMode)
                {
                    case RepairMode.None:
                        break;
                    case RepairMode.Tape:
                        AddTapePoint();
                        break;
                    case RepairMode.Gum:
                        AddGum();
                        break;
                    case RepairMode.Bolts:
                        break;
                    default:
                        break;
                }

                _elapsedTime = linePointsSetDelay;
            }
            RepairPart(5f);

            RepairSlider.instance.UpdateSliderValue(currentHealth/ maxHealth);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        print(gameObject.name + " selected");

        if (currentHealth < maxHealth)
        {
            isSelected = true;
            RepairSlider.instance.ToggleSlider(eventData.pressPosition, true);

        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        print(gameObject.name + " unselected");
        isSelected = false;
        RepairSlider.instance.ToggleSlider(eventData.pressPosition, false);        
    }

    private void RepairPart(float repairPointsPerSecond)
    {
        
        if(currentHealth == maxHealth)
        {
            isSelected = false;
            return;
            
        }

        currentHealth += Time.deltaTime * repairPointsPerSecond;
        LerpTransform(repairPointsPerSecond);

        string log = $"repairing: {currentHealth}/{maxHealth}";
        Debug.Log(log);

        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    public void SaveDefaultTransform()
    {
        _defaultPosition = transform.position;
        _defaultRotation = transform.rotation;


    }

    private void LerpTransform(float repairPointsPerSecond)
    {
        transform.position = Vector3.Lerp(transform.position, _defaultPosition, Time.deltaTime * repairPointsPerSecond/2);
        transform.rotation = Quaternion.Lerp(transform.rotation, _defaultRotation, Time.deltaTime * repairPointsPerSecond/2);
    }

    private void AddTapePoint()
    {
        _lineRenderer.positionCount++;
        Vector3 point1 = _mainCam.transform.position + Random.onUnitSphere * 100;
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, _meshCollider.ClosestPoint(point1));
    }

    private void AddGum()
    {
        Vector3 point1 = _meshCollider.ClosestPoint(_mainCam.transform.position + Random.onUnitSphere * 100);

        _particlePool.CreateParticle(point1, Random.Range(.25f, .5f), Vector3.zero, 0f);
    }

    public void PrepareForTestDrive()
    {
        BakeLineRenderer();
        AddRigidbody();
        _prevHealthRatio = currentHealth / maxHealth;
    }

    private void BakeLineRenderer()
    {
        GameObject lineObj = new GameObject("LineObj");
        lineObj.transform.parent = transform;

        var meshFilter = lineObj.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        _lineRenderer.BakeMesh(mesh);
        _lineRenderer.enabled = false;
        //DoubleSideMesh(ref mesh);

        meshFilter.sharedMesh = mesh;
        
        var meshRenderer = lineObj.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = _lineRenderer.material;
        

    }

    private void AddRigidbody()
    {
        if(!gameObject.TryGetComponent(out _rb))
        {
        _rb = gameObject.AddComponent<Rigidbody>();

        }
        _rb.isKinematic = true;
        _rb.mass = 10;
    }
    
    //return true if car part is destroyed
    public bool Damage(float damagePoints)
    {

        bool isDestroyed;

        currentHealth -= damagePoints;
        if (currentHealth <= 0)
        {
            currentHealth = 0;

            _rb.isKinematic = false;

            isDestroyed = true;
        }
        else
        {
            isDestroyed = false;

        }
        return isDestroyed;
    }

    private void DoubleSideMesh(ref Mesh mesh)
    {
        var vertices = mesh.vertices;
        var uv = mesh.uv;
        var normals = mesh.normals;
        var szV = vertices.Length;
        var newVerts = new Vector3[szV * 2];
        var newUv = new Vector2[szV * 2];
        var newNorms = new Vector3[szV * 2];
        for (var j = 0; j < szV; j++)
        {
            // duplicate vertices and uvs:
            newVerts[j] = newVerts[j + szV] = vertices[j];
            newUv[j] = newUv[j + szV] = uv[j];
            // copy the original normals...
            newNorms[j] = normals[j];
            // and revert the new ones
            newNorms[j + szV] = -normals[j];
        }
        var triangles = mesh.triangles;
        var szT = triangles.Length;
        var newTris = new int[szT * 2]; // double the triangles
        for (var i = 0; i < szT; i += 3)
        {
            // copy the original triangle
            newTris[i] = triangles[i];
            newTris[i + 1] = triangles[i + 1];
            newTris[i + 2] = triangles[i + 2];
            // save the new reversed triangle
            var j = i + szT;
            newTris[j] = triangles[i] + szV;
            newTris[j + 2] = triangles[i + 1] + szV;
            newTris[j + 1] = triangles[i + 2] + szV;
        }
        mesh.vertices = newVerts;
        mesh.uv = newUv;
        mesh.normals = newNorms;
        mesh.triangles = newTris;
    }
}
