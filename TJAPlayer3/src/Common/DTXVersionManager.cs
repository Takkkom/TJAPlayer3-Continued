using System;
using System.Collections.Generic;
using System.Text;

namespace TJAPlayer3
{
	/// <summary>
	/// <para>DTXMania のバージョン。</para>
	/// <para>例1："078b" → 整数部=078, 小数部=2000000 ('英字'+'yymmdd') </para>
	/// <para>例2："078a(100124)" → 整数部=078, 小数部=1100124 ('英字'+'yymmdd')</para>
	/// </summary>
    public class DTXVersionManager
	{
		// プロパティ

		/// <summary>
		/// <para>バージョンが未知のときに true になる。</para>
		/// </summary>
		public bool Unknown
		{
			get;
			private set;
		}

		/// <summary>
		/// <para>DTXMania のバージョンの整数部を表す。</para>
		/// <para>例1："078b" → 整数部=078</para>
		/// <para>例2："078a(100124)" → 整数部=078</para>
		/// </summary>
		public int IntegerPart;

		/// <summary>
		/// <para>DTXMania のバージョンの小数部を表す。</para>
		/// <para>小数部は、'英字(0～26) * 1000000 + 日付(yymmdd)' の式で表される整数。</para>
		/// <para>例1："078b" → 小数部=2000000 </para>
		/// <para>例2："078a(100124)" → 小数部=1100124</para>
		/// </summary>
		public int DecimalPart;


		// コンストラクタ

		public DTXVersionManager()
		{
			this.IntegerPart = 0;
			this.DecimalPart = 0;
			this.Unknown = true;
		}
		public DTXVersionManager( int integerPart )
		{
			this.IntegerPart = integerPart;
			this.DecimalPart = 0;
			this.Unknown = false;
		}
		public DTXVersionManager( string versionName )
		{
			this.IntegerPart = 0;
			this.DecimalPart = 0;
			this.Unknown = true;
			
			if( versionName.ToLower().Equals( "unknown" ) )
			{
				this.Unknown = true;
			}
			else
			{
				int num = 0;
				int length = versionName.Length;
				if( ( num < length ) && char.IsDigit( versionName[ num ] ) )
				{
					// 整数部　取得
					while( ( num < length ) && char.IsDigit( versionName[ num ] ) )
					{
						this.IntegerPart = ( this.IntegerPart * 10 ) + DTXVersionManager.DIG10.IndexOf( versionName[ num++ ] );
					}

					// 小数部(1)英字部分　取得
					while( ( num < length ) && ( ( versionName[ num ] == ' ' ) || ( versionName[ num ] == '(' ) ) )
					{
						num++;
					}
					if( ( num < length ) && ( DTXVersionManager.DIG36.IndexOf( versionName[ num ] ) >= 10 ) )
					{
						this.DecimalPart = DTXVersionManager.DIG36.IndexOf( versionName[ num++ ] ) - 10;
						if( this.DecimalPart >= 0x1a )
						{
							this.DecimalPart -= 0x1a;
						}
						this.DecimalPart++;
					}

					// 小数部(2)日付部分(yymmdd)　取得
					while( ( num < length ) && ( ( versionName[ num ] == ' ' ) || ( versionName[ num ] == '(' ) ) )
					{
						num++;
					}
					for( int i = 0; i < 6; i++ )
					{
						this.DecimalPart *= 10;
						if( ( num < length ) && char.IsDigit( versionName[ num ] ) )
						{
							this.DecimalPart += DTXVersionManager.DIG10.IndexOf( versionName[ num ] );
						}
						num++;
					}
					this.Unknown = false;
				}
				else
				{
					this.Unknown = true;
				}
			}
		}
		public DTXVersionManager( int integerPart, int decimalPart )
		{
			this.IntegerPart = integerPart;
			this.DecimalPart = decimalPart;
			this.Unknown = false;
		}

	
		// メソッド
		
		public string GetString()
		{
			var result = new StringBuilder( 32 );

			// 整数部
			result.Append( this.IntegerPart.ToString( "000" ) );

			// 英字部分（あれば）
			if( this.DecimalPart >= 1000000 )
			{
				int n英字 = Math.Min( this.DecimalPart / 1000000, 26 );	// 1～26
				result.Append( DTXVersionManager.DIG36[ 10 + ( n英字 - 1 ) ] );
			}

			// 日付部分（あれば）
			int date = this.DecimalPart % 1000000;
			if( date > 0 )
			{
				result.Append( '(' );
				result.Append( date.ToString( "000000" ) );
				result.Append( ')' );
			}

			return result.ToString();
		}

		public static bool operator ==( DTXVersionManager x, DTXVersionManager y )
		{
			return ( ( ( x.IntegerPart == y.IntegerPart ) && ( x.DecimalPart == y.DecimalPart ) ) && ( x.Unknown == y.Unknown ) );
		}
		public static bool operator >( DTXVersionManager x, DTXVersionManager y )
		{
			return ( ( x.IntegerPart > y.IntegerPart ) || ( ( x.IntegerPart == y.IntegerPart ) && ( x.DecimalPart > y.DecimalPart ) ) );
		}
		public static bool operator >=( DTXVersionManager x, DTXVersionManager y )
		{
			return ( ( x.IntegerPart > y.IntegerPart ) || ( ( x.IntegerPart == y.IntegerPart ) && ( x.DecimalPart >= y.DecimalPart ) ) );
		}
		public static bool operator !=( DTXVersionManager x, DTXVersionManager y )
		{
			if( ( x.IntegerPart == y.IntegerPart ) && ( x.DecimalPart == y.DecimalPart ) )
			{
				return ( x.Unknown != y.Unknown );
			}
			return true;
		}
		public static bool operator <( DTXVersionManager x, DTXVersionManager y )
		{
			return ( ( x.IntegerPart < y.IntegerPart ) || ( ( x.IntegerPart == y.IntegerPart ) && ( x.DecimalPart < y.DecimalPart ) ) );
		}
		public static bool operator <=( DTXVersionManager x, DTXVersionManager y )
		{
			return ( ( x.IntegerPart < y.IntegerPart ) || ( ( x.IntegerPart == y.IntegerPart ) && ( x.DecimalPart <= y.DecimalPart ) ) );
		}
		public override bool Equals(object obj)			// 2011.1.3 yyagi: warningを無くすために追加
		{
			if (obj == null)
			{
				return false;
			}
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			DTXVersionManager objCDTXVersion = (DTXVersionManager)obj;
			if (!int.Equals(this.IntegerPart, objCDTXVersion.IntegerPart) || !int.Equals(this.DecimalPart, objCDTXVersion.DecimalPart))
			{
				return false;
			}
			return true;
		}
		public override int GetHashCode()				// 2011.1.3 yyagi: warningを無くすために追加
		{
			string v = this.GetString();
			return v.GetHashCode();
		}

		// その他

		#region [ private ]
		//-----------------
		private const string DIG36 = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private const string DIG10 = "0123456789";
		//-----------------
		#endregion
	}
}
