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

namespace JoJa84Plus
{
	public class ModEntry: Mod
	{
		private bool CalcOpen = false;
		private JoJa84PlusMenu menu;

		public override void Entry(IModHelper helper)
		{
			helper.Events.Input.ButtonPressed += this.OnButtonPressed;
			helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
		}

		private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
		{
			this.menu = new JoJa84PlusMenu();
		}

		private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
		{
			// Ignore if player hasn't loaded a save yet
			if (!Context.IsWorldReady)
				return;

			// If the player presses F5, then open/close the calculator.
			if (e.Button == SButton.F5)
			{
				this.CalcOpen = !this.CalcOpen;
				if (this.CalcOpen)
				{
					Game1.activeClickableMenu = this.menu;
					Game1.playSound("bigSelect");
				}
				else 
				{
					Game1.exitActiveMenu();
					Game1.playSound("bigDeSelect");
				}
			}
			else if (e.Button == SButton.Escape && this.CalcOpen) 
			{
				this.CalcOpen = false;
			}
		}
	}

	public class JoJa84PlusMenu: IClickableMenu
	{
		private enum Operation
		{
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
		private bool currentInput = false;
		private int widthOnScreen = 0;
		private int heightOnScreen = 0;
		private int timer = 0;
		private List<ClickableComponent> numpad = new List<ClickableComponent>();
		private List<ClickableComponent> opButtons = new List<ClickableComponent>();
		private ClickableComponent clearButton;
		private ClickableComponent backspaceButton;
		private ClickableComponent enterButton;
		public JoJa84PlusMenu(): 
			base(
				Game1.viewport.Width / 2 - 256 / 2,
				Game1.viewport.Height / 2 - 256,
				256 + IClickableMenu.borderWidth * 2,
				512 + IClickableMenu.borderWidth * 2,
				showUpperRightCloseButton: false
			)
		{
			this.xPositionOnScreen = Game1.viewport.Width / 2 - 256 / 2;
			this.yPositionOnScreen = Game1.viewport.Height / 2 - 256;
			this.widthOnScreen = 256 + IClickableMenu.borderWidth * 2;
			this.heightOnScreen = 512 + IClickableMenu.borderWidth * 2;
			
			// Create the number-pad.
			for (int iy = 0; iy < 3; iy++)
			{
				for (int ix = 0; ix < 3; ix++)
				{
					int buttonWidth = ((this.widthOnScreen - IClickableMenu.borderWidth * 2) / 4);
					this.numpad.Add
					(
						new ClickableComponent
						(
							new Rectangle
							(
								this.xPositionOnScreen
									+ 40
									+ buttonWidth * ix,
								this.yPositionOnScreen
									+ IClickableMenu.borderWidth
									+ IClickableMenu.spaceToClearTopBorder
									+ buttonWidth * iy
									+ (Game1.smallFont.LineSpacing * 4),
								buttonWidth,
								buttonWidth
							),
						(Math.Abs(((3-iy)*3)+ix-2)).ToString()
						)
					);
				}
			}

			// Create the op buttons.
			for (int i = 0; i < 4; i++)
			{
				int xOffset = ((this.widthOnScreen - IClickableMenu.borderWidth * 2) / 4) * 3;
				this.opButtons.Add
				(
					new ClickableComponent
					(
						new Rectangle
						(
							this.xPositionOnScreen
								+ 40
								+ xOffset
								+ xOffset / 16,
							this.yPositionOnScreen
								+ IClickableMenu.borderWidth
								+ IClickableMenu.spaceToClearTopBorder
								+ ((xOffset) / 4) * i
								+ (Game1.smallFont.LineSpacing * 4),
							xOffset / 4,
							xOffset / 4
						),
						i switch
						{
							0 => "+",
							1 => "-",
							2 => "X",
							3 => "/",
							_ => ""
						}
					)
				);
			}

			// Create the clear button.
			this.clearButton = new ClickableComponent
			(
				new Rectangle
				(
					this.xPositionOnScreen
						+ 40,
					this.yPositionOnScreen
						+ IClickableMenu.borderWidth
						+ IClickableMenu.spaceToClearTopBorder
						+ (Game1.smallFont.LineSpacing * 2),
					(this.widthOnScreen - IClickableMenu.borderWidth * 2) / 2,
					Game1.smallFont.LineSpacing * 2
				),
				"clear",
				"Clear"
			);

			// Create the backspace button.
			this.backspaceButton = new ClickableComponent
			(
				new Rectangle
				(
					this.xPositionOnScreen
						+ 40
						+ (this.widthOnScreen - IClickableMenu.borderWidth * 2) / 2,
					this.yPositionOnScreen
						+ IClickableMenu.borderWidth
						+ IClickableMenu.spaceToClearTopBorder
						+ (Game1.smallFont.LineSpacing * 2),
					(this.widthOnScreen - IClickableMenu.borderWidth * 2) / 2,
					Game1.smallFont.LineSpacing * 2
				),
				"backspace",
				"<"
			);

			// Create the enter button.
			this.enterButton = new ClickableComponent
			(
				new Rectangle
				(
					this.xPositionOnScreen
						+ 40,
					this.yPositionOnScreen
						+ IClickableMenu.borderWidth
						+ IClickableMenu.spaceToClearTopBorder
						+ Game1.smallFont.LineSpacing * 2
						+ (this.widthOnScreen - IClickableMenu.borderWidth * 2),
					(this.widthOnScreen - IClickableMenu.borderWidth * 2),
					Game1.smallFont.LineSpacing * 2
				),
				"enter",
				"Enter"
			);
		}
		public override void draw(SpriteBatch b)
		{
			if (!Game1.options.showMenuBackground) b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
			Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, widthOnScreen, heightOnScreen, speaker: false, drawOnlyBox: true);
			// Draw JoJa watermark thing.
			b.DrawString
			(
				Game1.smallFont,
				"joja 84+",
				new Vector2
				(
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
			b.DrawString
			(
				Game1.smallFont,
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
			if (this.currentInput)
			{
				string prevInput = this.inputA 
					+ " "
					+ (this.op switch {
						Operation.Add => "+",
						Operation.Subtract => "-",
						Operation.Multiply => "X",
						Operation.Divide => "/",
						Operation.None => "ERR",
						_ => "ERR",
					});
				b.DrawString
				(
					Game1.smallFont,
					prevInput,
					new Vector2
					(
						(float)this.xPositionOnScreen
							+ this.widthOnScreen
							- 40
							- Game1.smallFont.MeasureString(prevInput).X,
						(float)this.yPositionOnScreen
							+ IClickableMenu.borderWidth
							+ IClickableMenu.spaceToClearSideBorder
							+ Game1.smallFont.LineSpacing * 2
					),
					Game1.textShadowColor
				);
			}

			// Draw the number pad.
			foreach(ClickableComponent button in this.numpad)
			{
				IClickableMenu.drawTextureBox
				(
					b,
					button.bounds.X,
					button.bounds.Y,
					button.bounds.Width,
					button.bounds.Height,
					(button.scale >= 0f) ? Color.Wheat : Color.White
				);
				Utility.drawBoldText
				(
					b,
					button.name,
					Game1.smallFont,
					new Vector2
					(
						(float)button.bounds.X
							+ (button.bounds.Width / 2)
							- (Game1.smallFont.MeasureString(button.name).X / 2),
						(float)button.bounds.Y
							+ (button.bounds.Height / 2)
							- (Game1.smallFont.MeasureString(button.name).Y / 2)
					),
					Game1.textColor,
					1f,
					-1f,
					2
				);
			}

			// Draw the operator buttons.
			foreach(ClickableComponent button in this.opButtons)
			{
				IClickableMenu.drawTextureBox
				(
					b,
					button.bounds.X,
					button.bounds.Y,
					button.bounds.Width,
					button.bounds.Height,
					(button.scale >= 0f) ? Color.Wheat : Color.White
				);
				b.DrawString
				(
					Game1.smallFont,
					button.name,
					new Vector2(
						(float)button.bounds.X
							+ (button.bounds.Width / 2)
							- (Game1.smallFont.MeasureString(button.name).X / 2),
						(float)button.bounds.Y
							+ (button.bounds.Height / 2)
							- (Game1.smallFont.MeasureString(button.name).Y / 2)
					),
					Game1.textColor
				);
			}

			// Draw the clear button.
			IClickableMenu.drawTextureBox
			(
				b,
				this.clearButton.bounds.X,
				this.clearButton.bounds.Y,
				this.clearButton.bounds.Width,
				this.clearButton.bounds.Height,
				(this.clearButton.scale >= 0f) ? Color.Wheat : Color.White
			);
			Utility.drawTextWithShadow
			(
				b,
				this.clearButton.label,
				Game1.smallFont,
				new Vector2
				(
					(float)this.clearButton.bounds.X
						+ (this.clearButton.bounds.Width / 2)
						- (Game1.smallFont.MeasureString(this.clearButton.label).X / 2),
					(float)this.clearButton.bounds.Y
						+ (this.clearButton.bounds.Height / 2)
						- (Game1.smallFont.MeasureString(this.clearButton.name).Y / 2)
				),
				Game1.textColor
			);

			// Draw the backspace button.
			IClickableMenu.drawTextureBox
			(
				b,
				this.backspaceButton.bounds.X,
				this.backspaceButton.bounds.Y,
				this.backspaceButton.bounds.Width,
				this.backspaceButton.bounds.Height,
				(this.backspaceButton.scale >= 0f) ? Color.Wheat : Color.White
			);
			Utility.drawTextWithShadow
			(
				b,
				this.backspaceButton.label,
				Game1.smallFont,
				new Vector2
				(
					(float)this.backspaceButton.bounds.X
						+ (this.backspaceButton.bounds.Width / 2)
						- (Game1.smallFont.MeasureString(this.backspaceButton.label).X / 2),
					(float)this.backspaceButton.bounds.Y
						+ (this.backspaceButton.bounds.Height / 2)
						- (Game1.smallFont.MeasureString(this.backspaceButton.name).Y / 2)
				),
				Game1.textColor
			);

			// Draw the enter button.
			IClickableMenu.drawTextureBox
			(
				b,
				this.enterButton.bounds.X,
				this.enterButton.bounds.Y,
				this.enterButton.bounds.Width,
				this.enterButton.bounds.Height,
				(this.enterButton.scale >= 0f) ? Color.Wheat : Color.White
			);
			Utility.drawTextWithShadow
			(
				b,
				this.enterButton.label,
				Game1.smallFont,
				new Vector2
				(
					(float)this.enterButton.bounds.X
						+ (this.enterButton.bounds.Width / 2)
						- (Game1.smallFont.MeasureString(this.enterButton.label).X / 2),
					(float)this.enterButton.bounds.Y
						+ (this.enterButton.bounds.Height / 2)
						- (Game1.smallFont.MeasureString(this.enterButton.name).Y / 2)
				),
				Game1.textColor
			);

			if (this.shouldDrawCloseButton()) base.draw(b);
			if (!Game1.options.hardwareCursor) b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
			
			this.timer++;
			if (this.timer >= 32) this.timer = 0;
		}
		public override void receiveKeyPress(Keys key)
		{
			char keyChar = key switch
			{
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
			if (keyChar != '_')
			{
				if (this.currentInput)
					this.inputB += keyChar;
				else
					this.inputA += keyChar;
			}
			else 
			{
				switch (key) 
				{
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
						this.op = Operation.Multiply;
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
					case Keys.Back:
						if (!this.currentInput)
						{
							if (this.inputA.Length >= 1) this.inputA = this.inputA.Substring(0, this.inputA.Length - 1);
						}
						else
						{
							if (this.inputB.Length >= 1) this.inputB = this.inputB.Substring(0, this.inputB.Length - 1);
						}
						break;
					case Keys.Escape:
						this.exitThisMenu();
						break;
				}
			}
		}

		public override void receiveLeftClick(int x, int y, bool playSound)
		{
			foreach(ClickableComponent button in this.numpad)
			{
				if (button.containsPoint(x, y))
				{
					if (this.currentInput)
						this.inputB += button.name;
					else
						this.inputA += button.name;
					Game1.playSound("smallSelect");
				}
			}
			foreach(ClickableComponent button in this.opButtons)
			{
				if (button.containsPoint(x, y))
				{
					switch(button.name)
					{
						case "+":
							if (!this.currentInput)
								this.currentInput = true;
							this.op = Operation.Add;
							break;
						case "-":
							if (!this.currentInput)
								this.currentInput = true;
							this.op = Operation.Subtract;
							break;
						case "X":
							if (!this.currentInput)
								this.currentInput = true;
							this.op = Operation.Multiply;
							break;
						case "/":
							if (!this.currentInput)
								this.currentInput = true;
							this.op = Operation.Divide;
							break;
					}
				}
			}
			if (this.enterButton.containsPoint(x, y)) this.DoCalculation();
			if (this.clearButton.containsPoint(x, y))
			{
				this.result = 0;
				this.inputA = "";
				this.inputB = "";
				this.currentInput = false;
			}
			if (this.backspaceButton.containsPoint(x, y))
			{
				if (!this.currentInput)
				{
					if (this.inputA.Length >= 1) this.inputA = this.inputA.Substring(0, this.inputA.Length - 1);
				}
				else
				{
					if (this.inputB.Length >= 1) this.inputB = this.inputB.Substring(0, this.inputB.Length - 1);
				}
			}
		}

		private void DoCalculation()
		{
			if (!this.currentInput) 
			{
				double.TryParse(this.inputA, out this.result);
				this.currentInput = false;
			}
			else
			{
				double inputA, inputB;
				double.TryParse(this.inputA, out inputA);
				double.TryParse(this.inputB, out inputB);
				switch (this.op)
				{
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
				if (this.inputA == "NaN") this.inputA = "";
				this.inputB = "";
				this.currentInput = false;
			}
		}
	}
}
