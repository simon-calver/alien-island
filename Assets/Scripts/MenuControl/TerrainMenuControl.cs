using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TerrainMenuControl : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{

    // Start menu button types
    public bool terrainButton;
    public bool playButton;

    // Game object for map generation
    public GameObject mapGenerator;

    // Text colours
    private Color mainColour = new Color(0.25f, 0.37f, 0.19f, 1);
    private Color highlightColour = new Color(0.27f, 0.18f, 0.18f, 1);
    private Color clickColour = new Color(0.27f, 0.18f, 0.18f, 1);

    void Start()
    {
        // Set the color to mainColour and font style to normal at start
        gameObject.GetComponent<Text>().color = mainColour;
        gameObject.GetComponent<Text>().fontStyle = FontStyle.Normal;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Make the text bold and change to highlightColour on mouse enter
        gameObject.GetComponent<Text>().color = highlightColour;
        gameObject.GetComponent<Text>().fontStyle = FontStyle.Bold;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Return to the original font style and colour on mouse exit
        gameObject.GetComponent<Text>().color = mainColour;
        gameObject.GetComponent<Text>().fontStyle = FontStyle.Normal;

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (terrainButton)
        {

            // Generate terrain and change text colour
            gameObject.GetComponent<Text>().color = clickColour;
            // Randomise dthv
            mapGenerator.GetComponent<MapGenerator>().RandomiseMap();
            mapGenerator.GetComponent<MapGenerator>().GenerateMap();

        }

        if (playButton)
        {
            // Save the terrain data
            WorldInfo saveData = new WorldInfo();
            saveData.heightOffset = mapGenerator.GetComponent<MapGenerator>().heightOffset;
            saveData.biomeOffset = mapGenerator.GetComponent<MapGenerator>().biomeOffset;

            // Save the initial position of the player
            saveData.playerPos = new Vector2(0f, 0f);

            // Save the data 
            DataSaver.saveData(saveData, "newWorldData");
            // Generate terrain and change text colour
            gameObject.GetComponent<Text>().color = clickColour;
            SceneManager.LoadScene("World");

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        // Return to original colour
        gameObject.GetComponent<Text>().color = mainColour;
    }

}
