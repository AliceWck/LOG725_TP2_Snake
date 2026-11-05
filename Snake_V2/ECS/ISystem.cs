using System.Collections.Generic;

namespace Snake.ECS
{
    // Interface de base pour tous les systèmes
    public interface ISystem
    {
        // Màj le système - appelé chaque frame/tick
        void Update(List<Entity> entities);
    }
}