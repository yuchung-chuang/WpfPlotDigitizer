using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace PlotDigitizer.Core.Test
{
	[TestClass()]
	public class QuickTest
	{
		[TestMethod]
		public void TempFolderTest()
		{
			var file = Path.GetTempFileName();
			var dir = Path.GetTempPath();
		}
	}
}
