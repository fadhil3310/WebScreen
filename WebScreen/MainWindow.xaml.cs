using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using ScreenCapture.NET;
using System.Drawing;
using GenHTTP.Engine;
using GenHTTP.Modules.IO;
using GenHTTP.Modules.Practices;
using GenHTTP.Modules.StaticWebsites;
using EmbedIO;
using System.Windows.Markup;
using SixLabors.ImageSharp;
using System.Numerics;
using System.Runtime.InteropServices;
using Windows.UI.Input.Preview.Injection;
using EmbedIO.WebSockets;
using System.Reflection;
using Windows.Storage.Streams;

namespace WebScreen
{
		/// <summary>
		/// Interaction logic for MainWindow.xaml
		/// </summary>
		public partial class MainWindow : Window
		{
				WebSocketBitmap webSocketBitmap;
				WebSocketInput webSocketInput;

				bool stopCapturing = false;

				Thread screenCaptureThread;

				InputInjector inputInjector;

				WebServer server;

				/*[DllImport("User32.dll")]
				static extern bool InitializeTouchInjection(uint maxCount, int mode);

				[DllImport("User32.dll")]
				static extern bool InjectTouch(uint count, int mode);

				struct POINTER_TOUCH_INFO
				{
						POINTER_INFO pointerInfo;
						int touchFlags;
						int touchMask;

				}

				struct POINTER_INFO
				{
						POINTER_INPUT_TYPE pointerType;
						uint pointerId;
						uint frameId;
						int pointerFlags;

				}

				enum POINTER_INPUT_TYPE
				{
						PT_POINTER = 1,
						PT_TOUCH = 2,
						PT_PEN = 3,
						PT_MOUSE = 4,
						PT_TOUCHPAD = 5
				}*/

				public MainWindow()
				{
						InitializeComponent();

						RunServer();
						StartCaptureScreen();

						inputInjector = InputInjector.TryCreate();
						inputInjector.InitializeTouchInjection(InjectedInputVisualizationMode.Default);
				}

				private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
				{
						stopCapturing = true;
						server.Dispose();
				}

				public void RunServer()
				{
						webSocketBitmap = new WebSocketBitmap("/bitmap");
						webSocketInput = new WebSocketInput("/input");
						webSocketInput.OnInput += ProcessInput;

						WebSocketLog webSocketLog = new WebSocketLog("/log", logList);

						server = new WebServer(8080)
								.WithLocalSessionManager()
								.WithModule(webSocketBitmap)
								.WithModule(webSocketInput)
								.WithModule(webSocketLog)
								.WithStaticFolder("/", "./Website", false);
								//.WithEmbeddedResources("/", typeof(MainWindow).Assembly, "WebScreen.Website");

						server.RunAsync();
				}

				public void StartCaptureScreen()
				{
						// Set up screen capture
						IScreenCaptureService screenCaptureService = new DX11ScreenCaptureService();
						IEnumerable<GraphicsCard> graphicsCards = screenCaptureService.GetGraphicsCards();
						IEnumerable<Display> displays = screenCaptureService.GetDisplays(graphicsCards.First());

						// Capture only the first displays from the first graphics cards
						IScreenCapture screenCapture = screenCaptureService.GetScreenCapture(displays.First());

						// Capture the whole screen
						CaptureZone captureZone = screenCapture.RegisterCaptureZone(0, 0, screenCapture.Display.Width, screenCapture.Display.Height);

						// Capture the screen on a seperate thread
						screenCaptureThread = new Thread(new ThreadStart(() =>
						{
								while (!stopCapturing)
								{
										screenCapture.CaptureScreen();

										webSocketBitmap.SendBitmap(captureZone.Buffer);

										//Thread.Sleep(50);
								}
						}));
						screenCaptureThread.Start();
				}

				//List<InjectedInputTouchInfo> ongoingTouches = new List<uint>();

				void ProcessInput (object sender, List<InputMessage> e)
				{
						List<InjectedInputTouchInfo> touchInfo = new List<InjectedInputTouchInfo>();
						
						foreach (InputMessage input in e)
						{
								logList.Dispatcher.Invoke(() => {
										logList.Items.Add(new Label { Content = input.ID + " " + input.Type });
										logList.ScrollIntoView(logList.Items[logList.Items.Count - 1]);
								});
								switch (input.Type)
								{
										case 1:
												{
														touchInfo.Add(
																new InjectedInputTouchInfo
																{
																		Contact = new InjectedInputRectangle
																		{
																				Left = input.X,
																				Right = input.X,
																				Top = input.Y,
																				Bottom = input.Y
																		},
																		PointerInfo = new InjectedInputPointerInfo
																		{
																				PointerId = input.ID,
																				PointerOptions =
																				InjectedInputPointerOptions.InRange |
																				InjectedInputPointerOptions.InContact |
																				InjectedInputPointerOptions.PointerDown,
																				TimeOffsetInMilliseconds = 0,
																				PixelLocation = new InjectedInputPoint
																				{
																						PositionX = input.X,
																						PositionY = input.Y
																				}
																				
																		},
																		Pressure = 1,
																		TouchParameters =
																		InjectedInputTouchParameters.Pressure |
																		InjectedInputTouchParameters.Contact
																});
														break;
												}
										case 2:
												{
														touchInfo.Add(
																new InjectedInputTouchInfo
																{
																		Contact = new InjectedInputRectangle
																		{
																				Left = input.X,
																				Right = input.X,
																				Top = input.Y,
																				Bottom = input.Y
																		},
																		PointerInfo = new InjectedInputPointerInfo
																		{
																				PointerId = input.ID,
																				PointerOptions =
																				InjectedInputPointerOptions.InRange |
																				InjectedInputPointerOptions.InContact |
																				InjectedInputPointerOptions.Update,
																				TimeOffsetInMilliseconds = 0,
																				PixelLocation = new InjectedInputPoint
																				{
																						PositionX = input.X,
																						PositionY = input.Y
																				}
																		},
																		Pressure = 1,
																		TouchParameters =
																		InjectedInputTouchParameters.Pressure |
																		InjectedInputTouchParameters.Contact
																});
														break;
												}
										case 3:
												{
														touchInfo.Add(
																new InjectedInputTouchInfo
																{
																		PointerInfo = new InjectedInputPointerInfo
																		{
																				PointerId = input.ID,
																				PointerOptions = InjectedInputPointerOptions.PointerUp
																		}
																});
														break;
												}
								}
						}
						
						

						inputInjector.InjectTouchInput(touchInfo);
				}

				public class WebSocketLog : WebSocketModule
				{
						private ListBox logList;

						public WebSocketLog(string urlPath, ListBox logList) : base(urlPath, true)
						{
								this.logList = logList;
						}

						protected override Task OnMessageReceivedAsync(IWebSocketContext context, byte[] buffer, IWebSocketReceiveResult result)
						{
								logList.Dispatcher.Invoke(() => {
										logList.Items.Add(new Label { Content = Encoding.Default.GetString(buffer) });
										logList.ScrollIntoView(logList.Items[logList.Items.Count - 1]);
								});
								return Task.CompletedTask;
						}
				}
		}
}
