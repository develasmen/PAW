using Microsoft.VisualStudio.TestTools.UnitTesting;
using PAW.Tasks;
using System.IO;

namespace PAW.Tasks.Tests;

[TestClass]
[DoNotParallelize]
public class SyncTests
{

    [TestMethod]
    public async Task Execute_ValidSeconds_PrintsExpectedMessages()
    {
        
        var originalOutput = Console.Out;
        using var writer = new StringWriter();

        try
        {
            Console.SetOut(writer);

           
            await Sync.Execute(3, "Test");

            
            var output = writer.ToString();

            StringAssert.Contains(output, "Test - Second 1");
            StringAssert.Contains(output, "Test - Second 2");
            StringAssert.Contains(output, "Test - Second 3");
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
    }


    [TestMethod]
    public async Task ExecuteEval_ValidSeconds_PrintsExpectedMessage()
    {
        
        var originalOutput = Console.Out;
        using var writer = new StringWriter();

        try
        {
            Console.SetOut(writer);

         
            await Sync.ExecuteEval(2m, "Test");

           
            var output = writer.ToString();

            StringAssert.Contains(output, "Test - Second 1");
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
    }


    [TestMethod]
    public async Task ExecuteItems_ValidSeconds_ReturnsExpectedItems()
    {
        
        int seconds = 3;
        string identifier = "Test";

       
        var result = await Sync.ExecuteItems(seconds, identifier);

        
        var items = result.ToList();

        Assert.HasCount (3, items);
        Assert.AreEqual("Test - Second 1", items[0]);
        Assert.AreEqual("Test - Second 2", items[1]);
        Assert.AreEqual("Test - Second 3", items[2]);
    }



}