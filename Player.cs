using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Gravitasjonskonstant
    public float gravity;

    // Spillerens hastighet og tilhørende begrensninger
    public Vector2 velocity;
    public float maxXVelocity = 100;
    public float maxAcceleration = 10;
    public float acceleration = 10;

    // Distanse og hoppvariabler
    public float distance = 0;
    public float jumpVelocity = 20;
    public float groundHeight = 10;
    public bool isGrounded = false;

    // Variabler for hopp og timer
    public bool isHoldingJump = false;
    public float maxHoldJumpTime = 0.4f;
    public float maxMaxHoldJumpTime = 0.4f;
    public float holdJumpTimer = 0.0f;

    // Terskelverdi for å vurdere om spilleren kan hoppe når han er nær bakken
    public float jumpGroundThreshold = 1;

    // Statusvariabel for spillerens liv
    public bool isDead = false;

    // Lagmasker for bakken og hindre
    public LayerMask groundLayerMask;
    public LayerMask obstacleLayerMask;

    // Update-metoden kalles hver eneste frame
    void Update()
    {
        // Henter posisjonen til spilleren
        Vector2 pos = transform.position;
        float groundDistance = Mathf.Abs(pos.y - groundHeight);

        // Sjekker om spilleren er på bakken eller nær nok til å hoppe
        if (isGrounded || groundDistance <= jumpGroundThreshold)
        {
            // Sjekker om brukeren trykker på mellomromstasten for å hoppe
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isGrounded = false;
                velocity.y = jumpVelocity;
                isHoldingJump = true;
                holdJumpTimer = 0;
            }
        }

        // Sjekker om brukeren slipper mellomromstasten for å avslutte hoppet
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isHoldingJump = false;
        }
    }

    // FixedUpdate-metoden kalles med jevne mellomrom uavhengig av frame rate
    private void FixedUpdate()
    {
        // Henter posisjonen til spilleren
        Vector2 pos = transform.position;

        // Sjekker om spilleren er død
        if (isDead)
        {
            return;
        }

        // Setter spilleren som død hvis han faller for langt ned
        if (pos.y < -20)
        {
            isDead = true;
        }

        // Håndterer logikken når spilleren ikke er på bakken
        if (!isGrounded)
        {
            // Sjekker om spilleren holder inne hoppet for å justere høyden
            if (isHoldingJump)
            {
                holdJumpTimer += Time.fixedDeltaTime;
                if (holdJumpTimer >= maxHoldJumpTime)
                {
                    isHoldingJump = false;
                }
            }

            // Oppdaterer vertikal posisjon basert på hastigheten
            pos.y += velocity.y * Time.fixedDeltaTime;

            // Beregner gravitasjonseffekten hvis ikke spilleren holder hoppet
            if (!isHoldingJump)
            {
                velocity.y += gravity * Time.fixedDeltaTime;
            }

            // Sjekker for kollisjon med bakken
            Vector2 rayOrigin = new Vector2(pos.x + 0.7f, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = velocity.y * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, groundLayerMask);
            if (hit2D.collider != null)
            {
                Ground ground = hit2D.collider.GetComponent<Ground>();
                if (ground != null)
                {
                    if (pos.y >= ground.groundHeight)
                    {
                        groundHeight = ground.groundHeight;
                        pos.y = groundHeight;
                        velocity.y = 0;
                        isGrounded = true;
                    }
                }
            }
            Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.red);

            // Sjekker for kollisjon med vegger for å begrense horisontal bevegelse
            Vector2 wallOrigin = new Vector2(pos.x, pos.y);
            RaycastHit2D wallHit = Physics2D.Raycast(wallOrigin, Vector2.right, velocity.x * Time.fixedDeltaTime, groundLayerMask);
            if (wallHit.collider != null)
            {
                Ground ground = wallHit.collider.GetComponent<Ground>();
                if (ground != null)
                {
                    if (pos.y < ground.groundHeight)
                    {
                        velocity.x = 0;
                    }
                }
            }
        }

        // Oppdaterer distansen som spilleren har beveget seg
        distance += velocity.x * Time.fixedDeltaTime;

        // Behandler logikk når spilleren er på bakken
        if (isGrounded)
        {
            // Justerer akselerasjon og hoppetid basert på horisontal hastighet
            float velocityRatio = velocity.x / maxXVelocity;
            acceleration = maxAcceleration * (1 - velocityRatio);
            maxHoldJumpTime = maxMaxHoldJumpTime * velocityRatio;

            // Øker horisontal hastighet og begrenser den til maksimal verdi
            velocity.x += acceleration * Time.fixedDeltaTime;
            if (velocity.x >= maxXVelocity)
            {
                velocity.x = maxXVelocity;
            }

            // Sjekker for kollisjon med luften over for å oppdatere isGrounded-statusen
            Vector2 rayOrigin = new Vector2(pos.x - 0.7f, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = velocity.y * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance);
            if (hit2D.collider == null)
            {
                isGrounded = false;
            }
            Debug.DrawRay(rayOrigin, rayDirection * rayDistance, Color.yellow);
        }

        // Sjekker for kollisjon med hindre i både x- og y-retning
        Vector2 obstOrigin = new Vector2(pos.x, pos.y);

        // Kollisjon i x-retning
        RaycastHit2D obstHitX = Physics2D.Raycast(obstOrigin, Vector2.right, velocity.x * Time.fixedDeltaTime, obstacleLayerMask);
        if (obstHitX.collider != null)
        {
            Obstacle obstacle = obstHitX.collider.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                hitObstacle(obstacle);
            }
        }

        // Kollisjon i y-retning
        RaycastHit2D obstHitY = Physics2D.Raycast(obstOrigin, Vector2.up, velocity.y * Time.fixedDeltaTime, obstacleLayerMask);
        if (obstHitY.collider != null)
        {
            Obstacle obstacle = obstHitY.collider.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                hitObstacle(obstacle);
            }
        }

        // Oppdaterer spillerens posisjon
        transform.position = pos;
    }

    // Metode for å håndtere kollisjon med hinder
    void hitObstacle(Obstacle obstacle)
    {
        Destroy(obstacle.gameObject);
        velocity.x *= 0.7f;
    }
}
