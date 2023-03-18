using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace FDK
{
	/// <summary>
	/// テクスチャの作成に失敗しました。
	/// </summary>
	public class TextureCreateFailedException : Exception
	{
		public TextureCreateFailedException()
		{
		}
		public TextureCreateFailedException( string message )
			: base( message )
		{
		}
		public TextureCreateFailedException( SerializationInfo info, StreamingContext context )
			: base( info, context )
		{
		}
		public TextureCreateFailedException( string message, Exception innerException )
			: base( message, innerException )
		{
		}
	}
}
