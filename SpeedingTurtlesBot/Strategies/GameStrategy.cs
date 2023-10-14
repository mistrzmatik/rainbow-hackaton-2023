namespace SpeedingTurtlesBot.Strategies;

public abstract class GameStrategy
{
    public static readonly KolorZolwia[] NonNanColors = { KolorZolwia.Red, KolorZolwia.Green, KolorZolwia.Blue, KolorZolwia.Yellow, KolorZolwia.Purple };

    private KolorZolwia _myColor;
    public KolorZolwia MyColor
    {
        protected get
        {
            return _myColor;
        }
        set
        {
            _myColor = value;
            NotMyColors = NonNanColors.Except(new[] { MyColor }).ToArray();
        }
    }

    protected KolorZolwia[] NotMyColors { get; private set; } = Array.Empty<KolorZolwia>();
    
    public abstract (Card, KolorZolwia) NextMove(List<Card> cards, Board board);

    protected void RemoveUnableToActCards(List<Card> cards, Board board)
    {
        if (!board.HaveAnyTurtle)
        {
            cards.RemoveAll(x => x.IsBack);
        }

        foreach (var nonNanColor in NonNanColors)
        {
            bool isAnyTurtleOnBoardWithThisColor = board.Any(x => x.Contains(nonNanColor));
            if (!isAnyTurtleOnBoardWithThisColor)
            {
                cards.RemoveAll(x => x.Color == nonNanColor && x.IsBack);
            }
        }
    }
}