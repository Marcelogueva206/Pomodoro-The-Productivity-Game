using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestAI : MonoBehaviour
{
    [Range(0 ,1)]private float indiceDeInputs; //0 a 1: 0 es ningún comentario y 1 es siempre
    static Dictionary<Dinosaurio, List<Dialogo>> DialogosPorCharacter = new Dictionary<Dinosaurio, List<Dialogo>>();
    //[SerializeField] private List<GameObject> CharacterMotivadoresActivos;



    private void Start()
    {
        //if(CharacterMotivadoresActivos.Count > 0)
        //{
        //    foreach (Dinosaurio characterMotivador in CharacterMotivadoresActivos)
        //    {
        //        DialogosPorCharacter.Add(characterMotivador, characterMotivador.GetComponent<Dinosaurio>().dialogosPorDecir);
        //    }
        //}
     
    }

    private void Update()
    {
        if( Input.GetKeyDown(KeyCode.Space) )
        {
            //seleciona la lista
            foreach (KeyValuePair<Dinosaurio, List<Dialogo>> entry in DialogosPorCharacter)
            {
                //elige los componentes
                Dinosaurio character = entry.Key;
                List<Dialogo> dialogos = entry.Value;

                Debug.Log($"Diálogos para {character.name}:");
                // imprime al priemr componente
                foreach (Dialogo dialogo in dialogos)
                {
                    //imprime todos sus sub componentes
                    Debug.Log($"cantidad de id validadas: {dialogo.idValidadas.Count}");
                }

      ; // Línea en blanco para separar personajes
            }

        }
  
    }

    public IEnumerator FiltrarDialogos(Dinosaurio emisor)
    {
        //absrove los dialogos de cada
        DialogosPorCharacter.Clear();
        DialogosPorCharacter.Add(emisor, emisor.dialogosPorDecir);

        //se espera para analisar todos los posibles
        yield return new WaitForSeconds(1);
        //quedar con una sola id validada

        foreach (KeyValuePair<Dinosaurio, List<Dialogo>> entry in DialogosPorCharacter)
        {
            Dinosaurio character = entry.Key;
            List<Dialogo> dialogos = entry.Value;

            Debug.Log($"Diálogos para {character.name}:");

            foreach (Dialogo dialogo in dialogos)
            {
                Debug.Log($"cantidad de id validadas: {dialogo.idValidadas.Count}");
            }

      ; // Línea en blanco para separar personajes
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
