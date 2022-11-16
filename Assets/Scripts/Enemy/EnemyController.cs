using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//Created by MattN six years ago

public enum EnemyStateNames {
    Wander,
    Chase,
    Attack,
    Death
}

public class EnemyController : MonoBehaviour {
    public GameObject projectile;
    public Transform projectilePos;
    public float wanderRange;
    public float sightDistance;
    public float fieldOfView;
    public float agroTime;
    public float soundSensitivty;
    public byte health;
    public float fireInterval; // time in seconds between shots
    public float gunStrength; // impulse initialiy applied to the bullets
    public AudioClip[] noises;

    private Animator enemyAnimator;
    private AudioSource enemyAudioSource;
    private NavMeshAgent enemyNavAgent;
    private Transform enemyHead;
    private Transform player;
    private LayerMask enemyLayerIgnore;
    private List<EnemyState> enemyStates = new List<EnemyState>();
    private EnemyState currentState;
    [HideInInspector] public float fireCooldown = 0; // time in seconds until they can shoot again
    [HideInInspector] public bool playerInSight = false;
    [HideInInspector] public bool isMoving = false;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool heardSound = false;
    [HideInInspector] public float navUpdateTime = 0f;
    [HideInInspector] public float currentAgroTime = 0f;

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
        }

        Rigidbody[] rbs = transform.GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rb3d in rbs)
            rb3d.isKinematic = true;

        player = GameController.instance.player.transform;

        enemyStates.Add(new EnemyStateWander(this, player, enemyNavAgent));
        enemyStates.Add(new EnemyStateChase(this, player, enemyNavAgent));
        enemyStates.Add(new EnemyStateAttack(this, player, enemyNavAgent, enemyAnimator));
        enemyStates.Add(new EnemyStateDeath(this));

        ChangeState(EnemyStateNames.Wander);
    }

    private void FixedUpdate() {
        if (GameController.instance.gamePaused)
            return;

        if (health <= 0) {
            ChangeState(EnemyStateNames.Death);
            return;
        }
        
        LookForPlayer();

        if (!enemyAudioSource.isPlaying)
            PlayNoise();

        isMoving = (enemyNavAgent.velocity != Vector3.zero);

        enemyAnimator.SetBool("isWalking", isMoving);
        enemyAnimator.SetBool("isAttacking", false);

        heardSound = false;

        fireCooldown -= Time.deltaTime;

        currentState.UpdateState();
    }

    public void ChangeState(EnemyStateNames state) {
        if (currentState != null)
            currentState.ExitState();

        switch (state) {
            case EnemyStateNames.Wander:
                currentState = enemyStates[0];
                break;
            case EnemyStateNames.Chase:
                currentState = enemyStates[1];
                break;
            case EnemyStateNames.Attack:
                currentState = enemyStates[2];
                break;
            case EnemyStateNames.Death:
                currentState = enemyStates[3];
                break;
            default:
                break;
        }

        currentState.EnterState();
    }

    public void EnemyDeath() {
        //If enemy has died then remove unneeded components and turn the enemy into a ragdoll

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
    public void changeNavDest(Vector3 pos, float offsetDist = 0f) {
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

    // shoot a bullet at the player
    public void Shoot() {
        Rigidbody bulletRb;
        if (GameController.instance.useObjectPooling) {
            bulletRb = ObjectPooler.instance.SpawnFromPool("Bullet", projectilePos.position, projectilePos.rotation).GetComponent<Rigidbody>();
        }
        else {
            bulletRb = Instantiate(projectile, projectilePos.position, projectilePos.rotation, ObjectPooler.instance.transform).GetComponent<Rigidbody>();
        }
        bulletRb.AddForce(projectilePos.forward * gunStrength, ForceMode.Impulse);
        fireCooldown = fireInterval;
    }

    public void OnNotify(Vector3 soundPosition, float soundDB) {
        //Stop infinite recursion
        if (heardSound)
            return;

        float soundLevel = soundDB * Mathf.Pow(2f / Vector3.Distance(transform.position, soundPosition), 2f);
        if (soundLevel < soundSensitivty)
            return;

        heardSound = true;
        currentAgroTime = agroTime;

        //Notify nearby enemies
        StealthController.instance.NotifyAll(transform.position, 10f);
    }
}
