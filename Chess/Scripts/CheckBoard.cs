using Godot;
using System;

public partial class CheckBoard : Sprite2D
{
	private PackedScene Piece;
	private PackedScene Highlight;
	public Piece[] board {get; set;}
	public Node2D[] highlight;
	public bool isWhitesTurn{get;set;}
	public bool[] canCastle = {false, false, false, false};
	public int turn;
	public int halfTurn;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Piece = ResourceLoader.Load<PackedScene>("res://Scenes/piece.tscn");
		Highlight = ResourceLoader.Load<PackedScene>("res://Scenes/PossibleMoveHighlight.tscn");
		
		//Sets up the board. empty spaces are nulls
		board = new Piece[64];
		highlight = new Node2D[64];
		for(int i = 0; i < 64; i++){
			GD.Print(i);
			board[i] = null;
			highlight[i] = GenerateHighlight(i);
		}
		/*
		GD.Print(CreateFenString() + "\n");
		
		GD.Print("-" + CheckMoves(8) + "-");
		MovePiece(8);
		
		/*
		MovePiece(9);
		MovePiece(17);
		
		MovePiece(0);
		MovePiece(8);
		
		MovePiece(1);
		
		MovePiece(2);
		
		MovePiece(3);
		
		MovePiece(4);
		
		*/
	
		/*
		fenPosition = "rnbqkbnr/pppppppp/8/8/3P4/8/PPP1PPPP/RNBQKBNR b KQkq - 0 1";
		GD.Print(fenPosition);
		LoadBoard(fenPosition);
		GD.Print(CreateFenString() + "\n");
		
		fenPosition = "pppppppp/pppppppp/pppppppp/pppppppp/pppppppp/pppppppp/ppppppppP/RNBQKBNR b KQkq - 0 1";
		GD.Print(fenPosition);
		LoadBoard(fenPosition);
		GD.Print(CreateFenString() + "\n");
		*/
	}
		
	private Node2D GenerateHighlight(int index){
		Node2D myHighlight = (Node2D)Highlight.Instantiate();
		AddChild(myHighlight);
		myHighlight.Visible = false;
		
		int xPos = ((index % 8) * 98) + 57;
		int yPos = (((int)Math.Floor((double)index / 8)) * 98) + 55;
		
		myHighlight.Position = new Vector2(xPos, yPos);
		
		return myHighlight;
	}
	
	//This method is used to create pieces
	public Piece GeneratePiece(char type, int index){
		Piece myPiece = (Piece)Piece.Instantiate();
		myPiece.frame = 0;
		
		//Checks if the piece is white or black
		if(Char.IsUpper(type)){
			myPiece.isWhite = true;
		}else{
			myPiece.isWhite = false;
			myPiece.frame += 6;
		}
		type = Char.ToLower(type);
		
		//Assigns instance variable with certain data depending on the type of piece
		switch(type){
			case 'p':
				myPiece.name = "pawn";
				myPiece.frame += 5;
				break;
			case 'r':
				myPiece.name = "rook";
				myPiece.frame += 4;
				break;
			case 'n':
				myPiece.name = "night";
				myPiece.frame += 3;
				break;
			case 'b':
				myPiece.name = "bishop";
				myPiece.frame += 2 ;
				break;
			case 'q':
				myPiece.name = "queen";
				myPiece.frame += 1 ;
				break;
			case 'k':
				myPiece.name = "king";
				break;
		}
		myPiece.index = index;
		AddChild(myPiece);
		myPiece.InitializePiece();

		return myPiece;
	}
	
	//Moves a piece from one position to another
	public void MovePiece(int index1){
		string vals = CheckMoves(index1);
		string[] temp;
		int[] possibleMoves;
		//GD.Print(vals);
		if(vals.Length > 0){
			temp = vals.Split(' ');
			possibleMoves = new int[temp.Length];
			
			Console.WriteLine("Please pick one of the following Possible Moves: ");
			for(int i = 0; i < temp.Length; i++){
				Console.WriteLine(i + ": " + temp[i]);
				possibleMoves[i] = Int32.Parse(temp[i]);
				highlight[possibleMoves[i]].Visible = true;
			}
			
			string userInput = "0";
			int userChoice = Int32.Parse(userInput);
			int index2 = possibleMoves[userChoice];
			
			GD.Print("Valid Choice Made " + userChoice + " -" + index2 + "-");
			if(board[index2] == null){
				board[index2] = board[index1];
				board[index2].index = index2;
				board[index2].InitializePiece();
				board[index1] = null;
			}else if(board[index1].isWhite != board[index2].isWhite){
				RemoveChild(board[index2]);
				board[index2] = board[index1];
				board[index2].index = index2;
				board[index2].InitializePiece();
				board[index1] = null;
			}else{
				GD.Print("Invalid Move!");
			}
		}
		else{
			GD.Print("No Valid Moves!");	
		}
	}
	
	public void MovePiece(int index1, int index2){
		GD.Print("Choices Made: " + index1 + ", " + index2);
		if(board[index2] == null){
			board[index2] = board[index1];
			board[index2].index = index2;
			board[index2].InitializePiece();
			board[index1] = null;
		}else if(board[index1].isWhite != board[index2].isWhite){
			RemoveChild(board[index2]);
			board[index2] = board[index1];
			board[index2].index = index2;
			board[index2].InitializePiece();
			board[index1] = null;
		}else{
			GD.Print("Invalid Move!");
		}
		if(board[index2].name == "pawn" && index2 / 8 >= 7){
			//FIXME: Allow player to choose which piece they would like to promote to with a small popup menu
			//For now, it promotes to queen
			board[index2].name = "queen";
			board[index2].frame -= 4; 
			board[index2].InitializePiece();
		}
	}
	
	//Checks the possible moves of a piece on a given coordinate
	public string CheckMoves(int index){
		string vals = "";

		GD.Print(board[index].toString());
		if(board[index].name == "pawn"){
			vals = GetPawnMoves(index, vals);
		}
		else if(board[index].name == "rook"){
			vals = GetRookMoves(index, vals);
		}
		else if(board[index].name == "night"){
			vals = GetKnightMoves(index, vals);
		}
		else if(board[index].name == "bishop"){
			vals = GetBishopMoves(index, vals);
		}
		else if(board[index].name == "queen"){
			vals = GetQueenMoves(index, vals);
		}
		else if(board[index].name == "king"){
			vals = GetKingMoves(index, vals);
		}
		
		return vals;
	}

	private string GetPawnMoves(int index, string vals) {
		if(board[index].isWhite){
			if(index - 8 >= 0 && board[index - 8] == null){
				vals += (index - 8) + " ";
				if(index - 18 > 0 && board[index - 16] == null && ((int)Math.Floor((double)index / 8)) == 6){
					vals += (index - 16) + " ";
				}
			}
			if(index - 7 >= 0 && board[index - 7] != null && board[index - 7].isWhite != board[index].isWhite){
				vals += (index - 7) + " ";
			}
			if(index - 9 >= 0 && board[index - 9] != null && board[index - 9].isWhite != board[index].isWhite){
				vals += (index - 9) + " ";
			}
		}
		else{
			if(index + 8 < 64 && board[index + 8] == null){
				vals += (index + 8) + " ";
				if(index + 16 < 64 && board[index + 16] == null && ((int)Math.Floor((double)index / 8)) == 1){
					vals += (index + 16) + " ";
				}
			}
			if(index + 9 < 64 && board[index + 9] != null && board[index + 9].isWhite != board[index].isWhite){
				vals += (index + 9) + " ";
			}
			if(index + 7 < 64 && board[index + 7] != null && board[index + 7].isWhite != board[index].isWhite){
				vals += (index + 7) + " ";
			}
		}
		if(vals.Length > 0){
			vals = vals.Substring(0, vals.Length - 1);
		}
		return vals;
	}

	private string GetRookMoves(int index, string vals) {
		for(int i = index - 8; i >= 0; i -= 8){
			if(board[i] != null){
				if(board[i].isWhite != board[index].isWhite){
					vals += i + " ";
				}
				break;
			}
			vals += i + " ";
		}
		for(int i = index + 8; i < 64; i += 8){
			if(board[i] != null){
				if(board[i].isWhite != board[index].isWhite){
					vals += i + " ";
				}
				break;
			}
			vals += i + " ";
		}
		if(index % 8 != 0){
			for(int i = index - 1; i >= 0; i--){
				if(board[i] != null){
					if(board[i].isWhite != board[index].isWhite){
						vals += i + " ";
					}
					break;
				}
				vals += i + " ";
				if(i % 8 == 0){
					break;
				}
			}
		}
		if(index % 8 != 7){
			for(int i = index + 1; i < 64; i++){
				if(board[i] != null){
					if(board[i].isWhite != board[index].isWhite){
						vals += i + " ";
					}
					break;
				}
				vals += i + " ";
				if(i % 8 == 7){
					break;
				}
			}
		}
		if(vals.Length > 0){
			vals = vals.Substring(0, vals.Length - 1);
		}
		return vals;
	}

	private string GetKnightMoves(int index, string vals) {
		if(index - 17 >= 0 && (board[index - 17] == null || board[index - 17].isWhite != board[index].isWhite)){
			vals += (index - 17) + " ";
		}
		if(index - 15 >= 0 && (board[index - 15] == null || board[index - 15].isWhite != board[index].isWhite)){
			vals += (index - 15) + " ";
		}
		if(index - 10 >= 0 && (board[index - 10] == null || board[index - 10].isWhite != board[index].isWhite)){
			vals += (index - 10) + " ";
		}
		if(index - 6 >= 0 && (board[index - 6] == null || board[index - 6].isWhite != board[index].isWhite)){
			vals += (index - 6) + " ";
		}
		if(index + 17 < 64 && (board[index + 17] == null || board[index + 17].isWhite != board[index].isWhite)){
			vals += (index + 17) + " ";
		}
		if(index + 15 < 64 && (board[index + 15] == null || board[index + 15].isWhite != board[index].isWhite)){
			vals += (index + 15) + " ";
		}
		if(index + 17 < 64 && (board[index + 10] == null || board[index + 10].isWhite != board[index].isWhite)){
			vals += (index + 10) + " ";
		}
		if(index + 6 < 64 && (board[index + 6] == null || board[index + 6].isWhite != board[index].isWhite)){
			vals += (index + 6) + " ";
		}
		if(vals.Length > 0){
			vals = vals.Substring(0, vals.Length - 1);
		}
		return vals;
	}

	private string GetBishopMoves(int index, string vals) {
		if(index % 8 != 7){
			for(int i = index + 9; i < 64; i += 9){
				if(board[i] != null){
					if(board[i].isWhite != board[index].isWhite){
						vals += i + " ";
					}
					break;
				}
				vals += i + " ";
				if(i % 8 == 0 || i % 8 == 7 || ((int)Math.Floor((double)i / 8)) == 0 || ((int)Math.Floor((double)i / 8)) == 7){
					break;
				}
			}
			for(int i = index - 7; i >= 0; i -= 7){
				if(board[i] != null){
					if(board[i].isWhite != board[index].isWhite){
						vals += i + " ";
					}
					break;
				}
				vals += i + " ";
				if(i % 8 == 0 || i % 8 == 7 || ((int)Math.Floor((double)i / 8)) == 0 || ((int)Math.Floor((double)i / 8)) == 7){
					break;
				}
			}
		}
		if(index % 8 != 0){
			for(int i = index + 7; i < 64; i += 7){
				if(board[i] != null){
					if(board[i].isWhite != board[index].isWhite){
						vals += i + " ";
					}
					break;
				}
				vals += i + " ";
				if(i % 8 == 0 || i % 8 == 7 || ((int)Math.Floor((double)i / 8)) == 0 || ((int)Math.Floor((double)i / 8)) == 7){
					break;
				}
			}
			for(int i = index - 9; i >= 0; i -= 9){
				if(board[i] != null){
					if(board[i].isWhite != board[index].isWhite){
						vals += i + " ";
					}
					break;
				}
				vals += i + " ";
				if(i % 8 == 0 || i % 8 == 7 || ((int)Math.Floor((double)i / 8)) == 0 || ((int)Math.Floor((double)i / 8)) == 7){
					break;
				}
			}
		}
		if(vals.Length > 0){
			vals = vals.Substring(0, vals.Length - 1);
		}
		return vals;
	}	

	private string GetQueenMoves(int index, string vals) {
		string newVals = "";
		newVals += GetRookMoves(index, vals);
		if(newVals.Length > 0){
			newVals += " ";
		}
		newVals += GetBishopMoves(index, vals);
		return newVals;
	}	

	private string GetKingMoves(int index, string vals) {
		if(index - 9 >= 0 && (board[index - 9] == null || board[index - 9].isWhite != board[index].isWhite)){
		vals += (index - 9) + " ";
		}
		if(index - 8 >= 0 && (board[index - 8] == null || board[index - 8].isWhite != board[index].isWhite)){
			vals += (index - 8) + " ";
		}
		if(index - 7 >= 0 && (board[index - 7] == null || board[index - 7].isWhite != board[index].isWhite)){
			vals += (index - 7) + " ";
		}
		if(index - 1 >= 0 && (board[index - 1] == null || board[index - 1].isWhite != board[index].isWhite)){
			vals += (index - 1) + " ";
		}
		if(index + 1 < 64 && (board[index + 1] == null || board[index + 1].isWhite != board[index].isWhite)){
			vals += (index + 1) + " ";
		}
		if(index + 7 < 64 && (board[index + 7] == null || board[index + 7].isWhite != board[index].isWhite)){
			vals += (index + 7) + " ";
		}
		if(index + 8 < 64 && (board[index + 8] == null || board[index + 8].isWhite != board[index].isWhite)){
			vals += (index + 8) + " ";
		}
		if(index + 9 < 64 && (board[index + 9] == null || board[index + 9].isWhite != board[index].isWhite)){
			vals += (index + 9) + " ";
		}
		if(vals.Length > 0){
			vals = vals.Substring(0, vals.Length - 1);
		}
			return vals;
	}

	public bool isCheck(int index1, int index2){
		for(int i = 0; i < 8; i++){
			for(int j = 0; j < 8; j++){
				if(board[index2].isWhite != board[index1].isWhite){
					
				}
			}
		}
		
		return false;
	}
	
	//Loops through the board to generate a Fen-style string of the data
	public string CreateFenString(){
		string fenString = "";
		int emptySpace = 0;
		
		for(int i = 0; i < 64; i++){
			if(i % 8 == 0 && i != 0){
				if(emptySpace > 0){
					emptySpace += 1;
					fenString += emptySpace.ToString();
					emptySpace = 0;
				}
				fenString += "/";
			}
			else if(board[i] != null){
				if(emptySpace > 0){
					fenString += emptySpace.ToString();
					emptySpace = 0;
				}
				if(board[i].isWhite == true){
					fenString += Char.ToUpper(board[i].name[0]);
				}else{
					fenString += board[i].name[0];
				}
			}
			else{
				emptySpace += 1;
			}
		}
		
		fenString += " ";
		if(isWhitesTurn){
			fenString += "w ";
		}else{
			fenString += "b ";
		}
		
		if(canCastle[0]){
			fenString += "K";
		}
		if(canCastle[1]){
			fenString += "Q";
		}
		if(canCastle[2]){
			fenString += "k";
		}
		if(canCastle[3]){
			fenString += "q";
		}
		
		//FIXME If there is EnPassant on the board
		//fenString += thatSpace;
		fenString += " - ";
		
		fenString += turn + " " + halfTurn;
		
		return fenString;
	}
	
}
