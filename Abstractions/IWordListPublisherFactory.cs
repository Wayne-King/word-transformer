using System.Collections.Generic;

namespace WayneKing.Practice.Abstractions
{
	public interface IWordListPublisherFactory
	{
		IWordListPublisher CreatePublisher(IList<string> words);
	}
}