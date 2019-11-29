using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WayneKing.Practice.Apps.Tests
{
    [TestClass]
    public class PerLinePublisherTests
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CtorThrowsOnNullWriter()
		{
			new PerLinePublisher(null, new string[0]);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CtorThrowsOnNullList()
		{
			new PerLinePublisher(TextWriter.Null, null);
		}

		[TestMethod]
		public void NoLinesPublishedWhenEmptyList()
		{
			var ctw = new CountingTextWriter();
			new PerLinePublisher(ctw, new string[0]).Publish();

			Assert.AreEqual(0, ctw.WriteLineCallCount);
		}

		[TestMethod]
		public void OneLinePerItemIsPublished()
		{
			var ctw = new CountingTextWriter();
			new PerLinePublisher(ctw, new string[] { "alpha", "beta" }).Publish();

			Assert.AreEqual(2, ctw.WriteLineCallCount);
		}


		private class CountingTextWriter : TextWriter
		{
			public override Encoding Encoding { get { return Encoding.ASCII; } }

			public int WriteLineCallCount { get; private set; } = 0;

			public override void WriteLine(string value)
			{
				WriteLineCallCount++;
				base.WriteLine(value);
			}
		}
	}
}
