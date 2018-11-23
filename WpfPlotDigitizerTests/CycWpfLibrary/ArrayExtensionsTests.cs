using CycWpfLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CycWpfLibrary.Tests
{
  [TestClass()]
  public class ArrayExtensionsTests
  {
    [TestMethod()]
    public void ResizeTest()
    {
      var array = new int[,]
      {
        { 1, 2, 3 },
        { 4, 5, 6 },
        { 7, 8, 9 },
      };
      var array2 = new int[]
      {
        1,2,3,4,5,6,7,8,9
      };
      var array3 = array.Resize();
      Assert.IsTrue(array2.SequenceEqual(array3));
    }

    [TestMethod()]
    public void ResizeTest1()
    {
      var array = new int[,,]
      {
        {
          { 1, 2, 3 },
          { 4, 5, 6 },
          { 7, 8, 9 },
        },
        {
          { 11, 12, 13 },
          { 14, 15, 16 },
          { 17, 18, 19 },
        },
        {
          { 21, 22, 23 },
          { 24, 25, 26 },
          { 27, 28, 29 },
        },
      };
      var array2 = new int[]
      {
        1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17, 18, 19, 21, 22, 23, 24, 25, 26, 27, 28, 29,
      };
      var array3 = array.Resize();
      Assert.IsTrue(array2.SequenceEqual(array3));
    }

    [TestMethod()]
    public void ResizeTest2()
    {
      var array = new int[,]
      {
        { 1, 2, 3 },
        { 4, 5, 6 },
      };
      var array2 = new int[,]
      {
        { 1, 2 },
        { 3, 4 },
        { 5, 6 },
      };
      var array3 = array.Resize(3, 2);
      Assert.IsTrue(array2.IsEqual(array3));
    }
  }
}