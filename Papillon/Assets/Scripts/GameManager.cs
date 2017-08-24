﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager gm = null;

    private BoardManager boardManager;
    private CraftManager craftManager;
    private ResearchManager researchManager;
    private SoundManager soundManager;
    private Player player;

    private EffectProcessor effectProcessor;

    //private GameObject inventory;
    private int scene;

    // game play related variables
    public int day;
    public bool exploreChance;  // whether player can go field or not
    public bool moveChance;     // whether player can move map or not
    public bool researchChance;     // whether player can research or not

    public GameObject msgPanel;
    public GameObject pauseMsgPanel;


    // think this better be Awake.
    private void Awake() {

        /*
         * GameManager Base Initializing
         */

        // Singleton object
        if (gm == null)
            gm = this;
        else if (gm != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        // initialize game
        initGame();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (GameObject.Find("MessagePanel(Clone)") == null && GameObject.Find("PauseMessagePanel(Clone)") == null)
                gamePause();
            else if (GameObject.Find("MessagePanel(Clone)") == null)
                GameObject.Find("PauseMessagePanel(Clone)").GetComponent<PauseMessagePanel>().close();
            else
                GameObject.Find("MessagePanel(Clone)").GetComponent<MessagePanel>().close();
    }

    private void initGame() {

        /*
         *  Initializing player
         *  
         */
        player = new Player(100, 1000, 0, 100f);

        /*
         * Initializing Databases
         * 
         */
        ItemDatabase.init();
        FieldDropDatabase.init();
        RecipeDatabase.init();
        TechnologyDatabase.init();

        /*
         * Other Managers Initializing
         * 
         * **CAUTION**
         * You must initialize managers after initializing databases, player
         * because initializing managers contain loading object, data from database, player
         */
        craftManager = GetComponent<CraftManager>();
        craftManager.init();
        researchManager = GetComponent<ResearchManager>();
        researchManager.init();
        boardManager = GetComponent<BoardManager>();
        boardManager.init();
        soundManager = GetComponentInChildren<SoundManager>();

        /*
         * Effect Processor Initializing
         * **CAUTION**
         * You must initialize this after all other initializing done
         */
        effectProcessor = new EffectProcessor();
        effectProcessor.init();

        ///*
        // * get Inventory
        // */

        //inventory = transform.Find("Canvas/InventoryPanel").gameObject;

        // default scene
        scene = SCENES.MAP;

        // game play variables initialize
        day = 1;
        exploreChance = true;
        moveChance = true;
        researchChance = true;

        // start game
        SceneManager.LoadScene(scene);
    }

    private void OnLevelWasLoaded (int level) {
        scene = level;
        //getInventory().SetActive(false);
        boardManager.boardSetup(level);
    }

    public Player getPlayer() {
        return player;
    }

    public BoardManager getBoardManager() {
        return boardManager;
    }

    public CraftManager getCraftManager() {
        return craftManager;
	}

    public ResearchManager getResearchManager() {
        return researchManager;
    }

    public EffectProcessor getEffectProcessor() {
        return effectProcessor;
    }

    //public GameObject getInventory() {
    //    return inventory;
    //}

#region Game Play Related Methods

    public bool doEffect(Effect effect, bool flag=true) {
        return effectProcessor.process(effect, flag);
    }

    public void playBGM(string name) {
        soundManager.playBGM(name);
    }

    public void playSE(string name) {
        soundManager.playSE(name);
    }

    public void showMessage(string text, bool flag=false) {
        if(flag) {
            GameObject panel = Instantiate(pauseMsgPanel, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            panel.transform.SetParent(GameObject.Find("Canvas").transform, false);
            panel.GetComponent<PauseMessagePanel>().setText(text);
        }
        else {
            GameObject panel = Instantiate(msgPanel, new Vector3(0f, 0f, 0f), Quaternion.identity) as GameObject;
            panel.transform.SetParent(GameObject.Find("Canvas").transform, false);
            panel.GetComponent<MessagePanel>().setText(text);
        }
    }


    public int getDay() {
        return day;
    }

    // go to next day
    public void goNextDay() {
        day += 1;
        exploreChance = true;
        moveChance = true;
        researchChance = true;

        player.changeSatiety(SATIETYPOINTS.SLEEP);
        playSE("snore");

        boardManager.nextDay(scene, day);
    }

    public bool isBlackholeMoveTurn(int day) {
        return day % 3 == 0;
    }

    // check player can explore
    public bool canPlayerExplore() {
        return exploreChance;
    }

    // check player can move
    public bool canPlayerMove() {
        return moveChance;
    }

    // check player can research
    public bool canPlayerResearch()
    {
        return researchChance;
    }

    /*
     * use explore chance
     *  - when user go to field
     *  - when user built base
     *  - when user do some research in base
     */
    public void useExploreChance() {
        exploreChance = false;
    }

    /*
     *  use move chance
     */
    public void useMoveChance() {
        moveChance = false;
    }
    
    /*
     *  use research chance
     */
    public void useResearchChance() {
        researchChance = false;
    }

#endregion

#region Game System Related Methods

    public void gameOver() {
        Debug.Log("GAMEOVER");
        playBGM("game-over");
        SceneManager.LoadScene(6);
    }

    public void gameClear() {
        Debug.Log("GAMECLEAR");
        playBGM("rocket-launch");
        playSE("rocket");
        SceneManager.LoadScene(7);
    }

    public void gamePause() {
        Debug.Log("GAME PAUSE");
        showMessage("게임 중지", true);
    }

    // check rocket launch is possible
    // and if so, do something (currently game end)
    public void checkRocketLaunch(Field field) {

        List<FieldItemElement> list = field.getItemList();

        foreach(FieldItemElement e in list) {
            // 부족한 부품
            if (e.currentCount == -1) {

                string msg = "이 곳의 잔해로 로켓을 발사하려면 다음의 부품이 필요하다.\n\n";
                msg += e.getItem().getName();
                showMessage(msg);

                // 플레이어가 해당 부품을 가지고 있음
                if (getPlayer().checkItemPossession(e.getItem().getId(), 1)) {
                    gameClear();
                    return;
                }


            }
        }
    }

#endregion
}

//SCENES
public static class SCENES {
    public const int FIELD = 1;
    public const int MAP = 2;
    public const int BASE = 3;
    //public const int LAB = 1;
    //public const int FACTORY = 2;
    //public const int FARM = 3;
    //public const int MAP = 4;
    //public const int MAIN = 5;
}

/*
 * 특정 행동에 따른 플레이어 체력, 허기 변화를 지정해놓은 것
 * 한 곳에 몰아두는 게 좋아보여서 따로 만들어 놓음
 */

public static class HEALTHPOINTS {

}

public static class SATIETYPOINTS {
    public const int MOVE = -100; // when player move on map
    public const int SLEEP = -50; // when player go to sleep (next day)
    public const int COLLECT = -10; // when player get item from field
    public const int CRAFTING = -5;
}
