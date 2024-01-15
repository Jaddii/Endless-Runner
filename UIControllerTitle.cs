using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControllerTitle : MonoBehaviour
{
    // Metode for å starte spillet
    public void play()
    {
        // Laster inn scenen "SampleScene"
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}
