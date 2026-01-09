using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApi.Tests;

public class CalculatorTests
{
    [Fact]
    public void Add_TwoNumbers_ReturnsSum()
    {
        //Arrange (setup)
        int a = 7;
        int b = 7;

        //Act (execute)
        int result = a + b;

        //Assert (verify)
        Assert.Equal(14, result);
    }
}
