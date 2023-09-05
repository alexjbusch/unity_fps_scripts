using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    public float firing_cooldown;
    private float firing_cooldown_timer;
    public bool ready_to_fire;
    public GameObject reticle;
    public string reticle_type;

    public fps_controller player;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        reticle = GameObject.FindGameObjectWithTag(reticle_type);
        player = transform.root.GetComponent<fps_controller>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!player.isLocalPlayer){
            return;
        }
        HandleCooldowns();
        HandleFiring();
        //transform.position = player.transform.position;
    }

    public abstract void Fire(int mouse_button);


    private void HandleFiring(){
        if (ready_to_fire)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Fire(0);
            }
            if (Input.GetKey(KeyCode.Mouse1))
            {
                Fire(1);
            }
        }
    }

    private void HandleCooldowns()
    {
        if (!ready_to_fire)
        {
            if (firing_cooldown_timer < firing_cooldown)
            {
                firing_cooldown_timer += Time.deltaTime;
            }
            else
            {
                firing_cooldown_timer = 0;
                ready_to_fire = true;
            }
        }
    }
}
