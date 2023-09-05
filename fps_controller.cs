using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class fps_controller : NetworkBehaviour
{
    // physics vars
    public float movementSpeed;
    public float jumpSpeed;
    public float dashSpeed;
    public float gravity;
    // this variable governs dash physics, grav lifts, explosions, and knockback
    private Vector3 force;

    // dash vars
    public float dashCooldown;
    private float dashCooldownTimer = 0;
    private bool dash_available = true;


    // camera vars
    public float lookSpeed;
    public float upperLookLimit;
    public float lowerLookLimit;

    public GameObject camera;

    // component vars
    CharacterController characterController;
    Transform spine;

    // assorted vars
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    [HideInInspector]
    public bool canMove = true;

    public GameObject object_to_spawn;
    public NetworkManager nm;

    public gamestate_script gs;

    void Start()
    {
        if (!isLocalPlayer)
        {
            camera.SetActive(false);
        }
        characterController = GetComponent<CharacterController>();
        spine = transform.Find("armature/Bone/hips/Spine");
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        nm = FindObjectOfType<NetworkManager>();
        gs = FindObjectOfType<gamestate_script>();
    }

    void Update()
    {
        if (!isLocalPlayer){
            return;
        }

        HandleCameraPlayerRotation();
        if (Input.GetKey(KeyCode.LeftShift)){
            Dash();
        }
        ApplyForces();
    }

    private void LateUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        // player rotation up and down
        spine.eulerAngles = new Vector3(rotationX, spine.eulerAngles.y, 0.0f);
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        HandleMovement();
        Vector3 dash_direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        HandleCooldowns();
    }

    private void HandleMovement(){
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);


        float curSpeedX = movementSpeed * Input.GetAxis("Vertical");
        float curSpeedY = movementSpeed * Input.GetAxis("Horizontal");
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetKey(KeyCode.Space) && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
    }


    private void HandleCameraPlayerRotation(){
        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);


            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    
    private void HandleCooldowns(){
        if (!dash_available)
        {
            if (dashCooldownTimer < dashCooldown)
            {
                dashCooldownTimer += Time.deltaTime;
            }
            else
            {
                dashCooldownTimer = 0;
                dash_available = true;
            }
        }
    }

    private void Dash(){
        if (dash_available)
        {
            
            Vector3 dash_direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (dash_direction != Vector3.zero)
            {
                AddForce(dash_direction, dashSpeed);
                dash_available = false;
            } else{
                if (!characterController.isGrounded){
                    AddForce(new Vector3(0,-1,0), dashSpeed);
                    dash_available = false;
                }
            }
        }
    }

    private void AddForce(Vector3 direction, float magnitude)
    {
        direction.Normalize();
        //if (direction.y < 0) direction.y = -direction.y; // reflect down force on the ground
        force += direction.normalized * magnitude;
    }
    private void ApplyForces(){
        if (force.magnitude > 0.2) characterController.Move(force * Time.deltaTime);
        // consumes the impact energy each cycle:
        force = Vector3.Lerp(force, Vector3.zero, 5 * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject);
    }


    public void spawn_object(GameObject obj){
        object_to_spawn = obj;
    }


    [Command]
    public void spawn_object_on_network(Vector3 spawn_pos, Vector3 launch_direction, uint player_netId){
        
        GameObject fired_bullet = Instantiate(nm.spawnPrefabs[1]);
        fired_bullet.GetComponent<Projectile>().associated_player_net_id = player_netId;
        fired_bullet.SetActive(true);
        fired_bullet.transform.position = spawn_pos;

        // launch the bullet towards the raycast
        fired_bullet.GetComponent<Rigidbody>().AddForce(launch_direction * fired_bullet.GetComponent<Projectile>().speed, ForceMode.Impulse);

        //spawn the bullet on the server
        NetworkServer.Spawn(fired_bullet);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "projectile"){
            Debug.Log("success");
            if (other.GetComponent<Projectile>().associated_player_net_id != netId){
                int score_int;
                int.TryParse(gs.score_text.text, out score_int);
                gs.score_text.text = (score_int + 1).ToString();
            }
        }
    }
}