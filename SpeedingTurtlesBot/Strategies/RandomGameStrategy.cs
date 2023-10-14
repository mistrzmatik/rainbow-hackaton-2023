using Google.Protobuf.Collections;

namespace SpeedingTurtlesBot.Strategies;

public class RandomGameStrategy : GameStrategy
{
    public override (Card, KolorZolwia) NextMove(List<Card> cards, Board board)
    {
        RemoveUnableToActCards(cards, board);
    
        var pickedCard = cards[Random.Shared.Next(cards.Count)];
    
        var pickedColor = KolorZolwia.Xxx;
        if (pickedCard.IsAny)
        {
            if (board.HaveAnyTurtle)
            {
                pickedColor = board.AllColors.FirstOrDefault();
            }
            else
            {
                pickedColor = NonNanColors[Random.Shared.Next(NonNanColors.Length)];
            }
        }

        if (pickedCard.IsLast)
        {
            pickedColor = board.LastColors.FirstOrDefault();
        }

        return (pickedCard, pickedColor);
    }
}