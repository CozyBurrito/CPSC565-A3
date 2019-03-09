﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    The class is used for creating and handling the evolution of a population of HexGrids
    A HexGrid is a genome (and its genes represented by its color string)
    A group of HexGrids forms a generaion 
 */

public class EvolutionManager : MonoBehaviour
{
    public HexGrid hexGridPrefab;

    public int numHexGridsPerGen = 8;   // The number of genomes per generation
    public List<HexGrid> hexGridsGen;   // The list of genomes for the current generation
    public List<List<HexGrid>> hexGridsGens; // The list of generations, helpful to see the changes
    public int genNum = -1;

    // The indexes  of the selected hexgrids to be used for next generation
    public int selHexGrid1 = -1;
    public int selHexGrid2 = -1;

    public bool isSelectionMode = false;

    public Button nextGenBtn;

    public Text genText;

    void Start()
    {
        selHexGrid1 = -1;
        selHexGrid2 = -1;
        isSelectionMode = false;

        // Initialize first generation randomly
        hexGridsGen = new List<HexGrid>();
        hexGridsGens = new List<List<HexGrid>>();
        hexGridsGens.Add(hexGridsGen);

        createHexGrids(true);

    }

    private void Update() {
        if(selHexGrid1 != -1 && selHexGrid2 != -1)
        {
            nextGenBtn.interactable = true;
        }
        else
        {
            nextGenBtn.interactable = false;
        }

        genText.text = "Generation: " + genNum.ToString();
    }

    // Draws the current generation to the screen
    void createHexGrids(bool randomize = false)
    {
        int rows = (int)Math.Sqrt(numHexGridsPerGen);
        int cols = (int)Math.Ceiling(numHexGridsPerGen / (float)rows);
        bool offByOne = (rows * cols) != numHexGridsPerGen;

        float x = 0f;
        float z = 0f;

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (r == rows - 1 && c == cols - 1 && offByOne)
                    continue;

                HexGrid h = Instantiate(hexGridPrefab);
                h.transform.SetPositionAndRotation(new Vector3(x, 0f, z), Quaternion.identity);
                h.name = "Hex Grid " + hexGridsGen.Count;
                h.transform.SetParent(this.gameObject.transform);

                if (randomize)
                {
                    h.randomizeGridColouring();
                }
                else
                {
                    //set the hexgrid string to a certain value from evolution algorithms
                }

                h.createHexGridFromString(hexGridsGen.Count);

                hexGridsGen.Add(h);

                x += 150f;
            }

            x = 0f;
            z -= 150f;
        }

        genNum++;
    }

    public void toggleSelectionMode()
    {
        isSelectionMode = !isSelectionMode;

        if(!isSelectionMode)
        {
            selHexGrid1 = -1;
            selHexGrid2 = -1;

            for (int i = 0; i < hexGridsGen.Count; i++)
            {
                hexGridsGen[i].isSet = false;
                hexGridsGen[i].unHighlight();
            }

        }
    }

    public bool setSelectedHexGrid(int hexGridId)
    {
        if(isSelectionMode)
        {
            // if it was already set, unset it
            if (selHexGrid1 == hexGridId)
            {
                selHexGrid1 = -1;
                return false;
            }
            if (selHexGrid2 == hexGridId)
            {
                selHexGrid2 = -1;
                return false;
            }

            // if space in one of them, set it
            if (selHexGrid1 == -1)
            {
                selHexGrid1 = hexGridId;
                return true;
            }
            if (selHexGrid2 == -1)
            {
                selHexGrid2 = hexGridId;
                return true;
            }
        }

        return false;
    }

    public void createNextGen()
    {
        genNum++;
    }

}