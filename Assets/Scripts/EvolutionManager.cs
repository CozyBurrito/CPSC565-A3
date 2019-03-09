using System;
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

    public int numHexGridsPerGen = 16;   // The number of genomes per generation
    public List<HexGrid> hexGridsGen;   // The list of genomes for the current generation
    public List<List<HexGrid>> hexGridsGens; // The list of generations
    public int genNum = -1;

    // The indexes of the selected hexgrids to be used for next generation
    public int selHexGrid1 = -1;
    public int selHexGrid2 = -1;
    HexGrid selectedHexGrid1;
    HexGrid selectedHexGrid2;

    public bool isSelectionMode = false;

    public Button nextGenBtn;

    public Text genText;
    public Text selHexGrid1Text;
    public Text selHexGrid2Text;

    public int numMutate = 8;
    public int numCross = 4;
    public int numRand = 2;

    public float mutationChance = 0.1f;

    public bool screenShot = false;

    void Start()
    {
        selHexGrid1 = -1;
        selHexGrid2 = -1;
        isSelectionMode = false;

        // Initialize first generation 
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

        if(selHexGrid1 != -1)
            selHexGrid1Text.text = "Hex Grid 1: " + selHexGrid1;
        else
            selHexGrid1Text.text = "Hex Grid 1: None";

        if(selHexGrid2 != -1)
            selHexGrid2Text.text = "Hex Grid 2: " + selHexGrid2;
        else
            selHexGrid2Text.text = "Hex Grid 2: None";
    }

    // Draws the current generation to the screen
    void createHexGrids(bool initialRand = false)
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

                // Add preexisting ones to thier previous locations
                if(hexGridsGen.Count == selHexGrid1)
                {
                    hexGridsGen.Add(selectedHexGrid1);
                    x += 150f;
                    continue;
                }

                if(hexGridsGen.Count == selHexGrid2)
                {
                    hexGridsGen.Add(selectedHexGrid2);
                    x += 150f;
                    continue;
                }

                HexGrid h = Instantiate(hexGridPrefab);
                h.transform.SetPositionAndRotation(new Vector3(x, 0f, z), Quaternion.identity);
                h.name = "Hex Grid " + hexGridsGen.Count;
                h.transform.SetParent(this.gameObject.transform);

                if (initialRand)
                {
                    h.randomizeGridColouring();
                }
                else
                {
                    if(numMutate > 0)
                    {
                        if(numMutate % 2 == 0)
                            h.mutateGridColouring(selectedHexGrid1.gridColouringStr, mutationChance);
                        else
                            h.mutateGridColouring(selectedHexGrid2.gridColouringStr, mutationChance);

                        numMutate--;
                    }
                    else if(numCross > 0)
                    {
                        h.crossoverGridColouring(selectedHexGrid1.gridColouringStr, selectedHexGrid2.gridColouringStr);
                        numCross--;
                    }
                    else if(numRand > 0)
                    {
                        h.randomizeGridColouring();
                        numRand--;
                    }
                    else
                    {
                        Debug.LogError("Could not create grid colouring for this HexGrid!!");
                    }
                        
                }

                h.createHexGridFromString(hexGridsGen.Count);

                hexGridsGen.Add(h);

                x += 150f;
            }

            x = 0f;
            z -= 150f;
        }

        genNum++;
        numMutate = 8;
        numCross = 4;
        numRand = 2;

        if (screenShot)
            ScreenCapture.CaptureScreenshot("Screenshots\\gen_" + genNum + ".png");
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
        selectedHexGrid1 = hexGridsGen[selHexGrid1];
        selectedHexGrid2 = hexGridsGen[selHexGrid2];

        for (int i = 0; i < numHexGridsPerGen; i++)
        {
            if (i == selHexGrid1 || i == selHexGrid2)
                continue;

            Destroy(hexGridsGen[i].gameObject);
        }

        hexGridsGen = new List<HexGrid>();
        hexGridsGens.Add(hexGridsGen);

        createHexGrids();
    }

}
