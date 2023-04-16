using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using EmbedIO;
using EmbedIO.WebSockets;
using Swan.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;
using PhotoSauce.MagicScaler;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using Vortice.Direct3D11;

namespace WebScreen
{
		class WebSocketBitmap : WebSocketModule
		{
				//Dictionary<string, bool> finishSending = new Dictionary<string, bool>();
				bool finishSending = true;

				public WebSocketBitmap(string urlPath) : base(urlPath, true)
				{
				}

				protected override Task OnMessageReceivedAsync(IWebSocketContext context, byte[] buffer, IWebSocketReceiveResult result)
				{
						return Task.CompletedTask;
				}
				protected override Task OnClientConnectedAsync(IWebSocketContext context)
				{
						//finishSending[context.Id] = true;
						//"Connected".Info();
						return Task.WhenAll(SendAsync(context, "Hello"));
				}

				protected override Task OnClientDisconnectedAsync(IWebSocketContext context)
				{
						//"Disconnected".Info();
						//finishSending.Remove(context.Id);
						return Task.CompletedTask;
				}

				public async Task SendBitmap(byte[] data)
				{
						/*foreach (IWebSocketContext context in ActiveContexts)
						{
								var contextFinishSending = finishSending[context.Id];
								if (contextFinishSending)
								{
										if (context.WebSocket.State == WebSocketState.Open)
										{
												contextFinishSending = false;
												//SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.LoadPixelData<Bgra32>(data, 1366, 768);

												MemoryStream memoryStream = new MemoryStream();

												MagicImageProcessor.ProcessImage(new RawBitmap(data, 1366, 768), memoryStream, new ProcessImageSettings { Width = 1366 / 2, Sharpen = false, Interpolation = InterpolationSettings.Average });


												//image.Mutate(c => c.Resize(image.Width / 2, image.Height / 2));
												//image.SaveAsPng(@"C:\Users\Fadhil\Documents\testimagesharp.png", new PngEncoder { IgnoreMetadata = true, CompressionLevel = PngCompressionLevel.Level9, ColorType = PngColorType.Rgb });
												//image.SaveAsPng(memoryStream, new PngEncoder { IgnoreMetadata = true, CompressionLevel = PngCompressionLevel.BestSpeed, ColorType = PngColorType.Rgb});
												//image.SaveAsJpeg(memoryStream, new JpegEncoder { Quality = 50, ColorType = JpegColorType.Rgb });
												await SendAsync(context, memoryStream.GetBuffer());
												contextFinishSending = true;
										}
								}
						}*/
						if (finishSending && ActiveContexts[0].WebSocket.State == WebSocketState.Open)
						{
								finishSending = false;
								MemoryStream memoryStream = new MemoryStream();
								SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.LoadPixelData<Bgra32>(data, 1366, 768);
								//MagicImageProcessor.ProcessImage(new RawBitmap(data, 1366, 768), memoryStream, new ProcessImageSettings { /*Width = 1366 / 2, Sharpen = false, Interpolation = InterpolationSettings.Average,*/ OrientationMode = OrientationMode.Preserve });


								image.Mutate(c => c.Resize(image.Width / 2, image.Height / 2));
								//image.SaveAsPng(@"C:\Users\Fadhil\Documents\testimagesharp.png", new PngEncoder { IgnoreMetadata = true, CompressionLevel = PngCompressionLevel.Level8, ColorType = PngColorType.Rgb });
								image.SaveAsPng(memoryStream, new PngEncoder { IgnoreMetadata = true, CompressionLevel = PngCompressionLevel.Level8, ColorType = PngColorType.Rgb});
								//image.SaveAsJpeg(memoryStream, new JpegEncoder { Quality = 10, ColorType = JpegColorType.Rgb });
								await SendAsync(ActiveContexts[0], memoryStream.GetBuffer());
								finishSending = true;
						}
						
				}
		}

		/*class RawImageContainer : IImageContainer
		{
				private byte[] data;

				private int bitmapWidth;
				private int bitmapHeight;

				public RawImageContainer(byte[] data, int width, int height)
				{
						this.data = data;
						bitmapWidth = width;
						bitmapHeight = height;
				}
				public string? MimeType => "image/jpeg";

				public int FrameCount => 1;

				public void Dispose()
				{
						
				}

				public IImageFrame GetFrame(int index)
				{
						return new ImageFrame(data, bitmapWidth, bitmapHeight);
				}

				class ImageFrame : IImageFrame
				{
						private byte[] data;

						private int bitmapWidth;
						private int bitmapHeight;

						public ImageFrame(byte[] data, int width, int height)
						{
								this.data = data;
								bitmapWidth = width;
								bitmapHeight = height;
						}

						public IPixelSource PixelSource => new RawBitmap(data, bitmapWidth, bitmapHeight) ;

						public void Dispose()
						{
								
						}
				}
		}*/

		class RawBitmap : IPixelSource
		{
				private byte[] data;

				private int bitmapWidth;
				private int bitmapHeight;

				private int stride;

				private int readOffset = 0;

				public int Width => bitmapWidth;

				public int Height => bitmapHeight;

				public Guid Format => PixelFormats.Bgra32bpp;

				public RawBitmap(byte[] data, int width, int height)
				{
						this.data = data;
						bitmapWidth = width;
						bitmapHeight = height;
						stride = width * 4;
				}

				public void CopyPixels(System.Drawing.Rectangle sourceArea, int cbStride, Span<byte> buffer)
				{
						//MessageBox.Show($"{sourceArea.X} {sourceArea.Y} {sourceArea.Width} {sourceArea.Height} {cbStride} {buffer.Length}");
						Span<byte> bytes = data;
						//MessageBox.Show($"{bytes.Length} {slicedBytes.Length} {buffer.Length} {readOffset}");
						bytes.Slice(readOffset, stride).CopyTo(buffer);
						readOffset += stride;
						//buffer.Length;
				}
		}
}
