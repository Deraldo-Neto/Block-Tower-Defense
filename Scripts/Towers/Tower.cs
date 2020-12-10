using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Element {STORM,FIRE,FROST,POISON,NONE }


public abstract class Tower : MonoBehaviour { //não consigo acessar, não consigo colocar no unity

    [SerializeField]
    private string projectileType;

    public int Level { get; protected set; }

    [SerializeField]
    private float debuffDuration;
    [SerializeField]
    private float proc; //% de chance de acontecer o debuff

    public TowerUpgrade[] Upgrades { get;protected set; } //protected eu consigo acessar no outro script

    private SpriteRenderer mySpriteRenderer;

    [SerializeField]
    private int damage;

    public Element ElementType { get; protected set; }

    public int Price { get; set; }

    private Monster target;

    public Monster Target
    {
        get { return target; }
    }

    private Queue<Monster> monsters = new Queue<Monster>();

    private bool canAttack = true;

    private float attackTimer;
    [SerializeField]
    private float attackCooldown;
    //private Animator myAnimator;

    [SerializeField]
    private float projectileSpeed;


    public float ProjectileSpeed
    {
        get { return projectileSpeed; } //eu pego o float projectileSpeed em outro script!!!
    }

    public int Damage
    {
        get
        {
            return damage;
        }
    }

    public float DebuffDuration
    {
        get
        {
            return debuffDuration;
        }

        set
        {
            this.debuffDuration = value;
        }
    }

    public float Proc
    {
        get
        {
            return proc;
        }

        set
        {
            this.proc = value;
        }
    }

    public TowerUpgrade NextUpgrade
    {
        get
        {
            if (Upgrades.Length > Level-1)
            {
                return Upgrades[Level - 1];
            }
            return null;
        }
    }

    // Use this for initialization
    void Awake () {
        //myAnimator = transform.parent.getComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        Level = 1;

    }
	
	// Update is called once per frame
	void Update () {
        Attack();
    }

    public void Select()
    {
        mySpriteRenderer.enabled = !mySpriteRenderer.enabled;
        GameManager.Instance.UpdateUpgradeTip();
        
    }

    private void Attack()
    {
        if (!canAttack)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackCooldown)
            {
                canAttack = true;
                attackTimer = 0;
            }
        }
        if (target == null && monsters.Count > 0 && monsters.Peek().IsActive)
        {
            target = monsters.Dequeue();

        }
        if (target != null && target.IsActive)
        {
            Vector2 dir = target.transform.position - transform.parent.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            transform.parent.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.parent.GetChild(0).rotation = Quaternion.identity;

            if (canAttack)
            {
                Shoot();
                //myAnimator.SetTrigger("Attack");

                canAttack = false;
            }
            
        }
        if (target != null && !target.Alive || target != null && !target.IsActive)
        {
            target = null;
        }

    }

    public virtual string GetStatus()
    {
        if (NextUpgrade != null)
        {
            return string.Format("\nLevel: {0} \nDamage: {1} <color=#00ff00ff> + {4}</color>\nProc: {2}% <color=#00ff00ff>+{5}%</color>\nDebuff: {3}sec <color=#00ff00ff>+{6}</color>", Level,damage,proc,DebuffDuration,NextUpgrade.Damage,NextUpgrade.ProcChance,NextUpgrade.DebuffDuration);
        }
        return string.Format("\nLevel: {0} \nDamage: {1} \nProc: {2} \nDebuff: {3}sec", Level, damage, proc, DebuffDuration);
    }

    private void Shoot()
    {
        Projectile projectile = GameManager.Instance.Pool.GetObject(projectileType).GetComponent<Projectile>();

        projectile.transform.position = transform.position;

        projectile.Initialized(this); //do inicio a essa função desse script
    }

    public virtual void Upgrade() //consigo "passar por cima" em outros scripts
    {
        GameManager.Instance.Currency -= NextUpgrade.Price;
        Price += NextUpgrade.Price;
        this.damage += NextUpgrade.Damage;
        this.proc += NextUpgrade.ProcChance;
        this.DebuffDuration += NextUpgrade.DebuffDuration;
        Level++;
        GameManager.Instance.UpdateUpgradeTip();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            monsters.Enqueue(other.GetComponent<Monster>()); //adiciona o monstro a fila

        }

    }

    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            if (target == null && monsters.Count == 0)
            {
                transform.parent.rotation = Quaternion.identity;
                transform.parent.GetChild(0).rotation = Quaternion.identity;
            }
        }

    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            GameObject go = other.gameObject;

            if (go.activeInHierarchy)
            {
                target = null;
            }
            if (target == null && monsters.Count == 0)
            {
                transform.parent.rotation = Quaternion.identity;
                transform.parent.GetChild(0).rotation = Quaternion.identity;
            }
        }

    }

    public abstract Debuff GetDebuff();

}
