using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class Snake : MonoBehaviour
{
    private enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }
    private enum State {
    Alive,
    Dead
    }
    private State state;
    private Direction GridMoveDirection;
    private Vector2Int GridPosition;
    private float GridMoveTimer;
    private float GridMoveTimerMax;
    private LevelGrid levelGrid;
    private int snakeBodySize;
    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> SnakeBodyPartList;
    public void Setup(LevelGrid levelGrid)
    {
        this.levelGrid = levelGrid;
    }
    private void Awake()
    {
        GridPosition = new Vector2Int(10, 10);
        GridMoveTimerMax = .125f;
        GridMoveTimer = 0f;
        GridMoveDirection = Direction.Right;
        snakeBodySize = 0;
        snakeMovePositionList = new List<SnakeMovePosition>();
        SnakeBodyPartList = new List<SnakeBodyPart>();
        state = State.Alive;
    }
    private void Update()
    {
        switch (state)
        {
            case State.Alive:
                HandleInput();
                HandleGridMovement();
                break;
            case State.Dead:
                break;

    }
    }
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (GridMoveDirection != Direction.Down)
            {
                GridMoveDirection = Direction.Up;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (GridMoveDirection != Direction.Up)
            {
                GridMoveDirection = Direction.Down;
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (GridMoveDirection != Direction.Left)
            {
                GridMoveDirection = Direction.Right;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (GridMoveDirection != Direction.Right)
            {
                GridMoveDirection = Direction.Left;
            }
        }
    }
    private void HandleGridMovement()
    {
        GridMoveTimer += Time.deltaTime;
        if (GridMoveTimer >= GridMoveTimerMax)
        {
            GridMoveTimer -= GridMoveTimerMax;
            SnakeMovePosition previousSnakeMovePosition = null;
            if (snakeMovePositionList.Count > 0)
            {
                previousSnakeMovePosition = snakeMovePositionList[0];
            }
            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition,GridPosition, GridMoveDirection);
            snakeMovePositionList.Insert(0, snakeMovePosition);
            Vector2Int gridMoveDirectionVector;
            switch (GridMoveDirection)
            {
                default:
                case Direction.Right: gridMoveDirectionVector = new Vector2Int(1, 0); break;
                case Direction.Left: gridMoveDirectionVector = new Vector2Int(-1, 0); break;
                case Direction.Up: gridMoveDirectionVector = new Vector2Int(0, 1); break;
                case Direction.Down: gridMoveDirectionVector = new Vector2Int(0, -1); break;
            }

            GridPosition += gridMoveDirectionVector;
            GridPosition= levelGrid.ValidateGridPosition(GridPosition); 
            bool snakeAteFood = levelGrid.TrySnakeEatFood(GridPosition);
            if (snakeAteFood)
            {
                snakeBodySize++;
                if(GridMoveTimerMax>0.07f)
                    GridMoveTimerMax -= 0.0025f;
                CreateSnakeBody();
            }
            if (snakeMovePositionList.Count >= snakeBodySize + 1)
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }
            UpdateSnakeBodyparts();

            foreach (SnakeBodyPart snakeBodyPart in SnakeBodyPartList)
            {
                Vector2Int snakeBodyPartGridPosition = snakeBodyPart.GetGridPosition();
                if(GridPosition==snakeBodyPartGridPosition)
                {
                    state = State.Dead;
                    GameHandler.SnakeDied();
                }
            }
            transform.position = new Vector3(GridPosition.x, GridPosition.y);
            transform.eulerAngles = new Vector3(0, 0, GetAnglefromVector(gridMoveDirectionVector) - 90);
        }
    }
    private void CreateSnakeBody()
    {
        SnakeBodyPartList.Add(new SnakeBodyPart(SnakeBodyPartList.Count));
    }
    private void UpdateSnakeBodyparts()
    {
        for (int i = 0; i < SnakeBodyPartList.Count; i++)
            SnakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
    }
    public Vector2Int GetGridPosition()
    {
        return GridPosition;
    }
    public List<Vector2Int> GetFullSnakePositionList()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { GridPosition };
        foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionList)
        {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
        return gridPositionList;
    }
    private float GetAnglefromVector(Vector2Int dir)
    {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0)
            n += 360;
        return n;
    }
    private class SnakeBodyPart
    {
        private Transform transform;
        private SnakeMovePosition snakeMovePosition;
        public SnakeBodyPart(int bodyIndex)
        {
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeBodySprite;
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
            transform = snakeBodyGameObject.transform;
        }
        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition)
        {
            this.snakeMovePosition = snakeMovePosition;
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);
            float angle;
            switch (snakeMovePosition.ReturnDirection())
            {
                default:
                case Direction.Up:
                    switch(snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 0;
                        break;
                        case Direction.Left:
                    angle =0+ 45;
                    break;
                case Direction.Right:
                            angle = 0-45;
                    break;
            }
            break;
                case Direction.Down:
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 180;
                            break;
                        case Direction.Left:
                            angle = 135;
                            break;
                        case Direction.Right:
                            angle = 45;
                            break;
                    }
                    break;
                case Direction.Left:
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = -90;
                            break;
                        case Direction.Down:
                            angle = 135;
                            break;
                        case Direction.Up:
                            angle = 45;
                            break;
                    }
                    break;
                case Direction.Right:
                    switch(snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 90;
                            break;
                        case Direction.Down:
                            angle = 45;
                            break;
                        case Direction.Up:
                            angle = -45;
                            break;
                    }
                    break;
            }
            transform.eulerAngles = new Vector3(0, 0, angle);
        }
        public Vector2Int GetGridPosition()
        {
            return snakeMovePosition.GetGridPosition();
        }
    }
    private class SnakeMovePosition
    {
        private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;
        public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector2Int gridPosition, Direction direction)
        {
            this.previousSnakeMovePosition = previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }
        public Vector2Int GetGridPosition()
        {
            return gridPosition;
        }
        public Direction ReturnDirection()
        {
            return direction;
        }
        public Direction GetPreviousDirection()
        {
            if (previousSnakeMovePosition == null)
            {
                return Direction.Right;
            }
            else
            {
                return previousSnakeMovePosition.direction;
            }
        }

    }
}