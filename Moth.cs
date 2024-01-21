using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class Moth
{
    public Vector2 position;
    public string texture_name;
    public Texture2D texture;
    public float speed = 60f;

    public Moth(Vector2 position, string texture_name)
    {
        this.position = position;
        this.texture_name = texture_name;
    }
}
