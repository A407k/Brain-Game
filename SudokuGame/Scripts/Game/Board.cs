﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    [SerializeField] private bool constantBoard = false; //Constant board in select level submenu
    [SerializeField] private KeyboardNumeric keyboard = null;

    public delegate void BoardFinishLoadDelegate();
    public delegate void BoardReadyToPlayDelegate();
    public delegate void SudokuCorrectDelegate();
    public event BoardFinishLoadDelegate BoardFinishedLoading;
    public event BoardReadyToPlayDelegate BoardReadyToPlay;
    public event SudokuCorrectDelegate SudokuCorrect;

    private BoardTile[,] tiles = new BoardTile[9, 9];

	void Start () {
        LoadTilesToArray();
    }

    //DEBUG
    void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) CheckBoard();
        if (Input.GetKeyDown(KeyCode.T)) tiles[Random.Range(0, 9), Random.Range(0, 9)].SetConstantValue(Random.Range(1, 10));
	}

    public void Clear() {
        foreach (var ti in tiles) ti.Clear();
    }

    public void SetLevel(Level level) {
        for(int y = 0; y < 9; ++y) {
            for(int x = 0; x < 9; ++x) {
                if (level.Display[x, y]) {
                    tiles[x, y].SetConstantValue(level.Board[x, y]);
                }
            }
        }

        if (BoardReadyToPlay != null) BoardReadyToPlay();
    }

    private void LoadTilesToArray() {
        foreach (Transform group in transform) {
            int groupX = (int)char.GetNumericValue(group.name[0]);
            int groupY = (int)char.GetNumericValue(group.name[2]);

            foreach (Transform tile in group) {
                int tileX = (int)char.GetNumericValue(tile.name[0]);
                int tileY = (int)char.GetNumericValue(tile.name[2]);

                int x = (groupX * 3) + tileX;
                int y = (groupY * 3) + tileY;

                tiles[x, y] = tile.GetComponent<BoardTile>();
                if(!constantBoard) {
                    tiles[x, y].TilePressed += keyboard.Display;
                    tiles[x, y].ValueChanged += CheckBoard;
                }
                else tile.GetComponent<BoxCollider2D>().enabled = false;
                //Debug.Log($"parent={group.name} tile={tile.name}   |   x={x} y={y}", tiles[x,y]);
            }
        }

        if (BoardFinishedLoading != null) BoardFinishedLoading();
    }

    private void CheckBoard() {
        if (!CheckRows()) return;
        if (!CheckColumns()) return;
        if (!CheckBoxes()) return;
        
        if (SudokuCorrect != null) SudokuCorrect();
    }

    private bool CheckRows() {
        var hash = new HashSet<int>();

        for (int y = 0; y < 9; ++y) {
            for (int x = 0; x < 9; ++x) {
                int value = tiles[x, y].Value;

                if (value == 0 || hash.Contains(value)) {
                    Debug.Log($"Incorrect row: {y}");
                    return false;
                }
                else hash.Add(value);
            }

            hash.Clear();
        }

        return true;
    }

    private bool CheckColumns() {
        var hash = new HashSet<int>();

        for (int x = 0; x < 9; ++x) {
            for (int y = 0; y < 9; ++y) {
                int value = tiles[x, y].Value;

                if (value == 0 || hash.Contains(value)) {
                    Debug.Log($"Incorrect column: {x}");
                    return false;
                }
                else hash.Add(value);
            }

            hash.Clear();
        }

        return true;
    }

    private bool CheckBoxes() {
        for(int y = 0; y < 3; ++y) {
            for(int x = 0; x < 3; ++x) {
                if(!CheckSingleBox(x, y)) {
                    Debug.Log($"Incorrect box: [{x}, {y}]");
                    return false;
                }
            }
        }

        return true;
    }

    private bool CheckSingleBox(int boxX, int boxY) {
        var hash = new HashSet<int>();
        int startX = boxX * 3;
        int startY = boxY * 3;

        for(int y = startY; y < startY + 3; ++y) {
            for(int x = startX; x < startX + 3; ++x) {
                int value = tiles[x, y].Value;

                if (value == 0 || hash.Contains(value)) return false;
                else hash.Add(value);
            }
        }

        return true;
    }
}
