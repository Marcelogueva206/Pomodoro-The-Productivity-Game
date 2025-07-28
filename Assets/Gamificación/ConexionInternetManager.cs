using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;


public class ConexionInternetManager : MonoBehaviour
{
    public static ConexionInternetManager Instance;// Singleton instance
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional si lo quieres persistente
        }
        else
        {
            Destroy(gameObject);
        }
    }



    public static bool HayInternet()
    {
        try
        {
            using (var cliente = new WebClient())
            using (cliente.OpenRead("http://www.google.com"))
                return true;
        }
        catch
        {
            return false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
