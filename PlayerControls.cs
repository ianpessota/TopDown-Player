using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PorterGame
{
   public enum MoveTo
    {
        Up,
        Left,
        Right,
        Down,
        None
    }
    public class PlayerControls : MonoBehaviour
    {
        public Vector3 StartPos;
        // Use this for initialization
        Vector3 position;
        float interp = 0.1f;
        float offset = 0.1f;
        RaycastHit2D rayGround;
        Vector3 direction;
        bool canMove = true;
        bool disableControls =false;
        public LevelManager levelManager;


        public bool DisableControls
        {
            get { return disableControls; }
            set { disableControls = value; }
        }
        public bool CanMove
        {
            get { return canMove; }
            set { canMove = value; }
        }
        MoveTo moveTo = MoveTo.None;
        void Start()
        {
            disableControls = false;
            this.transform.position = StartPos;
        }
        // Update is called once per frame
        void Update()
        {
            if(!disableControls)
            getInput();
            //Do movement
            SetAnimation(moveTo);

            if (canMove)
            {
                switch (moveTo)
                {
                    case MoveTo.None:
                      
                        return;
                    case MoveTo.Down:
                        Move();
                        return;
                    case MoveTo.Up:
                        Move();
                        return;
                    case MoveTo.Left:
                        Move();
                        return;
                    case MoveTo.Right:
                        Move();
                        return;
                }
            }
            else moveTo = MoveTo.None;
        }
        void getInput()
        {
            if (moveTo == MoveTo.None)//only start moving when he is idle
            {
                if(Input.GetKey(KeyCode.Escape))
                {
                    levelManager.sceneManager.GetComponent<SceneSwitch>().showMenu();
                    disableControls = true;
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    SetMovement(MoveTo.Down);               
                }
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    SetMovement(MoveTo.Up);
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    SetMovement(MoveTo.Right);
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    SetMovement(MoveTo.Left);
                }
            }
        }
        void SetMovement(MoveTo dir)
        {
            RoundPosition();
            if (dir == MoveTo.Down)
            {
                position = transform.position;
                position += Vector3.down;
                direction = Vector3.down;
                moveTo = MoveTo.Down;
            }
            if (dir == MoveTo.Up)
            {
                position = transform.position;
                position += Vector3.up;
                direction = Vector3.up;
                moveTo = MoveTo.Up;
            }
            if (dir == MoveTo.Right)
            {
                position = transform.position;
                position += Vector3.right;
                direction = Vector3.right;
                moveTo = MoveTo.Right;
            }
            if (dir == MoveTo.Left)
            {
                position = transform.position;
                position += Vector3.left;
                direction = Vector3.left;
                moveTo = MoveTo.Left;
            }
            rayGround = Physics2D.Raycast(this.transform.position, direction, 1.0f, 1 << LayerMask.NameToLayer("Ground"));
            if (rayGround.collider != null)
            {
                canMove = false;
            }
            else
                canMove = true;
        }
        //Move till next specified position on grid
        void Move()
        {
            levelManager.UpdateCheckers();
            if(levelManager.isLevelDone())
            {
                disableControls = true;
            }
            if (position != this.transform.position)
            {
                if (Vector3.Distance(this.transform.position, position) >= offset)
                    this.transform.position = Vector3.Lerp(this.transform.position, position, interp);
                else
                {
                    this.transform.position = position;
            
                }
            }
            else
            {
                moveTo = MoveTo.None;
                RoundPosition();
            }
        }
        void SetAnimation(MoveTo anim)
        { 
            GetComponent<Animator>().SetInteger("State", (int)anim);
        }
        void RoundPosition()
        {
            Vector3 pos = this.transform.position;
            pos.x = Mathf.Round(pos.x * 2) / 2f;
            pos.y = Mathf.Round(pos.y * 2) / 2f;
            this.transform.position = pos;
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            rayGround = Physics2D.Raycast(this.transform.position, direction, 1.0f, 1 << LayerMask.NameToLayer("Object"));
            if (collision.gameObject.layer == LayerMask.NameToLayer("Object"))
            {
                if (rayGround.collider != null)
                {
                    if (rayGround.transform.gameObject.GetComponent<MovableObjects>().isMovable(direction))
                    {
                        canMove = true;
                    }                    
                    else
                        canMove = false;
                }
            }
        }
    } 
}
