using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestAI : MonoBehaviour
{
    [Range(0 ,1)]private float indiceDeInputs; //0 a 1: 0 es ningún comentario y 1 es siempre
    Dictionary<GameObject, List<Dialogo>> DialogosPorCharacter = new Dictionary<GameObject, List<Dialogo>>();
    [SerializeField] private List<GameObject> CharacterMotivadoresActivos;



    private void Start()
    {
        if(CharacterMotivadoresActivos.Count > 0)
        {
            foreach (GameObject characterMotivador in CharacterMotivadoresActivos)
            {
                DialogosPorCharacter.Add(characterMotivador, characterMotivador.GetComponent<Dinosaurio>().dialogosPorDecir);
            }
        }
     
    }
    private int cantidadDeInputsDiario;

    private static int cantidadDeOutputsConsumidos;

    [Header("IA")]
    [SerializeField] private string gasURL;
    [SerializeField] private string prompt;
    [SerializeField] public string response;
    public static TestAI Gemini;

    private void Awake()
    {
        Gemini = this;
        gasURL = "https://script.google.com/macros/s/AKfycbxPLRvXPhBvVRBkgZAXthwZgDToSxW0aqt1JhXoXwbu8va3u4ePFTsUhAcajwG3B7FGcA/exec";
    }
    public IEnumerator UseGeminiAI(string input)
    {
        cantidadDeOutputsConsumidos++;
        WWWForm form = new WWWForm();

        form.AddField("parameter", input);

        UnityWebRequest www = UnityWebRequest.Post(gasURL, form);

        yield return www.SendWebRequest();
        response = "";

        if (www.result == UnityWebRequest.Result.Success)
        {
            response = www.downloadHandler.text;
        }
        else
        {
            response = "hubo un error";
        }
    }   


}
