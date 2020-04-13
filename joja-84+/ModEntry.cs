using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace JoJa84Plus {
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod {
		/*********
		** Private values
		**********/
		/// <summary>A bool that is `true` when the calculator is open.</summary>
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
					Game1.activeClickableMenu = new JoJa84PlusMenu();
					Game1.playSound("bigSelect");
				} else {
					Game1.exitActiveMenu();
					Game1.playSound("bigDeSelect");
				}
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
		private int result = 0;
		private int inputA = 0;
		private string inputAText = "TEST";
		private int inputB = 0;
		private string inputBText = "";
		private bool currentInput = false;
		private int xPositionOnScreen = 0;
		private int yPositionOnScreen = 0;
		private int widthOnScreen = 0;
		private int heightOnScreen = 0;
		private int timer = 0;
		private List<ClickableComponent> numpad = new List<ClickableComponent>(); 
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
						- 40),
				Game1.textColor);

			// Draw the current input.
			string currentContent = String.Concat(
				((this.currentInput) ? this.inputAText : this.inputBText),
				((this.timer >= 16) ? "|" : "")
			);
			b.DrawString(Game1.smallFont,
				currentContent,
				new Vector2(
					(float)this.xPositionOnScreen
						+ this.widthOnScreen
						- 40
						- Game1.dialogueFont.MeasureString(currentContent).X,
					(float)this.yPositionOnScreen
						+ IClickableMenu.borderWidth
						+ IClickableMenu.spaceToClearTopBorder
				),
				Game1.textColor
			);

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
	}
}
