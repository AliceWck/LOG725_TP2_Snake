using System.Collections.Generic;

namespace Snake.Patterns.Observer
{
    // Interface Observer pour objets qui veulent être notifiés des changements
    public interface IObserver
    {
        void OnNotify(GameEvent gameEvent);
    }

    // Interface Subject : objet observable qui notifie les observers
    public interface ISubject
    {
        void AddObserver(IObserver observer);
        void RemoveObserver(IObserver observer);
        void NotifyObservers(GameEvent gameEvent);
    }

    // Types d'événements possibles
    public enum GameEventType
    {
        FoodEaten,
        GameOver,
        ScoreChanged,
        GameStarted,
        GamePaused,
        GameResumed
    }

    // Représente un event de jeu
    public class GameEvent
    {
        public GameEventType Type { get; set; }
        public object Data { get; set; }

        public GameEvent(GameEventType type, object data = null)
        {
            Type = type;
            Data = data;
        }
    }

    // EventManager : central, gère tous les événements du jeu, implémente l'Observer
    public class EventManager : ISubject
    {
        private List<IObserver> m_Observers;

        public EventManager()
        {
            m_Observers = new List<IObserver>();
        }

        public void AddObserver(IObserver observer)
        {
            if (!m_Observers.Contains(observer))
            {
                m_Observers.Add(observer);
            }
        }

        public void RemoveObserver(IObserver observer)
        {
            m_Observers.Remove(observer);
        }

        public void NotifyObservers(GameEvent gameEvent)
        {
            // Créer une copie pour éviter les problèmes si un observer se défait pendant la notification
            var observersCopy = new List<IObserver>(m_Observers);
            
            foreach (var observer in observersCopy)
            {
                observer.OnNotify(gameEvent);
            }
        }
    }
}