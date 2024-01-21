using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace monogame_schmup
{
    internal class Projectile
    {
        public Vector2 position;
        public string texture_name;
        public Texture2D texture;
        public float speed = 150f;

        public Projectile(Vector2 position, string texture_name)
        {
            this.position = position;
            this.texture_name = texture_name;
        }
    }
}
