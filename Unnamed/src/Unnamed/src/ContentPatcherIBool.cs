using System.Collections;
using System.Collections.Generic;

namespace Unnamed
{
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