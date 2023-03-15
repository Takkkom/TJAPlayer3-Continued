using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using SlimDX;
using SlimDX.Direct3D9;

using Device = SampleFramework.DeviceCache;

namespace FDK
{
    public class FDKTexture : IDisposable
    {
        // プロパティ
        public bool IsAddBlend
        {
            get;
            set;
        }
        public bool IsMultiplyBlend
        {
            get;
            set;
        }
        public bool IsSubBlend
        {
            get;
            set;
        }
        public bool IsScreenBlend
        {
            get;
            set;
        }
        public float Rotation
        {
            get;
            set;
        }
        public int Opacity
        {
            get
            {
                return this._opacity;
            }
            set
            {
                if (value < 0)
                {
                    this._opacity = 0;
                }
                else if (value > 0xff)
                {
                    this._opacity = 0xff;
                }
                else
                {
                    this._opacity = value;
                }
            }
        }
        public Size TextureSize
        {
            get;
            private set;
        }
        public Size ImageSize
        {
            get;
            protected set;
        }
        public Texture texture
        {
            get;
            private set;
        }
        public Format Format
        {
            get;
            protected set;
        }
        public Vector3 Scaling;

        // 画面が変わるたび以下のプロパティを設定し治すこと。

        public static Size sz論理画面 = Size.Empty;
        public static Size sz物理画面 = Size.Empty;
        public static Rectangle rc物理画面描画領域 = Rectangle.Empty;
        /// <summary>
        /// <para>論理画面を1とする場合の物理画面の倍率。</para>
        /// <para>論理値×画面比率＝物理値。</para>
        /// </summary>
        public static float ScreenRatio = 1.0f;

        // コンストラクタ

        public FDKTexture()
        {
            this.ImageSize = new Size(0, 0);
            this.TextureSize = new Size(0, 0);
            this._opacity = 0xff;
            this.texture = null;
            this.cvPositionColoredVertexies = null;
            this.IsAddBlend = false;
            this.Rotation = 0f;
            this.Scaling = new Vector3(1f, 1f, 1f);
            //			this._txData = null;
        }

        /// <summary>
        /// <para>指定されたビットマップオブジェクトから Managed テクスチャを作成する。</para>
        /// <para>テクスチャのサイズは、BITMAP画像のサイズ以上、かつ、D3D9デバイスで生成可能な最小のサイズに自動的に調節される。
        /// その際、テクスチャの調節後のサイズにあわせた画像の拡大縮小は行わない。</para>
        /// <para>その他、ミップマップ数は 1、Usage は None、Pool は Managed、イメージフィルタは Point、ミップマップフィルタは
        /// None、カラーキーは 0xFFFFFFFF（完全なる黒を透過）になる。</para>
        /// </summary>
        /// <param name="device">Direct3D9 デバイス。</param>
        /// <param name="bitmap">作成元のビットマップ。</param>
        /// <param name="format">テクスチャのフォーマット。</param>
        /// <exception cref="CTextureCreateFailedException">テクスチャの作成に失敗しました。</exception>
        public FDKTexture(Device device, Bitmap bitmap, Format format)
            : this()
        {
            try
            {
                this.Format = format;
                this.ImageSize = new Size(bitmap.Width, bitmap.Height);
                this.TextureSize = this.GetOptimalTextureSizeNotExceedingSpecifiedSize(device, this.ImageSize);
                this.FullImage = new Rectangle(0, 0, this.ImageSize.Width, this.ImageSize.Height);

                using (var stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Bmp);
                    stream.Seek(0L, SeekOrigin.Begin);
                    int colorKey = unchecked((int)0xFF000000);
                    this.texture = Texture.FromStream(device.UnderlyingDevice, stream, this.TextureSize.Width, this.TextureSize.Height, 1, Usage.None, format, poolvar, Filter.Point, Filter.None, colorKey);
                }
            }
            catch (Exception e)
            {
                this.Dispose();
                throw new CTextureCreateFailedException("ビットマップからのテクスチャの生成に失敗しました。", e);
            }
        }

        /// <summary>
        /// <para>空の Managed テクスチャを作成する。</para>
        /// <para>テクスチャのサイズは、指定された希望サイズ以上、かつ、D3D9デバイスで生成可能な最小のサイズに自動的に調節される。
        /// その際、テクスチャの調節後のサイズにあわせた画像の拡大縮小は行わない。</para>
        /// <para>テクスチャのテクセルデータは未初期化。（おそらくゴミデータが入ったまま。）</para>
        /// <para>その他、ミップマップ数は 1、Usage は None、イメージフィルタは Point、ミップマップフィルタは None、
        /// カラーキーは 0x00000000（透過しない）になる。</para>
        /// </summary>
        /// <param name="device">Direct3D9 デバイス。</param>
        /// <param name="width">テクスチャの幅（希望値）。</param>
        /// <param name="height">テクスチャの高さ（希望値）。</param>
        /// <param name="format">テクスチャのフォーマット。</param>
        /// <exception cref="CTextureCreateFailedException">テクスチャの作成に失敗しました。</exception>
        public FDKTexture(Device device, int width, int height, Format format)
            : this(device, width, height, format, Pool.Managed)
        {
        }

        /// <summary>
        /// <para>指定された画像ファイルから Managed テクスチャを作成する。</para>
        /// <para>利用可能な画像形式は、BMP, JPG, PNG, TGA, DDS, PPM, DIB, HDR, PFM のいずれか。</para>
        /// </summary>
        /// <param name="device">Direct3D9 デバイス。</param>
        /// <param name="path">画像ファイル名。</param>
        /// <param name="format">テクスチャのフォーマット。</param>
        /// <param name="transparentBlack">画像の黒（0xFFFFFFFF）を透過させるなら true。</param>
        /// <exception cref="CTextureCreateFailedException">テクスチャの作成に失敗しました。</exception>
        public FDKTexture(Device device, string path, Format format, bool transparentBlack)
            : this(device, path, format, transparentBlack, Pool.Managed)
        {
        }
        public FDKTexture(Device device, byte[] data, Format format, bool transparentBlack)
            : this(device, data, format, transparentBlack, Pool.Managed)
        {
        }
        public FDKTexture(Device device, Bitmap bitmap, Format format, bool transparentBlack)
            : this(device, bitmap, format, transparentBlack, Pool.Managed)
        {
        }

        /// <summary>
        /// <para>空のテクスチャを作成する。</para>
        /// <para>テクスチャのサイズは、指定された希望サイズ以上、かつ、D3D9デバイスで生成可能な最小のサイズに自動的に調節される。
        /// その際、テクスチャの調節後のサイズにあわせた画像の拡大縮小は行わない。</para>
        /// <para>テクスチャのテクセルデータは未初期化。（おそらくゴミデータが入ったまま。）</para>
        /// <para>その他、ミップマップ数は 1、Usage は None、イメージフィルタは Point、ミップマップフィルタは None、
        /// カラーキーは 0x00000000（透過しない）になる。</para>
        /// </summary>
        /// <param name="device">Direct3D9 デバイス。</param>
        /// <param name="width">テクスチャの幅（希望値）。</param>
        /// <param name="height">テクスチャの高さ（希望値）。</param>
        /// <param name="format">テクスチャのフォーマット。</param>
        /// <param name="pool">テクスチャの管理方法。</param>
        /// <exception cref="CTextureCreateFailedException">テクスチャの作成に失敗しました。</exception>
        public FDKTexture(Device device, int width, int height, Format format, Pool pool)
            : this(device, width, height, format, pool, Usage.None)
        {
        }

        public FDKTexture(Device device, int width, int height, Format format, Pool pool, Usage usage)
            : this()
        {
            try
            {
                this.Format = format;
                this.ImageSize = new Size(width, height);
                this.TextureSize = this.GetOptimalTextureSizeNotExceedingSpecifiedSize(device, this.ImageSize);
                this.FullImage = new Rectangle(0, 0, this.ImageSize.Width, this.ImageSize.Height);

                using (var bitmap = new Bitmap(1, 1))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.FillRectangle(Brushes.Black, 0, 0, 1, 1);
                    }
                    using (var stream = new MemoryStream())
                    {
                        bitmap.Save(stream, ImageFormat.Bmp);
                        stream.Seek(0L, SeekOrigin.Begin);
#if TEST_Direct3D9Ex
						pool = poolvar;
#endif
                        // 中で更にメモリ読み込みし直していて無駄なので、Streamを使うのは止めたいところ
                        this.texture = Texture.FromStream(device.UnderlyingDevice, stream, width, height, 1, usage, format, pool, Filter.Point, Filter.None, 0);
                    }
                }
            }
            catch
            {
                this.Dispose();
                throw new CTextureCreateFailedException(string.Format("テクスチャの生成に失敗しました。\n({0}x{1}, {2})", width, height, format));
            }
        }

        /// <summary>
        /// <para>画像ファイルからテクスチャを生成する。</para>
        /// <para>利用可能な画像形式は、BMP, JPG, PNG, TGA, DDS, PPM, DIB, HDR, PFM のいずれか。</para>
        /// <para>テクスチャのサイズは、画像のサイズ以上、かつ、D3D9デバイスで生成可能な最小のサイズに自動的に調節される。
        /// その際、テクスチャの調節後のサイズにあわせた画像の拡大縮小は行わない。</para>
        /// <para>その他、ミップマップ数は 1、Usage は None、イメージフィルタは Point、ミップマップフィルタは None になる。</para>
        /// </summary>
        /// <param name="device">Direct3D9 デバイス。</param>
        /// <param name="path">画像ファイル名。</param>
        /// <param name="format">テクスチャのフォーマット。</param>
        /// <param name="transparentBlack">画像の黒（0xFFFFFFFF）を透過させるなら true。</param>
        /// <param name="pool">テクスチャの管理方法。</param>
        /// <exception cref="CTextureCreateFailedException">テクスチャの作成に失敗しました。</exception>
        public FDKTexture(Device device, string path, Format format, bool transparentBlack, Pool pool)
            : this()
        {
            MakeTexture(device, path, format, transparentBlack, pool);
        }
        public void MakeTexture(Device device, string path, Format format, bool transparentBlack, Pool pool)
        {
            if (!File.Exists(path))     // #27122 2012.1.13 from: ImageInformation では FileNotFound 例外は返ってこないので、ここで自分でチェックする。わかりやすいログのために。
                throw new FileNotFoundException(string.Format("ファイルが存在しません。\n[{0}]", path));

            Byte[] _txData = File.ReadAllBytes(path);
            MakeTexture(device, _txData, format, transparentBlack, pool);
        }

        public FDKTexture(Device device, byte[] txData, Format format, bool transparentBlack, Pool pool)
            : this()
        {
            MakeTexture(device, txData, format, transparentBlack, pool);
        }
        public void MakeTexture(Device device, byte[] txData, Format format, bool transparentBlack, Pool pool)
        {
            try
            {
                var information = ImageInformation.FromMemory(txData);
                this.Format = format;
                this.ImageSize = new Size(information.Width, information.Height);
                this.FullImage = new Rectangle(0, 0, this.ImageSize.Width, this.ImageSize.Height);
                int colorKey = (transparentBlack) ? unchecked((int)0xFF000000) : 0;
                this.TextureSize = this.GetOptimalTextureSizeNotExceedingSpecifiedSize(device, this.ImageSize);
#if TEST_Direct3D9Ex
				pool = poolvar;
#endif
                //				lock ( lockobj )
                //				{
                //Trace.TraceInformation( "CTexture() start: " );
                this.texture = Texture.FromMemory(device.UnderlyingDevice, txData, this.ImageSize.Width, this.ImageSize.Height, 1, Usage.None, format, pool, Filter.Point, Filter.None, colorKey);
                //Trace.TraceInformation( "CTexture() end:   " );
                //				}
            }
            catch
            {
                this.Dispose();
                // throw new CTextureCreateFailedException( string.Format( "テクスチャの生成に失敗しました。\n{0}", strファイル名 ) );
                throw new CTextureCreateFailedException(string.Format("テクスチャの生成に失敗しました。\n"));
            }
        }

        public FDKTexture(Device device, Bitmap bitmap, Format format, bool transparentBlack, Pool pool)
            : this()
        {
            MakeTexture(device, bitmap, format, transparentBlack, pool);
        }
        public void MakeTexture(Device device, Bitmap bitmap, Format format, bool transparentBlack, Pool pool)
        {
            try
            {
                this.Format = format;
                this.ImageSize = new Size(bitmap.Width, bitmap.Height);
                this.FullImage = new Rectangle(0, 0, this.ImageSize.Width, this.ImageSize.Height);
                int colorKey = (transparentBlack) ? unchecked((int)0xFF000000) : 0;
                this.TextureSize = this.GetOptimalTextureSizeNotExceedingSpecifiedSize(device, this.ImageSize);
#if TEST_Direct3D9Ex
				pool = poolvar;
#endif
                //Trace.TraceInformation( "CTExture() start: " );
                unsafe  // Bitmapの内部データ(a8r8g8b8)を自前でゴリゴリコピーする
                {
                    int tw =
#if TEST_Direct3D9Ex
					288;		// 32の倍数にする(グラフによっては2のべき乗にしないとダメかも)
#else
                    this.ImageSize.Width;
#endif
#if TEST_Direct3D9Ex
					this.texture = new Texture( device, tw, this.sz画像サイズ.Height, 1, Usage.Dynamic, format, Pool.Default );
#else
                    this.texture = new Texture(device.UnderlyingDevice, this.ImageSize.Width, this.ImageSize.Height, 1, Usage.None, format, pool);
#endif
                    BitmapData srcBufData = bitmap.LockBits(new Rectangle(0, 0, this.ImageSize.Width, this.ImageSize.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    DataRectangle destDataRectangle = texture.LockRectangle(0, LockFlags.Discard);  // None
#if TEST_Direct3D9Ex
					byte[] filldata = null;
					if ( tw > this.sz画像サイズ.Width )
					{
						filldata = new byte[ (tw - this.sz画像サイズ.Width) * 4 ];
					}
					for ( int y = 0; y < this.sz画像サイズ.Height; y++ )
					{
						IntPtr src_scan0 = (IntPtr) ( (Int64) srcBufData.Scan0 + y * srcBufData.Stride );
						destDataRectangle.Data.WriteRange( src_scan0, this.sz画像サイズ.Width * 4  );
						if ( tw > this.sz画像サイズ.Width )
						{
							destDataRectangle.Data.WriteRange( filldata );
						}
					}
#else
                    IntPtr src_scan0 = (IntPtr)((Int64)srcBufData.Scan0);
                    destDataRectangle.Data.WriteRange(src_scan0, this.ImageSize.Width * 4 * this.ImageSize.Height);
#endif
                    texture.UnlockRectangle(0);
                    bitmap.UnlockBits(srcBufData);
                }
                //Trace.TraceInformation( "CTExture() End: " );
            }
            catch
            {
                this.Dispose();
                // throw new CTextureCreateFailedException( string.Format( "テクスチャの生成に失敗しました。\n{0}", strファイル名 ) );
                throw new CTextureCreateFailedException(string.Format("テクスチャの生成に失敗しました。\n"));
            }
        }
        // メソッド

        // 2016.11.10 kairera0467 拡張
        // Rectangleを使う場合、座標調整のためにテクスチャサイズの値をそのまま使うとまずいことになるため、Rectragleから幅を取得して調整をする。
        public void t2D中心基準描画(Device device, int x, int y)
        {
            this.Draw2D(device, x - (this.TextureSize.Width / 2), y - (this.TextureSize.Height / 2), 1f, this.FullImage);
        }
        public void t2D中心基準描画(Device device, int x, int y, Rectangle rectangle)
        {
            this.Draw2D(device, x - (rectangle.Width / 2), y - (rectangle.Height / 2), 1f, rectangle);
        }
        public void t2D中心基準描画(Device device, float x, float y)
        {
            this.Draw2D(device, (int)x - (this.TextureSize.Width / 2), (int)y - (this.TextureSize.Height / 2), 1f, this.FullImage);
        }
        public void t2D中心基準描画(Device device, float x, float y, float depth, Rectangle rectangle)
        {
            this.Draw2D(device, (int)x - (rectangle.Width / 2), (int)y - (rectangle.Height / 2), depth, rectangle);
        }

        // 下を基準にして描画する(拡大率考慮)メソッドを追加。 (AioiLight)
        public void t2D拡大率考慮下基準描画(Device device, int x, int y)
        {
            this.Draw2D(device, x, y - (TextureSize.Height * this.Scaling.Y), 1f, this.FullImage);
        }
        public void t2D拡大率考慮下基準描画(Device device, int x, int y, Rectangle rectangle)
        {
            this.Draw2D(device, x, y - (rectangle.Height * this.Scaling.Y), 1f, rectangle);
        }
        public void t2D拡大率考慮下中心基準描画(Device device, int x, int y)
        {
            this.Draw2D(device, x - (this.TextureSize.Width / 2), y - (TextureSize.Height * this.Scaling.Y), 1f, this.FullImage);
        }
        public void t2D拡大率考慮下中心基準描画(Device device, float x, float y)
        {
            this.t2D拡大率考慮下中心基準描画(device, (int)x, (int)y);
        }

        public void t2D拡大率考慮下中心基準描画(Device device, int x, int y, Rectangle rectangle)
        {
            this.Draw2D(device, x - ((rectangle.Width / 2)), y - (rectangle.Height * this.Scaling.Y), 1f, rectangle);
        }
        public void t2D拡大率考慮下中心基準描画(Device device, float x, float y, Rectangle rectangle)
        {
            this.t2D拡大率考慮下中心基準描画(device, (int)x, (int)y, rectangle);
        }
        public void t2D下中央基準描画(Device device, int x, int y)
        {
            this.Draw2D(device, x - (this.TextureSize.Width / 2), y - (TextureSize.Height), this.FullImage);
        }
        public void t2D下中央基準描画(Device device, int x, int y, Rectangle rectangle)
        {
            this.Draw2D(device, x - (rectangle.Width / 2), y - (rectangle.Height), rectangle);
            //this.t2D描画(devicek x, y, rc画像内の描画領域;
        }


        public void t2D拡大率考慮中央基準描画(Device device, int x, int y)
        {
            this.Draw2D(device, x - (this.TextureSize.Width / 2 * this.Scaling.X), y - (TextureSize.Height / 2 * this.Scaling.Y), 1f, this.FullImage);
        }
        public void t2D拡大率考慮中央基準描画(Device device, float x, float y)
        {
            this.t2D拡大率考慮下中心基準描画(device, (int)x, (int)y);
        }


        /// <summary>
        /// テクスチャを 2D 画像と見なして描画する。
        /// </summary>
        /// <param name="device">Direct3D9 デバイス。</param>
        /// <param name="x">描画位置（テクスチャの左上位置の X 座標[dot]）。</param>
        /// <param name="y">描画位置（テクスチャの左上位置の Y 座標[dot]）。</param>
        public void Draw2D(Device device, int x, int y)
        {
            this.Draw2D(device, x, y, 1f, this.FullImage);
        }
        public void Draw2D(Device device, int x, int y, Rectangle rectangle)
        {
            this.Draw2D(device, x, y, 1f, rectangle);
        }
        public void Draw2D(Device device, float x, float y)
        {
            this.Draw2D(device, (int)x, (int)y, 1f, this.FullImage);
        }
        public void Draw2D(Device device, float x, float y, float depth, Rectangle rectangle)
        {
            if (this.texture == null)
                return;

            this.SetRendererState(device);

            if (this.Rotation == 0f)
            {
                #region [ (A) 回転なし ]
                //-----------------
                float f補正値X = -0.5f;    // -0.5 は座標とピクセルの誤差を吸収するための座標補正値。(MSDN参照)
                float f補正値Y = -0.5f;    //
                float w = rectangle.Width;
                float h = rectangle.Height;
                float f左U値 = ((float)rectangle.Left) / ((float)this.TextureSize.Width);
                float f右U値 = ((float)rectangle.Right) / ((float)this.TextureSize.Width);
                float f上V値 = ((float)rectangle.Top) / ((float)this.TextureSize.Height);
                float f下V値 = ((float)rectangle.Bottom) / ((float)this.TextureSize.Height);
                this.color4.Alpha = ((float)this._opacity) / 255f;
                int color = this.color4.ToArgb();

                if (this.cvTransformedColoredVertexies == null)
                    this.cvTransformedColoredVertexies = new TransformedColoredTexturedVertex[4];

                // #27122 2012.1.13 from: 以下、マネージドオブジェクト（＝ガベージ）の量産を抑えるため、new は使わず、メンバに値を１つずつ直接上書きする。

                this.cvTransformedColoredVertexies[0].Position.X = x + f補正値X;
                this.cvTransformedColoredVertexies[0].Position.Y = y + f補正値Y;
                this.cvTransformedColoredVertexies[0].Position.Z = depth;
                this.cvTransformedColoredVertexies[0].Position.W = 1.0f;
                this.cvTransformedColoredVertexies[0].Color = color;
                this.cvTransformedColoredVertexies[0].TextureCoordinates.X = f左U値;
                this.cvTransformedColoredVertexies[0].TextureCoordinates.Y = f上V値;

                this.cvTransformedColoredVertexies[1].Position.X = (x + (w * this.Scaling.X)) + f補正値X;
                this.cvTransformedColoredVertexies[1].Position.Y = y + f補正値Y;
                this.cvTransformedColoredVertexies[1].Position.Z = depth;
                this.cvTransformedColoredVertexies[1].Position.W = 1.0f;
                this.cvTransformedColoredVertexies[1].Color = color;
                this.cvTransformedColoredVertexies[1].TextureCoordinates.X = f右U値;
                this.cvTransformedColoredVertexies[1].TextureCoordinates.Y = f上V値;

                this.cvTransformedColoredVertexies[2].Position.X = x + f補正値X;
                this.cvTransformedColoredVertexies[2].Position.Y = (y + (h * this.Scaling.Y)) + f補正値Y;
                this.cvTransformedColoredVertexies[2].Position.Z = depth;
                this.cvTransformedColoredVertexies[2].Position.W = 1.0f;
                this.cvTransformedColoredVertexies[2].Color = color;
                this.cvTransformedColoredVertexies[2].TextureCoordinates.X = f左U値;
                this.cvTransformedColoredVertexies[2].TextureCoordinates.Y = f下V値;

                this.cvTransformedColoredVertexies[3].Position.X = (x + (w * this.Scaling.X)) + f補正値X;
                this.cvTransformedColoredVertexies[3].Position.Y = (y + (h * this.Scaling.Y)) + f補正値Y;
                this.cvTransformedColoredVertexies[3].Position.Z = depth;
                this.cvTransformedColoredVertexies[3].Position.W = 1.0f;
                this.cvTransformedColoredVertexies[3].Color = color;
                this.cvTransformedColoredVertexies[3].TextureCoordinates.X = f右U値;
                this.cvTransformedColoredVertexies[3].TextureCoordinates.Y = f下V値;

                device.SetTexture(0, this.texture);
                device.VertexFormat = TransformedColoredTexturedVertex.Format;
                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 0, 2, in this.cvTransformedColoredVertexies);
                //-----------------
                #endregion
            }
            else
            {
                #region [ (B) 回転あり ]
                //-----------------
                float f補正値X = ((rectangle.Width % 2) == 0) ? -0.5f : 0f;   // -0.5 は座標とピクセルの誤差を吸収するための座標補正値。(MSDN参照)
                float f補正値Y = ((rectangle.Height % 2) == 0) ? -0.5f : 0f;  // 3D（回転する）なら補正はいらない。
                float f中央X = ((float)rectangle.Width) / 2f;
                float f中央Y = ((float)rectangle.Height) / 2f;
                float f左U値 = ((float)rectangle.Left) / ((float)this.TextureSize.Width);
                float f右U値 = ((float)rectangle.Right) / ((float)this.TextureSize.Width);
                float f上V値 = ((float)rectangle.Top) / ((float)this.TextureSize.Height);
                float f下V値 = ((float)rectangle.Bottom) / ((float)this.TextureSize.Height);
                this.color4.Alpha = ((float)this._opacity) / 255f;
                int color = this.color4.ToArgb();

                if (this.cvPositionColoredVertexies == null)
                    this.cvPositionColoredVertexies = new PositionColoredTexturedVertex[4];

                // #27122 2012.1.13 from: 以下、マネージドオブジェクト（＝ガベージ）の量産を抑えるため、new は使わず、メンバに値を１つずつ直接上書きする。

                this.cvPositionColoredVertexies[0].Position.X = -f中央X + f補正値X;
                this.cvPositionColoredVertexies[0].Position.Y = f中央Y + f補正値Y;
                this.cvPositionColoredVertexies[0].Position.Z = depth;
                this.cvPositionColoredVertexies[0].Color = color;
                this.cvPositionColoredVertexies[0].TextureCoordinates.X = f左U値;
                this.cvPositionColoredVertexies[0].TextureCoordinates.Y = f上V値;

                this.cvPositionColoredVertexies[1].Position.X = f中央X + f補正値X;
                this.cvPositionColoredVertexies[1].Position.Y = f中央Y + f補正値Y;
                this.cvPositionColoredVertexies[1].Position.Z = depth;
                this.cvPositionColoredVertexies[1].Color = color;
                this.cvPositionColoredVertexies[1].TextureCoordinates.X = f右U値;
                this.cvPositionColoredVertexies[1].TextureCoordinates.Y = f上V値;

                this.cvPositionColoredVertexies[2].Position.X = -f中央X + f補正値X;
                this.cvPositionColoredVertexies[2].Position.Y = -f中央Y + f補正値Y;
                this.cvPositionColoredVertexies[2].Position.Z = depth;
                this.cvPositionColoredVertexies[2].Color = color;
                this.cvPositionColoredVertexies[2].TextureCoordinates.X = f左U値;
                this.cvPositionColoredVertexies[2].TextureCoordinates.Y = f下V値;

                this.cvPositionColoredVertexies[3].Position.X = f中央X + f補正値X;
                this.cvPositionColoredVertexies[3].Position.Y = -f中央Y + f補正値Y;
                this.cvPositionColoredVertexies[3].Position.Z = depth;
                this.cvPositionColoredVertexies[3].Color = color;
                this.cvPositionColoredVertexies[3].TextureCoordinates.X = f右U値;
                this.cvPositionColoredVertexies[3].TextureCoordinates.Y = f下V値;

                float n描画領域内X = x + (rectangle.Width / 2.0f);
                float n描画領域内Y = y + (rectangle.Height / 2.0f);
                var vc3移動量 = new Vector3(n描画領域内X - (((float)device.Viewport.Width) / 2f), -(n描画領域内Y - (((float)device.Viewport.Height) / 2f)), 0f);

                var matrix = Matrix.Identity * Matrix.Scaling(this.Scaling);
                matrix *= Matrix.RotationZ(this.Rotation);
                matrix *= Matrix.Translation(vc3移動量);
                device.SetTransform(TransformState.World, matrix);

                device.SetTexture(0, this.texture);
                device.VertexFormat = PositionColoredTexturedVertex.Format;
                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, in this.cvPositionColoredVertexies);
                //-----------------
                #endregion
            }
        }
        public void Draw2D(Device device, int x, int y, float depth, Rectangle rectangle)
        {
            if (this.texture == null)
                return;

            this.SetRendererState(device);

            if (this.Rotation == 0f)
            {
                #region [ (A) 回転なし ]
                //-----------------
                float f補正値X = -0.5f;    // -0.5 は座標とピクセルの誤差を吸収するための座標補正値。(MSDN参照)
                float f補正値Y = -0.5f;    //
                float w = rectangle.Width;
                float h = rectangle.Height;
                float f左U値 = ((float)rectangle.Left) / ((float)this.TextureSize.Width);
                float f右U値 = ((float)rectangle.Right) / ((float)this.TextureSize.Width);
                float f上V値 = ((float)rectangle.Top) / ((float)this.TextureSize.Height);
                float f下V値 = ((float)rectangle.Bottom) / ((float)this.TextureSize.Height);
                this.color4.Alpha = ((float)this._opacity) / 255f;
                int color = this.color4.ToArgb();

                if (this.cvTransformedColoredVertexies == null)
                    this.cvTransformedColoredVertexies = new TransformedColoredTexturedVertex[4];

                // #27122 2012.1.13 from: 以下、マネージドオブジェクト（＝ガベージ）の量産を抑えるため、new は使わず、メンバに値を１つずつ直接上書きする。

                this.cvTransformedColoredVertexies[0].Position.X = x + f補正値X;
                this.cvTransformedColoredVertexies[0].Position.Y = y + f補正値Y;
                this.cvTransformedColoredVertexies[0].Position.Z = depth;
                this.cvTransformedColoredVertexies[0].Position.W = 1.0f;
                this.cvTransformedColoredVertexies[0].Color = color;
                this.cvTransformedColoredVertexies[0].TextureCoordinates.X = f左U値;
                this.cvTransformedColoredVertexies[0].TextureCoordinates.Y = f上V値;

                this.cvTransformedColoredVertexies[1].Position.X = (x + (w * this.Scaling.X)) + f補正値X;
                this.cvTransformedColoredVertexies[1].Position.Y = y + f補正値Y;
                this.cvTransformedColoredVertexies[1].Position.Z = depth;
                this.cvTransformedColoredVertexies[1].Position.W = 1.0f;
                this.cvTransformedColoredVertexies[1].Color = color;
                this.cvTransformedColoredVertexies[1].TextureCoordinates.X = f右U値;
                this.cvTransformedColoredVertexies[1].TextureCoordinates.Y = f上V値;

                this.cvTransformedColoredVertexies[2].Position.X = x + f補正値X;
                this.cvTransformedColoredVertexies[2].Position.Y = (y + (h * this.Scaling.Y)) + f補正値Y;
                this.cvTransformedColoredVertexies[2].Position.Z = depth;
                this.cvTransformedColoredVertexies[2].Position.W = 1.0f;
                this.cvTransformedColoredVertexies[2].Color = color;
                this.cvTransformedColoredVertexies[2].TextureCoordinates.X = f左U値;
                this.cvTransformedColoredVertexies[2].TextureCoordinates.Y = f下V値;

                this.cvTransformedColoredVertexies[3].Position.X = (x + (w * this.Scaling.X)) + f補正値X;
                this.cvTransformedColoredVertexies[3].Position.Y = (y + (h * this.Scaling.Y)) + f補正値Y;
                this.cvTransformedColoredVertexies[3].Position.Z = depth;
                this.cvTransformedColoredVertexies[3].Position.W = 1.0f;
                this.cvTransformedColoredVertexies[3].Color = color;
                this.cvTransformedColoredVertexies[3].TextureCoordinates.X = f右U値;
                this.cvTransformedColoredVertexies[3].TextureCoordinates.Y = f下V値;

                device.SetTexture(0, this.texture);
                device.VertexFormat = TransformedColoredTexturedVertex.Format;
                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 0, 2, in this.cvTransformedColoredVertexies);
                //-----------------
                #endregion
            }
            else
            {
                #region [ (B) 回転あり ]
                //-----------------
                float f補正値X = ((rectangle.Width % 2) == 0) ? -0.5f : 0f;   // -0.5 は座標とピクセルの誤差を吸収するための座標補正値。(MSDN参照)
                float f補正値Y = ((rectangle.Height % 2) == 0) ? -0.5f : 0f;  // 3D（回転する）なら補正はいらない。
                float f中央X = ((float)rectangle.Width) / 2f;
                float f中央Y = ((float)rectangle.Height) / 2f;
                float f左U値 = ((float)rectangle.Left) / ((float)this.TextureSize.Width);
                float f右U値 = ((float)rectangle.Right) / ((float)this.TextureSize.Width);
                float f上V値 = ((float)rectangle.Top) / ((float)this.TextureSize.Height);
                float f下V値 = ((float)rectangle.Bottom) / ((float)this.TextureSize.Height);
                this.color4.Alpha = ((float)this._opacity) / 255f;
                int color = this.color4.ToArgb();

                if (this.cvPositionColoredVertexies == null)
                    this.cvPositionColoredVertexies = new PositionColoredTexturedVertex[4];

                // #27122 2012.1.13 from: 以下、マネージドオブジェクト（＝ガベージ）の量産を抑えるため、new は使わず、メンバに値を１つずつ直接上書きする。

                this.cvPositionColoredVertexies[0].Position.X = -f中央X + f補正値X;
                this.cvPositionColoredVertexies[0].Position.Y = f中央Y + f補正値Y;
                this.cvPositionColoredVertexies[0].Position.Z = depth;
                this.cvPositionColoredVertexies[0].Color = color;
                this.cvPositionColoredVertexies[0].TextureCoordinates.X = f左U値;
                this.cvPositionColoredVertexies[0].TextureCoordinates.Y = f上V値;

                this.cvPositionColoredVertexies[1].Position.X = f中央X + f補正値X;
                this.cvPositionColoredVertexies[1].Position.Y = f中央Y + f補正値Y;
                this.cvPositionColoredVertexies[1].Position.Z = depth;
                this.cvPositionColoredVertexies[1].Color = color;
                this.cvPositionColoredVertexies[1].TextureCoordinates.X = f右U値;
                this.cvPositionColoredVertexies[1].TextureCoordinates.Y = f上V値;

                this.cvPositionColoredVertexies[2].Position.X = -f中央X + f補正値X;
                this.cvPositionColoredVertexies[2].Position.Y = -f中央Y + f補正値Y;
                this.cvPositionColoredVertexies[2].Position.Z = depth;
                this.cvPositionColoredVertexies[2].Color = color;
                this.cvPositionColoredVertexies[2].TextureCoordinates.X = f左U値;
                this.cvPositionColoredVertexies[2].TextureCoordinates.Y = f下V値;

                this.cvPositionColoredVertexies[3].Position.X = f中央X + f補正値X;
                this.cvPositionColoredVertexies[3].Position.Y = -f中央Y + f補正値Y;
                this.cvPositionColoredVertexies[3].Position.Z = depth;
                this.cvPositionColoredVertexies[3].Color = color;
                this.cvPositionColoredVertexies[3].TextureCoordinates.X = f右U値;
                this.cvPositionColoredVertexies[3].TextureCoordinates.Y = f下V値;

                int n描画領域内X = x + (rectangle.Width / 2);
                int n描画領域内Y = y + (rectangle.Height / 2);
                var vc3移動量 = new Vector3(n描画領域内X - (((float)device.Viewport.Width) / 2f), -(n描画領域内Y - (((float)device.Viewport.Height) / 2f)), 0f);

                var matrix = Matrix.Identity * Matrix.Scaling(this.Scaling);
                matrix *= Matrix.RotationZ(this.Rotation);
                matrix *= Matrix.Translation(vc3移動量);
                device.SetTransform(TransformState.World, matrix);

                device.SetTexture(0, this.texture);
                device.VertexFormat = PositionColoredTexturedVertex.Format;
                device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, in this.cvPositionColoredVertexies);
                //-----------------
                #endregion
            }
        }
        public void VerticalFlipDraw2D(Device device, int x, int y)
        {
            this.VerticalFlipDraw2D(device, x, y, 1f, this.FullImage);
        }
        public void VerticalFlipDraw2D(Device device, int x, int y, Rectangle rectangle)
        {
            this.VerticalFlipDraw2D(device, x, y, 1f, rectangle);
        }
        public void VerticalFlipDraw2D(Device device, int x, int y, float depth, Rectangle rectangle)
        {
            if (this.texture == null)
                throw new InvalidOperationException("テクスチャは生成されていません。");

            this.SetRendererState(device);

            float fx = x * FDKTexture.ScreenRatio + FDKTexture.rc物理画面描画領域.X - 0.5f;   // -0.5 は座標とピクセルの誤差を吸収するための座標補正値。(MSDN参照)
            float fy = y * FDKTexture.ScreenRatio + FDKTexture.rc物理画面描画領域.Y - 0.5f;   //
            float w = rectangle.Width * this.Scaling.X * FDKTexture.ScreenRatio;
            float h = rectangle.Height * this.Scaling.Y * FDKTexture.ScreenRatio;
            float f左U値 = ((float)rectangle.Left) / ((float)this.TextureSize.Width);
            float f右U値 = ((float)rectangle.Right) / ((float)this.TextureSize.Width);
            float f上V値 = ((float)rectangle.Top) / ((float)this.TextureSize.Height);
            float f下V値 = ((float)rectangle.Bottom) / ((float)this.TextureSize.Height);
            this.color4.Alpha = ((float)this._opacity) / 255f;
            int color = this.color4.ToArgb();

            if (this.cvTransformedColoredVertexies == null)
                this.cvTransformedColoredVertexies = new TransformedColoredTexturedVertex[4];

            // 以下、マネージドオブジェクトの量産を抑えるため new は使わない。

            this.cvTransformedColoredVertexies[0].TextureCoordinates.X = f左U値;  // 左上	→ 左下
            this.cvTransformedColoredVertexies[0].TextureCoordinates.Y = f下V値;
            this.cvTransformedColoredVertexies[0].Position.X = fx;
            this.cvTransformedColoredVertexies[0].Position.Y = fy;
            this.cvTransformedColoredVertexies[0].Position.Z = depth;
            this.cvTransformedColoredVertexies[0].Position.W = 1.0f;
            this.cvTransformedColoredVertexies[0].Color = color;

            this.cvTransformedColoredVertexies[1].TextureCoordinates.X = f右U値;  // 右上 → 右下
            this.cvTransformedColoredVertexies[1].TextureCoordinates.Y = f下V値;
            this.cvTransformedColoredVertexies[1].Position.X = fx + w;
            this.cvTransformedColoredVertexies[1].Position.Y = fy;
            this.cvTransformedColoredVertexies[1].Position.Z = depth;
            this.cvTransformedColoredVertexies[1].Position.W = 1.0f;
            this.cvTransformedColoredVertexies[1].Color = color;

            this.cvTransformedColoredVertexies[2].TextureCoordinates.X = f左U値;  // 左下 → 左上
            this.cvTransformedColoredVertexies[2].TextureCoordinates.Y = f上V値;
            this.cvTransformedColoredVertexies[2].Position.X = fx;
            this.cvTransformedColoredVertexies[2].Position.Y = fy + h;
            this.cvTransformedColoredVertexies[2].Position.Z = depth;
            this.cvTransformedColoredVertexies[2].Position.W = 1.0f;
            this.cvTransformedColoredVertexies[2].Color = color;

            this.cvTransformedColoredVertexies[3].TextureCoordinates.X = f右U値;  // 右下 → 右上
            this.cvTransformedColoredVertexies[3].TextureCoordinates.Y = f上V値;
            this.cvTransformedColoredVertexies[3].Position.X = fx + w;
            this.cvTransformedColoredVertexies[3].Position.Y = fy + h;
            this.cvTransformedColoredVertexies[3].Position.Z = depth;
            this.cvTransformedColoredVertexies[3].Position.W = 1.0f;
            this.cvTransformedColoredVertexies[3].Color = color;

            device.SetTexture(0, this.texture);
            device.VertexFormat = TransformedColoredTexturedVertex.Format;
            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, in this.cvTransformedColoredVertexies);
        }
        public void VerticalFlipDraw2D(Device device, Point pt)
        {
            this.VerticalFlipDraw2D(device, pt.X, pt.Y, 1f, this.FullImage);
        }
        public void VerticalFlipDraw2D(Device device, Point pt, Rectangle rc画像内の描画領域)
        {
            this.VerticalFlipDraw2D(device, pt.X, pt.Y, 1f, rc画像内の描画領域);
        }
        public void VerticalFlipDraw2D(Device device, Point pt, float depth, Rectangle rc画像内の描画領域)
        {
            this.VerticalFlipDraw2D(device, pt.X, pt.Y, depth, rc画像内の描画領域);
        }

        public static Vector3 t論理画面座標をワールド座標へ変換する(int x, int y)
        {
            return FDKTexture.t論理画面座標をワールド座標へ変換する(new Vector3((float)x, (float)y, 0f));
        }
        public static Vector3 t論理画面座標をワールド座標へ変換する(float x, float y)
        {
            return FDKTexture.t論理画面座標をワールド座標へ変換する(new Vector3(x, y, 0f));
        }
        public static Vector3 t論理画面座標をワールド座標へ変換する(Point pt論理画面座標)
        {
            return FDKTexture.t論理画面座標をワールド座標へ変換する(new Vector3(pt論理画面座標.X, pt論理画面座標.Y, 0.0f));
        }
        public static Vector3 t論理画面座標をワールド座標へ変換する(Vector2 v2論理画面座標)
        {
            return FDKTexture.t論理画面座標をワールド座標へ変換する(new Vector3(v2論理画面座標, 0f));
        }
        public static Vector3 t論理画面座標をワールド座標へ変換する(Vector3 v3論理画面座標)
        {
            return new Vector3(
                (v3論理画面座標.X - (FDKTexture.sz論理画面.Width / 2.0f)) * FDKTexture.ScreenRatio,
                (-(v3論理画面座標.Y - (FDKTexture.sz論理画面.Height / 2.0f)) * FDKTexture.ScreenRatio),
                v3論理画面座標.Z);
        }

        /// <summary>
        /// テクスチャを 3D 画像と見なして描画する。
        /// </summary>
        public void Draw3D(Device device, Matrix mat)
        {
            this.Draw3D(device, mat, this.FullImage);
        }
        public void Draw3D(Device device, Matrix mat, Rectangle rectangle)
        {
            if (this.texture == null)
                return;

            float x = ((float)rectangle.Width) / 2f;
            float y = ((float)rectangle.Height) / 2f;
            float z = 0.0f;
            float f左U値 = ((float)rectangle.Left) / ((float)this.TextureSize.Width);
            float f右U値 = ((float)rectangle.Right) / ((float)this.TextureSize.Width);
            float f上V値 = ((float)rectangle.Top) / ((float)this.TextureSize.Height);
            float f下V値 = ((float)rectangle.Bottom) / ((float)this.TextureSize.Height);
            this.color4.Alpha = ((float)this._opacity) / 255f;
            int color = this.color4.ToArgb();

            if (this.cvPositionColoredVertexies == null)
                this.cvPositionColoredVertexies = new PositionColoredTexturedVertex[4];

            // #27122 2012.1.13 from: 以下、マネージドオブジェクト（＝ガベージ）の量産を抑えるため、new は使わず、メンバに値を１つずつ直接上書きする。

            this.cvPositionColoredVertexies[0].Position.X = -x;
            this.cvPositionColoredVertexies[0].Position.Y = y;
            this.cvPositionColoredVertexies[0].Position.Z = z;
            this.cvPositionColoredVertexies[0].Color = color;
            this.cvPositionColoredVertexies[0].TextureCoordinates.X = f左U値;
            this.cvPositionColoredVertexies[0].TextureCoordinates.Y = f上V値;

            this.cvPositionColoredVertexies[1].Position.X = x;
            this.cvPositionColoredVertexies[1].Position.Y = y;
            this.cvPositionColoredVertexies[1].Position.Z = z;
            this.cvPositionColoredVertexies[1].Color = color;
            this.cvPositionColoredVertexies[1].TextureCoordinates.X = f右U値;
            this.cvPositionColoredVertexies[1].TextureCoordinates.Y = f上V値;

            this.cvPositionColoredVertexies[2].Position.X = -x;
            this.cvPositionColoredVertexies[2].Position.Y = -y;
            this.cvPositionColoredVertexies[2].Position.Z = z;
            this.cvPositionColoredVertexies[2].Color = color;
            this.cvPositionColoredVertexies[2].TextureCoordinates.X = f左U値;
            this.cvPositionColoredVertexies[2].TextureCoordinates.Y = f下V値;

            this.cvPositionColoredVertexies[3].Position.X = x;
            this.cvPositionColoredVertexies[3].Position.Y = -y;
            this.cvPositionColoredVertexies[3].Position.Z = z;
            this.cvPositionColoredVertexies[3].Color = color;
            this.cvPositionColoredVertexies[3].TextureCoordinates.X = f右U値;
            this.cvPositionColoredVertexies[3].TextureCoordinates.Y = f下V値;

            this.SetRendererState(device);

            device.SetTransform(TransformState.World, mat);
            device.SetTexture(0, this.texture);
            device.VertexFormat = PositionColoredTexturedVertex.Format;
            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, in this.cvPositionColoredVertexies);
        }

        public void t3D左上基準描画(Device device, Matrix mat)
        {
            this.t3D左上基準描画(device, mat, this.FullImage);
        }
        /// <summary>
        /// ○覚書
        ///   SlimDX.Matrix mat = SlimDX.Matrix.Identity;
        ///   mat *= SlimDX.Matrix.Translation( x, y, z );
        /// 「mat =」ではなく「mat *=」であることを忘れないこと。
        /// </summary>
        public void t3D左上基準描画(Device device, Matrix mat, Rectangle rectangle)
        {
            //とりあえず補正値などは無し。にしても使う機会少なさそうだなー
            if (this.texture == null)
                return;

            float x = 0.0f;
            float y = 0.0f;
            float z = 0.0f;
            float f左U値 = ((float)rectangle.Left) / ((float)this.TextureSize.Width);
            float f右U値 = ((float)rectangle.Right) / ((float)this.TextureSize.Width);
            float f上V値 = ((float)rectangle.Top) / ((float)this.TextureSize.Height);
            float f下V値 = ((float)rectangle.Bottom) / ((float)this.TextureSize.Height);
            this.color4.Alpha = ((float)this._opacity) / 255f;
            int color = this.color4.ToArgb();

            if (this.cvPositionColoredVertexies == null)
                this.cvPositionColoredVertexies = new PositionColoredTexturedVertex[4];

            // #27122 2012.1.13 from: 以下、マネージドオブジェクト（＝ガベージ）の量産を抑えるため、new は使わず、メンバに値を１つずつ直接上書きする。

            this.cvPositionColoredVertexies[0].Position.X = -x;
            this.cvPositionColoredVertexies[0].Position.Y = y;
            this.cvPositionColoredVertexies[0].Position.Z = z;
            this.cvPositionColoredVertexies[0].Color = color;
            this.cvPositionColoredVertexies[0].TextureCoordinates.X = f左U値;
            this.cvPositionColoredVertexies[0].TextureCoordinates.Y = f上V値;

            this.cvPositionColoredVertexies[1].Position.X = x;
            this.cvPositionColoredVertexies[1].Position.Y = y;
            this.cvPositionColoredVertexies[1].Position.Z = z;
            this.cvPositionColoredVertexies[1].Color = color;
            this.cvPositionColoredVertexies[1].TextureCoordinates.X = f右U値;
            this.cvPositionColoredVertexies[1].TextureCoordinates.Y = f上V値;

            this.cvPositionColoredVertexies[2].Position.X = -x;
            this.cvPositionColoredVertexies[2].Position.Y = -y;
            this.cvPositionColoredVertexies[2].Position.Z = z;
            this.cvPositionColoredVertexies[2].Color = color;
            this.cvPositionColoredVertexies[2].TextureCoordinates.X = f左U値;
            this.cvPositionColoredVertexies[2].TextureCoordinates.Y = f下V値;

            this.cvPositionColoredVertexies[3].Position.X = x;
            this.cvPositionColoredVertexies[3].Position.Y = -y;
            this.cvPositionColoredVertexies[3].Position.Z = z;
            this.cvPositionColoredVertexies[3].Color = color;
            this.cvPositionColoredVertexies[3].TextureCoordinates.X = f右U値;
            this.cvPositionColoredVertexies[3].TextureCoordinates.Y = f下V値;

            this.SetRendererState(device);

            device.SetTransform(TransformState.World, mat);
            device.SetTexture(0, this.texture);
            device.VertexFormat = PositionColoredTexturedVertex.Format;
            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, 2, in this.cvPositionColoredVertexies);
        }

        #region [ IDisposable 実装 ]
        //-----------------
        public void Dispose()
        {
            if (!this.IsDisposed)
            {
                // テクスチャの破棄
                if (this.texture != null)
                {
                    this.texture.Dispose();
                    this.texture = null;
                }

                this.IsDisposed = true;
            }
        }
        //-----------------
        #endregion


        // その他

        #region [ private ]
        //-----------------
        private int _opacity;
        private bool IsDisposed;
        private PositionColoredTexturedVertex[] cvPositionColoredVertexies;
        protected TransformedColoredTexturedVertex[] cvTransformedColoredVertexies = new TransformedColoredTexturedVertex[]
        {
            new TransformedColoredTexturedVertex(),
            new TransformedColoredTexturedVertex(),
            new TransformedColoredTexturedVertex(),
            new TransformedColoredTexturedVertex(),
        };
        private const Pool poolvar =                                                // 2011.4.25 yyagi
#if TEST_Direct3D9Ex
			Pool.Default;
#else
            Pool.Managed;
#endif
        //		byte[] _txData;
        static object lockobj = new object();

        /// <summary>
        /// どれか一つが有効になります。
        /// </summary>
        /// <param name="device">Direct3Dのデバイス</param>
		private void SetRendererState(Device device)
        {
            if (this.IsAddBlend)
            {
                device.SetRenderState(RenderState.SourceBlend, SlimDX.Direct3D9.Blend.SourceAlpha);             // 5
                device.SetRenderState(RenderState.DestinationBlend, SlimDX.Direct3D9.Blend.One);                    // 2
            }
            else if (this.IsMultiplyBlend)
            {
                //参考:http://sylphylunar.seesaa.net/article/390331341.html
                //C++から引っ張ってきたのでちょっと不安。
                device.SetRenderState(RenderState.SourceBlend, SlimDX.Direct3D9.Blend.DestinationColor);
                device.SetRenderState(RenderState.DestinationBlend, SlimDX.Direct3D9.Blend.Zero);
            }
            else if (this.IsSubBlend)
            {
                //参考:http://www3.pf-x.net/~chopper/home2/DirectX/MD20.html
                device.SetRenderState(RenderState.BlendOperation, SlimDX.Direct3D9.BlendOperation.Subtract);
                device.SetRenderState(RenderState.SourceBlend, SlimDX.Direct3D9.Blend.One);
                device.SetRenderState(RenderState.DestinationBlend, SlimDX.Direct3D9.Blend.One);
            }
            else if (this.IsScreenBlend)
            {
                //参考:http://sylphylunar.seesaa.net/article/390331341.html
                //C++から引っ張ってきたのでちょっと不安。
                device.SetRenderState(RenderState.SourceBlend, SlimDX.Direct3D9.Blend.InverseDestinationColor);
                device.SetRenderState(RenderState.DestinationBlend, SlimDX.Direct3D9.Blend.One);
            }
            else
            {
                device.SetRenderState(RenderState.SourceBlend, SlimDX.Direct3D9.Blend.SourceAlpha);             // 5
                device.SetRenderState(RenderState.DestinationBlend, SlimDX.Direct3D9.Blend.InverseSourceAlpha); // 6
            }
        }
        private Size GetOptimalTextureSizeNotExceedingSpecifiedSize(Device device, Size size)
        {
            var deviceCapabilities = device.Capabilities;
            var deviceCapabilitiesTextureCaps = deviceCapabilities.TextureCaps;

            bool b条件付きでサイズは２の累乗でなくてもOK = (deviceCapabilitiesTextureCaps & TextureCaps.NonPow2Conditional) != 0;
            bool bサイズは２の累乗でなければならない = (deviceCapabilitiesTextureCaps & TextureCaps.Pow2) != 0;
            bool b正方形でなければならない = (deviceCapabilitiesTextureCaps & TextureCaps.SquareOnly) != 0;
            int n最大幅 = deviceCapabilities.MaxTextureWidth;
            int n最大高 = deviceCapabilities.MaxTextureHeight;
            var result = new Size(size.Width, size.Height);

            if (bサイズは２の累乗でなければならない && !b条件付きでサイズは２の累乗でなくてもOK)
            {
                // 幅を２の累乗にする
                int n = 1;
                do
                {
                    n *= 2;
                }
                while (n <= size.Width);
                size.Width = n;

                // 高さを２の累乗にする
                n = 1;
                do
                {
                    n *= 2;
                }
                while (n <= size.Height);
                size.Height = n;
            }

            if (size.Width > n最大幅)
                size.Width = n最大幅;

            if (size.Height > n最大高)
                size.Height = n最大高;

            if (b正方形でなければならない)
            {
                if (result.Width > result.Height)
                {
                    result.Height = result.Width;
                }
                else if (result.Width < result.Height)
                {
                    result.Width = result.Height;
                }
            }

            return result;
        }


        // 2012.3.21 さらなる new の省略作戦

        protected Rectangle FullImage;                              // テクスチャ作ったらあとは不変
        public Color4 color4 = new Color4(1f, 1f, 1f, 1f);  // アルファ以外は不変
                                                            //-----------------
        #endregion
    }
}
