using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;

// PRIA.p2

// App en Unity que mostre unha imaxe dunha cámara aleatoria
// das ofrecidas pola API de MeteoGalicia.

// @author Sonia Álvarez

public class GameManager : MonoBehaviour
{
    const string API = "https://servizos.meteogalicia.gal/mgrss/observacion/jsonCamaras.action";

    // As 'Image' en Unity deben ser importadas como Sprite para que funcionen
    // Para aceptar texturas débese usar RawImage (Imaxe en bruto)
    public RawImage rawImgCamara;

    ListaCamaras getCamaras;
    Camara meteoCamara;
    bool loadImaxeCamara;

    void Awake()
    {
        // As solicitudes web son asíncronas
        // Iniciando unha corrrutina de petición de datos por GET
        StartCoroutine( GetRequest( API ) );
    }

    void Start()
    {
        loadImaxeCamara = false;
    }

    void Update()
    {
        // Múdase de cámara pulsando a barra espaciadora
        // SEN REFRESCAR a API
        // Sempre que xa se descargara e mostrara unha foto
        if ( Input.GetKeyDown(KeyCode.Space) && loadImaxeCamara )
        {
            loadImaxeCamara = false;
            SelectCameraRandomly();
            StartCoroutine( GetTexture (meteoCamara.imaxeCamara) );
        }
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
                string dhText = wr.downloadHandler.text;

                if ( wr.downloadHandler.isDone )
                {
                    GetMeteoCamaras( dhText );
                }

                // Dev
                // consoleMsg += "Received: " + dhText;
                // consoleMsg += "Load jsonCamaras " + wr.downloadHandler.isDone;
                // print( consoleMsg );
            }

            else {
                consoleMsg += "Error: " + wr.error;
                Debug.LogError( consoleMsg );
            }
        }
    }


    IEnumerator GetTexture( string uri )
    {
        // DownloadHandlerTexture subclase DownloadHandler especializada en
        // descargar imaxes para usarse coma obxetos Texture

        using ( UnityWebRequest wrt = UnityWebRequestTexture.GetTexture(uri) )
        {
            // Enviando a solicitude i esperando resposta
            yield return wrt.SendWebRequest();

            string consoleMsg = "GameManager.GetTexture() \n";

            if ( wrt.result.Equals( UnityWebRequest.Result.Success ) )
            {
                // Get downloaded asset bundle
                rawImgCamara.texture = DownloadHandlerTexture.GetContent( wrt );

                if ( rawImgCamara.texture != null )
                {
                    // Boleano de descarga de textura exitosa
                    loadImaxeCamara = true;

                    PrintDataMeteogaliciaJsonCamara( meteoCamara );

                    // Dev
                    // consoleMsg += $"Textura [{uri}] descargada con éxito";
                    // print( consoleMsg );
                }
            }

            else
            {
                consoleMsg += "Error: " + wrt.error;
                Debug.LogError( consoleMsg );
            }
        }
    }

    // Asigna o json actualizado a getCamaras (ListaCamaras)
    void GetMeteoCamaras ( string jsonString )
    {
        // De chegados aquí obténse un obxeto de tipo ListaCamaras
        // que é unha lista de obxetos de tipo Cámara
        getCamaras = JsonUtility.FromJson<ListaCamaras>(jsonString);

        string consoleMsg = "GameManager.GetMeteoCamaras() \n";

        if ( getCamaras.listaCamaras.Count > 0 )
        {
            SelectCameraRandomly();
            StartCoroutine( GetTexture (meteoCamara.imaxeCamara) );
        }

        else {
            consoleMsg += "Algo fallou na serialización. Lista baleira";
            Debug.Log( consoleMsg );
        }
    }


    void SelectCameraRandomly()
    {
        int ncc = getCamaras.listaCamaras.Count;

        // Random' is an ambiguous reference between
        // 'UnityEngine.Random' and 'System.Random
        // int rdmCamara = Random.Range( 0, ncc );
        int rdmCamara = UnityEngine.Random.Range( 0, ncc );

        // Establecento a cámara global para esta petición
        meteoCamara = getCamaras.listaCamaras[rdmCamara];

        // print Dev
        // string consoleMsg = "GameManager.SelectCameraRandomly() \n";
        // consoleMsg += $"Recibidos datos de {ncc} cámaras de MeteoGalicia\n";
        // consoleMsg += $"Selecciónase a cámara {meteoCamara.nomeCamara} \n";
        // consoleMsg += $"Data da última foto: {meteoCamara.dataUltimaAct} \n";
        // consoleMsg += $"Url da foto: {meteoCamara.imaxeCamara} \n";
        // print( consoleMsg );
    }

    void PrintDataMeteogaliciaJsonCamara( Camara camara )
    {
        string consoleMsg = $"MeteoGalicia. Cámara {meteoCamara.nomeCamara} \n";
        consoleMsg += $"Concello {meteoCamara.concello} \n";
        consoleMsg += $"Provincia {meteoCamara.provincia} \n";
        consoleMsg += $"Latitude {meteoCamara.lat} \n";
        consoleMsg += $"Lonxitude {meteoCamara.lon} \n";
        consoleMsg += $"DataTime da imaxe {meteoCamara.dataUltimaAct} \n";
        consoleMsg += "\n --- --- --- --- --- --- --- --- --- --- --- \n";

        print( consoleMsg );
    }
}
