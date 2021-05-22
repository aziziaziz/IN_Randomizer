using System;
using System.Collections.Generic;
using System.Globalization;

namespace Model
{
  public class Characters
  {
    private char[] Alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
    private char[] Numbers = "0123456789".ToCharArray();
    private Random Rand = new Random();

    public string GetAlphaNumeric
    {
      get
      {
        var resultArray = new List<char>();
        resultArray.AddRange(GetRandomSpaces);

        var randTotalLength = Rand.Next(5, 15);
        while (randTotalLength >= 0) {
          switch (Rand.Next(2))
          {
              case 0:
                resultArray.Add(Alpha[Rand.Next(Alpha.Length)]);
                break;
              case 1:
                resultArray.Add(Numbers[Rand.Next(Numbers.Length)]);
                break;
          }

          randTotalLength--;
        }

        resultArray.AddRange(GetRandomSpaces);
        
        return new string(resultArray.ToArray());
      }
    }

    public int GetNumeric
    {
      get
      {
        var resultArray = new List<char>();
        var randTotalLength = Rand.Next(2, Numbers.Length);
        while (randTotalLength >= 0)
        {
          resultArray.Add(Numbers[Rand.Next(Numbers.Length)]);
          randTotalLength--;
        }

        return (int)Int64.Parse(new string(resultArray.ToArray()));
      }
    }

    public float GetFloat
    {
      get
      {
        var resultArray = new List<char>();
        var beforeDot = Rand.Next(2, 5);
        var afterDot = Rand.Next(1, Numbers.Length);

        while (beforeDot >= 0) {
          resultArray.Add(Numbers[Rand.Next(Numbers.Length)]);
          beforeDot--;
        }
        resultArray.Add('.');
        while (afterDot >= 0) {
          resultArray.Add(Numbers[Rand.Next(1, Numbers.Length - 1)]);
          afterDot--;
        }
        
        return float.Parse(new string(resultArray.ToArray()), CultureInfo.InvariantCulture.NumberFormat);
      }
    }

    private List<char> GetRandomSpaces
    {
      get
      {
        var spaces = new List<char>();
        var randSpaces = Rand.Next(3, 10);
        while (randSpaces >= 0) {
          spaces.Add(' ');
          randSpaces--;
        }
        return spaces;
      }
    }
  }
}