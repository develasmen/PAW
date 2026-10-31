
namespace PAW.Tasks;

public class Sync
{
    public static async Task Execute(int secs, string identifier)
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < secs; i++)
            {
                Console.WriteLine($"{identifier} - Second {i + 1}");
            }
        });
    }

    public static async Task ExecuteEval(decimal secs, string identifier)
    {
        await Task.Run(() =>
        {
            for (decimal i = 0; i < secs; i++)
            {
                if (i % 1000000000 == 0)
                {
                    Console.WriteLine($"{identifier} - Second {i + 1}");
                }
            }
        });
    }

    public static async Task<IEnumerable<string>> ExecuteItems(
        int secs, string identifier)
    {
        var items = new List<string>();

        await Task.Run(() =>
        {
            for (int i = 0; i < secs; i++)
            {
                items.Add($"{identifier} - Second {i + 1}");
            }
        });

        return items;
    }
}
