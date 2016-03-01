﻿namespace DCCC
{
    public class GameTile
    {
        public GameTile(CellPosition cell, uint value)
        {
            Position = cell;
            Value = value;
        }

        public MergeTile MergedFrom { get; set; }

        public void SavePosition() {
            _previousPosition = new CellPosition(Position.X, Position.Y);
        }

        public void UpdatePosition(CellPosition position)
        {
            Position = new CellPosition(position.X, Position.Y);
        }

        public CellPosition Position { get; private set; }
        public object Next { get; set; }
        public uint Value { get; private set; }
        private CellPosition _previousPosition;
    }

    public class MergeTile
    {
        public MergeTile(GameTile previous, GameTile next)
        {
            Previous = previous;
            Next = next;
        }
        public GameTile Previous { get; private set; }
        public GameTile Next { get; private set; }
    }
}