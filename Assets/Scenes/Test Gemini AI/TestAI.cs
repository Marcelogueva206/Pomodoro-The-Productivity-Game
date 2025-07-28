//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;

//public class TestAI : MonoBehaviour
//{
//    //Dictionary<GameObject, List<Dialogo>> DialogosPorCharacter = new Dictionary<GameObject, List<Dialogo>>();




//    //private void Start()
//    //{
//    //    if(EstadisticasManager.Instance.getCaracteresMotivadoresEnSistema().Count > 0)
//    //    {
//    //        foreach (Dinosaurio characterMotivadorComponente in EstadisticasManager.Instance.getCaracteresMotivadoresEnSistema())
//    //        {
//    //            DialogosPorCharacter.Add(characterMotivadorComponente.gameObject, characterMotivadorComponente.dialogosPorDecir);
//    //        }
//    //    }

//    //}


//    [Header("IA")]
//    [SerializeField] private string gasURL;
//    [SerializeField] private string prompt;
//    [SerializeField] public string response;
//    public static TestAI Gemini;

//    private void Awake()
//    {
//        Gemini = this;
//        gasURL = "https://script.google.com/macros/s/AKfycbx4Vihh897w6xf06egbhC4xvgLXWeVi5w06fFj9ZZBzQ9sPStIMKy66bYnkBUuKdDXUPg/exec";
//    }
//    public IEnumerator UseGeminiAI(string input)
//    {
//        WWWForm form = new WWWForm();

//        form.AddField("parameter", input);

//        UnityWebRequest www = UnityWebRequest.Post(gasURL, form);

//        yield return www.SendWebRequest();
//        response = "";

//        if (www.result == UnityWebRequest.Result.Success)
//        {
//            response = www.downloadHandler.text;
//        }
//        else
//        {
//            response = "hubo un error";
//        }
//    }   


//}

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class GeminiRequest
{
    public List<Content> contents;
}
[System.Serializable]
public class Part
{
    public string text;
}

[System.Serializable]
public class Content
{
    public string role;
    public List<Part> parts;
}

[System.Serializable]
public class Candidate
{
    public Content content;
    public string finishReason;
    public float avgLogprobs;
}

[System.Serializable]
public class GeminiResponse
{
    public List<Candidate> candidates;
}

public class TestAI : MonoBehaviour
{
    [Header("Gemini")]
    [SerializeField] private string apiKey = "AIzaSyBBXofChVgKGlNQhawRyHs4_qn3okGQrq0";
    [SerializeField] public string response;

    public static TestAI Instance;

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator UseGeminiAI(string input)
    {
        string url = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-pro:generateContent?key=" + apiKey;

        // Crear el JSON con el mensaje del usuario
        GeminiRequest requestData = new GeminiRequest
        {
            contents = new List<Content> {
                new Content {
                    role = "user",
                    parts = new List<Part> {
                        new Part { text = input }
                    }
                }
            }
        };

        string jsonData = JsonConvert.SerializeObject(requestData);

        // Crear la solicitud HTTP
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Respuesta de Gemini: " + request.downloadHandler.text);
            var json = request.downloadHandler.text;
            response = JsonConvert.DeserializeObject<GeminiResponse>(json).candidates[0].content.parts[0].text;
        }
        else
        {
            Debug.LogError("Error al llamar a Gemini: " + request.error);
            response = "Error: " + request.error;
        }
    }
}
