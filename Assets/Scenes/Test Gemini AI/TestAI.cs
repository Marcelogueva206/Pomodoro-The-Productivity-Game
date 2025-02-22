using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestAI : MonoBehaviour
{
    //Dictionary<GameObject, List<Dialogo>> DialogosPorCharacter = new Dictionary<GameObject, List<Dialogo>>();
   



    //private void Start()
    //{
    //    if(EstadisticasManager.Instance.getCaracteresMotivadoresEnSistema().Count > 0)
    //    {
    //        foreach (Dinosaurio characterMotivadorComponente in EstadisticasManager.Instance.getCaracteresMotivadoresEnSistema())
    //        {
    //            DialogosPorCharacter.Add(characterMotivadorComponente.gameObject, characterMotivadorComponente.dialogosPorDecir);
    //        }
    //    }
     
    //}


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
