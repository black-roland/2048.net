﻿using DCCC.Interfaces;
using System;
using Xamarin.Forms;

namespace DCCC.XF
{
    public class GamePage : ContentPage
    {
        private Grid _mainGrid;
        private GameGrid _gameGrid;
        private GameHeader _gameHeader;

        public GamePage()
        {
            Appearing += (s, e) =>
            {
                

                _mainGrid = new Grid
                {
                    BackgroundColor = Color.Transparent, // Necessary for PanGestureRecognizer to read pans outside content
                    RowSpacing = 0,
                    ColumnSpacing = 0
                };

                BackgroundColor = Color.Black;

                _gameHeader = new GameHeader();

                _gameGrid = new GameGrid(CalculateGridSize(), 4);
                _gameGrid.VerticalOptions = _gameGrid.HorizontalOptions = LayoutOptions.Center;

                _mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                _mainGrid.RowDefinitions.Add(new RowDefinition());

                _mainGrid.Children.Add(_gameHeader);
                Grid.SetRow(_gameGrid, 1);
                _mainGrid.Children.Add(_gameGrid);

                _mainGrid.Children.Add(BuildKeyboardInputEntry());
                Content = _mainGrid;
                Content.GestureRecognizers.Add(BuildSwipeRecognizer());

                Ready?.Invoke(this, EventArgs.Empty);
            };
        }

        private double CalculateGridSize()
        {
            if (Width > Height)
                return Height * .8;

            return Width * .9;
        }

        private PanGestureRecognizer BuildSwipeRecognizer()
        {
            var panGesture = new PanGestureRecognizer();
            var totalX = .0;
            var totalY = .0;
            panGesture.PanUpdated += (s, e) =>
            {
                switch (e.StatusType)
                {
                    case GestureStatus.Started:
                        break;
                    case GestureStatus.Running:
                        totalX += e.TotalX;
                        totalY += e.TotalY;
                        break;
                    case GestureStatus.Completed:
                        if (Math.Abs(totalX) > Math.Abs(totalY))
                            Move(totalX > 0 ? MoveDirection.Right : MoveDirection.Left);
                        else
                            Move(totalY > 0 ? MoveDirection.Down : MoveDirection.Up);

                        totalX = totalY = 0;
                        break;
                    case GestureStatus.Canceled:
                        break;
                    default:
                        break;
                }
            };

            return panGesture;
        }

        private Entry BuildKeyboardInputEntry()
        {
            var kbInput = new Entry
            {
                Opacity = 0,
                HeightRequest = 1,
                WidthRequest = 1,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start
            };

            kbInput.Unfocused += (s, e) => kbInput.Focus();
            kbInput.TextChanged += (s, e) =>
            {
                var t = e.NewTextValue.ToUpper();
                switch (t)
                {
                    case "A":
                        Move(MoveDirection.Left);
                        break;
                    case "W":
                        Move(MoveDirection.Up);
                        break;
                    case "D":
                        Move(MoveDirection.Right);
                        break;
                    case "S":
                        Move(MoveDirection.Down);
                        break;
                }

                kbInput.Text = string.Empty;
            };

            kbInput.Focus();
            return kbInput;
        }

        internal void Update(IGameState gameState)
        {
            _gameGrid.Update(gameState.Grid.Cells);
            _gameHeader.Update(gameState.Score, gameState.Score);
        }

        public event EventHandler<MoveEventArgs> Moved;

        private void Move(MoveDirection direction)
        {
            Moved?.Invoke(this, new MoveEventArgs(direction));
        }

        internal event EventHandler Ready;
    }

    public class MoveEventArgs : EventArgs
    {
        public MoveEventArgs(MoveDirection direction)
        {
            Direction = direction;
        }
        public MoveDirection Direction { get; private set; }
    }
}