using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;

// App en Unity que mostre unha imaxe dunha cámara aleatoria
// das ofrecidas pola API de MeteoGalicia.

public class GameManager : MonoBehaviour
{
    const string API = "https://servizos.meteogalicia.gal/mgrss/observacion/jsonCamaras.action";

    // As 'Image' deben ser importadas como Sprite para que funcionen
    // Para aceptar texturas débese usar RawImage (Imaxe en bruto)
    public RawImage rawImgCamara;

    ListaCamaras getCamaras;
    Camara meteoCamara;
    bool issetImaxeCamara, loadImaxeCamara;

    void Awake()
    {
        // As solicitudes web son asíncronas
        // Iniciando unha corrrutina de petición de datos por GET
        StartCoroutine( GetRequest( API ) );
    }

    void Start()
    {
        issetImaxeCamara = false;
        loadImaxeCamara = false;
    }

    void Update()
    {
        if ( issetImaxeCamara )
        {
            // O Player pode cambiar de cámara pulsando unha tecla
            // Sempre que xa foxe cargada e mostrada a imaxe anterior
            if ( Input.GetKeyDown(KeyCode.Space) && loadImaxeCamara )
            {
                loadImaxeCamara = false;
                dameCamara();

                StartCoroutine( GetTexture (meteoCamara.imaxeCamara) );
                // StartCoroutine( GetRequest( API ) );
            }
        }
    }

    void dameCamara()
    {
                print("Dame outra cámara \n");
        int ncc = getCamaras.listaCamaras.Count;

        // Random' is an ambiguous reference between
        // 'UnityEngine.Random' and 'System.Random
        // int rdmCamara = Random.Range( 0, ncc );
        int rdmCamara = UnityEngine.Random.Range( 0, ncc );

        // Establecento a cámara global para esta tarefa
        meteoCamara = getCamaras.listaCamaras[rdmCamara];
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
                    GetMeteoCamara( dhText );
                }

                // Dev
                // consoleMsg += "Received: " + dhText;
                // print( consoleMsg );

                consoleMsg += "Load json " + wr.downloadHandler.isDone;
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
        // descargar imaxes para usalas coma obxetos Texture

        using ( UnityWebRequest wrt = UnityWebRequestTexture.GetTexture(uri) )
        {
            // Enviando a solicitude i esperando resposta
            yield return wrt.SendWebRequest();

            string consoleMsg = "GameManager.GetTexture() \n";
            consoleMsg += $"DataTime foto : {meteoCamara.dataUltimaAct} \n";

            if ( wrt.result.Equals( UnityWebRequest.Result.Success ) )
            {
                // Get downloaded asset bundle
                rawImgCamara.texture = DownloadHandlerTexture.GetContent( wrt );

                if ( rawImgCamara.texture != null )
                {
                    // Boleano de descarga de textura exitosa
                    loadImaxeCamara = true;
                    // Dev
                    consoleMsg += "Textura descargada con éxito";
                    print( consoleMsg );
                }
            }

            else
            {
                consoleMsg += "Error: " + wrt.error;
                Debug.LogError( consoleMsg );
            }
        }
    }

    void GetMeteoCamara ( string jsonString )
    {
        // De chegados aquí obténse un obxeto de tipo ListaCamaras
        // que é unha lista de obxetos de tipo Cámara
        getCamaras = JsonUtility.FromJson<ListaCamaras>(jsonString);

        string consoleMsg = "GameManager.GetMeteoCamaras() \n";

        if ( getCamaras.listaCamaras.Count > 0 )
        {
            SelectCameraRandomly();

            // Se o json contén un value ca url da imaxe a texturizar
            if ( issetImaxeCamara )
            {
                // Iníciase unha sub-corrutina
                StartCoroutine( GetTexture (meteoCamara.imaxeCamara) );
            }
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

        // Establecento a cámara global para esta tarefa
        meteoCamara = getCamaras.listaCamaras[rdmCamara];

        // Buscando o dato da url da imaxe a mostrar
        // string imaxeCamara = meteoCamara.imaxeCamara;

        // Dev
        string consoleMsg = "GameManager.SelectCameraRandomly() \n";
        consoleMsg += $"Recibidos datos de {ncc} cámaras de MeteoGalicia\n";
        consoleMsg += $"Selecciónase a cámara do Concello de {meteoCamara.concello} \n";
        consoleMsg += $"Data da última foto: {meteoCamara.dataUltimaAct} \n";

        // Boleano global existe value en key imaxeCamara
        if ( meteoCamara.imaxeCamara != null )
        {
            issetImaxeCamara = true;
            consoleMsg += $"Url da foto: {meteoCamara.imaxeCamara} \n";
        }
        print( consoleMsg );
    }
}
