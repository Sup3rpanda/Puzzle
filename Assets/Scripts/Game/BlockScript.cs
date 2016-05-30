using UnityEngine;
using System.Collections;
using System;

public class BlockScript : MonoBehaviour {

    //Game Object vars
    public string color;
    public string special;

    public FieldController fieldScript;
    public SpriteRenderer blockRenderer;
    public FXController fxControllerScript;

    //Internal tags on status
    public int x;
    public int y;
    public bool isHeld = false;
    public bool isMoving = false;
    public bool isMatch = false;

    // Use this for initialization
    void Start() {

        fxControllerScript = GameObject.FindGameObjectWithTag("FX").GetComponent<MonoBehaviour>() as FXController;
        blockRenderer = this.GetComponentInParent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update () {

        //Tint blocks below or above the push line
        if (isMatch != true && y < 0 && blockRenderer != null)
        {
            blockRenderer.material.color = new Color(.5f, .5f, .5f);
        }
        else if (isMatch != true && y >= 0 && blockRenderer != null)
        {
            blockRenderer.material.color = new Color(1f, 1f, 1f);
        }

        //If held, move it around and check for matches, swaps or a hole
        if (isHeld == true)
        {
            float xMovement = Input.GetAxis("Mouse X") * 12f * Time.deltaTime;
            int xForPosition = fieldScript.BlockXForPosition(transform.position);

            //print("Held block (x,y),(pX): (" + x + ", " + y + ") (" + xForPosition + ")");

            if ( xMovement > 0 )
            {
                if (x < 5)
                {
                    transform.Translate(xMovement, 0f, 0f);
                    if (xForPosition != x)
                    {
                        ChangeBlock(xForPosition, y);
                    }
                }
            }
            else if ( xMovement < 0 )
            {
                if (x > 0)
                {
                    transform.Translate(xMovement, 0f, 0f);
                    if (xForPosition != x)
                    {
                        ChangeBlock(xForPosition, y);
                    }
                }
            }

            DropBlock(fieldScript.CheckForHolesAtBlock(this));

            fieldScript.CheckForMatchesAtBlock(this);

        }

        //Check for hole, then drop it
        if (isMatch == false && isMoving == false)
        {
            DropBlock(fieldScript.CheckForHolesAtBlock(this));
        }
    }

    #region Input
    void OnMouseDown()
    {
        PickUpBlock();
    }

    void OnMouseUp()
    {
        if (isHeld == true)
        {
            PutDownBlock();
        }
    }

    #endregion

    #region Movement

    public void DropBlock(int holeDepth)
    {
        if (holeDepth > 0)
        {
            //PrintBlock("Drop");

            PutDownBlock();
            MoveBlock();
            Invoke("DropBlockResolve", .075f);
        }
    }

    public void DropBlockResolve()
    {
        ChangeBlock(x, y - 1);
        StopBlock();
        fieldScript.CheckForMatchesAtBlock(this);
    }

    public void PickUpBlock()
    {
        if (isMatch == false)
        {
            isHeld = true;
            fieldScript.heldBlock = this;

            this.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            blockRenderer.sortingOrder = 5;
        }
    }

    public void PutDownBlock()
    {
        isHeld = false;
        isMoving = false;
        fieldScript.heldBlock = null;

        this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        blockRenderer.sortingOrder = 0;


        MoveBlock();
        ChangeBlock(x, y);
        StopBlock();
    }

    public void MoveBlock()
    {
        isMoving = true;
    }

    public void StopBlock()
    {
        isMoving = false;
        //CheckForMatches();
    }
    #endregion

    #region Field Evaluation and Matching

    public void PrintBlock(string op, string overload = "")
    {
        print(op + ": " + this + " (" + x + ", " + y + ")" + " " + overload);
    }

    public void CheckForSwap(int xSwapFrom, int xSwapTo, int atY)
    {
        BlockScript swapBlock = fieldScript.GetBlockForXY(xSwapFrom, atY);
        if (swapBlock != null && swapBlock != this)
        {
            swapBlock.ChangeBlock(xSwapTo, y);
            fxControllerScript.SendMessage("BlockSwap");
            fieldScript.CheckForMatchesAtBlock(this);
        }

    }

    public void ChangeBlock(int newX, int newY, string newColor = "None", string newSpecial = "None")
    {
        //print("ChangeBlock: " + this + " (" + x + "/" + newX  + "," + y + "/" + newY + ") held: " + isHeld + " isMov: " + isMoving);

        if ( isHeld == true ) //Update held blocks
        {
            CheckForSwap(newX, x, y);
        }
        else if (isMoving == true) //Update held blocks PutDown()
        {
            transform.Translate(Vector3.MoveTowards(transform.position, fieldScript.BlockPositionForFieldXY(newX, newY), 6f) - transform.position);
        }
        else if ( isHeld == false && newSpecial != "New" ) //Move existing blocks
        {
            transform.Translate(fieldScript.BlockPositionForFieldXY(newX, newY) - fieldScript.BlockPositionForFieldXY(x, y));
            
            x = newX;
            y = newY;
            fieldScript.CheckForMatchesAtBlock(this);
        }


        //Update all the variables
        if (x != newX || y != newY)
        {
            x = newX;
            y = newY;

        }
        if (newColor != "None" && color != newColor)
        {
            if (newColor != "New")
            {
                color = newColor;

                switch (newColor)
                {
                    case "Red":
                        blockRenderer.sprite = Resources.Load("Sprites/Blocks/Red", typeof(Sprite)) as Sprite;
                        break;
                    case "Yellow":
                        blockRenderer.sprite = Resources.Load("Sprites/Blocks/Yellow", typeof(Sprite)) as Sprite;
                        break;
                    case "Green":
                        blockRenderer.sprite = Resources.Load("Sprites/Blocks/Green", typeof(Sprite)) as Sprite;
                        break;
                    case "Blue":
                        blockRenderer.sprite = Resources.Load("Sprites/Blocks/Blue", typeof(Sprite)) as Sprite;
                        break;
                    case "Purple":
                        blockRenderer.sprite = Resources.Load("Sprites/Blocks/Purple", typeof(Sprite)) as Sprite;
                        break;
                    case "Pink":
                        blockRenderer.sprite = Resources.Load("Sprites/Blocks/Pink", typeof(Sprite)) as Sprite;
                        break;
                }
            }
        }
        if (newSpecial != "None" && special != newSpecial)
        {
            if (newSpecial != "New")
            {
                special = newSpecial;
            }
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
            ChangeBlock(x, y, fieldScript.RandomBlockColor(),"New");
        }

        ChangeColorToNoMatches();
    }


    #endregion

}
