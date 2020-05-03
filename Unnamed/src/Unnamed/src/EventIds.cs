namespace Unnamed {
	public class EventIds
	{
		private static string[] ids =
		{
			"52880001",
			"52880002"
		};

		public static string GetIdByNameAndKeyActor(string name, string actor)
		{
			switch (name)
			{
				case "SpouseCuddle":
					switch (actor)
					{
						case "Abigail":
							return ids[0];
						default:
							return "";
					}
				default:
					return "";
			}
		}
	}
}