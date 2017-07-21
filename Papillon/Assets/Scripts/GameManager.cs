﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager gm = null;

    private BoardManager boardManager;
    private Player player;
    private int scene;

    private void Awake() {
        
        // Singleton object
        if (gm == null)
            gm = this;
        else if (gm != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        boardManager = GetComponent<BoardManager>();
        player = new Player(100, 100, 0, 100);          // TODO: Better initialise different way.
        scene = SCENES.LAB;

        initGame();
    }

    private void OnLevelWasLoaded(int level) {
        scene = level;
        boardManager.boardSetup(scene);
    }

    private void initGame() {
        boardManager.boardSetup(scene);
    }

    public Player getPlayer() {
        return player;
    }
}

//SCENES
/*ITEMCOLLECT                    0
  LAB(FOR RESEARCH)              1
  FACTORY(FOR PRODUCTION)        2
  FARM(FOR FARMING)              3
  MAP                            4 */

public static class SCENES {
    public const int ITEMCOLLECT = 0;
    public const int LAB = 1;
    public const int FACTORY = 2;
    public const int FARM = 3;
    public const int MAP = 4;
}