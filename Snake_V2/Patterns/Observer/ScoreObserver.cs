using System;

namespace Snake.Patterns.Observer
{
    // Gère le score, écoute les events FoodEaten pour màj le score
    public class ScoreObserver : IObserver
    {
        private int m_Score;
        
        // Event pour notifier l'UI quand le score change
        public event Action<int> OnScoreChanged;

        public int Score 
        { 
            get { return m_Score; } 
            private set 
            { 
                m_Score = value;
                OnScoreChanged?.Invoke(m_Score);
            }
        }

        public ScoreObserver()
        {
            m_Score = 0;
        }

        public void OnNotify(GameEvent gameEvent)
        {
            switch (gameEvent.Type)
            {
                case GameEventType.FoodEaten:
                    // Incrémenter le score quand la nourriture est mangée
                    Score++;
                    break;

                case GameEventType.GameStarted:
                    // Réinitialiser le score au début d'une nouvelle partie
                    Score = 0;
                    break;
            }
        }

        public void ResetScore()
        {
            Score = 0;
        }
    }

    // UIObserver : màj l'UI
    public class UIObserver : IObserver
    {
        public event Action OnGameOver;
        public event Action OnGameStarted;
        public event Action OnGamePaused;
        public event Action OnGameResumed;

        public void OnNotify(GameEvent gameEvent)
        {
            switch (gameEvent.Type)
            {
                case GameEventType.GameOver:
                    OnGameOver?.Invoke();
                    break;

                case GameEventType.GameStarted:
                    OnGameStarted?.Invoke();
                    break;

                case GameEventType.GamePaused:
                    OnGamePaused?.Invoke();
                    break;

                case GameEventType.GameResumed:
                    OnGameResumed?.Invoke();
                    break;
            }
        }
    }
}