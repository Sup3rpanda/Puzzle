﻿using UnityEngine;
using System.Collections;
using System;

public enum BlockState { None, New, Moving, Held, Swap, Match }; //None is the default not doing anything state
public enum BlockColor { None, Red, Yellow, Green, Blue, Purple, Pink };
public enum BlockSpecial { None };

public class BlockScript : MonoBehaviour {

    //Game Object vars
    public FieldController fieldScript;
    public SpriteRenderer blockRenderer;
    public FXController fxControllerScript;

    //Internal tags on status
    public int key;
    public int x;
    public int y;
    public BlockColor color;
    public BlockSpecial special;
    public BlockState state;

    //Timing
    float dropDelay = .1f;

    // Use this for initialization
    void Start() {

        fxControllerScript = fieldScript.fxControllerScript;
        blockRenderer = this.GetComponentInParent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update () {

        //Tint blocks below or above the push line
        if (y < 0 && blockRenderer != null)
        {
            blockRenderer.material.color = new Color(.5f, .5f, .5f);
        }
        else if (state != BlockState.Match && state != BlockState.Moving && blockRenderer != null)
        {
            blockRenderer.material.color = new Color(1f, 1f, 1f);
        }
        
        //If held, move it around and check for matches, swaps or a hole
        if (state == BlockState.Held)
        {
            float xMovement = Input.GetAxis("Mouse X") * 12f * Time.deltaTime;
            int xForPosition = fieldScript.GetBlockXForPosition(transform.position);
            BlockScript swapBlock = null;

            //print("Held block (x,y),(pX): (" + x + ", " + y + ") (" + xForPosition + ")");

            if ( xMovement > 0 ) //Moving right
            {
                if (x < 5 )
                {
                    swapBlock = fieldScript.GetBlockForXY(x + 1, y);

                    if ( fieldScript.IsBlockEligibleForSwap(swapBlock) == true)
                    {
                        transform.Translate(xMovement, 0f, 0f);
                        if (xForPosition != x)
                        {
                            if (swapBlock != null)
                            {
                                swapBlock.SwapBlock(x, y);
                            }

                            ChangeBlock(xForPosition, y);
                        }
                    }
                }
            }
            else if ( xMovement < 0 ) //Moving Left
            {
                if (x > 0)
                {
                    swapBlock = fieldScript.GetBlockForXY(x - 1, y);

                    if (fieldScript.IsBlockEligibleForSwap(swapBlock) == true)
                    {
                        transform.Translate(xMovement, 0f, 0f);
                        if (xForPosition != x)
                        {
                            if (swapBlock != null)
                            {
                                swapBlock.SwapBlock(x, y);
                            }
                            ChangeBlock(xForPosition, y);
                        }
                    }
                }
            }

            DropBlock(fieldScript.CheckForHolesAtBlock(this));
            fieldScript.CheckForMatchesAtBlock(this, true);
            //fieldScript.CheckForMatchesAtSwap(this, swapBlock, true);

        }

        //Check for hole, then drop it
        if ( state == BlockState.None )
        {
            DropBlock(fieldScript.CheckForHolesAtBlock(this));
        }
    }

    #region Input
    void OnMouseDown()
    {
        if (fieldScript.gameControllerScript.IsPlayable() == true)
        {
            PickUpBlock();
        }
    }

    void OnMouseUp()
    {
        PutDownBlock();
    }

    #endregion

    #region Movement

    public void DropBlock(int holeDepth)
    {
        if (holeDepth > 0)
        {
            //PrintBlock("Drop", holeDepth.ToString());

            PutDownBlock();
            MoveBlock();
            Invoke("DropBlockResolve", dropDelay);
        }
    }

    public void DropBlockResolve()
    {
        ChangeBlock(x, y - 1);
        int holes = fieldScript.CheckForHolesAtBlock(this);
        if ( holes == 0)
        {
            fxControllerScript.SendMessage("BlockDrop");
            Invoke("CheckForMatchesAfterDrop", dropDelay);
        }
        else
        {
            DropBlock(holes);
        }
    }

    void CheckForMatchesAfterDrop()
    {
        fieldScript.CheckForMatchesAtBlock(this, true, true);
    }

    public void PickUpBlock()
    {
        if ( state == BlockState.None )
        {
            state = BlockState.Held;
            fieldScript.heldBlock = this;

            this.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            blockRenderer.sortingOrder = 5;
        }
    }

    public void PutDownBlock()
    {
        if (state == BlockState.Held)
        {
            fieldScript.heldBlock = null;
            this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            blockRenderer.sortingOrder = 0;

            MoveBlock();
            ChangeBlock(x, y);
        }
        if (state == BlockState.Match)
        {
            fieldScript.heldBlock = null;
            this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            blockRenderer.sortingOrder = 0;
        }
    }

    public void MoveBlock()
    {
        state = BlockState.Moving;
    }

    public void StopBlock()
    {
        state = BlockState.None;
        //CheckForMatches();
    }
    #endregion

    #region Field Evaluation and Matching

    public void PrintBlock(string op, string overload = "")
    {
        print(op + ": " + this + " (" + x + ", " + y + ")" + " State: " + state + " | "
            + overload);
    }

    public void SwapBlock(int xSwapTo, int atY)
    {
        //PrintBlock("Swap", " To: (" + xSwapTo + "," + atY + ")");

        state = BlockState.Swap;
        ChangeBlock(xSwapTo, y);
    }

    void SetBlockXY (int newX, int newY)
    {
        if (x != newX || y != newY)
        {
            x = newX;
            y = newY;
        }
    }

    void SetBlockColor (BlockColor newColor)
    {
        if (newColor != BlockColor.None && color != newColor )
        {
            color = newColor;

            if (blockRenderer != null)
            {
                switch (newColor)
                {
                    case BlockColor.Red:
                        blockRenderer.sprite = Resources.Load("Sprites/Blocks/Red", typeof(Sprite)) as Sprite;
                        break;
                    case BlockColor.Yellow:
                        blockRenderer.sprite = Resources.Load("Sprites/Blocks/Yellow", typeof(Sprite)) as Sprite;
                        break;
                    case BlockColor.Green:
                        blockRenderer.sprite = Resources.Load("Sprites/Blocks/Green", typeof(Sprite)) as Sprite;
                        break;
                    case BlockColor.Blue:
                        blockRenderer.sprite = Resources.Load("Sprites/Blocks/Blue", typeof(Sprite)) as Sprite;
                        break;
                    case BlockColor.Purple:
                        blockRenderer.sprite = Resources.Load("Sprites/Blocks/Purple", typeof(Sprite)) as Sprite;
                        break;
                    case BlockColor.Pink:
                        blockRenderer.sprite = Resources.Load("Sprites/Blocks/Pink", typeof(Sprite)) as Sprite;
                        break;
                }
            }
        }

    }

    void SetBlockSpecial ( BlockSpecial newSpecial)
    {
        if (special != newSpecial)
        {
            special = newSpecial;
        }
    }

    public void ChangeBlock(int newX, int newY, BlockColor newColor = BlockColor.None, BlockState newState = BlockState.None, BlockSpecial newSpecial = BlockSpecial.None)
    {
        //PrintBlock("ChangeBlock", " (" + x + "/" + newX  + "," + y + "/" + newY + ") State: " + state + "/" + newState);

        if ( state == BlockState.Held ) //Update held blocks
        {
            SetBlockXY(newX, newY);
            fxControllerScript.SendMessage("BlockSwap");
            //Dont change state
        }
        else if ( state == BlockState.Moving ) //Update moving blocks
        {
            transform.Translate(Vector3.MoveTowards(transform.position, fieldScript.GetBlockPositionForFieldXY(newX, newY), 6f) - transform.position);

            SetBlockXY(newX, newY);
            StopBlock();
        }
        else if ( state == BlockState.Match)
        {
            transform.Translate(Vector3.MoveTowards(transform.position, fieldScript.GetBlockPositionForFieldXY(newX, newY), 6f) - transform.position);

            SetBlockXY(newX, newY);
        }
        else if ( state == BlockState.Swap ) //Move swapped blocks, check for matches then set them back
        {
            SetBlockXY(newX, newY);


            transform.Translate(Vector3.MoveTowards(transform.position, fieldScript.GetBlockPositionForFieldXY(newX, newY), 6f) - transform.position);
            fieldScript.CheckForMatchesAtBlock(this);

            //PrintBlock("ChangeBlock:Swap");
            StopBlock();
        }
        else if (newState == BlockState.New) //set new blocks
        {
            SetBlockXY(newX, newY);
            SetBlockColor(newColor);

            StopBlock();
        }
    }

    public void MatchBlockResolve()
    {
        fieldScript.RemoveBlockFromFieldBlocks(this);
        Destroy(this.gameObject);
    }

    public void ChangeColorToNoMatches()
    {
        if ( fieldScript.CheckForMatchesAtBlock(this, false) == true)
        {
            ChangeBlock(x, y, fieldScript.RandomBlockColor(), BlockState.New );
        }

        ChangeColorToNoMatches();
    }
    #endregion

}
