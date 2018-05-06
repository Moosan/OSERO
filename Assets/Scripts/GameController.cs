using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class V2 
{
    public int X, Y;

    public V2(int x,int y)
    {
        X = x;
        Y = y;
    }

    public V2()
    {
        X = -1;
        Y = -1;
    }
}
public class GameController : MonoBehaviour
{
    public GameObject[][] Board = new GameObject[8][];
    public GameObject CoinPrefab;
    public static bool Turn;
    public static int[][] CheckArray;
    public static int TurnCount;
    public static bool Pass1;
    public static bool End;
    public GameObject[][] ReturnedCoin;
    private GameObject[] PutedCoin;
    private bool[] PassBools;
    public List<V2> PutablePos;
    public List<List<V2>> ReturnableCoin;
    public bool TurnChanging;
    public Text[] Texts;
    private bool Reseting;
    public GameObject ResetButton;
    public GameObject AI;

    public void Start()
    {
        ResetButton.SetActive(false);
        foreach (var text in Texts) {
            text.text = "";
        }
        CheckArray = new []
        {
            new  []{1, 0}, new [] {1, 1}, new [] {0, 1}, new [] {-1, 1}, new [] {-1, 0}, new [] {-1, -1},
            new [] {0, -1}, new [] {1, -1}
        };
        PutedCoin=new GameObject[64];
        ReturnedCoin=new GameObject[64][];
        PassBools=new bool[64];
        for (var i = 0; i < Board.Length; i++)
        {
            Board[i] = new GameObject[8];
            for (var j = 0; j < Board[i].Length; j++)
            {
                Board[i][j] = i == 3
                    ? (j == 3 ? Make(false, 3, 3) : (j == 4 ? Make(true, 3, 4) : null))
                    : (i == 4 ? (j == 3 ? Make(true, 4, 3) : (j == 4 ? Make(false, 4, 4) : null)) : null);
            }
        }
        Turn = true;
        Pass1 = false;
        End = false;
        TurnChanging = false;
        TurnCount = -1;
        MoveText("ゲーム開始");
        MoveText("黒のターンです。");
    }
    
    public void MoveText(string message)
    {
        var text = message;
        var next = "";
        for(int i = 0; i < Texts.Length; i++)
        {
            next = Texts[i].text;
            Texts[i].text = text;
            text = next;
        }
    }

    public GameObject Make(bool color, int i, int j)
    {
        var rotate = color ? 0 : 180;
        var obj = Instantiate(CoinPrefab, new Vector3(i, 0, -j), Quaternion.Euler(rotate, 0, 0));
        obj.transform.parent = transform;
        var coin = obj.GetComponent<Coin>();
        coin.Color = color;
        coin.BeforeColor = color;
        if (!color)
        {
            coin.Rotat = rotate;
            coin.Rotating = rotate;
        }
        TurnCount++;
        PutedCoin[TurnCount] = obj;
        return obj;
    }

    public void PutCoin(Vector3 key)
    {
        var x = key.x - (int) key.x < 0.5 ? (int) key.x : (int) key.x + 1;
        var z = (int) key.z - key.z < 0.5 ? (int) key.z : (int) key.z - 1;
        var putOut = true;
        if (x >= 0 && x <= 7 && z <= 0 && z >= -7 && !Board[x][-z])
        {
            var returnCoin = CheckCoin(Turn, x, -z);
            if (returnCoin.Count > 0)
            {
                Board[x][-z] = Make(Turn, x, -z);
                ReturnCoin(returnCoin, Turn);
                Turn = !Turn;
                if (TurnCount >= 59 && !End)
                {
                    End = true;
                    MoveText("ゲーム終了です。");
                    EndGame();
                    putOut = false;
                }
                else
                {
                    putOut = false;
                    TurnChange(Turn);
                    if (!End)
                    {
                        MoveText(Turn ? "黒のターンです" : "白のターンです");
                    }
                }
            }
        }
        if (putOut)
        {
            if (Reseting) {
                Reseting = false;
                return;
            }
            MoveText(End ? "ゲーム終了しとんねんで？" : "そこには置けないよん");
        }
    }

    public bool CheckColor(int i0, int i1, bool thisTurn)
    {
        var notEqual = false;
        if (i0 < 0 || i0 > 7 || i1 < 0 || i1 > 7) return false;
        if (!Board[i0][i1]) return false;
        if (Board[i0][i1].GetComponent<Coin>().Color != thisTurn)
        {
            notEqual = true;
        }
        return notEqual;
    }

    public List<GameObject> CheckCoin(bool putColor, int x, int z)
    {
        var allCoin = new List<GameObject>();
        for (var i = 0; i < 8; i++)
        {
            var i0 = x + CheckArray[i][0];
            var i1 = z + CheckArray[i][1];
            var getCoin = new List<GameObject>();
            var getCoinPos=new List<V2>();
            while (CheckColor(i0, i1, putColor))
            {
                getCoin.Add(Board[i0][i1]);
                getCoinPos.Add(new V2(i0,i1));
                i0 += CheckArray[i][0];
                i1 += CheckArray[i][1];
            }
            if (!CheckColor(i0, i1, !putColor)) continue;
            foreach (var t in getCoin)
            {
                allCoin.Add(t);
                if(TurnChanging)ReturnableCoin.Add(getCoinPos);
            }
        }
        return allCoin;
    }

    public void ReturnCoin(List<GameObject> returnCoin, bool newColor)
    {
        ReturnedCoin[TurnCount]=new GameObject[returnCoin.Count];
        for (var i = 0; i < returnCoin.Count; i++)
        {
            returnCoin[i].GetComponent<Coin>().Color = newColor;
            ReturnedCoin[TurnCount][i] = returnCoin[i];
        }
    }

    public void EndGame()
    {
        var brack = 0;
        var white = 0;
        foreach (var t in Board)
        {
            foreach (var t1 in t)
            {
                if (!t1) continue;
                var color = t1.GetComponent<Coin>().Color;
                if (color) brack++;
                else white++;
            }
        }
        if (brack == white) MoveText("引き分けですね");
        else if (brack > white) MoveText(brack + "対" + white + "で黒の勝ち～");
        else MoveText(brack + "対" + white + "で白の勝ち～");

        Invoke("GetResetButton",2.0f);
    }

    private void GetResetButton() {
        ResetButton.SetActive(true);
    }

    public void TurnChange(bool newTurn)
    {
        TurnChanging = true;
        var returnebleCoin=new List<GameObject>();
        PutablePos=new List<V2>();
        ReturnableCoin=new List<List<V2>>();
        for (var i = 0; i < Board.Length; i++)
        {
            for (var j = 0; j < Board[i].Length; j++)
            {
                if (Board[i][j]) continue;
                var checkCoin = CheckCoin(Turn, i, j);
                returnebleCoin.AddRange(checkCoin);
                if (checkCoin.Count > 0)
                {
                    PutablePos.Add(new V2(i,j));
                }
            }
        }
        if (returnebleCoin.Count <= 0)
        {
            if (Pass1)
            {
                End = true;
                MoveText("互いに置く場所が無いのでゲーム終了です");
                EndGame();
            }
            else
            {
                if (newTurn)
                {
                    MoveText("黒は置ける場所がありません");
                    Turn = false;
                }
                else
                {
                    MoveText("白は置ける場所がありません");
                    Turn = true;
                }
                PassBools[TurnCount] = true;
                MoveText("パスします");
                Pass1 = true;
                TurnChange(!newTurn);
            }
        }
        else Pass1 = false;
        TurnChanging = false;
    }

    public void ReTurnChange(int Count)
    {
        for (int j = 0; j < Count; j++)
        {
            if (TurnCount < 0) MoveText("もう戻せないよ(/ω＼)");
            else
            {
                ResetButton.SetActive(false);
                var reTurn = PutedCoin[TurnCount].GetComponent<Coin>().Color;
                for (var i = 0; i < ReturnedCoin[TurnCount].Length; i++)
                {
                    ReturnedCoin[TurnCount][i].GetComponent<Coin>().Color = !reTurn;
                }
                PutedCoin[TurnCount].GetComponent<Coin>().Color = !reTurn;
                Destroy(PutedCoin[TurnCount], 0.3f);
                ReturnedCoin[TurnCount] = null;
                PutedCoin[TurnCount] = null;
                Turn = PassBools[TurnCount] ? Turn : !Turn;
                PassBools[TurnCount] = false;
                TurnCount--;
                MoveText("操作を戻しました");
                End = false;
                MoveText(Turn ? "黒のターンです" : "白のターンです");
            }
        }
    }

    public void GameReset() {
        Reseting = true;
        foreach (var a in Board) {
            foreach (var b in a) {
                Destroy(b);
            }
        }
        Start();
    }
}
