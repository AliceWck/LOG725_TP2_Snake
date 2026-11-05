using System;
using System.Drawing;
using Snake.ECS;
using Snake.ECS.Components;
using Snake.ECS.Systems;
using Snake.Patterns.Observer;
using Snake.Patterns.State;
using Snake;

namespace Snake
{
    // Game - Version refactorisée avec ECS et patrons de conception (Utilise ECS, Observer Pattern, State Pattern)
    public class Game
    {
        // Dimensions de la grille
        private readonly int m_Width;
        private readonly int m_Height;
        
        // Architecture ECS
        private readonly World m_World;
        private MovementSystem m_MovementSystem;
        private CollisionSystem m_CollisionSystem;
        private AffichageSystem m_AffichageSystem;
        
        // Patrons de conception
        private readonly EventManager m_EventManager;
        private readonly GameStateManager m_StateManager;
        private readonly ScoreObserver m_ScoreObserver;
        
        // Références aux entités principales
        private Entity m_SnakeHead;
        private Entity m_Food;
        
        // Génération aléatoire pour la nourriture
        private readonly Random m_Rnd;

        public EventManager EventManager => m_EventManager;
        public GameStateManager StateManager => m_StateManager;
        public ScoreObserver ScoreObserver => m_ScoreObserver;



        public Game(int width, int height, Graphics graphics)
        {
            m_Width = width;
            m_Height = height;
            m_Rnd = new Random();
            
            // Initialiser l'ECS
            m_World = new World();
            
            // Initialiser les systèmes
            m_MovementSystem = new MovementSystem();
            m_CollisionSystem = new CollisionSystem(width, height);
            m_AffichageSystem = new AffichageSystem(graphics);
            
            // Ajouter les systèmes au monde
            m_World.AddSystem(m_MovementSystem);
            m_World.AddSystem(m_CollisionSystem);
            m_World.AddSystem(m_AffichageSystem);
            
            // Initialiser les patterns
            m_EventManager = new EventManager();
            m_StateManager = new GameStateManager(m_EventManager);
            m_ScoreObserver = new ScoreObserver();
            
            // S'abonner aux events
            m_EventManager.AddObserver(m_ScoreObserver);
            
            // Connecter les events du CollisionSystem à l'EventManager
            m_CollisionSystem.OnFoodEaten += HandleFoodEaten;
            m_CollisionSystem.OnGameOver += HandleGameOver;
            
            // Initialiser le jeu
            InitializeGame();
        }

        // Init le jeu : crée le snake et la nourriture
        private void InitializeGame()
        {
            // Nettoyer le monde
            m_World.Clear();
            
            // Créer le snake (tête + 1 segment initial)
            CreateSnake();
            
            // Créer nourriture
            CreateFood();
        }


        // Crée le snake avec 1 tête et 1 segment initial
        private void CreateSnake()
        {
            // Créer la tête du snake
            m_SnakeHead = new Entity();
            m_SnakeHead.AddComponent(new PositionComponent(5, 5));
            m_SnakeHead.AddComponent(new DirectionComponent(Direction.Down));
            m_SnakeHead.AddComponent(new AffichageComponent(Brushes.MidnightBlue, Piece.SIDE, Piece.SIDE));
            m_SnakeHead.AddComponent(new CollisionComponent(ColliType.SnakeHead));
            m_SnakeHead.AddComponent(new SnakeComponent(true)); // C'est la tête
            m_SnakeHead.AddComponent(new SizeComponent());
            
            m_World.AddEntity(m_SnakeHead);
            
            // Créer un segment initial
            var segment = new Entity();
            segment.AddComponent(new PositionComponent(5, 4));
            segment.AddComponent(new AffichageComponent(Brushes.MidnightBlue, Piece.SIDE, Piece.SIDE));
            segment.AddComponent(new CollisionComponent(ColliType.SnakeBody));
            segment.AddComponent(new SnakeComponent(false)); // Ce n'est pas la tête
            
            m_World.AddEntity(segment);
        }


        // Crée une nouvelle entité nourriture à une position aléatoire
        private void CreateFood()
        {
            int x, y;
            
            // Position pas occupée par le snake
            do
            {
                x = m_Rnd.Next(0, m_Width);
                y = m_Rnd.Next(0, m_Height);
            } while (IsPositionOccupiedBySnake(x, y));
            
            m_Food = new Entity();
            m_Food.AddComponent(new PositionComponent(x, y));
            m_Food.AddComponent(new AffichageComponent(Brushes.Violet, Piece.SIDE, Piece.SIDE));
            m_Food.AddComponent(new CollisionComponent(ColliType.Food));
            m_Food.AddComponent(new FoodComponent());
            
            m_World.AddEntity(m_Food);
        }

        // Check si une position est occupée par snake
        private bool IsPositionOccupiedBySnake(int x, int y)
        {
            foreach (var entity in m_World.GetEntities())
            {
                if (entity.HasComponent<SnakeComponent>())
                {
                    var pos = entity.GetComponent<PositionComponent>();
                    if (pos != null && pos.X == x && pos.Y == y)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Màj le jeu (appelé par le timer)
        public void Update()
        {
            if (m_StateManager.IsPlaying())
            {
                m_World.Update();
                m_StateManager.Update();
            }
        }


        // Dessine le jeu
        public void Draw(Graphics g)
        {
            // Le Affichageystem s'occupe de tout dessiner
            m_AffichageSystem.SetGraphics(g);
            m_AffichageSystem.Update(m_World.GetEntities());
        }


        // Change la direction du snake
        public void ChangeSnakeDirection(Direction direction)
        {
            if (m_SnakeHead != null)
            {
                var dirComp = m_SnakeHead.GetComponent<DirectionComponent>();
                if (dirComp != null)
                {
                    // Empêcher le demi-tour
                    var currentDir = dirComp.CurrentDirection;
                    if ((currentDir == Direction.Up && direction == Direction.Down) ||
                        (currentDir == Direction.Down && direction == Direction.Up) ||
                        (currentDir == Direction.Left && direction == Direction.Right) ||
                        (currentDir == Direction.Right && direction == Direction.Left))
                    {
                        return; // Direction opposée, fait rien
                    }
                    
                    dirComp.CurrentDirection = direction;
                }
            }
        }


        // Restart le jeu
        public void Restart()
        {
            InitializeGame();
            m_StateManager.RestartGame();
        }


        // Obtient le score actuel
        public int GetScore()
        {
            return m_ScoreObserver.Score;
        }


        // Gérer l'event quand la nourriture est mangée
        private void HandleFoodEaten()
        {
            // Notifier les observers
            m_EventManager.NotifyObservers(new GameEvent(GameEventType.FoodEaten));
            
            // Créer une nouvelle nourriture
            CreateFood();
        }


        // Gérer l'événement Game Over
        private void HandleGameOver()
        {
            m_StateManager.GameOver();
        }


        // Màj le Graphics pour le AffichageSystem
        public void SetGraphics(Graphics graphics)
        {
            m_AffichageSystem.SetGraphics(graphics);
        }
    }
}