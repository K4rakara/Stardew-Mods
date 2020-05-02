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
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using ContentPatcher;

namespace Unnamed
{
	public class ModEntry: Mod
	{
		private bool DoSpouseCuddleEvent = false;
		private IContentPatcherAPI api;
		private List<EventTimer> EventTimers = new List<EventTimer>();
		
		public override void Entry(IModHelper helper)
		{
			helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
			helper.Events.GameLoop.DayEnding += OnDayEnding;
			helper.Events.GameLoop.DayStarted += OnDayStarted;
			helper.Events.GameLoop.GameLaunched += OnGameLaunched;
		}

		private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
		{
			this.api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
			this.api.RegisterToken(this.ModManifest, "DoSpouseCuddleEvent", () =>
			{
				return new ContentPatcherIBool(this.DoSpouseCuddleEvent);
			});
		}

		private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
		{
			this.EventTimers.ForEach(eventTimer =>
			{
				eventTimer.Update();
			});
			for (int i = 0; i < EventTimers.Count; i++) {
				if (EventTimers[i].ReadyToBeRemoved())
				{
					this.EventTimers.Remove(EventTimers[i]);
				}
			}
		}

		private void OnDayEnding(object sender, DayEndingEventArgs e)
		{
			Game1.player.eventsSeen.Remove(Int32.Parse(EventIds.GetIdByNameAndKeyActor("SpouseCuddle", "Abigail")));
		}

		private void OnDayStarted(object sender, DayStartedEventArgs e)
		{
			this.DoSpouseCuddleEvent = true;
			if (this.DoSpouseCuddleEvent)
			{
				SafleySetTime(2000);
				// Reload the map so the event occurs.
				Game1.player.warpFarmer(new Warp(27, 13, "FarmHouse", 27, 13, false));
				Game1.player.currentEyes = 1;
				Game1.player.blinkTimer = -10000;
				Helper.Events.GameLoop.UpdateTicked += UpdateSpouseCuddleEvent;
			}
		}

		private void UpdateSpouseCuddleEvent(object sender, UpdateTickedEventArgs e)
		{
			if (Game1.eventOver)
			{
				OnSpouseCuddleEventFinished();
			}
			else
			{
				Game1.player.currentEyes = 1;
				Game1.player.blinkTimer = -10000;
			}
		}

		private void OnSpouseCuddleEventFinished()
		{
			this.EventTimers.Add(new EventTimer(40, OnSpouseCuddleEventFadeOut));
			Helper.Events.GameLoop.UpdateTicked -= UpdateSpouseCuddleEvent;
		}

		private void OnSpouseCuddleEventFadeOut()
		{
			SafleySetTime(600);
		}

		private void SafleySetTime(int time)
		{
			// I have no clue why this works.
			Game1.timeOfDay = time;
			Game1.performTenMinuteClockUpdate();
			string[] arguments = { "time", time.ToString() };
			Helper.ConsoleCommands.Trigger("debug", arguments);
			// Fix children, they dont like to cooperate with this time fuckery.
			FarmHouse farmHouse = (FarmHouse)Game1.getLocationFromName("FarmHouse");
			for (int i = 0; i < farmHouse.characters.Count; i++)
			{
				NPC thisNpc = farmHouse.characters[i];
				if (thisNpc is Child child)
				{
					int num = (int)Game1.MasterPlayer.UniqueMultiplayerID;
					Random r = new Random(Game1.Date.TotalDays + (int)Game1.uniqueIDForThisGame / 2 + num * 2);
					if (child.Age == 2)
					{
						child.speed = 1;
						Point randomOpenPointInHouse = (child.currentLocation as FarmHouse).getRandomOpenPointInHouse(r, 1, 60);
						if (!randomOpenPointInHouse.Equals(Point.Zero))
						{
							child.setTilePosition(randomOpenPointInHouse);
						}
						else
						{
							child.Position = new Vector2(16f, 4f) * 64f + new Vector2(0f, -24f);
						}
					}
				}
			}
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