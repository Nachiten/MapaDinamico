using UnityEngine;
using UnityEngine.UI;

public class ReadImage : MonoBehaviour
{
    [SerializeField]
    private Texture2D[] images;
    private Texture2D image;

    [SerializeField] 
    private GameObject wallObject;

    [SerializeField] 
    private GameObject groundObject;
    private void Start()
    {
        image = images[Random.Range(0, images.Length)]; 
        Color[] pix = image.GetPixels();

        int worldX = image.width; 
        int worldY = image.height;
        Vector3[] spawnPositions = new Vector3[pix.Length]; 

        Vector3 startingSpawnPosition = new Vector3(-Mathf.Round(worldX / 2), 0, -Mathf.Round(worldY / 2)); 
        Vector3 currentSpawnPos = startingSpawnPosition;

        int counter = 0;
        for (int y = 0; y < worldY; y++)
        {
            for (int x = 0; x < worldX; x++)
            {
                spawnPositions[counter] = currentSpawnPos; 
                counter++; 
                currentSpawnPos.x++;
            }
            currentSpawnPos.x = startingSpawnPosition.x; 
            currentSpawnPos.y++;
        }

        counter = 0;

        foreach (Vector3 pos in spawnPositions)
        {
            Color c = pix[counter];

            //GameObject objetoAInstanciar = null;

            if (c.Equals(Color.white))
            {
                Instantiate(wallObject, pos, Quaternion.identity);
            }
            //else if (c.Equals(Color.black))
            //{
            //    Instantiate(groundObject, pos, Quaternion.identity);
            //}

            counter++;
        }
    } 
}
