using AngleSharp.Text;

namespace BlazorGameWorldTests
{
	public class CounterTests : TestContext
	{
		[Fact]
		public void CounterShouldIncrementWhenClicked()
		{
			var cut = RenderComponent<Counter>();
			var initVal = Int32.Parse(string.Concat(cut.Find("p").TextContent.Where(Char.IsDigit)));
			cut.Find("button").Click();
			//cut.Find("p").MarkupMatches("<p role=\'status\'>Current count: 1</p>");
			var finalVal = Int32.Parse(string.Concat(cut.Find("p").TextContent.Where(Char.IsDigit)));

			Assert.Equal(initVal+1, finalVal);
		}
	}
}