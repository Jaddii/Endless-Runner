using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // Referanse til spillerobjektet
    Player player;

    // Awake-metoden kalles før Start-metoden
    private void Awake()
    {
        // Henter referansen til spillerobjektet
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // FixedUpdate-metoden kalles med jevne mellomrom uavhengig av frame rate
    private void FixedUpdate()
    {
        // Oppdaterer hinderets posisjon basert på spillerens hastighet
        Vector2 pos = transform.position;

        pos.x -= player.velocity.x * Time.fixedDeltaTime;

        // Ødelegger hinderet hvis det er utenfor venstregrensen
        if (pos.x < -100)
        {
            Destroy(gameObject);
        }

        transform.position = pos;
    }
}

