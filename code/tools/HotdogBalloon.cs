﻿namespace Sandbox.Tools
{
	[Library( "tool_hotdogballoon", Title = "Hotdog Balloon", Description = "Create Hotdog Balloons!", Group = "fun" )]
	public partial class HotdogBalloon : BaseTool
	{
		PreviewEntity previewModel;
		private static string ballonModel = "models/citizen_props/hotdog01.vmdl";

		public override void Activate()
		{
			base.Activate();

			if ( Host.IsServer )
			{
				
			}
		}

		protected override bool IsPreviewTraceValid( TraceResult tr )
		{
			if ( !base.IsPreviewTraceValid( tr ) )
				return false;

			if ( tr.Entity is BalloonEntity )
				return false;

			return true;
		}

		public override void CreatePreviews()
		{
			if ( TryCreatePreview( ref previewModel, ballonModel ) )
			{
				previewModel.RelativeToNormal = false;
			}
		}

		public override void Simulate()
		{
			if ( previewModel.IsValid() )
			{
				
			}

			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() )
			{
				bool useRope = Input.Pressed( InputButton.Attack1 );
				if ( !useRope && !Input.Pressed( InputButton.Attack2 ) )
					return;

				var startPos = Owner.EyePosition;
				var dir = Owner.EyeRotation.Forward;

				var tr = Trace.Ray( startPos, startPos + dir * MaxTraceDistance )
					.Ignore( Owner )
					.Run();

				if ( !tr.Hit )
					return;

				if ( !tr.Entity.IsValid() )
					return;

				CreateHitEffects( tr.EndPosition );

				if ( tr.Entity is BalloonEntity )
					return;

				var ent = new BalloonEntity
				{
					Position = tr.EndPosition,
				};

				ent.SetModel( ballonModel );
				ent.PhysicsBody.GravityScale = -0.1f;
				ent.RenderColor = Color.White;

				if ( !useRope )
					return;

				var rope = Particles.Create( "particles/rope.vpcf" );
				rope.SetEntity( 0, ent );

				var attachEnt = tr.Body.IsValid() ? tr.Body.GetEntity() : tr.Entity;
				var attachLocalPos = tr.Body.Transform.PointToLocal( tr.EndPosition ) * (1.0f / tr.Entity.Scale);

				if ( attachEnt.IsWorld )
				{
					rope.SetPosition( 1, attachLocalPos );
				}
				else
				{
					rope.SetEntityBone( 1, attachEnt, tr.Bone, new Transform( attachLocalPos ) );
				}

				var spring = PhysicsJoint.CreateLength( ent.PhysicsBody, PhysicsPoint.World( tr.Body, tr.EndPosition ), 100 );
				spring.SpringLinear = new( 5, 0.7f );
				spring.Collisions = true;
				spring.EnableAngularConstraint = false;
				spring.OnBreak += () =>
				{
					rope?.Destroy( true );
					spring.Remove();
				};
			}
		}
	}
}
