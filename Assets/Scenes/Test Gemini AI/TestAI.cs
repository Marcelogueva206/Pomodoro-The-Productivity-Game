using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestAI : MonoBehaviour
{
    [SerializeField] private string gasURL;
    [SerializeField] private string prompt;

    private IEnumerator sendDataToGas()
    {
        WWWForm form = new WWWForm();

        form.AddField("parameter",prompt);

        UnityWebRequest www = UnityWebRequest.Post(gasURL,form);    

        yield return www.SendWebRequest();
        string response = "";

        if(www.result == UnityWebRequest.Result.Success)
        {
            response = www.downloadHandler.text;
        }
        else
        {

            response = "hubo un error";
        }
        Debug.Log(response);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(sendDataToGas());
        }
    }

}
