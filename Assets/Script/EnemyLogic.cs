using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    public float hitPoints = 100f;
    public float turnSpeed = 20f;
    public Transform target;
    public float chaseRange;
    private UnityEngine.AI.NavMeshAgent agent;
    private float DistanceToTarget;
    private float DistanceToDefault;
    private Animator anim;
    Vector3 DefaultPosition;
    private int attackCount = 1;
    public QuizManager QuizManager;

    [Header("SFX")]
    public AudioClip attackSFX;
    public AudioClip getHitSFX;
    public AudioClip dieSFX;
    public AudioClip stepSFX;
    public AudioSource audioSource;

    [Header("VFX")]
    public ParticleSystem SlashEffect;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        anim = this.GetComponentInChildren<Animator>();
        anim.SetFloat("Hit Point", hitPoints);
        DefaultPosition = this.transform.position;
    }

    public void step()
    {
        audioSource.clip = stepSFX;
        audioSource.Play();
    }

    public void resetAnimator()
    {
        gameObject.SetActive(true);

        anim = this.GetComponentInChildren<Animator>();
        anim.Rebind();
        anim.Update(0f);

        hitPoints = 100f;
        anim.Play("Spawn 0");
        anim.SetInteger("AttackCount", 1);
        anim.SetFloat("Hit Point", hitPoints);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }

    public void SlashEffectToggleOn()
    {
        SlashEffect.Play();
    }
    
    void Update()
    {
        DistanceToTarget = Vector3.Distance(new Vector3(target.position.x, 0, target.position.z), new Vector3(transform.position.x, 0, transform.position.z));
        DistanceToDefault = Vector3.Distance(DefaultPosition, transform.position);

        if (DistanceToTarget <= chaseRange)
        {
            FaceTarget(target.position);
            if (DistanceToTarget > agent.stoppingDistance)
            {
                ChasceTarget();
                SlashEffect.Stop();
            }
            else if (DistanceToTarget <= agent.stoppingDistance)
            {
                float currentHealth = target.GetComponent<PlayerLogic>().HitPoints;
                if (currentHealth <= 0f)
                {
                    anim.SetBool("Run", false);
                    anim.SetBool("Attack", false);
                }
                else
                {
                    Attack();
                }
            }
        }
        else if (DistanceToTarget >= chaseRange * 2)
        {
            agent.SetDestination(DefaultPosition);
            FaceTarget(DefaultPosition);
            if (DistanceToDefault <= agent.stoppingDistance)
            {
                Debug.Log("Time To Stop");
                anim.SetBool("Run", false);
                anim.SetBool("Attack", false);
            }
        }
    }

    private void FaceTarget(Vector3 destination)
    {
        Vector3 direction = (destination - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
    }

    public void Attack()
    {
        agent.SetDestination(target.position);
        anim.SetBool("Run", false);
        anim.SetBool("Attack", true);
        anim.SetInteger("AttackCount", attackCount);
    }

    public void ChasceTarget()
    {
        agent.SetDestination(target.position);
        anim.SetBool("Run", true);
        anim.SetBool("Attack", false);
    }

    public void TakeDamage(float damage)
    {
        audioSource.clip = getHitSFX;
        audioSource.Play();
        hitPoints -= damage;
        anim.SetTrigger("GetHit");
        anim.SetFloat("Hit Point", hitPoints);
        if (hitPoints <= 0)
        {
            audioSource.clip = dieSFX;
            audioSource.Play();
            StartCoroutine(DieWithDelay(3f));
        }
    }

    private IEnumerator DieWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        QuizManager.ShowAllScrolls();
        gameObject.SetActive(false);
        hitPoints = 100f;
    }

    public void HitConnect()
    {
        if (DistanceToTarget <= agent.stoppingDistance)
        {
            if (attackCount == 1 || attackCount == 2)
            {
                attackCount++;
                anim.SetInteger("AttackCount", attackCount);
                target.GetComponent<PlayerLogic>().PlayerGetHit(10);
            }
            else if (attackCount == 3)
            {
                attackCount++;
                anim.SetInteger("AttackCount", attackCount);
                target.GetComponent<PlayerLogic>().PlayerGetHit(20);
                attackCount = 1;
                anim.SetInteger("AttackCount", attackCount);
            }
            else
            {
                attackCount = 1;
                anim.SetInteger("AttackCount", attackCount);
            }

            audioSource.clip = attackSFX;
            audioSource.Play();
        }
    }
}
