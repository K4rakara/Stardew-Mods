using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace JoJa84Plus {
	public class ModEntry : Mod {
		private bool CalcOpen = false;

		/*********
		** Public methods
		*********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper) {
			helper.Events.Input.ButtonPressed += this.OnButtonPressed;
		}

		/*********
		** Private methods
		*********/
		/// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
		/// <param name="sender">The event sender.</param>
		/// <param name="e">The event data.</param>
		private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {
			// Ignore if player hasn't loaded a save yet
			if (!Context.IsWorldReady)
				return;

			// If the player presses F5, then open/close the calculator.
			if (e.Button == SButton.F5) {
				if (!this.CalcOpen)
					this.Monitor.Log("Opening the JoJa84+!", LogLevel.Info);
				else
					this.Monitor.Log("Closing the JoJa84+!", LogLevel.Info);
				this.CalcOpen = !this.CalcOpen;
				if (this.CalcOpen) {
					JoJa84PlusMenu menu = new JoJa84PlusMenu();
					Game1.activeClickableMenu = menu;
					Game1.playSound("bigSelect");
				} else {
					Game1.exitActiveMenu();
					Game1.playSound("bigDeSelect");
				}
			} else if (e.Button == SButton.Escape && this.CalcOpen) {
				this.Monitor.Log("Closing the JoJa84+!", LogLevel.Info);
				this.CalcOpen = false;
			}
		}
	}

	public class JoJa84PlusMenu: IClickableMenu {
		private enum Operation {
			Add,
			Subtract,
			Multiply,
			Divide,
			None
		}
		private Operation op = Operation.None;
		private double result = 0;
		private string inputA = "";
		private string inputB = "";
		private bool enteringDecimal = false;
		private bool currentInput = false;
		private int widthOnScreen = 0;
		private int heightOnScreen = 0;
		private int timer = 0;
		private List<ClickableComponent> numpad = new List<ClickableComponent>();
		private Dictionary<Keys, char> numKeys = new Dictionary<Keys, char> {
			{Keys.NumPad0, '0'},
			{Keys.NumPad1, '1'},
		};
		public JoJa84PlusMenu(): base(Game1.viewport.Width / 2 - 256 / 2, Game1.viewport.Height / 2 - 256, 256 + IClickableMenu.borderWidth * 2, 512 + IClickableMenu.borderWidth * 2, showUpperRightCloseButton: false) {
			this.xPositionOnScreen = Game1.viewport.Width / 2 - 256 / 2;
			this.yPositionOnScreen = Game1.viewport.Height / 2 - 256;
			this.widthOnScreen = 256 + IClickableMenu.borderWidth * 2;
			this.heightOnScreen = 512 + IClickableMenu.borderWidth * 2;
			
		}
		public override void draw(SpriteBatch b) {
			if (!Game1.options.showMenuBackground) { b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f); }
			Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, widthOnScreen, heightOnScreen, speaker: false, drawOnlyBox: true);
			// Draw JoJa watermark thing.
			b.DrawString(Game1.smallFont,
				"joja 84+",
				new Vector2(
					(float)this.xPositionOnScreen
						+ 40,
					(float)this.yPositionOnScreen
						+ this.heightOnScreen
						- IClickableMenu.borderWidth 
						- 40
				),
				Game1.textColor
			);

			// Draw the current input.
			b.DrawString(Game1.smallFont,
				String.Concat(((this.currentInput) ? this.inputB : this.inputA), ((this.timer >= 16) ? "|" : "")),
				new Vector2(
					(float)this.xPositionOnScreen
						+ this.widthOnScreen
						- 40
						- Game1.smallFont.MeasureString((this.currentInput) ? this.inputB + "|" : this.inputA + "|").X,
					(float)this.yPositionOnScreen
						+ IClickableMenu.borderWidth
						+ IClickableMenu.spaceToClearTopBorder
				),
				Game1.textColor
			);

			// Draw the previous input, if currently entering the second number.
			if (this.currentInput) {
				string prevInput = this.inputA 
					+ " "
					+ (this.op switch {
						Operation.Add => "+",
						Operation.Subtract => "-",
						Operation.Multiply => "*",
						Operation.Divide => "/",
						Operation.None => "ERR",
						_ => "ERR",
					});
				b.DrawString(Game1.smallFont,
					prevInput,
					new Vector2(
						(float)this.xPositionOnScreen
							+ this.widthOnScreen
							- 40
							- Game1.smallFont.MeasureString(prevInput).X,
						(float)this.yPositionOnScreen
							+ IClickableMenu.borderWidth
							+ IClickableMenu.spaceToClearSideBorder
							+ Game1.smallFont.LineSpacing * 2
					),
					Game1.textShadowColor);
			}

			// Draw the buttons.
	

			if (this.shouldDrawCloseButton()) {
				base.draw(b);
			}
			if (!Game1.options.hardwareCursor) {
				b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
			}
			this.timer++;
			if (this.timer >= 32) {
				this.timer = 0;
			}
		}
		public override void receiveKeyPress(Keys key) {
			char keyChar = key switch {
				Keys.NumPad0 => '0',
				Keys.NumPad1 => '1',
				Keys.NumPad2 => '2',
				Keys.NumPad3 => '3',
				Keys.NumPad4 => '4',
				Keys.NumPad5 => '5',
				Keys.NumPad6 => '6',
				Keys.NumPad7 => '7',
				Keys.NumPad8 => '8',
				Keys.NumPad9 => '9',
				Keys.D0 => '0',
				Keys.D1 => '1',
				Keys.D2 => '2',
				Keys.D3 => '3',
				Keys.D4 => '4',
				Keys.D5 => '5',
				Keys.D6 => '6',
				Keys.D7 => '7',
				Keys.D8 => '8',
				Keys.D9 => '9',
				Keys.OemPeriod => '.',
				_ => '_',
			};
			if (keyChar != '_') {
				if (this.currentInput)
					this.inputB += keyChar;
				else
					this.inputA += keyChar;
			} else {
				switch (key) {
					case Keys.Add:
						if (!this.currentInput)
							this.currentInput = true;
						this.op = Operation.Add;
						break;
					case Keys.Subtract:
						if (!this.currentInput)
							this.currentInput = true;
						this.op = Operation.Subtract;
						break;
					case Keys.Multiply:
						if (!this.currentInput)
							this.currentInput = true;
						this.op = Operation.Divide;
						break;
					case Keys.Divide:
						if (!this.currentInput)
							this.currentInput = true;
						this.op = Operation.Divide;
						break;
					case Keys.Enter:
						this.DoCalculation();
						break;
					case Keys.Delete:
						this.result = 0;
						this.inputA = "";
						this.inputB = "";
						this.currentInput = false;
						break;
					case Keys.Escape:
						this.exitThisMenu();
						break;
				}
			}
		}
		private void DoCalculation() {
			if (!this.currentInput) {
				double.TryParse(this.inputA, out this.result);
				this.currentInput = false;
			} else {
				double inputA, inputB;
				double.TryParse(this.inputA, out inputA);
				double.TryParse(this.inputB, out inputB);
				switch (this.op) {
					case Operation.Add:
						this.result = inputA + inputB;
						break;
					case Operation.Subtract:
						this.result = inputA - inputB;
						break;
					case Operation.Multiply:
						this.result = inputA * inputB;
						break;
					case Operation.Divide:
						this.result = inputA / inputB;
						break;
					case Operation.None:
						break;
				}
				this.inputA = this.result.ToString();
				this.inputB = "";
				this.currentInput = false;
			}
		}
	}
}
