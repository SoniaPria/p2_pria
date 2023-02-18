using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

// App en Unity que mostre unha imaxe dunha cámara aleatoria
// das ofrecidas pola API de MeteoGalicia.

public class GameManager : MonoBehaviour
{
    const string API = "https://servizos.meteogalicia.gal/mgrss/observacion/jsonCamaras.action";

    ListaCamaras getCamaras;
    Camara meteoCamara;

    void Awake()
    {
        // Iniciando unha corrrutina de petición de datos por GET
        StartCoroutine( GetRequest( API ) );
    }

    // Corrutina baseada no ex. de UnityWebRequest.Get
    // que devolve un obxecto de datos de uri

    IEnumerator GetRequest( string uri )
    {
        using ( UnityWebRequest wr = UnityWebRequest.Get( uri ) )
        {
            // Enviando a solicitude i esperando resposta
            yield return wr.SendWebRequest();

            string consoleMsg = "GameManager.GetRequest() \n";

            if ( wr.result.Equals( UnityWebRequest.Result.Success ) )
            {
                // Descargando datos co método DownloadHandler
                string jsonString = wr.downloadHandler.text;

                // consoleMsg += "Received: " + jsonString;
                // print( consoleMsg );

                // De ter éxito devolve un obxeto de tipo ListaCamaras
                // que é unha lista de obxetos de tipo Cámara
                getCamaras = JsonUtility.FromJson<ListaCamaras>(jsonString);

                if ( getCamaras.listaCamaras.Count > 0 )
                {
                    SelectCameraRandomly();
                }
                else {
                    consoleMsg += "Algo fallou na serialización";
                    Debug.Log( consoleMsg );
                }
            }

            else {
                consoleMsg += "Error: " + wr.error;
                Debug.LogError( consoleMsg );
            }
        }
    }

    void SelectCameraRandomly()
    {
        int ncc = getCamaras.listaCamaras.Count;

        // Random' is an ambiguous reference between
        // 'UnityEngine.Random' and 'System.Random
        // int rdmCamara = Random.Range( 0, ncc );
        int rdmCamara = UnityEngine.Random.Range( 0, ncc );

        // Establecento a cámara global para esta tarefa
        meteoCamara = getCamaras.listaCamaras[rdmCamara];

        // Dev
        string consoleMsg = "GameManager.SelectCameraRandomly() \n";
        consoleMsg += $"Recibidos datos de {ncc} cámaras de MeteoGalicia\n";
        consoleMsg += $"Selecciónase a cámara do Concello de {meteoCamara.concello} \n";
        print( consoleMsg );
    }
}
