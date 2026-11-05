using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Snake.ECS.Components;

namespace Snake.ECS.Systems {
    // Système d'affichage/Rendu : dessiner les entités qui ont un composant d'affichage (PositionComponent et AffichageComponent)

    public class AffichageSystem : ISystem {
        private Graphics m_Graphics;

        public AffichageSystem(Graphics graphics)
        {
            m_Graphics = graphics;
        }

        // Màj Graphics si besoin
        public void SetGraphics(Graphics graphics)
        {
            m_Graphics = graphics;
        }

        public void Update(List<Entity> entities)
        {
            if (m_Graphics == null) {
                return;
            }

            // Parcourir toutes les entités qui ont à la fois Position et Affichage
            var affichableEntities = entities.Where(e => e.HasComponent<PositionComponent>() && e.HasComponent<AffichageComponent>());

            foreach (var entity in affichableEntities)
            {
                var pos = entity.GetComponent<PositionComponent>();
                var affichage = entity.GetComponent<AffichageComponent>();

                if (pos != null && affichage != null)
                {
                    // Dessiner l'entité sous forme de rectangle
                    m_Graphics.FillRectangle(
                        affichage.Color,
                        new Rectangle(
                            pos.X * affichage.Width,
                            pos.Y * affichage.Height,
                            affichage.Width,
                            affichage.Height
                        )
                    );
                }
            }
        }
    }
}