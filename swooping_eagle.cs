using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class swooping_eagle : Gun
{

    public GameObject ammo;
    public Transform bullet_spawn_point;

    protected override void Start()
    {
        base.Start();
        firing_cooldown = 1;
    }

    protected override void Update()
    {

        base.Update();
   
    }
    
    public override void Fire(int mouse_button){
        // if left click
        if (mouse_button == 0){
            shoot_primary_ammunition();
            ready_to_fire = false;
        }
    }

    private void shoot_primary_ammunition(){
        
        // raycast
        Vector3 target_point = Camera.main.ScreenToWorldPoint(reticle.transform.position);
        Ray ray = Camera.main.ScreenPointToRay(reticle.transform.position);
        Debug.Log(player.netId);
        player.spawn_object_on_network(bullet_spawn_point.position, ray.direction, player.netId);
    }
}
