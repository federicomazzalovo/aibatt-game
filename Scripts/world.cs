using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using MyFlyff.Session;
using System.Linq;
using System.Net.Http;

public partial class Character
{
	public double InitHp { get; set; }
	public double Hp { get; set; }
	public int Id { get; set; }
	public int Level { get; set; }
	public double Resistance { get; set; }
	public CharacterPosition Position { get; set; }
	public CharacterRotation Rotation { get; set; }
	public bool Dead { get { return this.Hp == 0; } }

	public bool IsPlayer { get; set; }
	public string Username { get; set; }
}

public class CharacterPosition
{
	public CharacterPosition(float x, float y, float z = 0)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public float x { get; set; }
	public float y { get; set; }
	public float z { get; set; }
}

public class CharacterRotation
{
	public CharacterRotation(float x, float y, float z = 0)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public float x { get; set; }
	public float y { get; set; }
	public float z { get; set; }
}

public partial class World : Node3D
{
	private CharacterNode playerNode;
	// List of character exclude the current player
	private List<CharacterNode> characterNodes = new List<CharacterNode>();

	private CharacterNode enemyModel;
	private IEnumerable<Character> enemies;
	private Character player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.CreateTrees();

		this.enemyModel = this.GetNode("Enemy") as CharacterNode;
		this.playerNode = this.GetNode("Player") as CharacterNode;

		this.LoadCharacters();

		var err = WebSocketService.GetInstance().Connect("simple-rpg-kata-ws");

		if (err != Error.Ok)
		{
			GD.Print("Unable to connect");
			this.SetProcess(false);
		}
	}

	private void CreateTrees()
	{
		GD.Print(DateTime.Now + " - Creating trees");
		var treeNode = this.GetNode<Node3D>("Tree");

		var random = new Random(100);
		for (int i = 0; i < 100; i++)
		{
			var newTree = treeNode.Duplicate() as Node3D;
			newTree.Name = "Tree" + i;
			newTree.Position = new Vector3(random.Next(-100, 100), treeNode.Position.Y, random.Next(-100, 100));
			GD.Print(newTree.Position);
			newTree.Visible = true;
			this.AddChild(newTree);
		}

		GD.Print(DateTime.Now + " - Done creating trees");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		WebSocketService.GetInstance().Poll();
		var state = WebSocketService.GetInstance().GetReadyState();
		if (state == WebSocketPeer.State.Open)
		{
			while (WebSocketService.GetInstance().GetAvailablePacketCount() > 0)
			{
				var message = WebSocketService.GetInstance().GetPacket();
				this.DataChanged(message);
			}
		}
		else if (state == WebSocketPeer.State.Closing)
		{
			// Keep polling to achieve proper close.
		}
		else if (state == WebSocketPeer.State.Closed)
		{
			var code = WebSocketService.GetInstance().GetCloseCode();
			var reason = WebSocketService.GetInstance().GetCloseReason();
			GD.Print($"WebSocket closed with code: {code}, reason {reason}. Clean: {code != -1}");
			SetProcess(false);
		}
	}

	private List<Character> RetrieveConnectedCharacters()
	{
		using (var client = new System.Net.Http.HttpClient())
		{
			HttpResponseMessage response = client.GetAsync($"{Configurations.API_URL}/api/character/all/connected").Result;

			string json = response.Content.ReadAsStringAsync().Result;

			return JsonConvert.DeserializeObject<List<Character>>(json);
		}
	}

	private void LoadCharacters()
	{
		var characters = this.RetrieveConnectedCharacters();

		this.player = characters.SingleOrDefault(character => character.Username == ConnectedUser.Username);
		this.enemies = characters.Where(character => character != this.player);

		this.playerNode.Initialize(this.player);
		this.characterNodes.Add(this.playerNode);

		foreach (Character character in this.enemies)
			this.AddEnemy(character);
	}

	private CharacterNode AddEnemy(Character character)
	{
		CharacterNode newEnemy = this.enemyModel.Duplicate() as CharacterNode;
		this.AddChild(newEnemy);

		newEnemy.Initialize(character);

		newEnemy.Show();

		this.characterNodes.Add(newEnemy);

		return newEnemy;
	}

	private void DataChanged(object e)
	{
		// For now  this method only receive a list of character position
		string message = e as string;

		try
		{
			WebSocketHandshake handshake = JsonConvert.DeserializeObject<WebSocketHandshake>(message);
			ConnectedUser.WSSessionId = handshake.sessionId;
			handshake.username = ConnectedUser.Username;
			string msgToSend = JsonConvert.SerializeObject(handshake);
			WebSocketService.GetInstance().SendMessage(msgToSend);
		}
		catch (Exception)
		{
			try
			{
				List<WebSocketParams> webSocketParamsList = JsonConvert.DeserializeObject<List<WebSocketParams>>(message);

				var changedCharactersIds = webSocketParamsList.Select(w => w.characterId);
				var existingCharactersIds = this.characterNodes.Select(w => w.Character.Id);
				List<CharacterNode> toUpdate = this.characterNodes.Where(x => changedCharactersIds.Contains(x.Character.Id)).ToList();
				List<CharacterNode> toDelete = this.characterNodes.Where(x => !changedCharactersIds.Contains(x.Character.Id)).ToList();
				List<WebSocketParams> toAdd = webSocketParamsList.Where(x => !existingCharactersIds.Contains(x.characterId)).ToList();


				foreach (CharacterNode node in toUpdate)
				{
					WebSocketParams param = webSocketParamsList.SingleOrDefault((Func<WebSocketParams, bool>)(x => x.characterId == node.Character.Id));
					node.UpdateCharacter(param);
				}

				foreach (WebSocketParams param in toAdd)
					this.AddEnemy(new Character() { 
						Id = param.characterId, 
						Hp = param.hp, 
						InitHp = param.initHp, 
						Level = param.level, 
						Position = new CharacterPosition(param.positionX, param.positionY, param.positionZ),
						Rotation = new CharacterRotation(param.rotationX, param.rotationY, param.rotationZ) 
					});

				foreach (CharacterNode node in toDelete)
				{
					this.RemoveChild(node);
					this.characterNodes.Remove(node);
				}
			}
			catch (Exception) { }
		}
	}
}
