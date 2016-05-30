using UnityEngine;
using System.Collections;
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
    BlockScript[] fieldBlocks;




    // Use this for initialization
    void Start () {
        print("-----------------------------------------------------------------------------------------------");

        //gameControllerScript = gameController.GetComponent<MonoBehaviour>() as GameController;
        fieldBlocks = new BlockScript[maxCol * maxRow + maxCol * 2]; //Main field, plus a row for push and lose cols

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
            foreach (BlockScript block in fieldBlocks)
            {
                if (block != null && block.y >= maxRow - 1)
                {
                    print(block + ", " + block.x + ", " + block.y);
                    gameControllerScript.StartLose();
                    break;
                }
            }
        }
        else if (gameControllerScript.isLosingPlayer == true)
        {
            bool losing = false;

            foreach (BlockScript block in fieldBlocks)
            {
                if (block != null && block.y >= maxRow - 1)
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
    public BlockScript GetMatchForBlockAtXY (BlockScript matchBlock, int x, int y)
    {
        BlockScript potentialMatchBlock = GetBlockForXY(x, y);
        if (potentialMatchBlock != null && potentialMatchBlock.color == matchBlock.color)
        {
            return potentialMatchBlock;
        }
        else
        {
            return null;
        }
    }

    public bool CheckForMatchesAtBlock (BlockScript matchBlock, bool autoResolve = true)
    {
        if (matchBlock.isMoving == false && CheckForHolesAtBlock(matchBlock) == 0)
        {
            BlockScript potentialMatchBlock;
            int potentialMatchXi = 0;
            int potentialMatchYi = 0;
            BlockScript[] fieldMatchesX = new BlockScript[6];
            BlockScript[] fieldMatchesY = new BlockScript[6];

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
                }
            }

            //Check X-1
            potentialMatchBlock = GetMatchForBlockAtXY(matchBlock, matchBlock.x - 1, matchBlock.y);
            if (potentialMatchBlock != null)
            {
                fieldMatchesX[potentialMatchXi] = potentialMatchBlock;
                potentialMatchXi++;

                //Check X+2
                potentialMatchBlock = GetMatchForBlockAtXY(matchBlock, matchBlock.x - 2, matchBlock.y);
                if (potentialMatchBlock != null)
                {
                    fieldMatchesX[potentialMatchXi] = potentialMatchBlock;
                    potentialMatchXi++;
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
                BlockScript[] fieldMatchesFinal = new BlockScript[12];
                fieldMatchesFinal[0] = matchBlock;

                if ( matchBlock.isHeld == true)
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

        if (checkBlock.y > 0 && checkBlock.isMoving == false)
        {
            foreach (BlockScript block in fieldBlocks)
            {
                if (block != null && block != this)
                {
                    //Count how many blocks are below this to count the hole
                    if (block.y >= 0 && block.x == checkBlock.x && block.y < checkBlock.y)
                    {
                        holeDepth--;
                    }
                }
            }

            return holeDepth;

        }

        return 0;
    }

    public BlockScript GetBlockForXY (int x, int y)
    {
        if ( x < maxCol && y < maxRow )
        {
            foreach (BlockScript block in fieldBlocks)
            {
                if (block != null && block.x == x && block.y == y)
                {
                    return block;
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
        fieldBlocks[fieldBlocksi] = addedBlock;
        fieldBlocksi++;
    }

    public void RemoveBlockFromFieldBlocks(BlockScript removedBlock)
    {
        int removedBlocki = -1;
        foreach (BlockScript block in fieldBlocks)
        {
            if (block != null && block == removedBlock)
            {
                //print(block + " " + block.x + ", " + block.y);
                removedBlocki = System.Array.IndexOf(fieldBlocks, block);
                break;
            }
        }
        if ( removedBlocki > -1)
        {
            fieldBlocks[fieldBlocksi] = null;
            fieldBlocksi--;
        }
    }

    void CreateBlock (GameObject blockPrefab, int newX, int newY, string newSpecial = "None")
    {
        GameObject block = Instantiate(blockPrefab, GetBlockPositionForFieldXY(newX, newY), Quaternion.identity) as GameObject;
        block.transform.parent = this.transform;

        BlockScript blockScript = block.GetComponent<MonoBehaviour>() as BlockScript;
        blockScript.ChangeBlock(newX, newY, "New", "New");

        AddBlockToFieldBlocks(blockScript);
        blockScript.fieldScript = this;
        //print("New Block: " + fieldBlocks[newX, newY] + ", " + blockScript.x + ", " + blockScript.y);

        if ( newSpecial == "Starting")
        {
            //blockScript.ChangeColorToNoMatches();
        }
    }

    void CreateRandomColorOfBlock (int newX, int newY, string newSpecial = "None") //Decide a random block then call CreateBlock()
    {
        string random = RandomBlockColor();

        switch (random)
        {
            case "Red":
                CreateBlock(redBlockPrefab, newX, newY, "Starting");
                break;
            case "Yellow":
                CreateBlock(yellowBlockPrefab, newX, newY, "Starting");
                break;
            case "Green":
                CreateBlock(greenBlockPrefab, newX, newY, "Starting");
                break;
            case "Blue":
                CreateBlock(blueBlockPrefab, newX, newY, "Starting");
                break;
            case "Purple":
                CreateBlock(purpleBlockPrefab, newX, newY, "Starting");
                break;
            case "Pink":
                CreateBlock(pinkBlockPrefab, newX, newY, "Starting");
                break;
        }
    }

    public string RandomBlockColor(int maxColors = 7)
    {
        int random = UnityEngine.Random.Range(1, maxColors); //never returns max unless its == to min

        switch (random)
        {
            case 1:
                return "Red";
            case 2:
                return "Yellow";
            case 3:
                return "Green";
            case 4:
                return "Blue";
            case 5:
                return "Purple";
            case 6:
                return "Pink";
        }
        return null;
    }

    public void PrintAllBlocks()
    {
        int i = 0;
        print("Printing all blocks: " + fieldBlocksi + " -----------------------------------------------------");
        foreach (BlockScript block in fieldBlocks)
        {
            if (block != null)
            {
                block.PrintBlock("PrintAllBlocks", i.ToString());
                i++;
            }
        }
    }

    public void Push(GameController script)
    {
        script.PushTimeReset();

        if(heldBlock != null)
        {
            //This is probably annoying, but it will at least correct make sur eit doesnt mess things up further
            heldBlock.PutDownBlock();
        }

        foreach (BlockScript block in fieldBlocks)
        {
            if (block != null)
            {
                block.MoveBlock();
                block.ChangeBlock(block.x, block.y + 1);
                block.StopBlock();
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
            print(block + " " + block.x + ", " + block.y);
            block.isMatch = true;
            fxControllerScript.Invoke("BlockMatch", delay);
            block.Invoke("MatchBlockResolve", gameControllerScript.matchDelay);
            block.blockRenderer.material.color = new Color(.75f, .75f, .75f, .5f);
        }
    }
}
