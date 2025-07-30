using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NM_PlayerController : MonoBehaviour
{
    [SerializeField] private Camera _cam;

    private LineRenderer _lineRenderer;

    private NavMeshAgent _agent;
    private bool _isLineRender;

    

    private void Awake()
    {
        if ( _cam == null ) _cam = Camera.main;

        _agent = GetComponent<NavMeshAgent>();

        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startColor = Color.white;
    }
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            CalculatePath();
        }

        _lineRenderer.enabled = _isLineRender;
    }

    private void CalculatePath()
    {
        NavMeshPath path = new NavMeshPath();
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            _agent.CalculatePath(hit.point, path);
            _agent.destination = hit.point;
        }

        if (path != null)
        {
            _isLineRender = true;
            //_lineRenderer.SetPosition(0, hit.point);
            Vector3[] linepoint = new Vector3[2];
            linepoint[0] = hit.point;
            linepoint[1] = _agent.transform.position;
            _lineRenderer.SetPositions(linepoint);
        }
        else _isLineRender = false;

    }

}
