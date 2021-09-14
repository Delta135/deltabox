using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Library("ent_dynamite", Title = "Dynamite", Spawnable = false)]
public partial class DynamiteEntity : Prop
{
	public override void Spawn()
	{
		base.Spawn();
	}

	public void Detonate()
	{
		Particles.Create( "particles/explosion.vpcf", Position );
		Sound.FromEntity( "explode", this);

		this.OnKilled();
	}
}
