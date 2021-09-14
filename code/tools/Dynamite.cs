namespace Sandbox.Tools
{
	[Library( "tool_dynamite", Title = "Dynamite", Description = "Boom goes the Dynamite", Group = "construction" )]
	public partial class Dynamite : BaseTool
	{
		/*[Net]
		public Color Tint { get; set; }*/

		const string dynamiteModel = "models/citizen_props/balloontall01.vmdl";
		PreviewEntity previewModel;

		string tag;

		public override void Activate()
		{
			base.Activate();

			if ( Host.IsServer )
			{
				Log.Info($"SERVER! {GetHashCode()}");
				return;
				//server stuff
			}
		}

		protected override bool IsPreviewTraceValid( TraceResult tr )
		{
			if ( !base.IsPreviewTraceValid( tr ) )
				return false;

			if ( tr.Entity is DynamiteEntity )
				return false;

			return true;
		}

		public override void CreatePreviews()
		{
			if ( TryCreatePreview( ref previewModel, dynamiteModel ) )
			{
				previewModel.RelativeToNormal = false;
				previewModel.Scale = 0.5f;
				previewModel.RenderColor = Color.Red;
			}
		}

		public override void Simulate()
		{
			if ( previewModel.IsValid() )
			{
				//previewModel.RenderColor = Tint;
			}

			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() )
			{
				//Putting this in Activate() does not work
				tag = $"Delta_Dynamite_{Owner.GetClientOwner().Name}";

				if ( Input.Pressed (InputButton.Reload) )
				{
					DetonateDynamite();
				}

				if ( !Input.Pressed( InputButton.Attack1 ) )
					return;

				var startPos = Owner.EyePos;
				var dir = Owner.EyeRot.Forward;

				var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
					.Ignore( Owner )
					.Run();

				if ( !tr.Hit || !tr.Entity.IsValid() )
					return;

				CreateHitEffects( tr.EndPos );

				var ent = new DynamiteEntity
				{
					Position = tr.EndPos,
					Scale = 0.5f,
					RenderColor = Color.Red,
				};

				ent.SetModel(dynamiteModel);
				ent.SetupPhysicsFromModel(PhysicsMotionType.Dynamic, true);
				ent.Tags.Add( tag );
			}
		}

		//same as in Game.Deltabox
		//No way to call cmds from code??
		public void DetonateDynamite()
		{
			Log.Info( $"DetonateDynamite called by {tag}" );

			foreach ( var ent in Entity.All )
			{
				if ( ent.Tags.Has( tag ) )
				{
					Log.Info( $"Found {ent} with tag '{tag}'" );
					if ( ent is DynamiteEntity dynamite )
					{
						dynamite.Detonate();
					}
					else
					{
						Log.Warning( "Ent was not a DynamiteEntity" );
					}
				}
			}
		}
	}
}
