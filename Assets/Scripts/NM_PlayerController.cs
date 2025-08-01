using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NM_PlayerController : MonoBehaviour
{
    [SerializeField] private Camera _cam;
    [SerializeField] private float _desiredDistance = 0.5f;

    private LineRenderer _lineRenderer;

    private NavMeshAgent _agent;
    private bool _isLineRender;

    private Vector3 _desiredPos;

    

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

        if (Vector3.Distance(_desiredPos, _agent.transform.position) <= _desiredDistance)
            _isLineRender = false;

        _lineRenderer.enabled = _isLineRender;
    }

    private void CalculatePath()
    {
        NavMeshPath path = new NavMeshPath();
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (!Input.GetButton("Fire3"))
            {
                _agent.CalculatePath(hit.point, path);
                _agent.destination = hit.point;
                _desiredPos = _agent.destination;

            }
            else _agent.ResetPath();
        }

        if (path != null)
        {
            _isLineRender = true;
            DrawPath(hit);
            EvaluatePathState(path);
        }
        else _isLineRender = false;
    }

    private void DrawPath(RaycastHit hit)
    {
        Vector3[] linePoints = new Vector3[2];
        linePoints[0] = hit.point;
        linePoints[1] = _agent.transform.position;
        _lineRenderer.SetPositions(linePoints);
    }

    private void EvaluatePathState(NavMeshPath path)
    {
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            _lineRenderer.startColor = Color.green;
            _lineRenderer.endColor = Color.green;
            _lineRenderer.material.color = Color.green;
        }
        else if (path.status == NavMeshPathStatus.PathPartial)
        {
            _lineRenderer.startColor = Color.yellow;
            _lineRenderer.endColor = Color.yellow;
            _lineRenderer.material.color = Color.yellow;

        }
        else if (path.status != NavMeshPathStatus.PathInvalid)
        {
            _lineRenderer.startColor = Color.red;
            _lineRenderer.endColor = Color.red;
            _lineRenderer.material.color = Color.red;
        }
    }

}
