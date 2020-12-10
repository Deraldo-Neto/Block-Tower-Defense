using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour {

    public Point GridPosition { get; private set; }

    public bool IsEmpty { get;  set; }

    private Tower myTower;

    private Color32 fullColor = new Color32(255, 118, 118, 255); //vermelho
    private Color32 emptyColor = new Color32(96, 255, 90, 255); //verde


    private SpriteRenderer spriteRenderer;


    public bool WalkAble { get; set; }

    public bool Debugging { get; set; }

    public Vector2 WorldPosition
    {
        get
        {
            return new Vector2(transform.position.x + (GetComponent<SpriteRenderer>().bounds.size.x / 2), transform.position.y - (GetComponent<SpriteRenderer>().bounds.size.y/2));
        }//centro do tile
    }

    public bool EventsSystem { get; private set; }

    // Use this for initialization
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Setup(Point gridPos, Vector3 worldPos, Transform parent)
    {
        WalkAble = true;
        IsEmpty = true;
        this.GridPosition = gridPos;
        transform.position = worldPos;
        transform.SetParent(parent);
        LevelManager.Instance.Tiles.Add(gridPos, this); // pego a posição e onde eu quero colocar e adiciono no dicionário

    }

    private void OnMouseOver()
    {
        
        if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn != null)
        {

            if(IsEmpty && !Debugging)
            {
                ColorTile(emptyColor);
            }
            if (!IsEmpty && !Debugging)
            {
                ColorTile(fullColor);
            }
            else if (Input.GetMouseButtonDown(0))
            {
                PlaceTower();
            }
        }
        else if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn == null && Input.GetMouseButtonDown(0))
        {
            if (myTower != null)
            {
                GameManager.Instance.SelectTower(myTower);
            }
            else
            {
                GameManager.Instance.DeselectTower();
            }
        }


    }
    private void OnMouseExit()
    {
        if (!Debugging)
        {
            ColorTile(Color.white);
        }

    }

    private void PlaceTower()
    {
        WalkAble = false;
        if (AStarr.GetPath(LevelManager.Instance.Entrada, LevelManager.Instance.Saida) == null)
        {
            WalkAble = true;
            return;
        }
        GameObject tower = (GameObject)Instantiate(GameManager.Instance.ClickedBtn.TowerPrefab, transform.position, Quaternion.identity); //Cria a torre
        tower.GetComponent<SpriteRenderer>().sortingOrder = GridPosition.Y; //troca o order in layer
        tower.transform.SetParent(transform); //Coloca a torre "parrente" da grama no chão
        tower.transform.position = tower.transform.position + new Vector3(0.65f, -0.65f,0);

        this.myTower = tower.transform.GetChild(1).GetComponent<Tower>();

        IsEmpty = false;
        ColorTile(Color.white);

        myTower.Price = GameManager.Instance.ClickedBtn.Price;
        GameManager.Instance.BuyTower();

        WalkAble = false;
    }

    private void ColorTile(Color newColor)
    {
        spriteRenderer.color = newColor;
    }
}
