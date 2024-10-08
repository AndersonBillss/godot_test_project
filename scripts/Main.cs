using Godot;
using System;

namespace Test_project.scripts{
	public partial class Main: Node{
		[Export] public PackedScene MobScene { get; set; }
		private int _score;

		public override void _Ready()
		{
			GetNode<Player>("Player").Hide();
		}

		public void GameOver()
		{
			GetNode<Timer>("MobTimer").Stop();
			GetNode<Timer>("ScoreTimer").Stop();
			GetNode<HUD>("HUD").ShowGameOver();
		}

		public void NewGame()
		{
			GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
			_score = 0;

			var player = GetNode<Player>("Player");
			var startPosition = GetNode<Marker2D>("StartPosition");
			player.Start(startPosition.Position);

			GetNode<Timer>("StartTimer").Start();
			
			var hud = GetNode<HUD>("HUD");
			hud.UpdateScore(_score);
			hud.ShowMessage("Get Ready!");
		}

		// We also specified this function name in PascalCase in the editor's connection window.
		private void OnScoreTimerTimeout()
		{
			_score++;
			GetNode<HUD>("HUD").UpdateScore(_score);
			GetNode<HUD>("HUD").ShowMessage("");
		}

		// We also specified this function name in PascalCase in the editor's connection window.
		private void OnStartTimerTimeout()
		{
			GetNode<HUD>("HUD").ShowMessage("");
			GetNode<Timer>("MobTimer").Start();
			GetNode<Timer>("ScoreTimer").Start();
		}

		private void OnMobTimerTimeout(){
			// Create a new instance of the Mob scene.
			Mob mob = MobScene.Instantiate<Mob>();

			// Choose a random location on Path2D.
			var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
			mobSpawnLocation.ProgressRatio = GD.Randf();

			// Set the mob's direction perpendicular to the path direction.
			float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

			// Set the mob's position to a random location.
			mob.Position = mobSpawnLocation.Position;

			// Add some randomness to the direction.
			direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
			mob.Rotation = direction;

			// Choose the velocity.
			var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
			mob.LinearVelocity = velocity.Rotated(direction);

			// Spawn the mob by adding it to the Main scene.
			AddChild(mob);
		}
	}
}
