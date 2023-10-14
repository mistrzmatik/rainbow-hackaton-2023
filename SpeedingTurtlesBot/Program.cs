using Grpc.Net.Client;
using SpeedingTurtlesBot;
using SpeedingTurtlesBot.Strategies;

var argsList = args.ToList();

var o = new Options
{
    Serwer = GetStringArgument(argsList, "addr", "a", "http://localhost:50051"),
    GraId = GetStringArgument(argsList, "gra", "g"),
    Nazwa = GetStringArgument(argsList, "nazwa", null, "Skręcone kostki"),
    CzyNowa = GetBoolArgument(argsList, "nowa", "n"),
    LiczbaGraczy = 2
};

Console.WriteLine("Speeding Turtles Bot");
Console.WriteLine("Łączenie z serwerem...");
string serverAddress = o.Serwer;
using var channel = GrpcChannel.ForAddress(serverAddress); 
var client = new Gra.GraClient(channel);

string gameId = o.GraId;
if (o.CzyNowa)
{
    CreateNewGame(client, gameId, o.LiczbaGraczy);
}

string name = o.Nazwa;
if (string.IsNullOrEmpty(name)) name = "Skręcone kostki";

await JoinTheGame(client, gameId, name, new MyColorGameStrategy());

//CompareStrategies(client, new RandomGameStrategy(), new MyColorGameStrategy());

return;

async Task<int> JoinTheGame(Gra.GraClient client, string gameId, string name, GameStrategy gameStrategy)
{
    Console.WriteLine($"Dołączam do gry {gameId}...");
    var stanGry = await client.DolaczDoGryAsync(new Dolaczanie
    {
        GraID = gameId,
        NazwaGracza = name
    });
    var playerId = stanGry.GraczID;
    var myColor = stanGry.TwojKolor;
    gameStrategy.MyColor = myColor;
    Console.WriteLine($"Jesteś graczem {playerId} i masz kolor {myColor}.");

    while (!stanGry.CzyKoniec)
    {
        Console.WriteLine($"Plansza: {stanGry.Plansza}");
        Console.WriteLine($"Grasz jako {myColor}. Karty: {stanGry.TwojeKarty}");

        var cards = stanGry.TwojeKarty.Select(x => new Card(x)).ToList();
        var board = new Board(stanGry.Plansza.Select(x => new Field(x.Zolwie)), myColor);
        var (pickedCard, pickedColor) = gameStrategy.NextMove(cards, board);
    
        Console.WriteLine($"Wybrałem kartę {pickedCard} i kolor {pickedColor}.");
        stanGry = await client.MojRuchAsync(new RuchGracza
        {
            GraID = gameId,
            GraczID = playerId,
            ZagranaKarta = pickedCard,
            KolorWybrany = pickedColor
        });
    }

    Console.WriteLine($"Koniec gry. Grę wygrał: {stanGry.KtoWygral}");

    return stanGry.KtoWygral;
}

string CreateNewGame(Gra.GraClient client, string gameId, int playersCount = 2)
{
    Console.WriteLine("Tworzę nową grę...");
    var gra = client.NowyMecz(new KonfiguracjaGry
    {
        LiczbaGraczy = playersCount,
        GraID = gameId
    });
    return gra.GraID;
}

void CompareStrategies(Gra.GraClient client, params GameStrategy[] strategies)
{
    var points = new int[strategies.Length];
    for (int i = 0; i < 1000; i++)
    {
        string gameId = CreateNewGame(client, Guid.NewGuid().ToString());

        Task<int>[] tasks = new Task<int>[strategies.Length];
        for (int j = 0; j < strategies.Length; j++)
        {
            tasks[j] = JoinTheGame(client, gameId, Guid.NewGuid().ToString(), strategies[j]); 
        }

        bool isReversed = i % 2 == 0;
        if (isReversed)
        {
            tasks = tasks.Reverse().ToArray();
        }

        foreach (var task in tasks)
        {
            Task.Delay(100);
            task.Wait();
        }

        if (isReversed)
        {
            points[tasks.Length - tasks[0].Result]++;
        }
        else
        {
            points[tasks[0].Result - 1]++;
        }
    }

    Console.WriteLine($"Wyniki porównania strategii:");
    for (int i = 0; i < points.Length; i++)
    {
        Console.WriteLine($"{strategies[i].GetType().Name} = {points[i]}");
    }
}

string GetStringArgument(List<string> args, string longName, string? shortName = null, string defaultValue = "")
{
    int longNameIndex = args.IndexOf("--" + longName);
    if (longNameIndex != -1 && args.Count > longNameIndex + 1)
    {
        return args[longNameIndex + 1];
    }

    if (shortName is not null)
    {
        int shortNameIndex = args.IndexOf("-" + shortName);
        if (shortNameIndex != -1 && args.Count > shortNameIndex + 1)
        {
            return args[shortNameIndex + 1];
        }
    }

    return defaultValue;
}

bool GetBoolArgument(List<string> args, string longName, string? shortName = null)
{
    int longNameIndex = args.IndexOf("--" + longName);
    if (longNameIndex != -1 && args.Count > longNameIndex + 1)
    {
        return true;
    }

    if (shortName is not null)
    {
        int shortNameIndex = args.IndexOf("-" + shortName);
        if (shortNameIndex != -1 && args.Count > shortNameIndex + 1)
        {
            return true;
        }
    }

    return false;
}