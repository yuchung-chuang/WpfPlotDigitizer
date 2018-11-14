using CycWpfLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CycWpfLibrary.Tests
{
  internal enum MyEnum
  {
    a = 1,
    b = 2,
    c = 4,
  }
  [TestClass()]
  public class EnumExtensionTests
  {
    [TestMethod()]
    public void EnumTest()
    {
      MyEnum A = (MyEnum)MyEnum.a.Add(MyEnum.b);
      var B = MyEnum.b;
      Assert.IsTrue(A.Contain(B));
      A = (MyEnum)A.Remove(MyEnum.a);
      Assert.AreEqual(A, B);
    }
  }

  
}