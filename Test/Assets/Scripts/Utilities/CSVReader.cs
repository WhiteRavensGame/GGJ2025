using UnityEngine;
using System.IO;
using System.Text;

public class CSVReader : MonoBehaviour
{
    public GameObject deathSpot;
    public int levelToCheck = 2;

    void Start()
    {
        if (!GameManager.Instance.displayDeathSpots)
            Destroy(this.gameObject);
        else
        {
            //string filePath = Path.Combine(Application.dataPath, "ExampleHeatmap_Points.csv");
            string filePath = Path.Combine(Application.dataPath, "DeathPoints.csv");
            ReadCSV(filePath);
        }
    }

    void ReadCSV(string filePath)
    {
        try
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Debug.Log(line);
                    string[] values = line.Split(',');
                    float x = float.Parse(values[0]);
                    float y = float.Parse(values[1]);
                    int level = int.Parse(values[2]);

                    if(level == levelToCheck)
                    {
                        Instantiate(deathSpot, new Vector3(x, y, 0), Quaternion.identity);

                        // Process the coordinates (e.g., store them, visualize them, etc.)
                        Debug.Log("Player death at coordinates: " + x + ", " + y);
                    }
                    
                }
            }
        }
        catch (IOException e)
        {
            Debug.Log("The file could not be read:");
            Debug.Log(e.Message);
        }
    }
}