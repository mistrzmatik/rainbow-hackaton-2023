namespace SpeedingTurtlesBot;

public class Field : List<KolorZolwia>
{
    public Field(IEnumerable<KolorZolwia> kolory)
    {
        AddRange(kolory);
    }
}