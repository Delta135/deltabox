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

	}

	public void Detonate()
	{
		Log.Info("BOOM!");

		Particles.Create( "particles/explosion.vpcf", Position );
		Sound.FromWorld( "sounds/common/explosions/explo_gas_can_01.vsnd", Position );

		this.OnKilled();
	}
}
