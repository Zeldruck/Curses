using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap,
        ColorMap,
        GradientNoise
    };

    [Header("Drawing")]
    [SerializeField] private Renderer textureRenderer;
    public DrawMode drawMode = DrawMode.NoiseMap;
    public bool autoUpdate = false;

    [Header("Start")]
    [SerializeField] private Renderer textureRendererStarter;
    public Vector2 start;
    public Vector2 end;
    public Tilemap tilemap;
    public Tile tile;
    private Vector3Int previous;

    [Header("Noise")]
    [SerializeField] private float mapWidth;
    [SerializeField] private float mapHeight;
    [SerializeField] private float noiseScale;
    [Space]
    [SerializeField] private int octaves;
    [SerializeField] [Range(0, 1)] private float persistance;
    [SerializeField] private float lacunarity;
    [Space]
    [SerializeField] private int seed;
    public Vector2 offset;

    [Header("Gradient Noise")]
    [SerializeField] private bool applyGradientNoise = false;
    [SerializeField] [Range(0f, 10f)] private float gradientA;
    [SerializeField] [Range(0f, 10f)] private float gradientB;

    [Header("Terrain")]
    public TerrainType[] regions;

    private bool goodCenter = false;
    private bool isGenerating = false;
    private bool isMapGenerated = false;

    private float[,] gradientNoise;
    private float[,] noiseMap;

    private float[] colorPercent;
    private Color[] colorMap;

    private float timer = 0f;
    private float timerAllGenerated = 0f;

    private void Start()
    {
        tilemap.transform.position = new Vector3(-((mapWidth - 1) / 4f), -((mapHeight - 1) / 4f), 0f);
        tilemap.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        gradientNoise = Noise.GenerateNoiseGradientMap((int)mapWidth, (int)mapHeight, gradientA, gradientB);

        seed = Random.Range(0, 100000);
        offset.x = Random.Range(0f, 1000f);
        offset.y = Random.Range(0f, 1000f);
    }

    private void Update()
    {
        if (!isMapGenerated && !isGenerating && !goodCenter)
        {
            if (timer > 20f)
            {
                timer = 0f;

                seed = Random.Range(0, 100000);
                offset.x = Random.Range(0f, 1000f);
                offset.y = Random.Range(0f, 1000f);
            }

            GenerateMap();
        }

        if (!isMapGenerated)
        {
            timer += Time.deltaTime;
            timerAllGenerated += Time.deltaTime;
        }
    }

    public void GenerateMap()
    {
        /*if (drawMode == DrawMode.NoiseMap)
        {
            float[,] noiseMap = Noise.GenerateNoiseMap((int)mapWidth, (int)mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

            DrawNoiseMap(noiseMap);
        }
        else if (drawMode == DrawMode.GradientNoise)
        {
            float[,] noiseMap = Noise.GenerateNoiseMap((int)mapWidth, (int)mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

            float[,] gradientNoise = Noise.GenerateNoiseGradientMap((int)mapWidth, (int)mapHeight, gradientA, gradientB);

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float substractedValue = noiseMap[x, y] - gradientNoise[x, y];
                    noiseMap[x, y] = Mathf.Clamp01(substractedValue);
                }
            }

            DrawNoiseMap(noiseMap);
        }*/
        /*if (drawMode == DrawMode.ColorMap)
        {}*/

        isGenerating = true;

        noiseMap = Noise.GenerateNoiseMap((int)mapWidth, (int)mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset, start, end, out goodCenter);

        if (!goodCenter)
        {
            seed++;
        }
        else
        {
            colorMap = CalculateColorMap(noiseMap, out colorPercent);

            float numberP = 0f;

            for (int i = 0; i < regions.Length; i++)
            {
                numberP += colorPercent[i];
            }

            for (int i = 0; i < regions.Length; i++)
            {
                regions[i].percentage = (colorPercent[i] / numberP) * 100f;
            }

            DrawNoiseMap(colorMap);

            for (int y = 0; y < mapHeight; y++)
            {
                //if (y % 2 > 0)
                    //continue;

                for (int x = 0; x < mapWidth; x++)
                {
                    //if (x % 2 > 0)
                        //continue;

                    float currentHeight = noiseMap[x, y];

                    for (int i = 0; i < regions.Length; i++)
                    {
                        if (currentHeight <= regions[i].height)
                        {
                            Vector3Int currentPos = tilemap.LocalToCell(new Vector3(x * 4, y * 4, textureRenderer.transform.position.z));
                            tilemap.SetTile(currentPos, regions[i].tileStr.tiles[0]);
                            break;
                        }
                    }
                }
            }

            isMapGenerated = true;
            Debug.LogWarning("Temps de génération: " + timerAllGenerated + " sec");

        }
        
        isGenerating = false;
    }

    public Color[] CalculateColorMap(float[,] noiseMap, out float[] colorPercent)
    {
        Color[] colorMap = new Color[(int)mapWidth * (int)mapHeight];

        float[] colorPercentage = new float[regions.Length];

        for (int y = 0; y < mapHeight; y++)
        {
            //if (y % 2 > 0)
                //continue;

            for (int x = 0; x < mapWidth; x++)
            {
                //if (x % 2 > 0)
                    //continue;

                if (applyGradientNoise)
                { 
                    float substractedValue = noiseMap[x, y] - gradientNoise[x, y];
                    noiseMap[x, y] = Mathf.Clamp01(substractedValue);
                }

                float currentHeight = noiseMap[x, y];

                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colorMap[y * (int)mapWidth + x] = regions[i].color;
                        colorPercentage[i]++;
                        break;
                    }
                }
            }
        }

        colorPercent = colorPercentage;

        return colorMap;
    }

    public void DrawNoiseMap(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        Color[] colorMap = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }

        texture.SetPixels(colorMap);
        texture.Apply();

        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(width / 20f, 1, height / 20f);
    }

    public void DrawNoiseMap(Color[] colorMap)
    {
        Texture2D texture = new Texture2D((int)mapWidth, (int)mapHeight);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        texture.SetPixels(colorMap);
        texture.Apply();

        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(mapWidth / 20f, 1, mapHeight / 20f);
    }

    public void DrawStarter()
    {
        Color[] colorMap = new Color[(int)mapWidth * (int)mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (y >= start.y && y <= end.y
                    && x >= start.x && x <= end.x)
                {
                    colorMap[y * (int)mapWidth + x] = regions[1].color;
                }
                else
                {
                    colorMap[y * (int)mapWidth + x] = regions[0].color;
                }
            }
        }

        Texture2D texture = new Texture2D((int)mapWidth, (int)mapHeight);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        texture.SetPixels(colorMap);
        texture.Apply();

        textureRendererStarter.sharedMaterial.mainTexture = texture;
        textureRendererStarter.transform.localScale = new Vector3(mapWidth / 20f, 1, mapHeight / 20f);
    }

    private void OnValidate()
    {
        if (mapWidth < 1)
            mapWidth = 1;

        if (mapHeight < 1)
            mapHeight = 1;

        if (noiseScale <= 0)
            noiseScale = 0.0001f;

        if (lacunarity < 1)
            lacunarity = 1;

        if (octaves < 0)
            octaves = 0;
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
    public TileStruct tileStr;
    [Space]
    public float percentage;
}

[System.Serializable]
public struct TileStruct
{
    public Tile[] tiles;
}