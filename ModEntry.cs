using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using ContentPatcher;

namespace Unnamed
{
	public class ModEntry: Mod
	{
		private bool DoSpouseCuddleEvent = false;
		private IContentPatcherAPI api;

		public override void Entry(IModHelper helper)
		{
			helper.Events.GameLoop.DayEnding += OnDayEnding;
			helper.Events.GameLoop.DayStarted += OnDayStarted;
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
		}

		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			this.api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
			this.api.RegisterToken(this.ModManifest, "DoSpouseCuddleCutscene", () =>
			{
				return new ContentPatcherIBool(this.DoSpouseCuddleEvent);
			});
		}

		private void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			this.DoSpouseCuddleEvent = true;
		}

		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			this.DoSpouseCuddleEvent = false;
		}
	}

	public class ContentPatcherIBool: IEnumerable<string> {
		List<string> _elements;

		public ContentPatcherIBool(bool value)
		{
			string[] valueArray = {((value) ? "1" : "0")};
			this._elements = new List<string>(valueArray);
		}

		IEnumerator<string> IEnumerable<string>.GetEnumerator()
		{
			return this._elements.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._elements.GetEnumerator();
		}
	}
}