using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyStatus))]
public class EnemyMove : MonoBehaviour
{
    [SerializeField] private LayerMask raycastLayerMask;

    private NavMeshAgent _agent;
    private RaycastHit[] _raycastHits = new RaycastHit[10];
    private EnemyStatus _status;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();    
        _status = GetComponent<EnemyStatus>();
    }

    public void OnDetectObject(Collider collider)
    {
        // spawner を利用した際にStart()を経由しない？ため、ここで初期化処理を行う
        _status = _status ?? GetComponent<EnemyStatus>();
        _agent = _agent ?? GetComponent<NavMeshAgent>();

        if (!_status.IsMovable)
        {
            _agent.isStopped = true;
            return;
        }

        if (collider.CompareTag("Player"))
        {
            // 自身とプレイヤーの座標差分を計算
            var positionDiff = collider.transform.position - transform.position;
            // プレイヤーとの距離を計算
            var distance = positionDiff.magnitude;
            // プレイヤーへの方向
            var direction = positionDiff.normalized;

            // var hitCount = Physics.RaycastNonAlloc(transform.position, direction, _raycastHits, distance);
            var hitCount = Physics.RaycastNonAlloc(transform.position, direction, _raycastHits, distance, raycastLayerMask);

            Debug.Log("hitCount:" + hitCount);

            if (hitCount == 0)
            {
                // 本作のプレイヤーはCharacterControllerを使っていて、Clliderは使っていないのでRaycastはヒットしない
                // つまり、ヒット数が0であればプレイヤーとの間に障害物が無いということになる
                _agent.isStopped = false;
                _agent.destination = collider.transform.position;
            }
            else
            {
                _agent.isStopped = true;
            }
        }

    }
}
