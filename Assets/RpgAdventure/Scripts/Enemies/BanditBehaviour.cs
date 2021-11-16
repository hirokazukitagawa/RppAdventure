using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace RppAdventure
{


    public class BanditBehaviour : MonoBehaviour
    {
        public float detectionRadius = 10.0f;
        public float detectionAngle = 90.0f;
        public float timeToStopPursuit = 2.0f;
        public float timeToWaitOnPursuit = 2.0f;

        private PlayerController m_Target;
        private NavMeshAgent m_NavMeshAgent;
        private float m_TimeSinceLostTarget = 0;
        private Vector3 m_OriginPosition;

        private void Awake()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
            m_OriginPosition = transform.position;
        }


        private void Update()
        {
           var target = LookForPlayer();

            if (m_Target == null)
            {
                if(target != null)
                {
                    m_Target = target;
                }
            }
            else
            {
                m_NavMeshAgent.SetDestination(m_Target.transform.position);

                if (target == null)
                {
                    m_TimeSinceLostTarget += Time.deltaTime;

                    if (m_TimeSinceLostTarget >= timeToStopPursuit)
                    {
                        m_Target = null;
                        m_NavMeshAgent.isStopped = true;
                        StartCoroutine(WaitOnPursuit());
                    }
                }
                     else
                    {
                    m_TimeSinceLostTarget = 0;
                    }
                 }


        }

        private IEnumerator WaitOnPursuit()
        {
            yield return new WaitForSeconds(timeToWaitOnPursuit);
            m_NavMeshAgent.isStopped = false;
            m_NavMeshAgent.SetDestination(m_OriginPosition);
        }

        private PlayerController LookForPlayer()
        {
            if(PlayerController.Instance == null)
            {
                return null;
            }
            Vector3 enemyPosition = transform.position;
            Vector3 toPlayer = PlayerController.Instance.transform.position - enemyPosition;
            toPlayer.y = 0;

            if(toPlayer.magnitude <= detectionRadius)
            {
                if(Vector3.Dot(toPlayer.normalized, transform.forward) >
                    Mathf.Cos(detectionAngle * 0.5f * Mathf.Deg2Rad))
                {
                    return PlayerController.Instance;
                }
            }

            return null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Color c = new Color(0, 0, 0.7f, 0.4f);
            UnityEditor.Handles.color = c;

            Vector3 rotatedForward = Quaternion.Euler(
                0,
                -detectionAngle * 0.5f,
                0) * transform.forward;

            UnityEditor.Handles.DrawSolidArc(
                transform.position,
                Vector3.up,
                rotatedForward,
                detectionAngle,
                detectionRadius);

        }
#endif
    }

 }
