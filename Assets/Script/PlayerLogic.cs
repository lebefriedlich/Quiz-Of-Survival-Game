using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{
    private Rigidbody rb;
    public float walkspeed, runspeed, jumppower, fallspeed, airMultiplier;
    public Transform PlayerOrientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    bool grounded = true, aerialboost = true;
    public Animator anim;
    public CameraLogic camlogic;
    public float HitPoints = 100f;
    public Transform target;
    private float DistanceToTarget;
    private bool canAttack = true;
    private float MaxHealth;
    public float attackCooldown;
    public UIGameplayLogic UIGameplayLogic;

    [Header("SFX")]
    public AudioClip attackSFX;
    public AudioClip stepSFX;
    public AudioClip getHitSFX;
    public AudioSource audioSource;
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        MaxHealth = HitPoints;
        UIGameplayLogic.updateHealthBar(HitPoints, MaxHealth);

        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    void Update()
    {
        Movement();
        Jump();
        AttackLogic();

        if (Input.GetKey(KeyCode.F))
        {
            PlayerGetHit(100f);
        }

        // Shortcut Menang
        if (Input.GetKey(KeyCode.Alpha1))
        {
            UIGameplayLogic.GameResult(true);
        }

        // Shortcut Kalah
        if (Input.GetKey(KeyCode.Alpha2))
        {
            UIGameplayLogic.GameResult(false);
        }
    }

    public void step()
    {
        audioSource.clip = stepSFX;
        audioSource.Play();
    }

    private void Movement()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        moveDirection = PlayerOrientation.forward * verticalInput + PlayerOrientation.right * horizontalInput;

        if (grounded && moveDirection != Vector3.zero)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                anim.SetBool("Run", true);
                anim.SetBool("Walk", false);
                rb.velocity = new Vector3(moveDirection.x * runspeed, rb.velocity.y, moveDirection.z * runspeed);
            }
            else
            {
                anim.SetBool("Walk", true);
                anim.SetBool("Run", false);
                rb.velocity = new Vector3(moveDirection.x * walkspeed, rb.velocity.y, moveDirection.z * walkspeed);
            }
        }
        else
        {
            anim.SetBool("Walk", false);
            anim.SetBool("Run", false);
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            float forwardVelocity = rb.velocity.z;
            float sideVelocity = rb.velocity.x;

            Debug.Log(forwardVelocity);
            Debug.Log(sideVelocity);

            rb.velocity = new Vector3(sideVelocity, 0f, forwardVelocity);
            rb.AddForce(PlayerOrientation.up * jumppower, ForceMode.Impulse);
            grounded = false;
            anim.SetBool("Jump", true);
        }
        else if (!grounded)
        {
            rb.AddForce(Vector3.down * fallspeed * rb.mass, ForceMode.Force);  // Memberikan gaya jatuh saat di udara
            if (aerialboost)
            {
                rb.AddForce(moveDirection.normalized * walkspeed * 10f * airMultiplier, ForceMode.Impulse);
                aerialboost = false;
            }
        }
    }

    // Fungsi untuk mengubah status grounded saat menyentuh tanah
    public void groundedchanger()
    {
        grounded = true;
        aerialboost = true;
        anim.SetBool("Jump", false);
    }

    private void AttackLogic()
    {
        if (Input.GetKey(KeyCode.Mouse1) && canAttack)
        {
            if (moveDirection.normalized != Vector3.zero)
            {
                anim.SetBool("Attack", false);
            }
            else
            {
                anim.SetBool("Attack", true);
                StartCoroutine(AttackCooldown());
            }
        }
        else
        {
            anim.SetBool("Attack", false);
        }
    }

    private IEnumerator AttackCooldown()
    {
        this.canAttack = false; // Tidak bisa menyerang selama cooldown
        yield return new WaitForSeconds(attackCooldown); // Tunggu waktu cooldown
        this.canAttack = true; // Setelah cooldown selesai, bisa menyerang lagi
    }


    public void PlayerGetHit(float damage)
    {
        audioSource.clip = getHitSFX;
        audioSource.Play();
        Debug.Log("Player Receive Damage - " + damage);
        HitPoints = HitPoints - damage;
        UIGameplayLogic.updateHealthBar(HitPoints, MaxHealth);

        if (HitPoints <= 0f)
        {
            anim.SetBool("Death", true);
        }
    }

    public void HitConnect()
    {
        audioSource.PlayOneShot(attackSFX);
        if (target != null)
        {
            DistanceToTarget = Vector3.Distance(target.position, transform.position);
            if (DistanceToTarget <= 8f)
            {
                target.GetComponent<EnemyLogic>().TakeDamage(20f);
            }
        }
    }
}