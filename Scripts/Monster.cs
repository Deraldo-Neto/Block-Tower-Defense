using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {

    [SerializeField]
    private Stat health;

    [SerializeField]
    private float speed;

    [SerializeField]
    private Element elementType;

    private int invulnerabilty = 2;

    private Stack<Node> path;

    private List<Debuff> debuffs = new List<Debuff>();

    private List<Debuff> debuffsToRemove = new List<Debuff>();
    private List<Debuff> newDebuffs = new List<Debuff>();

    private Animator myAnimator;
    public Point GridPosition { get; set; }

    private Vector3 destination;

    private SpriteRenderer spriteRenderer;

    public bool IsActive { get; set; }

    public float MaxSpeed { get; set; }

    public bool Alive
    {
        get { return health.CurrentVal > 0; }
    }

    public Element ElementType
    {
        get
        {
            return elementType;
        }
    }

    public float Speed
    {
        get
        {
            return speed;
        }

        set
        {
            speed = value;
        }
    }

    private void Awake()
    {
        health.Initialize();
        spriteRenderer = GetComponent<SpriteRenderer>();
        MaxSpeed = speed;
    }

    private void Update()
    {
        HandleDebuffs();
        Move();

    }



    public void Spawn(int health)
    {
        transform.position = LevelManager.Instance.EntradaPortal.transform.position;
        this.health.Bar.reset();
        this.health.MaxVal = health;
        this.health.CurrentVal = this.health.MaxVal;

        myAnimator = GetComponent<Animator>();
        StartCoroutine(Scale(new Vector3(0.1f, 0.1f), new Vector3(1, 1),false));

        SetPath(LevelManager.Instance.Path);
    }

    public IEnumerator Scale(Vector3 from, Vector3 to, bool remove)
    {
        float progress = 0;
        while (progress <= 1)
        {
            transform.localScale = Vector3.Lerp(from, to, progress);
            progress += Time.deltaTime;

            yield return null;
        }

        transform.localScale = to;

        IsActive = true;
        if (remove)
        {
            Release();
        }
    }

    private void Move()
    {
        if (IsActive)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, Speed * Time.deltaTime); //faz o monstro andar

            if (transform.position == destination) //proximo tile que preciso ir
            {
                if (path != null && path.Count > 0)
                {
                    Animate(GridPosition, path.Peek().GridPosition);

                    GridPosition = path.Peek().GridPosition;
                    destination = path.Pop().WorldPosition;
                }
            }
        }
        
    }

    private void SetPath(Stack<Node> newPath)
    {
        if (newPath != null)
        {
            this.path = newPath;

            Animate(GridPosition, path.Peek().GridPosition);

            GridPosition = path.Peek().GridPosition;

            destination = path.Pop().WorldPosition;
        }
    }

    private void Animate(Point currentPos, Point newPos)
    {
        if (currentPos.Y > newPos.Y) //movendo pra baixo
        {
            myAnimator.SetInteger("Horizontal", 0);
            myAnimator.SetInteger("Vertical", 1);
        }
        if (currentPos.Y < newPos.Y) //movendo pra cima
        {
            myAnimator.SetInteger("Horizontal", 0);
            myAnimator.SetInteger("Vertical", -1);
        }
        if (currentPos.Y == newPos.Y)
        {
            if (currentPos.X > newPos.X) //para a esquerda
            {
                myAnimator.SetInteger("Horizontal", -1);
                myAnimator.SetInteger("Vertical", 0);
            }
            else if (currentPos.X < newPos.X) //para a direita
            {
                myAnimator.SetInteger("Horizontal", 1);
                myAnimator.SetInteger("Vertical", 0);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Saida")
        {
            StartCoroutine(Scale(new Vector3(1, 1), new Vector3(0.1f, 0.1f),true));

            GameManager.Instance.Lives--;
        }
        if (other.tag == "Tile")
        {
            spriteRenderer.sortingOrder = other.GetComponent<TileScript>().GridPosition.Y;
        }

    }

    public void Release()
    {
        debuffs.Clear(); //limpa todos os debuffs
        IsActive = false;
        GridPosition = LevelManager.Instance.Entrada;
        GameManager.Instance.Pool.ReleaseObject(gameObject);
        GameManager.Instance.RemoveMonster(this); //remove ele mesmo da lista
    }

    public void TakeDamage(float damage, Element dmgSource)
    {
        if (IsActive)
        {
            if (dmgSource == elementType)
            {
                damage = damage / invulnerabilty;
                invulnerabilty++;
            }

            health.CurrentVal -= damage;

            if (health.CurrentVal <= 0)
            {
                SoundManager.Instance.PLaySFX("Hit");
                GameManager.Instance.Currency += 2;
                myAnimator.SetTrigger("Die");
                IsActive = false;
                GetComponent<SpriteRenderer>().sortingOrder--;
            }
        }

    }

    public void AddDebuff(Debuff debuff)
    {
        if (!debuffs.Exists(x => x.GetType() == debuff.GetType()))
        {
            newDebuffs.Add(debuff);
        }
    }

    public void RemoveDebuff(Debuff debuff)
    {
        debuffsToRemove.Add(debuff);
    }

    private void HandleDebuffs()
    {
        if (newDebuffs.Count > 0)
        {
            debuffs.AddRange(newDebuffs);

            newDebuffs.Clear();
        }
        foreach (Debuff debuff in debuffsToRemove)
        {
            debuffs.Remove(debuff);
        }

        debuffsToRemove.Clear();

        foreach (Debuff debuff in debuffs) //percorre a lista
        {
            debuff.Update();
        }
    }

}
