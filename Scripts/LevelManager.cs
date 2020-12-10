using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelManager : Singleton<LevelManager> {

    [SerializeField] //conseguir setar o objeto
    private GameObject[] tilePrefabs; //pega o objeto tile

    [SerializeField]
    private CameraMovement cameraMovement;

    [SerializeField]
    private Transform map;

    private Point entrada, saida;

    [SerializeField]
    private GameObject entradaPrefab;

    [SerializeField]
    private GameObject saidaPrefab;

    public Portal EntradaPortal { get; set; }

    private Point mapSize;

    private Stack<Node> path;

    public Stack<Node> Path //ajuda a cada monstro ter seu próprio caminho
    {
        get
        {
            if (path == null)
            {
                GeneratePath();
            }

            return new Stack<Node>(new Stack<Node>(path));
        }
    }

    public Dictionary<Point, TileScript> Tiles { get; set; }

    public float TileSize
    {
        get { return tilePrefabs[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x; }//pego o tamanho em x do tile (como eles são proporcionais, tanto faz o x ou o y) 

    }

    public Point Entrada
    {
        get
        {
            return entrada;
        }

    }

    public Point Saida
    {
        get
        {
            return saida;
        }

    }

    void Start ()
    {
        CreateLevel();
    }
	
	
	void Update () {
		
	}


    private void CreateLevel()
    {
        Tiles = new Dictionary<Point, TileScript>(); //vou armazenar a posição e aonde eu quero colocar os tiles em um dicionary

        string[] mapData = ReadLevelText();

        mapSize = new Point(mapData[0].ToCharArray().Length, mapData.Length);

        int mapX = mapData[0].ToCharArray().Length; //conta quantos numeros possuem dentro da matriz
        int mapY = mapData.Length; //conta quantas matrizes tem

        Vector3 maxTile = Vector3.zero;

        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height)); //pega o canto superior esquerdo da camera

        for (int y = 0; y < mapY; y++)
        {
            char[] newTiles = mapData[y].ToCharArray(); //pega cada valor dentro da matriz e transforma cada 1 em 1 char

            for (int x = 0; x < mapX; x++)
            {
               PlaceTile(newTiles[x].ToString(),x,y, worldStart); //coloca os tiles no jogo
            }
        }

        maxTile = Tiles[new Point(mapX - 1, mapY - 1)].transform.position; //defino que a posição máxima da camera vai ser essa

        cameraMovement.SetLimits(new Vector3(maxTile.x + TileSize, maxTile.y - TileSize)); //Seta os limites da camera para o máximo do tile

        SpawnPortals();
    }

    private void PlaceTile(string tileType, int x, int y, Vector3 wordStart)
    {
        int TileIndex = int.Parse(tileType); //faz "1" virar 1

        TileScript newTile = Instantiate(tilePrefabs[TileIndex]).GetComponent<TileScript>(); //crio um novo tile

        newTile.Setup(new Point(x, y), new Vector3(wordStart.x + (TileSize * x), wordStart.y - (TileSize * y), 0), map); //pego a posiçao do tile e aonde eu quero colocar e joga para o "TileScript"


    }
    private string[] ReadLevelText()
    {
        TextAsset bindData = Resources.Load("Level") as TextAsset;

        string data = bindData.text.Replace(Environment.NewLine, string.Empty); //pego numero e coloco em uma string só

        return data.Split('-'); //quando encontrar esse simbolo, ele sai da função, portanto, pula a linha
    }

    private void SpawnPortals()
    {
        entrada = new Point(0, 0);

        GameObject tmp = (GameObject)Instantiate(entradaPrefab, Tiles[Entrada].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
        EntradaPortal = tmp.GetComponent<Portal>();
        EntradaPortal.name = "Portais";
        saida = new Point(16, 6);

        Instantiate(saidaPrefab, Tiles[saida].GetComponent<TileScript>().WorldPosition, Quaternion.identity);

    }

    public bool InBounds(Point position)
    {
        return position.X >= 0 && position.Y >= 0 && position.X < mapSize.X && position.Y < mapSize.Y;
    }

    public void GeneratePath()
    {
        path = AStarr.GetPath(Entrada, saida);
    }

}
