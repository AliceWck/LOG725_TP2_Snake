using System.Collections.Generic;
using System.Windows.Forms;

namespace Snake.Patterns.Command
{
    // Gère les inputs et les mappe aux commandes 
    public class InputHandler
    {
        private Dictionary<Keys, ICommand> m_KeyBindings;

        public InputHandler()
        {
            m_KeyBindings = new Dictionary<Keys, ICommand>();
        }

        // Associe une touche à une commande
        public void BindKey(Keys key, ICommand command)
        {
            if (m_KeyBindings.ContainsKey(key))
            {
                m_KeyBindings[key] = command;
            }
            else
            {
                m_KeyBindings.Add(key, command);
            }
        }

        // Retire l'association d'une touche
        public void UnbindKey(Keys key)
        {
            if (m_KeyBindings.ContainsKey(key))
            {
                m_KeyBindings.Remove(key);
            }
        }

        // Gère l'appui sur une touche et exécute la commande associée
        public void HandleInput(Keys key)
        {
            if (m_KeyBindings.ContainsKey(key))
            {
                m_KeyBindings[key].Execute();
            }
        }

        // Réinitialise toutes les associations de touches
        public void ClearBindings()
        {
            m_KeyBindings.Clear();
        }
    }
}