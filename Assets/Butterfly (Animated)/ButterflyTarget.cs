using UnityEngine;

public class ButterflyTarget : MonoBehaviour
{
    public Animator ButterflyAnimator;
    public Transform LandTarget;
    public float FlySpeed = 1f;
    public float FlightPathVariance = 1f;
    public float TurnSpeed = 1f;
    public float TargetVelocityTolerance = 0.1f;
    public float AcceptableTargetDistance = 0.01f;

    private Vector3 m_targetLastPos = Vector3.zero;
    private float m_targetVelocity = 0f;
    private Vector3 m_targetDirection;
    private float m_targetDistance = 0f;

    private int m_bflyForwardId;
    private int m_bflyBackwardId;
    private int m_bflyRestId;
    private bool m_bflyAnimForward;
    private bool m_bflyAnimBackward;
    private bool m_bflyAnimRest;

    private void Start()
    {
        if (LandTarget != null)
            transform.LookAt(LandTarget.position);
        m_bflyAnimForward = m_bflyAnimBackward = m_bflyAnimRest = false;
        m_bflyForwardId = Animator.StringToHash("FlyForward");
        m_bflyBackwardId = Animator.StringToHash("FlyBackward");
        m_bflyRestId = Animator.StringToHash("Rest");
    }

    private void Update()
    {
        if (LandTarget == null) return;

        //Target distance and direction
        m_targetDirection = LandTarget.position - transform.position;
        m_targetDistance = m_targetDirection.sqrMagnitude;
        m_targetDirection.Normalize();

        //Target velocity
        m_targetVelocity = (LandTarget.position - m_targetLastPos).sqrMagnitude / Time.deltaTime;
        float angle = Vector3.SignedAngle(transform.forward, m_targetDirection, transform.up);

        if (m_targetVelocity > TargetVelocityTolerance)
        {
            //wait for target to stabilize            
            transform.Rotate(transform.up, angle * Time.deltaTime);
            transform.Translate(-m_targetDirection * FlySpeed * 2f * Time.deltaTime, Space.World);

            //update animation
            if (!m_bflyAnimBackward)
            {
                ButterflyAnimator.SetBool(m_bflyBackwardId, true);
                ButterflyAnimator.SetBool(m_bflyForwardId, false);
                ButterflyAnimator.SetBool(m_bflyRestId, false);
                m_bflyAnimBackward = true;
                m_bflyAnimForward = m_bflyAnimRest = false;
            }
        }
        else if (m_targetDistance > AcceptableTargetDistance * AcceptableTargetDistance)
        {
            //Approach the target            
            transform.Rotate(transform.up, angle * TurnSpeed * Time.deltaTime);
            transform.Translate((m_targetDirection * FlySpeed + Random.insideUnitSphere * FlightPathVariance) * Time.deltaTime, Space.World);

            //update animation
            if (!m_bflyAnimForward)
            {
                ButterflyAnimator.SetBool(m_bflyBackwardId, false);
                ButterflyAnimator.SetBool(m_bflyForwardId, true);
                ButterflyAnimator.SetBool(m_bflyRestId, false);
                m_bflyAnimForward = true;
                m_bflyAnimBackward = m_bflyAnimRest = false;
            }
        }
        else
        {
            //rest animation
            if (!m_bflyAnimRest)
            {
                ButterflyAnimator.SetBool(m_bflyBackwardId, false);
                ButterflyAnimator.SetBool(m_bflyForwardId, false);
                ButterflyAnimator.SetBool(m_bflyRestId, true);
                m_bflyAnimRest = true;
                m_bflyAnimBackward = m_bflyAnimForward = false;
            }
        }
        m_targetLastPos = LandTarget.position;
    }
}
/*
 __  _ _____   ____  _ _____  
|  \| | __\ `v' /  \| |_   _| 
| | ' | _| `. .'| | ' | | |   
|_|\__|_|   !_! |_|\__| |_|
 
*/