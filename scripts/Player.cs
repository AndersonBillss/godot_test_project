using Godot;
using System;

namespace Test_project.scripts{
	public partial class Player : Area2D
	{
		[Export]
		public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).
		public Vector2 ScreenSize; // Size of the game window.
		[Signal]
		public delegate void HitEventHandler();

		public void Start(Vector2 position)
		{
			Position = position;
			Show();
			GetNode<CollisionShape2D>("CollisionShape2D2").Disabled = false;
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			// Hide();
			ScreenSize = GetViewportRect().Size;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			Vector2 velocity = Vector2.Zero;
			if (Input.IsActionPressed("move_right"))
			{
				velocity.X += 1;
			}

			if (Input.IsActionPressed("move_left"))
			{
				velocity.X -= 1;
			}

			if (Input.IsActionPressed("move_down"))
			{
				velocity.Y += 1;
			}

			if (Input.IsActionPressed("move_up"))
			{
				velocity.Y -= 1;
			}


			AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D2");
			if (velocity.X != 0)
			{
				animatedSprite2D.Animation = "walk";
				animatedSprite2D.FlipV = false;
				// See the note below about the following boolean assignment.
				animatedSprite2D.FlipH = velocity.X < 0;
			}
			else if (velocity.Y != 0)
			{
				animatedSprite2D.Animation = "up";
				animatedSprite2D.FlipV = velocity.Y > 0;
			}

			if(velocity.Length() > 0)
			{
				velocity = velocity.Normalized() * Speed;
				animatedSprite2D.Play();
				Position += velocity * (float)delta;
				Position = new Vector2(
				x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
				y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
				);
			}
			else {
				animatedSprite2D.Stop();
			}

		}

		private void _on_body_entered(Node2D body)
		{
			Hide(); // Player disappears after being hit.
			EmitSignal(SignalName.Hit);
			// Must be deferred as we can't change physics properties on a physics callback.
			GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		}
	}
}
