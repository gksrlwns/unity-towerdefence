﻿using BackEnd;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("게임 영향")]
    public int restLife;
    public int money;
    public int maxMonsterNum;
    public int stage;
    public int towerCount;
    public int tower2Count;
    [Header("시간")]
    public float spawnTimer;
    public float StageTimer;
    public float restTimer;
    [Header("몬스터")]
    public int monsterNum = 0;
    public int clearMonsterNum;
    public Transform enemy_Tr;
    public GameObject[] enemyPrefabs;
    //public GameObject enemyPrefab1;
    //public GameObject enemyPrefab2;
    //public GameObject enemyPrefab3;
    [Header("타워빌드")]
    public Transform tower_Tr;
    private Node[] nodes;
    public GameObject Tower2Clone;

    public GameObject[] Tower2Prefab;
    //public GameObject Tower2Prefab1;
    //public GameObject Tower2Prefab2;
    //public List<Transform> TowerTr1;
    //public List<Transform> TowerTr2;
    public List<Transform>[] TowerTr;
    //public int node_TC1;
    //public int node_TC2;
    [Header("나머지")]
    public bool isCreate;
    public bool isDelete;
    public bool isStageClear;
    public bool isGameEnd;
    public bool isPannelMove;
    public bool isPause;
    public Button mainBtn;
    public Button puaseBtn;
    public GameObject Startobj;
    public GameObject Endobj;
    public GameObject PanelObj;
    public GameObject clearPanelObj;
    public GameObject gameOverPanelObj;
    public GameObject pausePanel;
    private Animation anim;
    public Vector3 EnemyPosition;
    public Vector3 EnemyRotation;
    public Animation _ani;
    public Text restTimeText;
    public Text stageTimeText;
    public Text moneyText;
    public Text restLifeText;
    public Text gameOverText;
    public Text gameEndText;
    public Sprite pauseImage;
    public Sprite resumeImage;
    public Sprite restImgae;
    
    
    //public Tower.TowerKind towerKind;

    public enum StageEnum
    {
        stage1 = 1,
        stage2,
        stage3,
        clear,
    }

    public StageEnum _stageEnum;
    

    private void Awake()
    {
        nodes = FindObjectsOfType<Node>();
        anim = PanelObj.GetComponent<Animation>();

    }
    void Start()
    {
        isPannelMove = false;
        monsterNum = 0;
        money = BackEndManager.instance.nowPlayerData.money;
        restLife = BackEndManager.instance.nowPlayerData.restLife;
        _stageEnum = (StageEnum)BackEndManager.instance.nowPlayerData.stage;
        StageTimer = 0;
        restTimer = 30f;
        isCreate = false;
        isStageClear = true;
        isGameEnd = false;
        isPause = false;
        _stageEnum = StageEnum.stage1;
        print($"{(int)_stageEnum}");
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].enabled = false;
        }
        BackEndManager.instance.SaveFile();

    }

    // Update is called once per frame
    void Update()
    {
        if (isGameEnd == true) return;
        
        if(isPause == true)
        {
            this.enabled = false;
            return;
        }
        
        if (isStageClear == false)
        {
            StageTimer += Time.deltaTime;
            
            if (_stageEnum < StageEnum.clear)
            {
                SpawnEnemy(enemyPrefabs[(int)_stageEnum - 1]);
                if (clearMonsterNum >= maxMonsterNum)
                {
                    isStageClear = true;
                    
                    _stageEnum++;
                    StageTimer = 0;
                    clearMonsterNum = 0;
                    monsterNum = 0;
                    stageTimeText.text = "";
                    spawnTimer = 0f;
                    PlayerDataSave();
                    BackEndManager.instance.SaveFile();
                    
                }
            }
            if (_stageEnum == StageEnum.clear)
            {
                GameEnd();
                
            }
            
        }
        else if (isStageClear == true && monsterNum == 0)
        {
            restTimer -= Time.deltaTime;
            restTimeText.text = restTimer.ToString("00");//$"다음 스테이지 시작까지 남은 시간은 {restTimer.ToString("00")}초 입니다.";
            if (restTimer <= 0f)
            {
                isStageClear = false;
                restTimeText.text = "";
                restTimer = 30f;
            }
        }
        moneyText.text = money.ToString();
        restLifeText.text = $"{restLife.ToString()} / 20";
        if(restLife == 0)
        {
            GameOver();
        }
        if (isCreate == true || isDelete == true)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].enabled = true;
            }
        }
    }

    void GameOver()
    {
        gameOverPanelObj.SetActive(true);
        isGameEnd = true;
        isCreate = false;
        isDelete = false;

    }
    void GameEnd()
    {
        clearPanelObj.SetActive(true);
        isGameEnd = true;
        isCreate = false;
        isDelete = false;
    }
    

    
    void SpawnEnemy(GameObject enemyPrefab)
    {
        spawnTimer += Time.deltaTime;
        
        if(spawnTimer >= 1f) // 1f
        {
           spawnTimer = 0f;
           if (monsterNum == maxMonsterNum) return;
           Instantiate(enemyPrefab,  Startobj.transform.position + EnemyPosition, Startobj.transform.rotation , enemy_Tr);
           monsterNum++;
           
        }
    }

    void PlayerDataSave()
    {
        BackEndManager.instance.nowPlayerData.money = money;
        BackEndManager.instance.nowPlayerData.restLife = restLife;
        BackEndManager.instance.nowPlayerData.stage = (int)_stageEnum;
        for(int i = 0; i < nodes.Length; i++)
        {
            if(nodes[i].standardTower || nodes[i].upgradeTower)
            {
                TowerData data = new TowerData(nodes[i].towerTypeNum, nodes[i].GetNodePosition());
                BackEndManager.instance.nowPlayerData.towers.Add(data);
            }
        }
    }

    //타워 갯수 측정
    public void TowerCount()
    {
        towerCount = tower2Count = 0;
        for (int i = 0; i < nodes.Length; i++)
        {
            if(nodes[i].standardTower)
            {
                towerCount++;
            }
            if(nodes[i].upgradeTower)
            {
                tower2Count++;
            }
        }
    }

    public void TowerCreate()
    {
        isCreate = true;
        isDelete = false;
    }
    public void TowerDelete()
    {
        isDelete = true;
        isCreate = false;
    }
    public void NextStage()
    {
        isStageClear = false;
        restTimeText.text = "";
        restTimer = 30f;
    }

    public void MainScene()
    {
        SceneManager.LoadScene(0);
    }

    public void Pause()
    {
        isPause = !isPause;
        if(isPause == true)
        {
            pausePanel.SetActive(true);
            puaseBtn.GetComponent<Image>().sprite = resumeImage;
        }
        else if(isPause == false)
        {
            pausePanel.SetActive(false);
            puaseBtn.GetComponent<Image>().sprite = pauseImage;
            this.enabled = true;
        }
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PanelAnimationBtn()
    {
        isPannelMove = !isPannelMove;
        if (isPannelMove == false)
        {
            anim.Play("Panel_01");
            return;
        }
        else if (isPannelMove == true)
        {
            anim.Play("Panel_02");
            return;
        }
    }
    
}
