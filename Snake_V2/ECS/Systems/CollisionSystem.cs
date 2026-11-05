using System.Collections.Generic;
using System.Linq;
using Snake.ECS.Components;

namespace Snake.ECS.Systems
{
    // Système collision (détection et gestion); vérif snake/nourriture, snake/mur, snake/snake
    public class CollisionSystem : ISystem
    {
        private readonly int m_GridWidth;
        private readonly int m_GridHeight;

        // Snake mange > événement
        public event System.Action OnFoodEaten;

        // Snake meurt > événement
        public event System.Action OnGameOver;

        public CollisionSystem(int gridWidth, int gridHeight)
        {
            m_GridWidth = gridWidth;
            m_GridHeight = gridHeight;
        }

        public void Update(List<Entity> entities)
        {
            // Récupérer head snake
            var headEntity = entities.FirstOrDefault(e => e.HasComponent<SnakeComponent>() && e.GetComponent<SnakeComponent>().IsHead);

            if (headEntity == null)
            {
                return;
            }

            var headPos = headEntity.GetComponent<PositionComponent>();
            if (headPos == null)
            {
                return;
            }

            // Check collision murs
            if (CheckWallCollision(headPos))
            {
                OnGameOver?.Invoke();
                return;
            }

            // Check collision avec corps snake
            if (CheckSelfCollision(headEntity, entities))
            {
                OnGameOver?.Invoke();
                return;
            }

            // Check collision nourriture
            CheckFoodCollision(headEntity, entities);
        }

        // Check si head touche les murs
        private bool CheckWallCollision(PositionComponent headPos)
        {
            return headPos.X < 0 || headPos.X >= m_GridWidth || headPos.Y < 0 || headPos.Y >= m_GridHeight;
        }

        // Check si tête du snake le touche lui-même
        private bool CheckSelfCollision(Entity headEntity, List<Entity> entities)
        {
            var headPos = headEntity.GetComponent<PositionComponent>();

            // Check collision avec les segments du corps, hors tête
            var bodySegments = entities.Where(e =>
                e.HasComponent<SnakeComponent>() &&
                !e.GetComponent<SnakeComponent>().IsHead &&
                e.HasComponent<PositionComponent>());

            foreach (var segment in bodySegments)
            {
                var segmentPos = segment.GetComponent<PositionComponent>();
                if (segmentPos.X == headPos.X && segmentPos.Y == headPos.Y)
                {
                    return true;
                }
            }

            return false;
        }


        // Check si head touche nourriture
        private void CheckFoodCollision(Entity headEntity, List<Entity> entities)
        {
            var headPos = headEntity.GetComponent<PositionComponent>();
            var growthComp = headEntity.GetComponent<SizeComponent>();

            // Trouver la nourriture
            var foodEntity = entities.FirstOrDefault(e => e.HasComponent<FoodComponent>());
            if (foodEntity == null)
                return;

            var foodPos = foodEntity.GetComponent<PositionComponent>();
            if (foodPos == null)
                return;

            // Vérifier si la tête la touche
            if (headPos.X == foodPos.X && headPos.Y == foodPos.Y)
            {
                // Faire grandir le serpent
                if (growthComp != null)
                {
                    growthComp.AddSize(1);
                }

                // Supprimer l'ancienne nourriture
                entities.Remove(foodEntity);

                // Notifier que la nourriture a été mangée (en générer une nouvelle)
                OnFoodEaten?.Invoke();
            }
        }
    }
}