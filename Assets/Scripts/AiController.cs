using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoolCoin
{
    public bool Exist;
    public bool Color;

    public BoolCoin(bool color)
    {
        Exist = true;
        Color = color;
    }

    public BoolCoin()
    {
        Exist = false;
    }
}
public class AiController : MonoBehaviour
{
    public GameController GameController;
    public bool AiTurn;
    public bool ThisTurn;
    public int TurnCount;
    public int[][] ScoreFunction;
    public bool[] PhaseBools;
    public GameObject[][] Board;
    public V2[] SecondPhasePos;
    public V2[] Corner;
    public V2[] PutCoins;
    public V2[][] ReturnCoin;
    public BoolCoin[][] BoolBoard;
    public bool Putting;
    public int[][] CheckArray;



    public void AIStart()
    {
        ScoreFunction = new int[8][];
        ScoreFunction[0] = new [] {60, -12, 0, -1, -1, 0, -12, 60};
        ScoreFunction[1] = new [] {-12, -30, -3, -3, -3, -3, -30, -12};
        ScoreFunction[2] = new [] {0, -3, 0, -1, -1, 0, -3, 0};
        ScoreFunction[3] = new [] {-1, -3, -1, -1, -1, -1, -3, -1};
        ScoreFunction[4] = ScoreFunction[3];
        ScoreFunction[5] = ScoreFunction[2];
        ScoreFunction[6] = ScoreFunction[1];
        ScoreFunction[7] = ScoreFunction[0];
        SecondPhasePos=new []
        {
            new V2(0,2),  new V2(0,5),
            new V2(2,0),new V2(5,0),
            new V2(2,7),new V2(5,7),
            new V2(7,2),new V2(7,5)
        };
        Corner = new[] {new V2(0, 0), new V2(0, 7), new V2(7, 0), new V2(7, 7)};
        AiTurn = false;
        PhaseBools=new bool[3];
        Putting = false;
        SecBool = false;
    }
    public void Update()
    {
        if (GameController.TurnCount == -1) AIStart();
        ThisTurn = GameController.Turn;
        if (AiTurn != ThisTurn || GameController.End || Putting) return;
        Putting = true;
        Invoke("AiMove", 0.6f);
    }

    public void AiMove()
    {
        Board = GameController.Board;
        CheckArray = GameController.CheckArray;
        TurnCount = GameController.TurnCount + 1;
        var count = GameController.PutablePos.Count;
        PutCoins=new V2[count];
        ReturnCoin=new V2[count][];
        for (var i = 0; i < count; i++)
        {
            PutCoins[i] = GameController.PutablePos[i];
            var count2 = GameController.ReturnableCoin[i].Count;
            ReturnCoin[i]=new V2[count2];
            for (var j = 0; j < count2; j++)
            {
                ReturnCoin[i][j] = GameController.ReturnableCoin[i][j];
            }
        }
        //var returnPos = TurnCount < 44 ? SecondPhaseBoolean() ? Second() : First() : Last2();
        //var returnpos = First();
        if (TurnCount < 44)
        {
            AiPut(First());
        }
        else {
            ScoreChange();
            AiPut(First());
        }
        Putting = false;
    }
    
    public V2 First()
    {
        var goodPos=new List<int>();
        var maxScore = ScoreFunction[PutCoins[0].X][PutCoins[0].Y];
        goodPos.Add(0);
        for (var i = 1; i < PutCoins.Length; i++)
        {
            var thisScore = ScoreFunction[PutCoins[i].X][PutCoins[i].Y];
            if (maxScore < thisScore)
            {
                goodPos=new List<int>();
                maxScore = thisScore;
                goodPos.Add(i);
            }
            else if(maxScore==thisScore)goodPos.Add(i);
        }
        var greatPos=new List<int>();
        var minLength = ReturnCoin[goodPos[0]].Length;
        greatPos.Add(0);
        for (var i = 1; i < goodPos.Count; i++)
        {
            var thisLength = ReturnCoin[goodPos[i]].Length;
            if (minLength > thisLength)
            {
                greatPos=new List<int>();
                minLength = thisLength;
                greatPos.Add(i);
            }
            else if (minLength == thisLength)
            {
                greatPos.Add(i);
            }
        }
        var returnPos = greatPos.Count == 1
            ? PutCoins[goodPos[greatPos[0]]]
            : PutCoins[goodPos[greatPos[Random.Range(0, greatPos.Count - 1)]]];
        return returnPos;
    }

    public V2 Second()
    {
        ScoreChange();
        var goodPos = new List<int>();
        var maxScore = ScoreFunction[PutCoins[0].X][PutCoins[0].Y];
        goodPos.Add(0);
        for (var i = 1; i < PutCoins.Length; i++)
        {
            var thisScore = ScoreFunction[PutCoins[i].X][PutCoins[i].Y];
            if (maxScore < thisScore)
            {
                goodPos = new List<int>();
                maxScore = thisScore;
                goodPos.Add(i);
            }
            else if (maxScore == thisScore) goodPos.Add(i);
        }
        var greatPos = new List<int>();
        var maxLength = ReturnCoin[goodPos[0]].Length;
        greatPos.Add(0);
        for (var i = 1; i < goodPos.Count; i++)
        {
            var thisLength = ReturnCoin[goodPos[i]].Length;
            if (maxLength < thisLength)
            {
                greatPos = new List<int>();
                maxLength = thisLength;
                greatPos.Add(i);
            }
            else if (maxLength == thisLength)
            {
                greatPos.Add(i);
            }
        }
        var returnPos = greatPos.Count == 1
            ? PutCoins[goodPos[greatPos[0]]]
            : PutCoins[goodPos[greatPos[Random.Range(0, greatPos.Count - 1)]]];
        return returnPos;
    }

    public V2 Last1()
    {
        var returnPos = new V2();
        var firstBoard = new BoolCoin[8][];
        var maxScore = 0;
        for (var i = 0; i < 8; i++)
        {
            firstBoard[i]=new BoolCoin[8];
            for (var j = 0; j <8; j++)
            {
                if (GameController.Board[i][j])
                {
                    firstBoard[i][j] = new BoolCoin(GameController.Board[i][j].GetComponent<Coin>().Color);
                }
                else
                {
                    firstBoard[i][j]=new BoolCoin();
                }
            }
        }
        var firstPutablePos = PutablePos(AiTurn, firstBoard);
        Debug.Log("count:"+firstPutablePos.Count);
        foreach (var pos in firstPutablePos)
        {
            var thisScore = 0;
            var nextBoard = MakeBoard(firstBoard);
            PutBool(pos,AiTurn,nextBoard);
            var playerPutable = PutablePos(!AiTurn, nextBoard);
            if (playerPutable.Count == 0)
            {
                return pos;
            }
            foreach (var playerpos in playerPutable)
            {
                var nextnextBoard = MakeBoard(nextBoard);
                PutBool(playerpos,!AiTurn,nextnextBoard);
                var aiPutable = PutablePos(AiTurn, nextnextBoard);
                var allGet = (from aiPut in aiPutable let lastBoard = MakeBoard(nextnextBoard) select PutBoolCoinResult(aiPut, AiTurn, lastBoard).Count).Sum();
                thisScore += allGet;
                Debug.Log("allGet:"+allGet);
            }
            Debug.Log("thisScore:"+thisScore);
            if (maxScore > thisScore) continue;
            maxScore = thisScore;
            returnPos = pos;
            Debug.Log("("+pos.X+","+pos.Y+")");
        }
        Debug.Log("("+returnPos.X+","+returnPos.Y+")");
        return returnPos;
    }

    public V2 Last2()
    {
        Debug.Log("Last2");
        BoolBoard=new BoolCoin[GameController.Board.Length][];
        for (var i = 0; i < BoolBoard.Length; i++)
        {
            BoolBoard[i]=new BoolCoin[GameController.Board[i].Length];
            for (var j = 0; j < BoolBoard[i].Length; j++)
            {
                if (GameController.Board[i][j])
                {
                    BoolBoard[i][j]=new BoolCoin(GameController.Board[i][j].GetComponent<Coin>().Color);
                }
                else
                {
                    BoolBoard[i][j]=new BoolCoin();
                }
            }
        }
        var maxScore = -9999;
        var goodPos=new List<V2>();
        foreach (var pos in PutCoins)
        {
            var thisScore = 0;
            PutBool(pos,AiTurn,BoolBoard);
            thisScore += CoinCount(AiTurn, BoolBoard);
            thisScore -= Score(PutablePos(!AiTurn, BoolBoard));
            if (maxScore < thisScore)
            {
                goodPos=new List<V2>{pos};
                maxScore = thisScore;
            }
            else if (maxScore == thisScore)
            {
                goodPos.Add(pos);
            }
        }
        var returnPos = goodPos[Random.Range(0, goodPos.Count - 1)];
        return returnPos;
    }

    /*public V2 NewFirst()
    {
        Debug.Log("NewFirst");
        var goodPos = new List<V2>();
        var EnemyPutablePositionCunt = 64;
        foreach (var pos in PutCoins)
        {
            
        }
    }*/

    public void AiPut(V2 putPos)
    {
        GameController.PutCoin(new Vector3(putPos.X,0,-putPos.Y));
    }

    public void PhaseCheck()
    {
        if (!PhaseBools[0])
        {
            if (SecondPhasePos.Any(t => GameController.Board[t.X][t.Y]))
            {
                PhaseBools[0] = true;
            }
        }else if (!PhaseBools[2])
        {
            if (TurnCount >= 30&&!PhaseBools[2])
            {
                var cornerCount = Corner.Count(i => Board[i.X][i.Y] && Board[i.X][i.Y].GetComponent<Coin>().Color == AiTurn);
                if (cornerCount >= 2) PhaseBools[1] = true;
            }
            if (TurnCount >= 44)
            {
                PhaseBools[2] = true;
            }
        }
    }

    public List<V2> PutBoolCoinResult(V2 position,bool coinColor,BoolCoin[][] board)
    {
        var allCoin = new List<V2>();
        for (var i = 0; i < 8; i++)
        {
            var i0 = position.X + CheckArray[i][0];
            var i1 = position.Y + CheckArray[i][1];
            var getCoin = new List<V2>();
            while (CheckColor(new V2(i0,i1), coinColor,board))
            {
                getCoin.Add(new V2(i0,i1));
                i0 += CheckArray[i][0];
                i1 += CheckArray[i][1];
            }
            if (CheckColor(new V2(i0,i1), !coinColor,board))
            {
                allCoin.AddRange(getCoin);
            }
        }
        return allCoin;
    }

    public bool CheckColor(V2 position,bool coinColor,BoolCoin[][] board)
    {
        var i0 = position.X;
        var i1 = position.Y;
        var notEqual = false;
        if (i0 < 0 || i0 > 7 || i1 < 0 || i1 > 7) return false;
        var coin = board[i0][i1];
        if(coin.Exist && coin.Color != coinColor)notEqual = true;
        return notEqual;
    }

    public List<V2> PutablePos(bool putColor,BoolCoin[][] board)
    {
        var putableCoins=new List<V2>();
        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8 ; j++)
            {
                var putCoins = PutBoolCoinResult(new V2(i,j), putColor,board);
                if(putCoins.Count!=0)putableCoins.Add(new V2(i,j));
            }
        }
        return putableCoins;
    }

    public int Score(List<V2> sum)
    {
        return sum.Sum(pos => ScoreFunction[pos.X][pos.Y]);
    }

    public void PutBool(V2 pos,bool turn,BoolCoin[][] board)
    {
        board[pos.X][pos.Y].Exist = true;
        board[pos.X][pos.Y].Color = turn;
        var returnCoin = PutBoolCoinResult(pos, turn,board);
        foreach (var posi in returnCoin)
        {
            board[posi.X][posi.Y].Color = turn;
        }
    }

    public V2 PutSearch(bool turn,BoolCoin[][] board)
    {
        var putSearch = new V2();
        var putablePos = PutablePos(turn,board);
        var score = -9999;
        foreach (var putPos in putablePos)
        {
            var thisScore = Score(PutBoolCoinResult(putPos, turn,board))+ScoreFunction[putPos.X][putPos.Y];
            if (thisScore < score) continue;
            putSearch = putPos;
            score = thisScore;
        }
        return putSearch;
    }

    public BoolCoin[][] MakeBoard(BoolCoin[][] originBoard)
    {
        var newBoard=new BoolCoin[originBoard.Length][];
        for (var i = 0; i < originBoard.Length; i++)
        {
            newBoard[i]=new BoolCoin[originBoard[i].Length];
            for (var j = 0; j < originBoard[i].Length; j++)
            {
                newBoard[i][j] = new BoolCoin
                {
                    Exist = originBoard[i][j].Exist,
                    Color = originBoard[i][j].Color
                };
            }
        }
        return newBoard;
    }

    public bool CornerExist(List<V2> coins)
    {
        return coins.Any(pos => Corner.Any(corner => pos.X==corner.X&&pos.Y==corner.Y));
    }

    public int CoinCount(bool color, BoolCoin[][] board)
    {
        return board.Sum(line => line.Count(coin => color == coin.Color));
    }

    /*public V2 LastWin(bool color,BoolCoin[][] board,int gameTurnCount)
    {
        var newBoard = MakeBoard(board);
        var putablePos = PutablePos(color, newBoard);
        if()
    }*/

    public bool SecBool;
    public bool SecondPhaseBoolean()
    {
        if (!SecBool)
        {
            var nowBoard = new List<V2>();
            for (var i = 0; i < Board.Length; i++)
            {
                for (var j = 0; j < Board[i].Length; j++)
                {
                    if (Board[i][j]) nowBoard.Add(new V2(i, j));
                }
            }
            var count = (from pos in nowBoard from secPos in SecondPhasePos where pos.X == secPos.X && pos.Y == secPos.Y select pos).Count();
            return count > 2;
        }
        else return true;
    }
    public void ScoreChange() {
        Debug.Log((bool)Board[7][7]);
        if (Board[0][0])
        {
            ScoreFunction[0][1] = 50;
            ScoreFunction[1][0] = 50;
            ScoreFunction[1][1] = 40;
        }
        else {
            ScoreFunction[0][1] = -100;
            ScoreFunction[1][0] = -100;
            ScoreFunction[1][1] = -90;
        }

        if (Board[0][7])
        {
            ScoreFunction[0][6] = 50;
            ScoreFunction[1][7] = 50;
            ScoreFunction[1][6] = 40;
        }
        else
        {
            ScoreFunction[0][6] = -100;
            ScoreFunction[1][7] = -100;
            ScoreFunction[1][6] = -90;
        }
        if (Board[7][0])
        {
            ScoreFunction[6][0] = 50;
            ScoreFunction[7][1] = 50;
            ScoreFunction[6][1] = 40;
        }
        else
        {
            ScoreFunction[6][0] = -100;
            ScoreFunction[7][1] = -100;
            ScoreFunction[6][1] = -90;
        }
        if (Board[7][7])
        {
            ScoreFunction[7][6] = 50;
            ScoreFunction[6][7] = 50;
            ScoreFunction[6][6] = 40;
        }
        else
        {
            ScoreFunction[7][6] = -100;
            ScoreFunction[6][7] = -100;
            ScoreFunction[6][6] = -90;
        }
    }
}