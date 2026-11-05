using System;
using System.Collections.Generic;

// m_ privés et s_ statique convention


namespace Snake.ECS {
    // Entity = ID + composants. Pas de logique, juste données
    public class Entity {

        private static int s_NextId = 0; // A incrémenter, pour le faire en automatique et pas manuel
        public int Id {
            get;
            private set;
        } // Lecture ok mais modifiable que depuis Entity
        private Dictionary<Type, IComponent> m_Components;

        public Entity() {
            Id = s_NextId++; // Incrémenter
            m_Components = new Dictionary<Type, IComponent>();
        }


        // Ajouter composant à l'entité
        public void AddComponent(IComponent component) {
            Type compoType = component.GetType();
            if (!m_Components.ContainsKey(compoType)) {
                m_Components.Add(compoType, component);
            }
        }

        // Type T = générique, tout type
        // Vérifier si l'entité possède un composant de type T
        public T GetComponent<T>() where T : IComponent {
            Type compoType = typeof(T);
            if (m_Components.TryGetValue(compoType, out IComponent component)) { // Si dico m_Components contient composant du type voulu, trygetvalue le récupère et le stocke dans var "component" --> return ce component en type T
                return (T)component;
            }
            return default;
        }

        // Enlever composant de l'entité
        public void RemoveComponent<T>() where T : IComponent {
            Type compoType = typeof(T); // Récup type du component à retirer

            if (m_Components.ContainsKey(compoType)) { // Si le dico contient ce type de compo, le supprime
                m_Components.Remove(compoType);
            }
        }

        // Récupérer tous les composants de l'entité
        public IEnumerable<IComponent> GetAllComponents() {
            return m_Components.Values;
        }

        public bool HasComponent<T>() where T : IComponent {
            return m_Components.ContainsKey(typeof(T));
        }
    }
}
