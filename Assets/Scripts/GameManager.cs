using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
   string api = "https://servizos.meteogalicia.gal/mgrss/observacion/jsonCamaras.action";

   //TrivialCs getResults;
   //public QuestionCs getQuestion;

   // Plantilla extraída de
   // Unity Docs | UnityWebRequest.Get
    void Awake()
    {
        // A correct website page.
        StartCoroutine( GetRequest( api ) );
    }

    IEnumerator GetRequest( string uri )
    {
        using ( UnityWebRequest wr = UnityWebRequest.Get(uri) )
        {
            // Request and wait for the desired page.
            yield return wr.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (wr.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + wr.error);
                break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + wr.error);
                break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + wr.downloadHandler.text);

                    //CreateFromJSON( wr.downloadHandler.text );
                break;
            }
        }
    }

/*
    // JsonUtility.FromJson
    void CreateFromJSON( string jsonString )
    {
        getResults = JsonUtility.FromJson<TrivialCs>(jsonString);
        getQuestion = getResults.results[0];

        PrintGetQuestion();
    }

    void PrintGetQuestion()
    {
        // Imprime na consola o número de preguntas solicitadas
        // e 4 datos da primeira pregunta

        int nqq = getResults.results.Count;

        print("O número de preguntas solicitadas é: " + nqq );

        print("Pregunta 1");
        print("Categoría " + getResults.results[0].category);
        print("Tipo " + getResults.results[0].type);
        print("Dificultad " + getResults.results[0].difficulty);
        print("Atención Pregunta! " + getResults.results[0].question);
    }
*/
}

