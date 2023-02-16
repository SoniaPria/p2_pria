using System;

// JsonUtility .FromJson :
// Debe ser una clase/estructura simple
// marcada con el atributo Serializable

[Serializable]
public class Camara
{
    public string concello;
    public string dataUltimaAct;
    public int idConcello;
    public int identificador;
    public string imaxeCamara;
    public string imaxeCamaraMini;
    public float lat;
    public float lon;
    public string nomeCamara;
    public string provincia;
}
