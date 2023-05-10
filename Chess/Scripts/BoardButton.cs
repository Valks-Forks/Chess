using Godot;
using System;

public partial class BoardButton : Button
{
	public int boardPos {get; set;}
	public Piece piece {get; set;}
	private bool buttonHeld;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		buttonHeld = false;
		piece = null;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(ButtonPressed == true) {
			if(buttonHeld == false) {
				buttonHeld = true;
				if(piece != null) {
					GD.Print("Position: " + boardPos + ", Piece: " + piece.name);
				}
				else {
					GD.Print("Position: " + boardPos + ", Piece: NULL");
				}
			}
		}
		else {
			buttonHeld = false;
		}
	}

	public void InitializeBoardButton(int position) {
		boardPos = position;
		this.Position = new Vector2(((position % 8) * 98) - 393, (int)(((Math.Floor(((float)(position))/8.0)) * 98) - 393));
	}
}
