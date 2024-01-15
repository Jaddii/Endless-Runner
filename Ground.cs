using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    // Referanse til spillerobjektet
    Player player;

    // Variabler for bakkenes høyde og posisjon
    public float groundHeight;
    public float groundRight;
    public float screenRight;
    BoxCollider2D collider;

    // Variabel for å sjekke om bakken allerede er generert
    bool didGenerateGround = false;

    // Mal for hinderobjekt
    public Obstacle boxTemplate; 

    // Awake-metoden kalles før Start-metoden
    private void Awake()
    {
        // Henter referansen til spillerobjektet og kollideren
        player = GameObject.Find("Player").GetComponent<Player>();
        collider = GetComponent<BoxCollider2D>();

        // Beregner høyden og høyre posisjon av bakken i forhold til kollisjonsboksen
        groundHeight = transform.position.y + (collider.size.y / 2);
        screenRight = Camera.main.transform.position.x * 2;
    }

    // FixedUpdate-metoden kalles med jevne mellomrom uavhengig av frame rate
    private void FixedUpdate()
    {
        // Oppdaterer bakken basert på spillerens hastighet
        Vector2 pos = transform.position;
        pos.x -= player.velocity.x * Time.fixedDeltaTime;

        groundRight = transform.position.x + (collider.size.x / 2);

        // Ødelegger bakken hvis den er utenfor skjermen
        if (groundRight < 0)
        {
            Destroy(gameObject);
            return;
        }

        // Genererer ny bakke hvis visse betingelser er oppfylt
        if (!didGenerateGround)
        {
            if (groundRight < screenRight)
            {
                didGenerateGround = true;
                generateGround();
            }
        }

        transform.position = pos;
    }

    // Metode for å generere ny bakke
    void generateGround()
    {
        // Genererer ny bakkeobjekt og plasserer det tilfeldig på skjermen
        GameObject go = Instantiate(gameObject);
        BoxCollider2D goCollider = go.GetComponent<BoxCollider2D>();
        Vector2 pos;

        // Beregner høyde og bredde av hinderne basert på spillerens hopp og plasserer dem tilfeldig på bakken (Fikk hjelp via forum og youtube)
        float h1 = player.jumpVelocity * player.maxHoldJumpTime;
        float t = player.jumpVelocity / -player.gravity;
        float h2 = player.jumpVelocity * t + (0.5f * (player.gravity * (t * t)));
        float maxJumpHeight = h1 + h2;
        float maxY = maxJumpHeight * 0.6f;
        maxY += groundHeight;
        float minY = 1;
        float actualY = Random.Range(minY, maxY);

        pos.y = actualY - goCollider.size.y / 2;
        if (pos.y > 2.7)
            pos.y = 2.7f;

        float t1 = t + player.maxHoldJumpTime;
        float t2 = Mathf.Sqrt((2.0f * (maxY - actualY)) / -player.gravity);
        float totalTime = t1 + t2;
        float maxX = totalTime * player.velocity.x;
        maxX *= 0.7f;
        maxX += groundRight;
        float minX = screenRight + 5;
        float actualX = Random.Range(minX, maxX);

        pos.x = actualX + goCollider.size.x / 2;
        go.transform.position = pos;

        // Oppdaterer bakkenes egenskaper
        Ground goGround = go.GetComponent<Ground>();
        goGround.groundHeight = go.transform.position.y + (goCollider.size.y / 2);

        // Genererer tilfeldig antall hindere og plasserer dem på bakken
        int obstacleNum = Random.Range(0, 4);
        for (int i = 0; i < obstacleNum; i++)
        {
            GameObject box = Instantiate(boxTemplate.gameObject);
            float y = goGround.groundHeight;
            float halfWidth = goCollider.size.x / 2 - 1;
            float left = go.transform.position.x - halfWidth;
            float right = go.transform.position.x + halfWidth;
            float x = Random.Range(left, right);
            Vector2 boxPos = new Vector2(x, y);
            box.transform.position = boxPos;
        }
    }
}
