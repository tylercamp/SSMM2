using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SSMM2.Debug
{
	/// <summary>
	/// Dispatches commands and displays the console when Enabled, and interprets input.
	/// TODO: Fully document, organize.
	/// </summary>
	public class Commandline : Core.Widget
	{
		String m_ConsoleText = "";

		int m_BlinkRate = 500; // ms

		int m_CurrentHistoryPosition = -1;
		//	The first command in the list is the most recent
		List<String> m_PreviousCommands = new List<string>();
		//	Current command being entered by the user; if the user is currently modifying the current command, then
		//		this Value is null
		String m_CurrentCommand = null;

		SpriteFont m_DebugFont;

		Core.PrimitiveBatch m_PrimitiveBatch;
		SpriteBatch m_SpriteBatch; // Maintain our own SpriteBatch so that text always appears above other sprites

		Core.TextInputRecord m_Record = null;

		public static Commandline Context;

		bool m_IsActive = false;

		/// <summary>
		/// Color of the command prompt overlay.
		/// </summary>
		public Color OverlayColor = Color.Black * 0.3F;

		/// <summary>
		/// Text color used within the command prompt.
		/// </summary>
		public Color TextColor = Color.LightGreen;

		/// <summary>
		/// Maximum height of the output buffer that will be displayed. Text is automatically shifted up to make
		/// sure that this maximum is met.
		/// </summary>
		public int MaxHeight = 300;

		/// <summary>
		/// Clears the output buffer.
		/// </summary>
		public void Clear()
		{
			m_ConsoleText = "";
		}

		/// <summary>
		/// Directly writes the text to whatever's currently in the commandline's output buffer.
		/// </summary>
		/// <param name="text">Text to be written.</param>
		public void Write(String text)
		{
			m_ConsoleText += text;
		}

		/// <summary>
		/// Writes the given text, followed by a newline.
		/// </summary>
		/// <param name="text">Text to be written. Optional.</param>
		public void WriteLine(String text = "")
		{
			Write(text + "\n");
		}

		/// <summary>
		/// Processes the given command and splits parameters then fires the given signal with the split parameters
		/// as arguments to the signal.
		/// </summary>
		/// <param name="command">Command to be executed.</param>
		/// <param name="allowExceptions">Whether or not to run the signal within a try/catch block. If set to false
		/// (the default), a summary of the exception will be automatically written to the commandline.</param>
		public void ExecuteCommand(String command, bool allowExceptions = false)
		{
			m_PreviousCommands.Insert(0, command);

			Core.SignalDispatcher dispatcher = Core.SignalDispatcher.Instance;

			command = command.Trim();
			command = command.Replace(" ", "");

			if (command[0] == '!')
			{
				allowExceptions = true;
				command = command.Substring(1);
			}

			String finalSignal = "";
			object finalData = null;

			//	No parameters, just a signal
			if (command.IndexOf(":") == -1)
			{
				finalSignal = command;
				finalData = null;
			}
			else
			{
				//	Otherwise there are parameters to be parsed
				finalSignal = command.Substring(0, command.IndexOf(":"));
				command = command.Substring(command.IndexOf(":") + 1);

				String[] delimiters = {
									  ","
								   };

				String[] parameters = command.Split(delimiters, StringSplitOptions.None);
				if (parameters.Length == 0)
				{
					finalData = null;
				}

				if (parameters.Length == 1)
				{
					if (parameters[0] == "")
						finalData = null;
					else
						finalData = command;
				}

				if (parameters.Length > 1)
				{
					object[] data = new object[parameters.Length];
					for (int i = 0; i < parameters.Length; i++)
					{
						if (parameters[i] != "")
							data[i] = parameters[i];
						else
							data[i] = null;
					}

					finalData = data;
				}
			}

			object[] results = null;

			if (allowExceptions)
			{
				results = dispatcher.FireSignal(finalSignal, finalData);
			}
			else
			{
				try
				{
					results = dispatcher.FireSignal(finalSignal, finalData);
				}
				catch (Exception e)
				{
					WriteLine("Attempt to fire signal " + finalSignal + " failed with exception.");
					WriteLine("  Message: " + e.Message);
					WriteLine("  SourceMethod: " + e.TargetSite);

					if (e.Data.Count == 0)
					{
						WriteLine("No exception cache data found.");
					}
					else
					{
						WriteLine("  Exception Cache:");

						foreach (KeyValuePair<String, String> data in e.Data)
						{
							WriteLine("    " + data.Key.ToString() + ": " + data.Value.ToString());
						}
					}
					WriteLine("Run command again with '!' prefixed to view the exception in VS debugger (if in debug mode).");
				}
			}

			if (results != null)
			{
				bool isMultiOutput = false;
				String output = "";
				//	See if all results are null; if they are, don't print anything.
				foreach (object o in results)
				{
					if (o != null)
					{
						if (output != "")
						{
							output += "\n";
							isMultiOutput = true;
						}
						output += "-- " + o.ToString();
					}
				}

				if (output != "")
				{
					if (isMultiOutput)
					{
						WriteLine("- Non-null Results:");
					}

					WriteLine(output);
				}
			}
		}

		public override void LoadContent()
		{
			Core.ResourceManager resource = Core.ResourceManager.Instance;
			m_DebugFont = resource.GetResource<SpriteFont>("debug assets/debugfont");

			m_PrimitiveBatch = new Core.PrimitiveBatch(SquareManMan.Instance.GraphicsDevice);
			m_SpriteBatch = new SpriteBatch(SquareManMan.Instance.GraphicsDevice);

			Core.InputManager input = Core.InputManager.Instance;
			input.Bind("console", Keys.OemTilde);
			input.Bind("confirm", Keys.Enter);
			input.Bind("cancel", Keys.Escape);
			input.Bind("rewind", Keys.Up);
			input.Bind("fastforward", Keys.Down);
		}

		public override void PreUpdate()
		{
			Core.InputManager input = Core.InputManager.Instance;

			if (input.CheckPressed("console", Core.InputManager.MasterAuthKey))
			{
				m_IsActive = !m_IsActive;
				if (m_IsActive)
				{
					input.Lock(Core.InputManager.MasterAuthKey);

					m_Record = input.StartTextRecording(Core.InputManager.MasterAuthKey, m_Record);
				}
				else
				{
					input.Unlock(Core.InputManager.MasterAuthKey);

					input.StopTextRecording(m_Record);
				}
			}

			if (!m_IsActive)
				return;

			if (m_CurrentHistoryPosition != -1)
			{
				if (m_Record.EnteredText != m_PreviousCommands[m_CurrentHistoryPosition])
				{
					//	In this case the user has modified the command and should be considered
					//		the new command being entered by the user
					m_CurrentHistoryPosition = -1;
					m_CurrentCommand = null;
				}
			}

			if (input.CheckPressed("rewind", Core.InputManager.MasterAuthKey))
			{
				if (m_CurrentHistoryPosition + 1 < m_PreviousCommands.Count)
				{
					if (m_CurrentCommand == null)
						m_CurrentCommand = m_Record.EnteredText;

					m_Record.EnteredText = m_PreviousCommands[++m_CurrentHistoryPosition];
					m_Record.CaretLocation = Core.TextInputRecord.End;
				}
			}

			if (input.CheckPressed("fastforward", Core.InputManager.MasterAuthKey))
			{
				if (m_CurrentHistoryPosition == 0 && m_CurrentCommand != null)
				{
					m_Record.EnteredText = m_CurrentCommand;
					m_Record.CaretLocation = Core.TextInputRecord.End;
					m_CurrentCommand = null;
					m_CurrentHistoryPosition = -1;
				}

				if (m_CurrentHistoryPosition > 0)
				{
					m_Record.EnteredText = m_PreviousCommands[--m_CurrentHistoryPosition];
				}
			}

			if (input.CheckPressed("confirm", Core.InputManager.MasterAuthKey))
			{
				if (m_Record.EnteredText.Length != 0)
				{
					WriteLine(m_Record.EnteredText);
					ExecuteCommand(m_Record.EnteredText);
					m_Record.EnteredText = "";

					//	Reset the current command, reset the history Position
					m_CurrentCommand = null;
					m_CurrentHistoryPosition = -1;
				}
			}

			if (input.CheckPressed("cancel", Core.InputManager.MasterAuthKey))
			{
				m_Record.EnteredText = "";
			}
		}

		public override void PostDraw()
		{
			base.PostDraw();

			if (!m_IsActive)
				return;

			m_PrimitiveBatch.ActiveColor = OverlayColor;
			m_PrimitiveBatch.AddRectangle(SquareManMan.Instance.GraphicsDevice.Viewport.Bounds);
			m_PrimitiveBatch.DrawPolygons();

			String enteredTextPrefix = "> ";

			String drawText = m_ConsoleText;
			if (drawText.Length == 0)
				drawText += enteredTextPrefix + m_Record.EnteredText;
			else
			{
				if (drawText.Last() != '\n')
					drawText += '\n';
				drawText += enteredTextPrefix + m_Record.EnteredText;
			}

			Vector2 textSize = m_DebugFont.MeasureString(drawText);

			Vector2 textPosition = new Vector2();
			textPosition.X = 20.0F;
			textPosition.Y = Math.Min(5.0f, MaxHeight - textSize.Y);

			m_SpriteBatch.Begin();
			m_SpriteBatch.DrawString(m_DebugFont, drawText, textPosition, TextColor);
			m_SpriteBatch.End();

			//	Draw caret at intervals
			if ((DateTime.Now.Ticks / 10000 / m_BlinkRate) % 2 == 0)
			{
				m_PrimitiveBatch.ActiveColor = TextColor;
				Vector2 activeTextSize = m_DebugFont.MeasureString(enteredTextPrefix + m_Record.EnteredText.Substring(0, m_Record.CaretLocation));
				m_PrimitiveBatch.AddLine(
					new Vector2(textPosition.X + activeTextSize.X, textPosition.Y + textSize.Y - activeTextSize.Y + 2),
					new Vector2(textPosition.X + activeTextSize.X, textPosition.Y + textSize.Y - 2)
					);
				m_PrimitiveBatch.DrawPolygons();
			}
		}
	}
}
