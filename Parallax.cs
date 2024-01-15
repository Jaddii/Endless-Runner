using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    // Dybdeparameter for parallakseffekt
    public float depth = 1;

    // Referanse til spillerobjektet
    Player player;

    // Awake-metoden kalles før Start-metoden
    private void Awake()
    {
        // Henter referansen til spillerobjektet
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // FixedUpdate-metoden kalles med jevne mellomrom uavhengig av frame rate
    void FixedUpdate()
    {
        // Beregner den faktiske horisontale hastigheten med hensyn til parallakseffekten
        float realVelocity = player.velocity.x / depth;

        // Oppdaterer posisjonen basert på den justerte hastigheten
        Vector2 pos = transform.position;
        pos.x -= realVelocity * Time.fixedDeltaTime;  

        // Flytter objektet til motsatt side hvis det går utenfor venstregrensen
        if (pos.x <= -25)
            pos.x = 80;

        transform.position = pos;
    }
}
