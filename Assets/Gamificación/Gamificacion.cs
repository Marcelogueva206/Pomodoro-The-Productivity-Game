using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Gamificacion : MonoBehaviour
{
    [SerializeField]private GameObject Limite1;
    [SerializeField]private GameObject Limite2;
    [SerializeField]public float ProductivityPointsTotal = 0;
    [SerializeField]public float ProductivityPointsDaily = 0;
    public static Gamificacion gamificacionManager;
    [SerializeField] private TextMeshProUGUI textoProductivityPointsTotal;
    [SerializeField] private TextMeshProUGUI textoProductivityPointsDaily;
    public static float MinY { get => Mathf.Min(gamificacionManager.Limite1.transform.position.y, gamificacionManager.Limite2.transform.position.y); }
    public static float MaxY { get => Mathf.Max(gamificacionManager.Limite1.transform.position.y, gamificacionManager.Limite2.transform.position.y); }
    public static float MaxX { get => Mathf.Max(gamificacionManager.Limite1.transform.position.x, gamificacionManager.Limite2.transform.position.x); }
    public static float MinX { get => Mathf.Min(gamificacionManager.Limite1.transform.position.x, gamificacionManager.Limite2.transform.position.x); }

    // Start is called before the first frame update
    void Start()
    {
        gamificacionManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void MostrarProductivityPoints()
    //{
    //    textoProductivityPointsDaily.text = ProductivityPointsTotal.ToString();
    //}

}
