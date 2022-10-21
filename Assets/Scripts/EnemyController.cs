using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//Created by MattN six years ago

public class EnemyController : MonoBehaviour {
    public GameObject player;
    public float minAccurateDist;
    public float sightDistance;
    public float FieldOfView;
    public float agroTime;
    public byte health;
    public AudioClip[] noises;
    [HideInInspector] public Animator anim;
    [HideInInspector] public bool isAttacking;

    private AudioSource aS;
    private ParticleSystem pS;
    private NavMeshAgent agent;
    private Transform enemyHead;
    private LayerMask enemyLayerIgnore;
    private bool playerInSight;
    private bool isMoving;
    private float navUpdateTime;
    private float agro;

    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        aS = GetComponent<AudioSource>();

        enemyLayerIgnore = ~(1 << LayerMask.NameToLayer("Enemy"));

        //makes a list of all the children (or child in child, child in child in child, ect)
        Transform[] children = transform.GetComponentsInChildren<Transform>();
        foreach (Transform child in children) {
            if (child.gameObject.name == "Head")
                enemyHead = child;

            if (child.gameObject.name == "Particle Effects")
                pS = child.GetComponent<ParticleSystem>();
        }

        Rigidbody[] rbs = transform.GetComponentsInChildren<Rigidbody>();
        foreach(Rigidbody rb3d in rbs)
            rb3d.isKinematic = true;
    }

    private void FixedUpdate() {
        //Bad code cuz EnemyController Start() is after GameController Start()
        if (player == null) {
            player = GameController.instance.player;
            return;
        }
        //ParticleSystem.MainModule tempMain = pS.main;

        //If enemy has died then remove unneeded components and turn the enmy into a ragdoll
        if (health == 0) {
            //tempMain.loop = false;

            //Destroy(pS, 5);
            Destroy(anim);
            Destroy(agent);
            Destroy(GetComponent<CapsuleCollider>());

            Rigidbody[] rbs = transform.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb3d in rbs)
                rb3d.isKinematic = false;

            Destroy(this);

            return;
        }

        LookForPlayer();

        if (!aS.isPlaying)
            PlayNoise();

        //particles fade depending on the distance of the player
        /*ParticleSystem.MinMaxGradient tempGradient = tempMain.startColor;
        Color tempColor = tempGradient.color;
        tempColor.a = Mathf.Clamp(1 - (Vector3.Distance(transform.position, player.transform.position) / 25), 0, 1);
        tempGradient.color = tempColor;
        tempMain.startColor = tempGradient;*/

        isMoving = (agent.velocity != Vector3.zero);

        anim.SetBool("isWalking", isMoving);
        anim.SetBool("isAttacking", false);

        isAttacking = anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy Attack");

        //enemy cannot move while attacking
        agent.enabled = !isAttacking;

        //if the player is far away then only update the nav dest every five seconds
        if (Vector3.Distance(player.transform.position, transform.position) >= minAccurateDist) {
            if (navUpdateTime <= 0f) {
                changeNavDest(player.transform.position, minAccurateDist - 1f);
                navUpdateTime = 5f;
            }

            navUpdateTime -= Time.fixedDeltaTime;

            return;
        }

        //if the enemy is targeting a player then go directly to it's position
        //if it's close then attack and always look at the player
        if (agro >= 1) {
            if (Vector3.Distance(transform.position, player.transform.position) <= agent.stoppingDistance && playerInSight || isAttacking) {
                Vector3 tempVector = player.transform.position - transform.position;
                tempVector.y = 0;
                transform.rotation = Quaternion.LookRotation(tempVector);
            }

            if (Vector3.Distance(transform.position, player.transform.position) <= agent.stoppingDistance && playerInSight) {
                anim.SetBool("isAttacking", true);
                return;
            }

            changeNavDest(player.transform.position);

            agro -= Time.fixedDeltaTime;

            return;
        }

        //if the enemy is neither targeting the player or far away then change the dest just before it reaches it's destination
        if (Vector3.Distance(transform.position, agent.destination) <= agent.stoppingDistance + 1)
            changeNavDest(player.transform.position, minAccurateDist - 1);
    }

    //changes the nav dest to the given posision and will offset the distance randomly if a max offset distance is given
    void changeNavDest(Vector3 pos, float offsetDist = 0) {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Zombie Attack"))
            return;

        Vector3 offset = Vector3.zero;

        if (offsetDist != 0)
            offset += Quaternion.Euler(Vector3.up * Random.Range(0, 360)) * Vector3.forward * Random.Range(0, offsetDist);

        agent.SetDestination(pos + offset);
    }

    //1.-make sure the distance between the player and enemy is in the sight distance
    //creates five points for linecasts. head, left chest, middle chest, right chest, and feet
    void LookForPlayer() {
        playerInSight = false;

        if (Vector3.Distance(player.transform.position, enemyHead.position) > sightDistance)
            return;

        Vector3[] offsets = new Vector3[5];

        offsets[0] = Vector3.up;
        offsets[1] = Vector3.zero;
        offsets[2] = Vector3.down;
        offsets[3] = Quaternion.LookRotation(player.transform.position - enemyHead.position) * Vector3.right * 0.35f;
        offsets[4] = Quaternion.LookRotation(player.transform.position - enemyHead.position) * Vector3.left * 0.35f;

        foreach (Vector3 offsetValue in offsets)
            checkOffset(offsetValue);
    }

    //check if point is within it's field of view
    //checks if the linecast hit anything
    //check if the object that was hit is the player
    //if so set it's agro time to max
    void checkOffset(Vector3 offset) {
        Vector3 lineEnd = player.transform.position + offset;

        float angle = Vector3.SignedAngle(lineEnd - enemyHead.position, enemyHead.forward, Vector3.up);

        if (Mathf.Abs(angle) > FieldOfView * 0.5f)
            return;

        RaycastHit hit;

        if (!Physics.Linecast(enemyHead.position, lineEnd, out hit, enemyLayerIgnore, QueryTriggerInteraction.Ignore))
            return;

        if (!hit.transform.gameObject.CompareTag("Player"))
            return;

        agro = agroTime;
        playerInSight = true;

        Debug.DrawLine(enemyHead.position, lineEnd, Color.red, 0.03f);
    }

    private void PlayNoise() {
        if (noises.Length == 0)
            return;

        // pick & play a random enemy sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, noises.Length);
        aS.clip = noises[n];
        aS.PlayOneShot(aS.clip);
        // move picked sound to index 0 so it's not picked next time
        noises[n] = noises[0];
        noises[0] = aS.clip;
    }
}
