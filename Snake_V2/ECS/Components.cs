using System.Drawing;
using Snake;

namespace Snake.ECS.Components {

    // Composant position : coordonnées X Y
    public class PositionComponent : IComponent {
        public int X {
            get;
            set;
        }

        public int Y {
            get;
            set;
        }

        public PositionComponent(int x, int y) {
            X = x;
            Y = y;
        }
    }

    // Composant affichage : couleur, tailles
    public class AffichageComponent : IComponent {
        public Brush Color {
            get;
            set;
        }

        public int Width {
            get;
            set;
        }

        public int Height {
            get;
            set;
        }

        public AffichageComponent(Brush color, int width, int height) {
            Color = color;
            Width = width;
            Height = height;
        }
    }

    // Composant direction/vitesse pour le mouvement du snake
    public class DirectionComponent : IComponent {
        public Direction CurrentDirection{
            get;
            set;
        }

        public DirectionComponent(Direction initialDirection) {
            CurrentDirection = initialDirection;
        }
    }

    // Composant d'ajout de segments au serpent
    public class SizeComponent : IComponent {
        public int SegmentsToAdd {
            get;
            set;
        }

        public SizeComponent() {
            SegmentsToAdd = 0;
        }

        public void AddSize(int amount = 1) {
            SegmentsToAdd += amount; // Direct 1 ? Pas modulaire
        }
    }

    // Composant collision avec type d'entité pour collision
    public class CollisionComponent : IComponent {
        public ColliType Type {
            get;
            set;
        }

        public CollisionComponent(ColliType type) {
            Type = type;
        }
    }


    // Types de collisions possibles
    public enum ColliType {
        SnakeHead, // Différencier head/body pour savoir si nourriture, ou besoins, ou pertes
        SnakeBody,
        Food,
        Wall
    }

    // Composant tag snake, pour identifier les entités du serpent et regrouper ça sous un tag
    public class SnakeComponent : IComponent {
        public bool IsHead {
            get;
            set;
        }

        public SnakeComponent(bool isHead) {
            IsHead = isHead;
        }
    
    }
        

    // Composant tag pour nourriture (vide)
    public class FoodComponent : IComponent {}
}