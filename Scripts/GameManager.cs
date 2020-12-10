using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public delegate void CurrencyChanged();

public class GameManager : Singleton<GameManager>
{

    public TowerBtn ClickedBtn { get; set; }
    [SerializeField]
    private Text statText;

    public event CurrencyChanged Changed;

    [SerializeField]
    private GameObject statsPanel;

    private int currency;

    private int wave = 0;

    private int lives;

    [SerializeField]
    private Text waveTxt;

    [SerializeField]
    private Text currencyTxt;

    [SerializeField]
    private Text livesTxt;

    [SerializeField]
    private GameObject waveBtn;

    [SerializeField]
    private GameObject gameOverMenu;

    [SerializeField]
    private GameObject upgradePanel;

    [SerializeField]
    private Text upgradePrice;

    [SerializeField]
    private Text sellText;

    [SerializeField]
    private GameObject inGameMenu;

    [SerializeField]
    private GameObject optionsMenu;

    private int health = 15;

    private Tower selectedTower;

    private bool gameOver = false;

    private List<Monster> activeMonsters = new List<Monster>();

    public ObjectPool Pool { get; set; }

    public bool WaveActive
    {
        get { return activeMonsters.Count > 0; } //sai verdade quando a lista for maior q 0, e falso quando for menor q 0
    }

    public int Currency
    {
        get
        {
            return currency;
        }

        set
        {
            this.currency = value;
            this.currencyTxt.text = value.ToString() + " <color=lime>$</color>";

            OnCurrencyChanged();
        }
    }

    public int Lives
    {
        get
        {
            return lives;
        }
        set
        {
            this.lives = value;
            if (lives <= 0)
            {
                this.lives = 0;
                GameOver();
            }

            livesTxt.text = lives.ToString();
        }
    }


    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
    }

    void Start () {

        Lives = 10;
        Currency = 50;
	}
	
	// Update is called once per frame
	void Update () {
        HandleEscape();
	}

    public void PickTower(TowerBtn towerBtn)
    {
        if (Currency >= towerBtn.Price && !WaveActive)
        {
            this.ClickedBtn = towerBtn;
            Hover.Instance.Activate(towerBtn.Sprite);

        }

    }

    public void BuyTower()
    {
        if (Currency >= ClickedBtn.Price)
        {
            Currency -= ClickedBtn.Price;
            Hover.Instance.Deactivate();
        }
    }

    public void OnCurrencyChanged()
    {
        if (Changed != null)
        {
            Changed();
        }
    }

    private void HandleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (selectedTower == null && !Hover.Instance.IsVisible)
            {
                ShowInGameMenu();
            }

            else if (Hover.Instance.IsVisible)
            {
                DropTower();
            }
            else if (selectedTower != null)
            {
                DeselectTower();
            }
        }
    }

    public void SelectTower(Tower tower)
    {
        if (selectedTower != null)
        {
            selectedTower.Select();
        }
        selectedTower = tower;
        selectedTower.Select();
        sellText.text = "+ " + (selectedTower.Price / 2).ToString() + " $";
        upgradePanel.SetActive(true);


    }

    public void DeselectTower()
    {
        if (selectedTower != null)
        {
            selectedTower.Select();
        }
        selectedTower = null;
        upgradePanel.SetActive(false);
    }

    public void StartWave()
    {

        wave++;

        waveTxt.text = string.Format("Wave: <color=lime>{0}</color>", wave); //o escopo substitui o wave
        StartCoroutine(SpawnWave());

        waveBtn.SetActive(false);


    }

    private IEnumerator SpawnWave()
    {
        LevelManager.Instance.GeneratePath(); //vai gerar um caminho quando eu clicar no botão NEXT WAVE
        for (int i = 0; i < wave; i++)
        {

            int monsterIndex = Random.Range(0, 4);
            string type = string.Empty;

            switch (monsterIndex)
            {
                case 0:
                    type = "BlueMonster";
                    break;
                case 1:
                    type = "RedMonster";
                    break;
                case 2:
                    type = "GreenMonster";
                    break;
                case 3:
                    type = "PurpleMonster";
                    break;
            }

            Monster monster = Pool.GetObject(type).GetComponent<Monster>();
            monster.Spawn(health);

            if (wave % 3 == 0) //a cada 3 waves, a vida aumenta em 5
            {
                health += 5;
            }

            activeMonsters.Add(monster); //todos os monstros ativos estão nesta lista agora isso significa que a gente pode olhar essa lista e verificar se existe monstro.

            yield return new WaitForSeconds(2.5f);
        }

    }
    public void RemoveMonster(Monster monster) //preciso remover os monstros da lista, pra saber se posso passar a wave
    {
        activeMonsters.Remove(monster); //quando o monstro tocar o portal vermelho, preciso remove-lo da lista

        if (!WaveActive && !gameOver)
        {
            waveBtn.SetActive(true);

        }
    }

    public void GameOver()
    {
        if (!gameOver)
        {
            gameOver = true;
            gameOverMenu.SetActive(true);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void SellTower()
    {
        if (selectedTower != null)
        {
            Currency += selectedTower.Price / 2;
            selectedTower.GetComponentInParent<TileScript>().IsEmpty = true; //ERRADO
            Destroy(selectedTower.transform.parent.gameObject); //errado

            DeselectTower();
        }
    }

    public void ShowStats()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);

    }
    public void ShowSelectedTowerStatus()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
        UpdateUpgradeTip();
    }

    public void SetToolTipText(string txt)
    {
        statText.text = txt;
    }

    public void UpdateUpgradeTip()
    {
        if (selectedTower != null)
        {
            sellText.text = "+ " + (selectedTower.Price / 2).ToString() + " $";
            SetToolTipText(selectedTower.GetStatus());

            if (selectedTower.NextUpgrade != null)
            {
                upgradePrice.text = selectedTower.NextUpgrade.Price.ToString() + " $";
            }
            else
            {
                upgradePrice.text = string.Empty;
            }
        }
    }

    public void UpgradeTower()
    {
        if (selectedTower != null)
        {
            if (selectedTower.Level <= selectedTower.Upgrades.Length && Currency >= selectedTower.NextUpgrade.Price)
            {
                selectedTower.Upgrade();
            }
        }
    }

    public void ShowInGameMenu()
    {
        if (optionsMenu.activeSelf)
        {
            ShowMain();
        }
        else
        {
            inGameMenu.SetActive(!inGameMenu.activeSelf);
            if (!inGameMenu.activeSelf)
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;
            }
        }

    }

    public void DropTower()
    {
        ClickedBtn = null;
        Hover.Instance.Deactivate();
    }
     
    public void ShowOptions()
    {
        inGameMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void ShowMain()
    {
        inGameMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }
}
