using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    [Range(0.05f, 100f)] // Crea un slider en el Inspector
    public float timeScale = 1f;

    void Update()
    {
        Time.timeScale = timeScale; // Ajusta la escala de tiempo
    }
}
