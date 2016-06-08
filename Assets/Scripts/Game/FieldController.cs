using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FieldController : MonoBehaviour {

    //Game Object vars
    public GameObject gameController;
    public GameObject redBlockPrefab;
    public GameObject yellowBlockPrefab;
    public GameObject greenBlockPrefab;
    public GameObject blueBlockPrefab;
    public GameObject purpleBlockPrefab;
    public GameObject pinkBlockPrefab;
    public GameObject whiteBlockPrefab;

    public GameController gameControllerScript;
    public FXController fxControllerScript;

    public BlockScript heldBlock;

    //Positioning Magic
    public Renderer fieldRenderer;
    public Vector3 blockCenterOffset; // = new Vector3(-4.55f, -4.5f, 0f);
    public Vector3 blockSize = new Vector3(.62f, .62f, 0f);

    public int maxCol = 6;
    public int maxRow = 12;
    int fieldBlocksi = 0;
    //BlockScript[] fieldBlocks;
    Dictionary<int, BlockScript> fieldBlocks = new Dictionary<int, BlockScript>();



    // Use this for initialization
    void Start () {
        print("-----------------------------------------------------------------------------------------------");

        //gameControllerScript = gameController.GetComponent<MonoBehaviour>() as GameController;
        //fieldBlocks = new BlockScript[maxCol * maxRow + maxCol * 2]; //Main field, plus a row for push and lose cols

        //Find the far left corner of the field sprite and add half of a block size to it to find the position of 0,0, then increase x to above the push line
        fieldRenderer = GetComponent<Renderer>();
        blockCenterOffset = fieldRenderer.bounds.min + (blockSize / 2);

        fxControllerScript = GameObject.FindGameObjectWithTag("FX").GetComponent<MonoBehaviour>() as FXController;

        CreateStartingBlocks(-1, 6);
    }

    // Update is called once per frame
    void Update () {

        //Check and toggle lose conditions
        if (gameControllerScript.isLosingPlayer == false)
        {
            foreach (KeyValuePair<int, BlockScript> kvp in fieldBlocks)
            {
                if (kvp.Value != null && kvp.Value.y >= maxRow - 1)
                {
                    kvp.Value.PrintBlock("Losing");
                    gameControllerScript.StartLose();
                    break;
                }
            }
        }
        else if (gameControllerScript.isLosingPlayer == true)
        {
            bool losing = false;

            foreach (KeyValuePair<int, BlockScript> kvp in fieldBlocks)
            {
                if (kvp.Value != null && kvp.Value.y >= maxRow - 1)
                {
                    losing = true;
                }
            }

            if (losing == false)
            {
                gameControllerScript.StopLose();
            }
        }


    }
    public Boolean IsBlockEligibleForMatch(BlockScript block)
    {
        if (block != null)
        {
            if (block.state != BlockState.Moving &&
                block.state != BlockState.Match &&
                block.state != BlockState.New &&
                block.y >= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public Boolean AreBlocksEligibleForMatch(BlockScript matchBlock, BlockScript potentialMatch)
    {
        if (IsBlockEligibleForMatch(matchBlock) == true && IsBlockEligibleForMatch(potentialMatch) == true)
        {
            if (matchBlock.color == potentialMatch.color)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public BlockScript GetMatchForBlockAtXY (BlockScript matchBlock, int x, int y)
    {
        BlockScript potentialMatchBlock = GetBlockForXY(x, y);
        if ( AreBlocksEligibleForMatch(matchBlock, potentialMatchBlock) == true )
        {
            return potentialMatchBlock;
        }
        else
        {
            return null;
        }
    }

    public bool CheckForMatchesAtSwap (BlockScript matchBlock, BlockScript swapBlock, bool autoResolve = true)
    {
        if ( swapBlock != null )
        {
            matchBlock.PrintBlock("Matches at Swap: Match", "-----");
            swapBlock.PrintBlock("Matches at Swap: Swap");
        }
        if ( CheckForMatchesAtBlock(matchBlock, autoResolve) && CheckForMatchesAtBlock(swapBlock, autoResolve) )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckForMatchesAtBlock (BlockScript matchBlock, bool autoResolve = true)
    {
        if (IsBlockEligibleForMatch(matchBlock) == true && CheckForHolesAtBlock(matchBlock) == 0)
        {
            BlockScript potentialMatchBlock;
            int potentialMatchXi = 0;
            int potentialMatchYi = 0;
            BlockScript[] fieldMatchesX = new BlockScript[12];
            BlockScript[] fieldMatchesY = new BlockScript[12];

            //matchBlock.PrintBlock("CheckforMatchesAtBlock", "------------------------------------------------------------------------");

            //Check X+1
            potentialMatchBlock = GetMatchForBlockAtXY(matchBlock, matchBlock.x + 1, matchBlock.y);
            if (potentialMatchBlock != null)
            {
                fieldMatchesX[potentialMatchXi] = potentialMatchBlock;
                potentialMatchXi++;

                //Check X+2
                potentialMatchBlock = GetMatchForBlockAtXY(matchBlock, matchBlock.x + 2, matchBlock.y);
                if (potentialMatchBlock != null)
                {
                    fieldMatchesX[potentialMatchXi] = potentialMatchBlock;
                    potentialMatchXi++;

                    //Check X+3
                    potentialMatchBlock = GetMatchForBlockAtXY(matchBlock, matchBlock.x + 3, matchBlock.y);
                    if (potentialMatchBlock != null)
                    {
                        fieldMatchesX[potentialMatchXi] = potentialMatchBlock;
                        potentialMatchXi++;
                    }
                }
            }

            //Check X-1
            potentialMatchBlock = GetMatchForBlockAtXY(matchBlock, matchBlock.x - 1, matchBlock.y);
            if (potentialMatchBlock != null)
            {
                fieldMatchesX[potentialMatchXi] = potentialMatchBlock;
                potentialMatchXi++;

                //Check X-2
                potentialMatchBlock = GetMatchForBlockAtXY(matchBlock, matchBlock.x - 2, matchBlock.y);
                if (potentialMatchBlock != null)
                {
                    fieldMatchesX[potentialMatchXi] = potentialMatchBlock;
                    potentialMatchXi++;

                    //Check X-3
                    potentialMatchBlock = GetMatchForBlockAtXY(matchBlock, matchBlock.x - 3, matchBlock.y);
                    if (potentialMatchBlock != null)
                    {
                        fieldMatchesX[potentialMatchXi] = potentialMatchBlock;
                        potentialMatchXi++;
                    }
                }
            }

            //Check Y+1
            potentialMatchBlock = GetMatchForBlockAtXY(matchBlock, matchBlock.x, matchBlock.y + 1);
            if (potentialMatchBlock != null)
            {
                fieldMatchesY[potentialMatchYi] = potentialMatchBlock;
                potentialMatchYi++;

                //Check Y+2
                potentialMatchBlock = GetMatchForBlockAtXY(matchBlock, matchBlock.x, matchBlock.y + 2);
                if (potentialMatchBlock != null)
                {
                    fieldMatchesY[potentialMatchYi] = potentialMatchBlock;
                    potentialMatchYi++;

                    //Check Y+3
                    potentialMatchBlock = GetMatchForBlockAtXY(matchBlock, matchBlock.x, matchBlock.y + 3);
                    if (potentialMatchBlock != null)
                    {
                        fieldMatchesY[potentialMatchYi] = potentialMatchBlock;
                        potentialMatchYi++;
                    }
                }
            }

            //Check Y-1
            potentialMatchBlock = GetMatchForBlockAtXY(matchBlock, matchBlock.x, matchBlock.y - 1);
            if (potentialMatchBlock != null)
            {
                if (potentialMatchBlock.y >= 0)
                {
                    fieldMatchesY[potentialMatchYi] = potentialMatchBlock;
                    potentialMatchYi++;

                    //Check y-2
                    potentialMatchBlock = GetMatchForBlockAtXY(matchBlock, matchBlock.x, matchBlock.y - 2);
                    if (potentialMatchBlock != null)
                    {
                        if (potentialMatchBlock.y >= 0)
                        {
                            fieldMatchesY[potentialMatchYi] = potentialMatchBlock;
                            potentialMatchYi++;

                            //Check y-3
                            potentialMatchBlock = GetMatchForBlockAtXY(matchBlock, matchBlock.x, matchBlock.y - 3);
                            if (potentialMatchBlock != null)
                            {
                                fieldMatchesY[potentialMatchYi] = potentialMatchBlock;
                                potentialMatchYi++;
                            }
                        }
                    }
                }
            }

            if (potentialMatchXi >= 2 && autoResolve == false || potentialMatchYi >= 2 && autoResolve == false)
            {
                return true;
            }

            //If enough matches, drop it and clear them
            if (potentialMatchXi >= 2 || potentialMatchYi >= 2)
            {
                int matchSize = 1;
                BlockScript[] fieldMatchesFinal = new BlockScript[24];
                fieldMatchesFinal[0] = matchBlock;

                if ( matchBlock.state == BlockState.Held)
                {
                    matchBlock.PutDownBlock();
                }

                if (potentialMatchXi >= 2)
                {
                    Array.Copy(fieldMatchesX, 0, fieldMatchesFinal, matchSize, fieldMatchesX.Length);
                    matchSize += potentialMatchXi;
                }
                if (potentialMatchYi >= 2)
                {
                    Array.Copy(fieldMatchesY, 0, fieldMatchesFinal, matchSize, fieldMatchesY.Length);
                    matchSize += potentialMatchYi;
                }

                MatchBlocks(fieldMatchesFinal, matchSize);
            }

        }

        
        return false;
    }

    public int CheckForHolesAtBlock(BlockScript checkBlock)
    {
        int holeDepth = checkBlock.y;

        if (checkBlock.y > 0 && 
            checkBlock.state != BlockState.Moving && 
            checkBlock.state != BlockState.Match &&
            checkBlock.state != BlockState.New)
        {
            foreach (KeyValuePair<int, BlockScript> kvp in fieldBlocks)
            {
                if (kvp.Value != null && kvp.Value != this)
                {
                    //Count how many blocks are below this to count the hole
                    if (kvp.Value.y >= 0 && kvp.Value.x == checkBlock.x && kvp.Value.y < checkBlock.y)
                    {
                        holeDepth--;
                    }
                }
            }

            return holeDepth;
        }

        return 0;
    }

    public Boolean IsBlockEligibleForSwap(BlockScript block)
    {
        if (block == null || IsBlockEligibleForMatch(block) == true )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Boolean IsXYEligibleForSwap(int x, int y)
    {
        BlockScript block = GetBlockForXY(x, y);

        if (block == null || IsBlockEligibleForMatch(block) == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public BlockScript GetBlockForXY (int x, int y)
    {
        if ( x < maxCol && y < maxRow )
        {
            foreach (KeyValuePair<int, BlockScript> kvp in fieldBlocks)
            {
                if (kvp.Value != null && kvp.Value.x == x && kvp.Value.y == y)
                {
                    return kvp.Value;
                }
            }

            return null;
        }
        else return null;
    }

    public Vector3 GetBlockPositionForFieldXY (int x, int y)
    {
        //Multiply offset by the x,y to find the spot it should be at.
        Vector3 pos = blockCenterOffset + new Vector3(blockSize.x * x, blockSize.y * y, 0);

        //print("BlockPositionForFieldXY(" + x + "," + y +"): " + pos);

        return pos;
    }

    public int GetBlockXForPosition (Vector3 pos)
    {
        int x;
        int y;

        Vector3 reversePos = pos;
        reversePos = reversePos - blockCenterOffset;
        reversePos.x = reversePos.x / blockSize.x;
        reversePos.y = reversePos.y / blockSize.y;

        y = Mathf.RoundToInt(reversePos.y);
        x = Mathf.RoundToInt(reversePos.x);
        
        //print("BlockXYForPosition(" + x + "," + y + "): " + pos);

        return x;
    }

    public int GetBlockYForPosition(Vector3 pos)
    {
        int y;
        Vector3 reversePos = pos;

        /*
        //Multiply offset by the x,y to find the spot it should be at.
        reversePos = blockCenterOffset - new Vector3(pos.x, pos.y, 0);
        reversePos.y = reversePos.y / blockSize.y + blockSize.y;
        */
        reversePos = reversePos - blockCenterOffset;
        reversePos.y = reversePos.y / blockSize.y;


        y = Mathf.RoundToInt(reversePos.y) - 2;

        return -y;
    }

    void AddBlockToFieldBlocks (BlockScript addedBlock)
    {
        fieldBlocks.Add(fieldBlocksi, addedBlock);
        addedBlock.key = fieldBlocksi;
        fieldBlocksi++;
    }

    public void RemoveBlockFromFieldBlocks(BlockScript removedBlock)
    {
        fieldBlocks.Remove(removedBlock.key);
    }

    void CreateBlock (GameObject blockPrefab, int newX, int newY, BlockState newState = BlockState.New, BlockSpecial newSpecial = BlockSpecial.None)
    {
        GameObject block = Instantiate(blockPrefab, GetBlockPositionForFieldXY(newX, newY), Quaternion.identity) as GameObject;
        block.transform.parent = this.transform;

        BlockScript blockScript = block.GetComponent<MonoBehaviour>() as BlockScript;
        blockScript.ChangeBlock(newX, newY, BlockColor.None, newState, newSpecial);

        AddBlockToFieldBlocks(blockScript);
        blockScript.fieldScript = this;

        //print("New Block: " + blockScript + ", " + blockScript.x + ", " + blockScript.y + " State: " + blockScript.state);
    }

    void CreateRandomColorOfBlock (int newX, int newY, string newSpecial = "None") //Decide a random block then call CreateBlock()
    {
        BlockColor random = RandomBlockColor();

        switch (random)
        {
            case BlockColor.Red:
                CreateBlock(redBlockPrefab, newX, newY);
                break;
            case BlockColor.Yellow:
                CreateBlock(yellowBlockPrefab, newX, newY);
                break;
            case BlockColor.Green:
                CreateBlock(greenBlockPrefab, newX, newY);
                break;
            case BlockColor.Blue:
                CreateBlock(blueBlockPrefab, newX, newY);
                break;
            case BlockColor.Purple:
                CreateBlock(purpleBlockPrefab, newX, newY);
                break;
            case BlockColor.Pink:
                CreateBlock(pinkBlockPrefab, newX, newY);
                break;
        }
    }

    public BlockColor RandomBlockColor(int maxColors = 7)
    {
        int random = UnityEngine.Random.Range(1, maxColors); //never returns max unless its == to min

        switch (random)
        {
            case 1:
                return BlockColor.Red;
            case 2:
                return BlockColor.Yellow;
            case 3:
                return BlockColor.Green;
            case 4:
                return BlockColor.Blue;
            case 5:
                return BlockColor.Purple;
            case 6:
                return BlockColor.Pink;
        }
        return BlockColor.None;
    }

    public void PrintAllBlocks()
    {
        print("Printing all blocks: " + fieldBlocks.Count + " -----------------------------------------------------");
        foreach (KeyValuePair<int, BlockScript> kvp in fieldBlocks)
        {
            if (kvp.Value != null)
            {
                kvp.Value.PrintBlock("     PrintAllBlocks", kvp.Key.ToString());
            }
        }
    }

    public void Push(GameController script)
    {
        script.PushTimeReset();
        print("PUSH!");

        if(heldBlock != null)
        {
            //This is probably annoying, but it will at least correct make sur eit doesnt mess things up further
            heldBlock.PutDownBlock();
        }

        foreach (KeyValuePair<int, BlockScript> kvp in fieldBlocks)
        {
            if (kvp.Value.state != BlockState.Match)
            {
                kvp.Value.MoveBlock();
            }
            kvp.Value.ChangeBlock(kvp.Value.x, kvp.Value.y + 1);
        }
        foreach (KeyValuePair<int, BlockScript> kvp in fieldBlocks)
        {
            if ( kvp.Value.y == 0)
            {
                CheckForMatchesAtBlock(kvp.Value);
            }
        }

        CreateBlocksAtY(-1);
    }

    void CreateBlocksAtY (int y, bool x0 = true, bool x1 = true, bool x2 = true, bool x3 = true, bool x4 = true, bool x5 = true)
    {
        for (int x = 0; x <= 5; x++)
        {
            if (x == 0 && x0)
            {
                CreateRandomColorOfBlock(x, y);
            }
            if (x == 1 && x1)
            {
                CreateRandomColorOfBlock(x, y);
            }
            if (x == 2 && x2)
            {
                CreateRandomColorOfBlock(x, y);
            }
            if (x == 3 && x3)
            {
                CreateRandomColorOfBlock(x, y);
            }
            if (x == 4 && x4)
            {
                CreateRandomColorOfBlock(x, y);
            }
            if (x == 5 && x5)
            {
                CreateRandomColorOfBlock(x, y);
            }
        }
    }

    void CreateStartingBlocks (int yMin, int yMax)
    {
        int y;

        for (y = yMin; y < yMax; y++)
        {
            CreateBlocksAtY(y);
        }
    }

    void MatchBlocks(BlockScript[] fieldMatches, int matchSize)
    {
        float delay = .25f;
        print("Match Size/Block: (" + matchSize + ") " + fieldMatches[0] + " -------------------------------------------------------");
        fxControllerScript.Invoke("Match", 0f);

        foreach (BlockScript block in fieldMatches)
        {
            delay += .25f;
            MatchBlock(block, delay);
        }

        gameControllerScript.MatchScore(matchSize);
    }

    void MatchBlock (BlockScript block, float delay)
    {
        if (block != null)
        {
            //print("     " + block + " " + block.x + ", " + block.y);
            block.state = BlockState.Match;
            fxControllerScript.Invoke("BlockMatch", delay);
            block.Invoke("MatchBlockResolve", gameControllerScript.matchDelay);

            block.blockRenderer.material.color = new Color(.75f, .75f, .75f, .5f);
        }
    }
}
