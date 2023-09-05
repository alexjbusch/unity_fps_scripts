using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
    public float speed;
    public Rigidbody rb;
    public uint associated_player_net_id;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Server]
    public void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
