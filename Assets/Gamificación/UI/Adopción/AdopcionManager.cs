using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdopcionManager : MonoBehaviour
{
    // Instancia única del Singleton
    public static AdopcionManager Instance { get; private set; }

    private Especie adoptadoEspecie;
    private Rareza adoptadoRareza;

    public GameObject MotivadorPrefab;

    void Awake()
    {
        // Verificar si ya existe una instancia
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destruir esta instancia si ya existe otra
            return;
        }

        // Asignar esta instancia y evitar que se destruya al cambiar de escena
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Métodos y lógica específica del AdopcionManager


    public Especie GetRandomEspecie()
    {
        // Define rarity probabilities (must sum to 100%)
        var spaceChances = new[]
        {
            new { Specie = Especie.Stegosaurus, Probability = 40 },
            new { Specie = Especie.Velociraptor, Probability = 20 },
            new { Specie = Especie.Triceratops, Probability = 30 },
            new { Specie = Especie.TiranosaurioRex, Probability = 10 }
        };

        // Generate a random number between 0 and 99
        int randomNumber = UnityEngine.Random.Range(0, 100);

        int cumulativeProbability = 0;

        foreach (var rarityChance in spaceChances)
        {
            cumulativeProbability += rarityChance.Probability;
            if (randomNumber < cumulativeProbability)
            {
                return rarityChance.Specie;
            }
        }

        throw new InvalidOperationException("Probabilities do not sum to 100%.");
    }
    public Rareza GetRandomRareza()
    {
        // Define rarity probabilities (must sum to 100%)
        var rarityChances = new[]
        {
            new { Rarity = Rareza.Comun, Probability = 50 },
            new { Rarity = Rareza.Rara, Probability = 30 },
            new { Rarity = Rareza.SuperRara, Probability = 15 },
            new { Rarity = Rareza.Legendaria, Probability = 5 }
        };

        // Generate a random number between 0 and 99
        int randomNumber = UnityEngine.Random.Range(0, 100);

        int cumulativeProbability = 0;

        foreach (var rarityChance in rarityChances)
        {
            cumulativeProbability += rarityChance.Probability;
            if (randomNumber < cumulativeProbability)
            {
                return rarityChance.Rarity;
            }
        }

        throw new InvalidOperationException("Probabilities do not sum to 100%.");
    }



    public void ProcesoAdoptarBotón()
    {
        adoptadoEspecie = GetRandomEspecie();
        adoptadoRareza = GetRandomRareza();
        VentanaAdopcion.Instance.MostrarVentanaAdopcion(adoptadoEspecie,adoptadoRareza);
    }




    public void CrearMotivador( string nombre)
    {
        Debug.Log("xdxd");
        GameObject motivadorNuevo = Instantiate(MotivadorPrefab);
        motivadorNuevo.GetComponent<Dinosaurio>().SerAdoptado(adoptadoEspecie, adoptadoRareza, nombre);
        motivadorNuevo.transform.position = new Vector3(0, 0, 0);
    }

}

