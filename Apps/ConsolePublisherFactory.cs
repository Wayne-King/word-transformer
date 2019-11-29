using System;
using System.Collections.Generic;
using WayneKing.Practice.Abstractions;

namespace WayneKing.Practice.Apps
{
	public class ConsolePublisherFactory : IWordListPublisherFactory
	{
		public ConsolePublisherFactory()
		{
		}

		public IWordListPublisher CreatePublisher(IList<string> words)
		{
			return new PerLinePublisher(Console.Out, words);
		}
	}
}