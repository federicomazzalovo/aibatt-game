using Godot;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

public partial class Login : Control
{
	private OptionButton classesDropDownList;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.classesDropDownList = this.GetNode("VBoxContainer/ClassesDropDownList") as OptionButton;
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }

	private void _on_Button_pressed()
	{
		LineEdit usernameInput = this.GetNode("VBoxContainer/UsernameInput") as LineEdit;
		string usernameValue = usernameInput.Text;
		if (String.IsNullOrWhiteSpace(usernameValue))
			return;

		// Session.Username = usernameValue;
		// Session.ClassId = selectedClassId;

		var values = new { username = usernameValue, characterClass = 1 };

		var content = new StringContent(JsonConvert.SerializeObject(values), Encoding.UTF8, "application/json");

		using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
		{
			HttpResponseMessage response = client.PostAsync($"http://localhost:8080/api/user/login", content).Result;
			GD.Print(response);

			if (response.IsSuccessStatusCode)
				this.GetTree().ChangeSceneToFile("res://Scene/World.tscn");
		}
	}

}


