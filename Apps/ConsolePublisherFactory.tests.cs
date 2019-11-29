using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WayneKing.Practice.Abstractions;

namespace WayneKing.Practice.Apps
{
    [TestClass]
	public class ConsolePublisherFactoryTests
	{
		public ConsolePublisherFactoryTests()
		{
		}

		[TestMethod]
		public void CreatedPublisherWritesToConsole()
		{
			var original = Console.Out;
			try
			{
				var sb = new StringBuilder();
				Console.SetOut(new StringWriter(sb));

				new ConsolePublisherFactory().CreatePublisher(new string[] { "word" }).Publish();

				Assert.IsTrue(sb.Length > 0);
			}
			finally
			{
				Console.SetOut(original);
			}
		}
	}
}