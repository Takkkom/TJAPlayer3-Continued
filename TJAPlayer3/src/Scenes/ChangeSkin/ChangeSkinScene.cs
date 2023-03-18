using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO;
using FDK;
using System.Runtime.Serialization.Formatters.Binary;


namespace TJAPlayer3
{
	/// <summary>
	/// box.defによるスキン変更時に一時的に遷移する、スキン画像の一切無いステージ。
	/// </summary>
	internal class ChangeSkinScene : BaseScene
	{
		// コンストラクタ

		public ChangeSkinScene()
		{
			base.eステージID = BaseScene.Eステージ.ChangeSkin;
			base.NotActivated = true;
		}


		// CStage 実装

		public override void Activate()
		{
			Trace.TraceInformation( "スキン変更ステージを活性化します。" );
			Trace.Indent();
			try
			{
				base.Activate();
				Trace.TraceInformation( "スキン変更ステージの活性化を完了しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
		}
		public override void Deactivate()
		{
			Trace.TraceInformation( "スキン変更ステージを非活性化します。" );
			Trace.Indent();
			try
			{
				base.Deactivate();
				Trace.TraceInformation( "スキン変更ステージの非活性化を完了しました。" );
			}
			finally
			{
				Trace.Unindent();
			}
		}
		public override void ManagedCreateResources()
		{
			if( !base.NotActivated )
			{
				base.ManagedCreateResources();
			}
		}
		public override void ManagedReleaseResources()
		{
			if( !base.NotActivated )
			{
				base.ManagedReleaseResources();
			}
		}
		public override int Draw()
		{
			if( !base.NotActivated )
			{
				if ( base.JustStartedUpdate )
				{
					base.JustStartedUpdate = false;
					return 0;
				}

                //スキン変更処理
                TJAPlayer3.app.RefleshSkin();

                return 1;
			}
			return 0;
		}
		//public void tChangeSkinMain()
		//{
		//	Trace.TraceInformation( "スキン変更:" + CDTXMania.Skin.GetCurrentSkinSubfolderFullName( false ) );

		//	CDTXMania.act文字コンソール.On非活性化();

		//	CDTXMania.Skin.PrepareReloadSkin();
		//	CDTXMania.Skin.ReloadSkin();


  //          CDTXMania.Tx.DisposeTexture();
  //          CDTXMania.Tx.LoadTexture();

		//	CDTXMania.act文字コンソール.On活性化();
		//}
	}
}
