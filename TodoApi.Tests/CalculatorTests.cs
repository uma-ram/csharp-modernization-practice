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


    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(5, 5, 10)]
    [InlineData(0, 0, 0)]
    // Multiple test cases with different data
    public void Add_MultipleInputs_ReturnsCorrectSum(int a, int b, int expected)
    {
        int result = a + b;
        Assert.Equal(expected, result);
    }

}
