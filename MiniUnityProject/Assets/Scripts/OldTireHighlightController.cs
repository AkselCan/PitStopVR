// C#
// File: OldTireHighlightController.cs (VERSIONE AGGIORNATA)

using UnityEngine;

public class OldTireHighlightController : MonoBehaviour
{
    // The Renderer component, which is on the Parent of this GameObject
    private Renderer tireRenderer;

    // Store the original material color
    private Color originalColor;

    // The color to use when the tire is highlighted
    [SerializeField]
    private Color highlightColor = Color.yellow; 

    // Unity's built-in initialization function
    void Awake()
    {
        // MODIFICA CHIAVE: Cerca il Renderer sul GameObject attuale o sui suoi genitori
        tireRenderer = GetComponentInParent<Renderer>(); 
        
        // Save the current color as the original color
        if (tireRenderer != null)
        {
            // NOTA: Usiamo .material per ottenere una copia modificabile.
            originalColor = tireRenderer.material.color;
        }
        else
        {
            // Messaggio d'errore per debugging se non trova il Renderer
            Debug.LogError("OldTireHighlightController on " + gameObject.name + 
                           " cannot find a Renderer component on itself or any parent object.");
        }
    }

    // Called when the "laser" starts pointing at the tire (Hover Enter)
    public void HighlightTire()
    {
        if (tireRenderer != null)
        {
            // Set the material color to the highlight color
            tireRenderer.material.color = highlightColor; 
        }
    }

    // Called when the "laser" stops pointing at the tire (Hover Exit)
    public void UnhighlightTire()
    {
        if (tireRenderer != null)
        {
            // Restore the material to the original color
            tireRenderer.material.color = originalColor;
        }
    }
}