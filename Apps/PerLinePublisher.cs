using System;
using System.Collections.Generic;
using System.IO;
using WayneKing.Practice.Abstractions;

namespace WayneKing.Practice.Apps
{
	public class PerLinePublisher : IWordListPublisher
	{
		private readonly TextWriter writer;
		private readonly IList<string> words;

		public PerLinePublisher(TextWriter writer, IList<string> words)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));
			if (words == null)
				throw new ArgumentNullException(nameof(words));

			this.writer = writer;
			this.words = words;
		}

		public void Publish()
		{
			foreach (string word in words)
			{
				writer.WriteLine(word);
			}
		}
	}
}
