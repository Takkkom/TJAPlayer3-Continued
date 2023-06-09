﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace FDK
{
	public class FDKCommon
	{
		// 解放

		public static void Dispose<T>( ref T obj )
		{
			if( obj == null )
				return;

			var d = obj as IDisposable;

			if( d != null )
			{
				d.Dispose();
				obj = default( T );
			}
		}
		public static void Dispose<T>( T obj )
		{
			if( obj == null )
				return;

			var d = obj as IDisposable;

			if( d != null )
				d.Dispose();
		}
		public static void ReleaseCOMObject<T>( ref T obj )
		{
			if( obj != null )
			{
				try
				{
					Marshal.ReleaseComObject( obj );
				}
				catch
				{
					// COMがマネージドコードで書かれている場合、ReleaseComObject は例外を発生させる。
					// http://www.infoq.com/jp/news/2010/03/ReleaseComObject-Dangerous
				}

				obj = default( T );
			}
		}

		public static void RunGarbageCollector()
		{
			GC.Collect();					// アクセス不可能なオブジェクトを除去し、ファイナライぜーション実施。
			GC.WaitForPendingFinalizers();	// ファイナライゼーションが終わるまでスレッドを待機。
			GC.Collect();					// ファイナライズされたばかりのオブジェクトに関連するメモリを開放。

			// 出展: http://msdn.microsoft.com/ja-jp/library/ms998547.aspx#scalenetchapt05_topic10
		}


		// ログ

		public static void LogBlock( string text, MethodInvoker method )
		{
			Trace.TraceInformation( "--------------------" );
			Trace.TraceInformation( "開始 - " + text );
			Trace.Indent();
			try
			{
				method();
			}
			finally
			{
				Trace.Unindent();
				Trace.TraceInformation( "終了 - " + text );
				Trace.TraceInformation( "--------------------" );
			}
		}
		public static void LogDetailsOfExceptions( Exception e )
		{
			Trace.WriteLine( "---例外ここから----" );
			Trace.WriteLine( e.ToString() );
			Trace.WriteLine( "---例外ここまで----" );
		}


		// IO

		public static string SearchesForFilesWithaGivenExtension( string dirPath, List<string> extensions )
		{
			string[] files = Directory.GetFiles( dirPath );		// GetFiles() は完全パスを返す。


			// ファイル順より拡張子順を優先して検索する。→ 拡張子リストの前方の拡張子ほど先に発見されるようにするため。

			foreach( string ext in extensions )
			{
				foreach( string file in files )
				{
					string fileExt = Path.GetExtension( file );

					if( fileExt.Equals( ext, StringComparison.OrdinalIgnoreCase ) )
						return file;		// あった
				}
			}

			return null;	// なかった
		}

		public static void ReadXML<T>( string path, out T xmlObject )
		{
			xmlObject = default( T );

			FileStream fs = null;
			StreamReader sr = null;
			try
			{
				fs = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite );	// FileShare を付けとかないと、Close() 後もロックがかかる。
				sr = new StreamReader( fs, Encoding.UTF8 );
				var xmlsl = new System.Xml.Serialization.XmlSerializer( typeof( T ) );
				xmlObject = (T) xmlsl.Deserialize( sr );
			}
			finally
			{
				if( sr != null )
					sr.Close();		// fr も一緒にClose()される
			}
		}
		public static void SaveXML<T>( string path, T xmlObject )
		{
			FileStream fs = null;
			StreamWriter sw = null;
			try
			{
				fs = new FileStream( path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite );	// FileShare を付けとかないと、Close() 後もロックがかかる。
				sw = new StreamWriter( fs, Encoding.UTF8 );
				var xmlsl = new System.Xml.Serialization.XmlSerializer( typeof( T ) );
				xmlsl.Serialize( sw, xmlObject );
			}
			finally
			{
				if( sw != null )
					sw.Close();		// fs も一緒にClose()される
			}
		}


		// 数学

		public static double DegreeToRadian( double angle )
		{
			return ( ( Math.PI * angle ) / 180.0 );
		}
		public static double RadianToDegree( double angle )
		{
			return ( angle * 180.0 / Math.PI );
		}
		public static float DegreeToRadian( float angle )
		{
			return (float) DegreeToRadian( (double) angle );
		}
		public static float RadianToDegree( float angle )
		{
			return (float) RadianToDegree( (double) angle );
		}

        public static bool bToggleBoolian( ref bool bFlag )
        {
            if( bFlag == true ) bFlag = false;
            else if( bFlag == false ) bFlag = true;

            return true;
        }
	}	
}
