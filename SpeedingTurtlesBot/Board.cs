using SpeedingTurtlesBot.Strategies;

namespace SpeedingTurtlesBot;

public class Board : List<Field>
{
    private readonly KolorZolwia _myColor;

    public Board(IEnumerable<Field> fields, KolorZolwia myColor)
    {
        _myColor = myColor;
        AddRange(fields);
    }
    
    /// <summary>
    /// Wszytskie kolory na planszy.
    /// </summary>
    public KolorZolwia[] AllColors => this.SelectMany(x => x).ToArray();

    public bool HaveAnyTurtle => AllColors.Length != 0;
    
    public Field? FieldWithMyColor => this.FirstOrDefault(x => x.Contains(_myColor));
    public Field? FieldClosestToStart => this.FirstOrDefault(x => x.Count != 0);
    public bool IsMyColorInFieldClosestToStart => FieldClosestToStart == FieldWithMyColor;
    public Field? FieldClosestToFinish => this.LastOrDefault(x => x.Count != 0);
    public bool IsMyColorInFieldClosestToFinish => FieldClosestToFinish == FieldWithMyColor;

    public KolorZolwia[] ColorsUnderMyColor => FieldWithMyColor?.AsEnumerable().TakeWhile(x => x != _myColor).ToArray() ?? Array.Empty<KolorZolwia>();
    public KolorZolwia[] ColorsAboveMyColor => FieldWithMyColor?.AsEnumerable().Reverse().TakeWhile(x => x != _myColor).Reverse().ToArray() ?? Array.Empty<KolorZolwia>();
    
    public KolorZolwia[] LastColors => AllColors.Length == 5 ? this.First(x => x.Any()).ToArray() : GameStrategy.NonNanColors.Where(x => !AllColors.Contains(x)).ToArray();
    public bool IsMyColorInLastColors => LastColors.Contains(_myColor);
    
    /// <summary>
    /// Kolory dalej mety, które nie są na moim polu.
    /// </summary>
    public KolorZolwia[] ColorsBeforeMyField => FieldWithMyColor != null ? this.AsEnumerable().TakeWhile(x => x != FieldWithMyColor).SelectMany(x => x).ToArray() :  Array.Empty<KolorZolwia>();
    /// <summary>
    /// Kolory bliżej mety, które nie są na moim polu.
    /// </summary>
    public KolorZolwia[] ColorsAfterMyField => FieldWithMyColor != null ? this.AsEnumerable().Reverse().TakeWhile(x => x != FieldWithMyColor).Reverse().SelectMany(x => x).ToArray() : AllColors;
}