using System.Collections.Generic;
using System.Linq;
using Snake.ECS.Components;

namespace Snake.ECS.Systems {
    // Système de mouvements (déplacement snake, agit sur entités qui ont PositionComponent, DirectionComponent et SnakeComponent)
    public class MovementSystem : ISystem {
        public void Update(List<Entity> entities) {
            // Récupération des segments du snake avec ordre
            var snakeEntities = entities
                .Where(e => e.HasComponent<SnakeComponent>())
                .OrderBy(e => e.Id) // Ordre via ID dans l'ordre
                .ToList();
            
            if (snakeEntities.Count == 0) {
                return;
            }

            // Récupération de la tête
            var headEntity = snakeEntities.FirstOrDefault(e => e.GetComponent<SnakeComponent>().IsHead);
            
            if (headEntity == null) {
                return;
            }

            var headPos = headEntity.GetComponent<PositionComponent>();
            var direction = headEntity.GetComponent<DirectionComponent>();
            var size = headEntity.GetComponent<SizeComponent>();

            if (headPos == null || direction == null) {
                return;
            }

            // Save les anciennes positions de chaque segment
            var oldPositions = new List<(int x, int y)>();

            foreach (var segment in snakeEntities) {
                var position = segment.GetComponent<PositionComponent>();
                if (position != null) {
                    oldPositions.Add((position.X, position.Y));
                }
            }

            // Déplacer la tête suivant la direction
            switch (direction.CurrentDirection) {
                case Direction.Up:
                    headPos.Y -= 1;
                    break;
                case Direction.Down:
                    headPos.Y += 1;
                    break;
                case Direction.Left:
                    headPos.X -= 1;
                    break;
                case Direction.Right:
                    headPos.X += 1;
                    break;
            }

            // Déplacer chaque segment du corps (position du segment d'avant)
            for (int i=1; i < snakeEntities.Count; i++) {
                var segmentPos = snakeEntities[i].GetComponent<PositionComponent>();
                if (segmentPos != null && i-1 < oldPositions.Count) {
                    segmentPos.X = oldPositions[i-1].x;
                    segmentPos.Y = oldPositions[i-1].y;
                }
            }

            // Croissance snake : ajout segment si besoin
            if (size != null && size.SegmentsToAdd > 0 ) {
                // Pos segment = l'ancienne de celui d'avant
                var lastOldPos = oldPositions[oldPositions.Count -1];

                var newSegment = new Entity();
                newSegment.AddComponent(new PositionComponent(lastOldPos.x, lastOldPos.y));
                newSegment.AddComponent(new SnakeComponent(false)); // Sauf tête

                // Copier les composants d'affichage et collision de la tête
                var headAffichage = headEntity.GetComponent<AffichageComponent>();
                var headCollision= headEntity.GetComponent<CollisionComponent>();

                if (headAffichage != null) {
                    newSegment.AddComponent(new AffichageComponent(headAffichage.Color, headAffichage.Width, headAffichage.Height));
                }

                if (headCollision != null) {
                    newSegment.AddComponent(new CollisionComponent(ColliType.SnakeBody));
                }

                entities.Add(newSegment);
                size.SegmentsToAdd--;
            }

        }
    }
}