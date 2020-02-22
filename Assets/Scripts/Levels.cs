[System.Serializable]
public class Levels
{
    public bool useSum = true;
    public bool useSub = false;
    public bool useMul = false;
    public bool useDiv = false;
    public bool usePar = true; // group elements in parenthesis
    public bool useNeg = false; // allow negative results
    public int Decimals = 0; // returns decimal numbers (0 for ints)
    public float minBaseNumber = 0f; // bottom random number for elem (use negs if needed)
    public float maxBaseNumber = 9f; // top random number for elem
    public int opsNumElems = 3;  // number of elements (ie  1 + 4 + 5) is 3 elems
    public int levelNumElems = 10; // number of questions in level
    public int numberOptions = 3; // number of possible answers
    public int maxseconds = 15; // number of seconds allowed per question
}
