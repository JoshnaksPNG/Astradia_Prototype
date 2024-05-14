using Godot;
using Godot.Collections;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;

public partial class NPCScript : Node2D
{
	[Export]
	Area2D InteractionArea;
	
	[Export]
	Node2D PartyHead;
	
	[Export]
	Json Behavior;
	Dictionary<string, string> BehaviorDictionary;
	Dictionary<string, Dictionary<string, string>> DialogueDictionary;

	[Export]
	Control UI;
	Control UIDialogueBox;
	TextureRect UISpeakerImg;
	RichTextLabel UISpeakerName;
	RichTextLabel UIDialogueText;

	Array<string> nextAction;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BehaviorDictionary = Behavior.Data.AsGodotDictionary<string, string>();
		DialogueDictionary = Json.ParseString(BehaviorDictionary["dialogue"]).AsGodotDictionary<string, Dictionary<string, string>>();

        nextAction = new Array<string>();

        nextAction.Add(BehaviorDictionary["on_interact"]);
        nextAction.Add(BehaviorDictionary["action-context"]);


        UIDialogueBox = UI.GetNode<Control>("./DialogueBox");
		UISpeakerImg = UIDialogueBox.GetNode<TextureRect>("./SpeakerImage");
		UISpeakerName = UIDialogueBox.GetNode<RichTextLabel>("./SpeakerName");
		UIDialogueText = UIDialogueBox.GetNode<RichTextLabel>("./DialogueText");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(InteractionArea != null)
		{
			if(InteractionArea.OverlapsBody(PartyHead))
			{
				if (Input.IsActionJustPressed("ui_interact"))
				{
					npc_interact();
				}
			}
		}
	}

	public void npc_interact()
	{
		string interaction = nextAction[0];
		string context = nextAction[1];

		npc_action(interaction, context);
    }

	public void npc_action(string action, string actionContext)
	{
        switch (action)
        {
            case "speak":
				npc_speak(actionContext);
                break;

			case "none":
				npc_end_interaction();
				break;

            default:
                GD.PushError("Bad NPC Action Recieved: " + action);
                break;
        }
    }

	public void npc_speak(string dialogue_label)
	{
		if(!UIDialogueBox.Visible) 
		{
			UIDialogueBox.Visible = true;
		}

        ((world_party_head)PartyHead).isInInteraction = true;

        UISpeakerImg.Texture = GD.Load<Texture2D>(DialogueDictionary[dialogue_label]["image"]);
		UISpeakerName.Text = DialogueDictionary[dialogue_label]["title"];
        UIDialogueText.Text = DialogueDictionary[dialogue_label]["text"];

		nextAction[0] = DialogueDictionary[dialogue_label]["next-action"];
		nextAction[1] = DialogueDictionary[dialogue_label]["action-context"];
    }

	// Resets NPC Interaction
	public void npc_end_interaction()
	{
		UIDialogueBox.Visible = false;

        UISpeakerImg.Texture = null;
        UISpeakerName.Text = "";
        UIDialogueText.Text = "";

        nextAction[0] = BehaviorDictionary["on_interact"];
        nextAction[1] = BehaviorDictionary["action-context"];

		((world_party_head)PartyHead).isInInteraction = false;
    }
}
