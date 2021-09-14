namespace Sandbox.Tools
{
	[Library( "tool_dynamite", Title = "Dynamite", Description = "Attack 1 to place Dynamite\nReload to Detonate it", Group = "construction" )]
	public partial class Dynamite : BaseTool
	{
		private const string dynamiteModel = "models/citizen_props/balloontall01.vmdl";
		private PreviewEntity previewModel;

		private string tag;

		public override void Activate()
		{
			base.Activate();
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
			}
		}

		public override void Simulate()
		{
			if ( previewModel.IsValid() )
			{
				previewModel.RenderColor = Color.Red;
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
		private void DetonateDynamite()
		{
			foreach ( var ent in Entity.All )
			{
				if ( ent.Tags.Has( tag ) )
				{
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
