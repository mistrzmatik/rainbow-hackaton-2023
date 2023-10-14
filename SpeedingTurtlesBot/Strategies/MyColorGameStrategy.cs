namespace SpeedingTurtlesBot.Strategies;

public class MyColorGameStrategy : GameStrategy
{
    public override (Card, KolorZolwia) NextMove(List<Card> cards, Board board)
    {
        RemoveUnableToActCards(cards, board);

        // na początku weź randomową. 
        var pickedCard = cards[Random.Shared.Next(cards.Count)];

        // jesli nie jesteśmy ostatni to nie wybierajmy karty IsLast
        var notIsLast = cards.FirstOrDefault(x => !board.IsMyColorInLastColors && !x.IsLast);
        if (notIsLast != null)
        {
            pickedCard = notIsLast;
        }

        // jeśli mam kartę która cofnie przeciwnika, który jest za mną
        var backForOpponentBeforeMyColorCard = cards.FirstOrDefault(x => x.IsBack && board.ColorsBeforeMyField.Any(c => c == x.Color));
        if (backForOpponentBeforeMyColorCard != null)
        {
            pickedCard = backForOpponentBeforeMyColorCard;
        }
        
        // jeśli mam kartę która cofnie przeciwnika, który jest przedemną
        var backForOpponentAfterMyColorCard = cards.FirstOrDefault(x => x.IsBack && (board.ColorsAfterMyField.Any(c => c == x.Color) || board.ColorsAboveMyColor.Any(c => c == x.Color)));
        if (backForOpponentAfterMyColorCard != null)
        {
            pickedCard = backForOpponentAfterMyColorCard;
        }
        
        // wybierz kartę koloru który jest pod moją karta
        var colorBelowMyColorForwardCard = cards.FirstOrDefault(x => x.IsForward && board.ColorsUnderMyColor.Any(c => c == x.Color));
        if (colorBelowMyColorForwardCard != null)
        {
            pickedCard = colorBelowMyColorForwardCard;
        }
        
        // jest to karta mojego koloru lub każdego koloru która idzie do przodu
        var myColorForwardCard = cards.FirstOrDefault(x => x.IsForward && (x.Color == MyColor || x.IsAny));
        if (myColorForwardCard != null)
        {
            pickedCard = myColorForwardCard;
        }
        
        var pickedColor = KolorZolwia.Xxx;
        if (pickedCard.IsAny)
        {
            if (pickedCard.IsForward)
            {
                pickedColor = MyColor;
            }
            else
            {
                if (board.ColorsAfterMyField.Any())
                {
                    pickedColor = board.ColorsAfterMyField.First();
                }
                else if (board.ColorsAboveMyColor.Any())
                {
                    pickedColor = board.ColorsAboveMyColor.First();
                }
                else if (board.ColorsBeforeMyField.Any())
                {
                    pickedColor = board.ColorsBeforeMyField.First();
                }
                else
                {
                    pickedColor = MyColor;
                }
            }
        }

        if (pickedCard.IsLast)
        {
            if (board.IsMyColorInLastColors)
            {
                pickedColor = MyColor;
            }
            else
            {
                pickedColor = board.LastColors.FirstOrDefault();
            }
        }

        return (pickedCard, pickedColor);
    }
}