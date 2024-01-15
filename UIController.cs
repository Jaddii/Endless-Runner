using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    // Referanse til spillerobjektet
    Player player;

    // Tekstfelt for visning av distanse
    Text distancetext;

    // Referanser til tekst og resultatelementer
    GameObject results;
    Text FinalDistance;

    // Awake-metoden kalles før Start-metoden
    private void Awake()
    {
        // Henter referanser til spillerobjektet og UI-elementer
        player = GameObject.Find("Player").GetComponent<Player>();
        distancetext = GameObject.Find("DistanceText").GetComponent<Text>();

        FinalDistance = GameObject.Find("FinalDistance").GetComponent<Text>();
        results = GameObject.Find("Results");

        // Skjuler resultatelementet ved start
        results.SetActive(false);
    }

    // Update-metoden kalles hver eneste frame
    void Update()
    {
        // Oppdaterer tekstfeltet med distansen
        int distance = Mathf.FloorToInt(player.distance);
        distancetext.text = distance + "m";

        // Viser resultatelementet når spilleren er død
        if (player.isDead)
        {
            results.SetActive(true);
            FinalDistance.text = distance + "m";
        }
    }

    // Metode for å avslutte spillet og gå til menyen
    public void Quit()
    {
        SceneManager.LoadScene("Menu");
    }

    // Metode for å starte spillet på nytt
    public void Retry()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
