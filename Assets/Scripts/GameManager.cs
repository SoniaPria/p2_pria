using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
   string api = "https://servizos.meteogalicia.gal/mgrss/observacion/jsonCamaras.action";

   ListaCamaras listaCamaras;
   public Camara camara;

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
                    //Debug.Log(pages[page] + ":\nReceived: " + wr.downloadHandler.text);

                    CreateFromJSON( wr.downloadHandler.text );
                break;
            }
        }
    }

    // JsonUtility.FromJson
    void CreateFromJSON( string jsonString )
    {
        listaCamaras = JsonUtility.FromJson<ListaCamaras>(jsonString);
        //camara = listaCamaras.listaCamaras;

        print("Nº de cámaras: " + listaCamaras.listaCamaras.Count);


        //PrintGetQuestion();
    }


    void PrintGetImageCamara()
    {
        //int ncc = camara.Count;
        // print("O número de cámaras é: " + ncc );

    }
}

