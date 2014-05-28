using NUnit.Framework;

namespace ColloSys.QueryBuilder.Test.QueryExecutionTests
{
    //TODO: MAHENDRA
    //1. formula(output) > number and number > column
    //2. number > formula and column > number
    //3. formula(conditional) and number > formula(output)
    //a. and similar all combinations
    //b. also one more thing: formula can contain other fomulas but only condtion/output formula
    //c. also while string generation formula should be replaced by its generated string
    //d. so in case 1 above output shoudl be e.g. (number + column) > number and ....

    //also name the test as per the logic and not as per the field
    //do i need to tell you, how to name variables/methods???????
    [TestFixture]
    public class AndOrQueryTests
    {
        
    }
}
