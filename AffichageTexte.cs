using UnityEngine;
using UnityEngine.UI;

public class AffichageTexte : MonoBehaviour
{
    public Transform joueur;
    public GameObject texteUI;
    public float distanceAffichage = 3f; // La distance à partir de laquelle le texte s'affiche
    private bool texteVisible = false;

    void Start()
    {
        texteUI.SetActive(false);
    }

    void Update()
    {
        float distanceJoueur = Vector3.Distance(transform.position, joueur.position);

        if (distanceJoueur <= distanceAffichage && !texteVisible)
        {
            texteUI.SetActive(true);
            texteVisible = true;
        }
        else if (distanceJoueur > distanceAffichage && texteVisible)
        {
            texteUI.SetActive(false);
            texteVisible = false;
        }
    }
}
