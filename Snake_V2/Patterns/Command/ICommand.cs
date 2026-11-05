using Snake.ECS;
using Snake.ECS.Components;
using System.Linq;
using Snake;
using Snake.Patterns.State;

namespace Snake.Patterns.Command
{
    // Interface de base pour toutes les commandes
    public interface ICommand {
        void Execute();
    }

    // Command pour changer la direction du serpent
    public class DirectionCommand : ICommand
    {
        private readonly Game m_Game;
        private readonly Direction m_Direction;

        public DirectionCommand(Game game, Direction direction)
        {
            m_Game = game;
            m_Direction = direction;
        }

        public void Execute()
        {
            m_Game.ChangeSnakeDirection(m_Direction);
        }
    }


    // Command pour mettre le jeu en pause
    public class PauseGameCommand : ICommand
    {
        private readonly GameStateManager m_GameStateManager;

        public PauseGameCommand(GameStateManager gameStateManager)
        {
            m_GameStateManager = gameStateManager;
        }

        public void Execute()
        {
            m_GameStateManager.TogglePause();
        }
    }


    // Command pour restart le jeu
    public class RestartGameCommand : ICommand
    {
        private readonly GameStateManager m_GameStateManager;

        public RestartGameCommand(GameStateManager gameStateManager)
        {
            m_GameStateManager = gameStateManager;
        }

        public void Execute()
        {
            m_GameStateManager.RestartGame();
        }
    }
}