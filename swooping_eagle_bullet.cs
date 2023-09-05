using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swooping_eagle_bullet : Projectile
{

    public float bullet_drop;
    public float seconds_to_despawn_after_collision;

    private Collider collider;

    protected override void Start()
    {
        base.Start();
        speed = 100;
        collider = GetComponent<Collider>();
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(Vector3.down*bullet_drop);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "wall"){
            Despawn();
        }
    }

    private void Despawn(){
        collider.enabled = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        StartCoroutine(Wait_and_then_self_destruct());
    }

    IEnumerator Wait_and_then_self_destruct(){

        yield return new WaitForSeconds(seconds_to_despawn_after_collision);
        DestroySelf();
    }
}
