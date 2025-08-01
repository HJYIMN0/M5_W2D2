using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class NM_EnemyController : MonoBehaviour
{
    [SerializeField] private Transform[] _waypoints;
    [SerializeField] private float _distance = 1f;
    [SerializeField] private float _waitingTime = 2f;
    [SerializeField] private float _radius = 5f;
    [SerializeField] private LayerMask _playerLayer;


    private GameObject _player;

    private enum ENEMYSTATE { PATROLLING, FOLLOWING }
    private ENEMYSTATE _enemyState = ENEMYSTATE.PATROLLING;

    private NavMeshAgent _agent;
    private int _index;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null) Debug.Log("Manca il NavMeshAgent!");
    }

    private void Start()
    {
        if (_agent == null || _waypoints == null || _waypoints.Length == 0)
        {
            Debug.Log("There's something wrong!");
            return;
        }


        _player = GameObject.FindGameObjectWithTag("Player");
        _enemyState = ENEMYSTATE.PATROLLING;
        _index = 0;
        StartCoroutine("ChoosePath");

    }

    public void Update()
    {
        if (Physics.CheckSphere(transform.position, _radius, _playerLayer))
        {
            _enemyState = ENEMYSTATE.FOLLOWING;
            FollowPlayer();
        }
        else _enemyState = ENEMYSTATE.PATROLLING;

        Debug.Log(_enemyState);
    }
    IEnumerator ChoosePath()
    {
        while (_enemyState == ENEMYSTATE.PATROLLING)
        {
            while (true)
            {
                NavMeshPath path = new();

                while (_index < _waypoints.Length)
                {
                    Vector3 target = _waypoints[_index].position;

                    _agent.CalculatePath(target, path);
                    _agent.SetPath(path);
                    _agent.destination = target;

                    // Aspetta finché il path non è calcolato e il nemico non è arrivato
                    while (_agent.remainingDistance > _distance)
                    {
                        yield return null; // aspetta il prossimo frame
                    }

                    // Arrivato
                    _agent.ResetPath();
                    yield return new WaitForSeconds(_waitingTime);
                    _index++;
                }

                _index = 0;
            }
        }
    }

    public void FollowPlayer()
    {
        if (_enemyState != ENEMYSTATE.FOLLOWING)
        {
            Debug.Log("Non sto inseguendo nessuno!");
            return;
        }
        else
        {
            _agent.ResetPath();
            NavMeshPath path = new();
            Vector3 target = _player.transform.position;

            _agent.CalculatePath(target, path);
            _agent.SetPath(path);
            _agent.destination = target;

        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

}