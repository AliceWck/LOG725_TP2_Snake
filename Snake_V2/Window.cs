using System;
using System.Drawing;
using System.Windows.Forms;
using Snake.Patterns.Command;
using Snake.Patterns.Observer;
using Snake;

namespace Snake
{
    public partial class Window : Form
    {
        private const int WIDTH = 12;
        private const int HEIGHT = 16;
        private const string SCORE_STRING = "Score: {0}";
        
        private readonly Color m_BackgroundColor = Color.CornflowerBlue;
        private readonly Game m_Game;
        private readonly Bitmap m_GameField;
        private readonly Graphics m_GameGraphics;
        
        // Patron Command
        private readonly InputHandler m_InputHandler;
        
        // Patron Observer
        private UIObserver m_UIObserver;

        public Window()
        {
            InitializeComponent();
            
            // Initialiser le bitmap et graphics
            m_GameField = new Bitmap(WIDTH * Piece.SIDE, HEIGHT * Piece.SIDE);
            m_GameGraphics = Graphics.FromImage(m_GameField);
            m_GameGraphics.PageUnit = GraphicsUnit.Pixel;
            
            ClientSize = new Size(m_GameField.Width, m_GameField.Height + m_RestartBtn.Height);
            
            // Créer le jeu avec l'architecture ECS
            m_Game = new Game(WIDTH, HEIGHT, m_GameGraphics);
            
            // Initialiser le InputHandler (Command Pattern)
            m_InputHandler = new InputHandler();
            SetupInputBindings();
            
            // Initialiser les observers
            SetupObservers();
            
            // Démarrer le timer
            m_Game.StateManager.SetTimerEnabled = (enabled) => m_Timer.Enabled = enabled;
            m_Timer.Start();
        }


        /// Configure les bindings clavier avec le Command Pattern
        private void SetupInputBindings()
        {
            // World  nécessaire pour les commandes, pas l'EventManager (passer une référence au Game pour accéder au World)
            var upCommand = new DirectionCommand(m_Game, Direction.Up);
            var downCommand = new DirectionCommand(m_Game, Direction.Down);
            var leftCommand = new DirectionCommand(m_Game, Direction.Left);
            var rightCommand = new DirectionCommand(m_Game, Direction.Right);
            
            // Commands de contrôle du jeu
            var pauseCommand = new PauseGameCommand(m_Game.StateManager);
            var restartCommand = new RestartGameCommand(m_Game.StateManager);
            
            // Lier les touches aux commandes
            m_InputHandler.BindKey(Keys.Up, upCommand);
            m_InputHandler.BindKey(Keys.Down, downCommand);
            m_InputHandler.BindKey(Keys.Left, leftCommand);
            m_InputHandler.BindKey(Keys.Right, rightCommand);
            m_InputHandler.BindKey(Keys.W, upCommand);
            m_InputHandler.BindKey(Keys.S, downCommand);
            m_InputHandler.BindKey(Keys.A, leftCommand);
            m_InputHandler.BindKey(Keys.D, rightCommand);
            m_InputHandler.BindKey(Keys.P, pauseCommand);
            m_InputHandler.BindKey(Keys.Space, pauseCommand);
            m_InputHandler.BindKey(Keys.R, restartCommand);
        }


        /// Configure les observers avec le Observer Pattern
        private void SetupObservers()
        {
            // S'abonner aux changements de score
            m_Game.ScoreObserver.OnScoreChanged += UpdateScoreDisplay;
            
            // Créer et configurer le UIObserver
            m_UIObserver = new UIObserver();
            m_UIObserver.OnGameOver += HandleGameOver;
            m_UIObserver.OnGameStarted += HandleGameStarted;
            
            // Ajouter le UIObserver à l'EventManager
            m_Game.EventManager.AddObserver(m_UIObserver);
        }

        // Màj l'affichage du score
        private void UpdateScoreDisplay(int score)
        {
            scoreLbl.Text = string.Format(SCORE_STRING, score);
        }

        // Gérer l'événement Game Over
        private void HandleGameOver()
        {
            m_RestartBtn.Enabled = true;
        }

        // Gérer l'event de début de partie
        private void HandleGameStarted()
        {
            m_RestartBtn.Enabled = false;
            UpdateScoreDisplay(0);
        }


        // Tick du timer - màj le jeu
        private void OnTimerTick(object sender, EventArgs e)
        {
            m_Game.Update();
            Invalidate(); // Force le redessin
        }


        // Gérer les entrées clavier via le Command Pattern
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            m_InputHandler.HandleInput(e.KeyCode);
        }

        // Dessine le jeu
        private void OnPaint(object sender, PaintEventArgs e)
        {
            m_GameGraphics.Clear(m_BackgroundColor);
            m_Game.Draw(m_GameGraphics);
            e.Graphics.DrawImage(m_GameField, 0, m_RestartBtn.Height);
        }

        // Bouton Restart cliqué
        private void OnRestartBtnClick(object sender, EventArgs e)
        {
            m_Game.Restart();
        }
    }
}