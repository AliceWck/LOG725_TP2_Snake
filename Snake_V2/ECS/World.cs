using System.Collections.Generic;

namespace Snake.ECS
{
    // World : gestion centrale de l'ECS, contient toutes les entités et tous les systèmes, coordonne les màj
    public class World
    {
        private List<Entity> m_Entities;
        private List<ISystem> m_Systems;

        public World()
        {
            m_Entities = new List<Entity>();
            m_Systems = new List<ISystem>();
        }

        // Ajoute une entité au monde
        public void AddEntity(Entity entity)
        {
            if (!m_Entities.Contains(entity))
            {
                m_Entities.Add(entity);
            }
        }

        // Retire une entité du monde
        public void RemoveEntity(Entity entity)
        {
            m_Entities.Remove(entity);
        }

        // Récupère toutes les entités
        public List<Entity> GetEntities()
        {
            return m_Entities;
        }

        // Ajoute un système au monde
        public void AddSystem(ISystem system)
        {
            if (!m_Systems.Contains(system))
            {
                m_Systems.Add(system);
            }
        }

        // Màj tous les systèmes (appelé chaque frame/tick)
        public void Update()
        {
            foreach (var system in m_Systems)
            {
                system.Update(m_Entities);
            }
        }

        // Retire toutes les entités = nettoie
        public void Clear()
        {
            m_Entities.Clear();
        }
    }
}