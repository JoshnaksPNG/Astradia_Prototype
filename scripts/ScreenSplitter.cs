using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class ScreenSplitter : Node2D
{
	[Export]
	Polygon2D Left;

	[Export]
	Polygon2D Right;

    [Export]
    Polygon2D Basis;

	[Export]
	public AnimationPlayer Animator;

    public bool moving = false;

	public float PanelSpeed = 800f;

	Vector2 PanelSlope;

	Image CurrentImage;

    [Signal]
    public delegate void ScreenSplitterPanelsClearEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		List<float> leftExtents = PolygonExtents(Left);
		List<float> basisExtents = PolygonExtents(Basis);

		float run = ((leftExtents[1] - leftExtents[0]) - ((basisExtents[1] - basisExtents[0]) / 2)) * 2;
		float rise = basisExtents[3] - basisExtents[2];

		PanelSlope = new Vector2(-run, rise).Normalized();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (moving)
		{
			Left.Position += PanelSlope * (float)delta * PanelSpeed;
            Right.Position += PanelSlope * (float)delta * PanelSpeed * -1;

			if (Left.Position.Y > 1440 && Right.Position.Y < -1440)
			{
				EmitSignal(SignalName.ScreenSplitterPanelsClear);
			}
        }
	}

    public void SetCamera(Viewport viewport)
    {
        ViewportTexture windowTexture = viewport.GetTexture();

		ImageTexture txture = new ImageTexture();

		txture.SetImage(windowTexture.GetImage());

        Left.Texture = txture;
		Right.Texture = txture;

        Right.TextureOffset = Vector2.Zero;
		Left.TextureOffset = Vector2.Zero;
    }

	public static List<float> PolygonExtents(Polygon2D polygon) 
	{
        float xMin = float.MaxValue;
        float xMax = float.MinValue;
        float yMin = float.MaxValue;
        float yMax = float.MinValue;

		foreach(Vector2 vertex in polygon.Polygon) 
		{
			if(vertex.X > xMax) 
			{
				xMax = vertex.X;
			}

            if (vertex.X < xMin)
            {
                xMin = vertex.X;
            }

            if (vertex.Y > yMax)
            {
                yMax = vertex.Y;
            }

            if (vertex.Y < yMin)
            {
                yMin = vertex.Y;
            }
        }

		//Debug.WriteLine($"{polygon.Name}: Width: {xMax - xMin}  Height: {yMax - yMin}");

		return new() { xMin, xMax, yMin, yMax };
	}
}
