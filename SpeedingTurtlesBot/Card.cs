namespace SpeedingTurtlesBot;

public class Card
{
    private readonly Karta _original;
    
    public KolorZolwia? Color { get; }
    public int Direction { get; }
    public bool IsLast { get; }
    public bool IsAny { get; }
    public bool IsBack => Direction < 0;
    public bool IsForward => Direction > 0;
    
    public Card(Karta karta)
    {
        _original = karta;
        
        var kartaString = karta.ToString("G");
        switch (kartaString[0])
        {
            case 'R':
                Color = KolorZolwia.Red;
                break;
            case 'G':
                Color = KolorZolwia.Green;
                break;
            case 'B':
                Color = KolorZolwia.Blue;
                break;
            case 'Y':
                Color = KolorZolwia.Yellow;
                break;
            case 'P':
                Color = KolorZolwia.Purple;
                break;
            case 'A':
                IsAny = true;
                break;
            case 'L':
                IsLast = true;
                break;
        }

        Direction = short.Parse(kartaString[1].ToString());
        if (kartaString.Length == 3)
        {
            Direction = -Direction;
        }
    }

    public override string ToString()
    {
        return _original.ToString();
    }

    public static implicit operator Karta(Card c) => c._original;
}