using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//Created by MattN six years ago

enum EnemyState {
    Wander,
    Chase,
    Attack,
    Death
}

public class EnemyController : MonoBehaviour {
    public float wanderRange;
    public float sightDistance;
    public float fieldOfView;
    public float agroTime;
    public byte health;
    public AudioClip[] noises;

    private Animator enemyAnimator;
    private AudioSource enemyAudioSource;
    private ParticleSystem enemyParticleSystem;
    private NavMeshAgent enemyNavAgent;
    private Transform enemyHead;
    private Transform player;
    private LayerMask enemyLayerIgnore;
    private EnemyState currentState;
    private bool playerInSight;
    private bool isMoving;
    private bool isAttacking;
    private float navUpdateTime;
    private float currentAgroTime;

    private void Start() {
        enemyNavAgent = GetComponent<NavMeshAgent>();
        enemyAnimator = transform.GetChild(0).GetComponent<Animator>();
        enemyAudioSource = GetComponent<AudioSource>();

        enemyLayerIgnore = ~(1 << LayerMask.NameToLayer("Enemy"));

        //makes a list of all the children (or child in child, child in child in child, ect)
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        foreach (Transform child in children) {
            if (child.gameObject.name == "Head")
                enemyHead = child;

            if (child.gameObject.name == "Particle Effects")
                enemyParticleSystem = child.GetComponent<ParticleSystem>();
        }

        Rigidbody[] rbs = transform.GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rb3d in rbs)
            rb3d.isKinematic = true;

        player = GameController.instance.player.transform;

        currentState = EnemyState.Wander;
    }

    private void FixedUpdate() {
        if (GameController.instance.gamePaused)
            return;

        LookForPlayer();

        if (!enemyAudioSource.isPlaying)
            PlayNoise();

        isMoving = (enemyNavAgent.velocity != Vector3.zero);

        enemyAnimator.SetBool("isWalking", isMoving);
        enemyAnimator.SetBool("isAttacking", false);

        switch (currentState) {
            case EnemyState.Wander:
                RunWanderState();
                break;
            case EnemyState.Chase:
                RunChaseState();
                break;
            case EnemyState.Attack:
                RunAttackState();
                break;
            case EnemyState.Death:
                RunDeathState();
                break;
            default:
                break;
        }
    }

    private void RunWanderState() {
        if (currentAgroTime > 0f) {
            currentState = EnemyState.Chase;
            return;
        }

        //if enemy is outside the wander range then only update the nav dest every five seconds
        if (Vector3.Distance(player.position, transform.position) >= wanderRange) {
            if (navUpdateTime <= 0f) {
                changeNavDest(player.position, wanderRange - 1f);
                navUpdateTime = 5f;
            }

            navUpdateTime -= Time.fixedDeltaTime;

            return;
        }

        //change the dest just before it reaches it's destination
        if (Vector3.Distance(transform.position, enemyNavAgent.destination) <= enemyNavAgent.stoppingDistance + 1f)
            changeNavDest(player.position, wanderRange - 1f);
    }

    private void RunChaseState() {
        //if the enemy is targeting a player then go directly to it's position

        if (!playerInSight)
            currentAgroTime -= Time.fixedDeltaTime;

        if (currentAgroTime <= 0f) {
            currentAgroTime = 0f;
            currentState = EnemyState.Wander;

            return;
        }


        if (Vector3.Distance(transform.position, player.transform.position) <= enemyNavAgent.stoppingDistance && playerInSight || isAttacking) {
            currentState = EnemyState.Attack;

            return;
        }

        changeNavDest(player.transform.position);
    }

    private void RunAttackState() {
        isAttacking = enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack");

        //enemy cannot move while attacking
        enemyNavAgent.enabled = !isAttacking;

        //Change to maxAttackRange?
        if (!isAttacking && (!playerInSight || Vector3.Distance(transform.position, player.transform.position) >= enemyNavAgent.stoppingDistance)) {
            currentState = EnemyState.Chase;

            return;
        }

        if (!isAttacking)
            enemyAnimator.SetBool("isAttacking", true);

        //consider moving code so that enemy cant change direction during attack animation, prolly too OP
        //consider using Chase state for changing directions with above in mind, also smoothed rotation
        Vector3 tempVector = player.transform.position - transform.position;
        tempVector.y = 0;
        transform.rotation = Quaternion.LookRotation(tempVector);
    }

    private void RunDeathState() {
        //If enemy has died then remove unneeded components and turn the enmy into a ragdoll

        //ParticleSystem.MainModule tempMain = enemyParticleSystem.main;
        //tempMain.loop = false;

        //Destroy(pS, 5);
        Destroy(enemyAnimator);
        Destroy(enemyNavAgent);
        Destroy(GetComponent<CapsuleCollider>());

        Rigidbody[] rbs = transform.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb3d in rbs)
            rb3d.isKinematic = false;

        Destroy(this);
    }

    //changes the nav dest to the given posision and will offset the distance randomly if a max offset distance is given
    private void changeNavDest(Vector3 pos, float offsetDist = 0f) {
        if (enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack"))
            return;

        Vector3 offset = Vector3.zero;

        if (offsetDist != 0f)
            offset += Quaternion.Euler(Vector3.up * Random.Range(0f, 360f)) * Vector3.forward * Random.Range(0f, offsetDist);

        enemyNavAgent.SetDestination(pos + offset);
    }

    //1.-make sure the distance between the player and enemy is in the sight distance
    //creates five points for linecasts. head, left chest, middle chest, right chest, and feet
    private void LookForPlayer() {
        playerInSight = false;

        if (Vector3.Distance(player.position, enemyHead.position) > sightDistance)
            return;

        Vector3[] offsets = new Vector3[5];

        offsets[0] = Vector3.up;
        offsets[1] = Vector3.zero;
        offsets[2] = Vector3.down;
        offsets[3] = Quaternion.LookRotation(player.position - enemyHead.position) * Vector3.right * 0.35f;
        offsets[4] = Quaternion.LookRotation(player.position - enemyHead.position) * Vector3.left * 0.35f;

        //init variables once to use in for loop
        Vector3 lineEnd = Vector3.zero;
        float angle = 0f;
        RaycastHit hit;

        //check if point is within it's field of view
        //checks if the linecast hit anything
        //check if the object that was hit is the player
        //if so set it's agro time to max
        foreach (Vector3 offsetValue in offsets) {
            lineEnd = player.position + offsetValue;
            angle = Vector3.SignedAngle(lineEnd - enemyHead.position, enemyHead.forward, Vector3.up);

            if (Mathf.Abs(angle) > fieldOfView * 0.5f || 
                !Physics.Linecast(enemyHead.position, lineEnd, out hit, enemyLayerIgnore, QueryTriggerInteraction.Ignore) || 
                !hit.transform.gameObject.CompareTag("Player"))
                continue;

            currentAgroTime = agroTime;
            playerInSight = true;

            Debug.DrawLine(enemyHead.position, lineEnd, Color.red, 0.03f);
        }
    }

    private void PlayNoise() {
        if (noises.Length == 0)
            return;

        //play a random sound from the array, excluding sound at index 0
        int n = Random.Range(1, noises.Length);
        enemyAudioSource.clip = noises[n];
        enemyAudioSource.PlayOneShot(enemyAudioSource.clip);
        //move picked sound to index 0 so it's not picked next time
        noises[n] = noises[0];
        noises[0] = enemyAudioSource.clip;
    }
}
