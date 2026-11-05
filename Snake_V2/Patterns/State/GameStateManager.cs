using Snake.Patterns.Observer;

namespace Snake.Patterns.State
{
    // Interface pour les états du jeu (Patron State : comportement selon l'état)
    public interface IGameState
    {
        void Enter();
        void Update();
        void Exit();
    }

    // État : Jeu en cours
    public class PlayingState : IGameState
    {
        private readonly GameStateManager m_Manager;

        public PlayingState(GameStateManager manager)
        {
            m_Manager = manager;
        }

        public void Enter()
        {
            // Démarrer/reprendre le timer du jeu
            m_Manager.SetTimerEnabled(true);
        }

        public void Update()
        {
            // Le jeu tourne 
        }

        public void Exit()
        {
            // Ràf
        }
    }

    // État : Jeu en pause
    public class PausedState : IGameState
    {
        private readonly GameStateManager m_Manager;

        public PausedState(GameStateManager manager)
        {
            m_Manager = manager;
        }

        public void Enter()
        {
            // Stopper le timer
            m_Manager.SetTimerEnabled(false);
            m_Manager.NotifyEvent(new GameEvent(GameEventType.GamePaused));
        }

        public void Update()
        {
            // Jeu en pause, rien ne se passe
        }

        public void Exit()
        {
            m_Manager.NotifyEvent(new GameEvent(GameEventType.GameResumed));
        }
    }

    // État : Game Over*
    public class GameOverState : IGameState
    {
        private readonly GameStateManager m_Manager;

        public GameOverState(GameStateManager manager)
        {
            m_Manager = manager;
        }

        public void Enter()
        {
            // Stopper le timer
            m_Manager.SetTimerEnabled(false);
            m_Manager.NotifyEvent(new GameEvent(GameEventType.GameOver));
        }

        public void Update()
        {
            // Attendre que le joueur redémarre
        }

        public void Exit()
        {
            // Ràf
        }
    }

    // GameStateManager : gestion transitions entre états du jeu (implémente State)
    public class GameStateManager
    {
        private IGameState m_CurrentState;
        private readonly PlayingState m_PlayingState;
        private readonly PausedState m_PausedState;
        private readonly GameOverState m_GameOverState;
        private readonly EventManager m_EventManager;

        // Permet au manager de lancer/arrêter le timer depuis l'extérieur
        public System.Action<bool> SetTimerEnabled { get; set; }

        public GameStateManager(EventManager eventManager)
        {
            m_EventManager = eventManager;
            
            // Créer les états
            m_PlayingState = new PlayingState(this);
            m_PausedState = new PausedState(this);
            m_GameOverState = new GameOverState(this);

            // État initial
            m_CurrentState = m_PlayingState;
        }

        public void ChangeState(IGameState newState)
        {
            m_CurrentState?.Exit();
            m_CurrentState = newState;
            m_CurrentState?.Enter();
        }

        public void Update()
        {
            m_CurrentState?.Update();
        }

        // Basculer entre pause et jeu
        public void TogglePause()
        {
            if (m_CurrentState == m_PlayingState)
            {
                ChangeState(m_PausedState);
            }
            else if (m_CurrentState == m_PausedState)
            {
                ChangeState(m_PlayingState);
            }
        }


        // Passer à l'état Game Over
        public void GameOver()
        {
            ChangeState(m_GameOverState);
        }

        // Restart le jeu
        public void RestartGame()
        {
            NotifyEvent(new GameEvent(GameEventType.GameStarted));
            ChangeState(m_PlayingState);
        }

        // Check si jeu en cours
        public bool IsPlaying()
        {
            return m_CurrentState == m_PlayingState;
        }

        // Check si jeu en pause
        public bool IsPaused()
        {
            return m_CurrentState == m_PausedState;
        }

        // Check si jeu terminé
        public bool IsGameOver()
        {
            return m_CurrentState == m_GameOverState;
        }

        public void NotifyEvent(GameEvent gameEvent)
        {
            m_EventManager?.NotifyObservers(gameEvent);
        }
    }
}