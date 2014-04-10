namespace ReflectionExtension.Tests.NSubstitude.Test
{
    public interface INsubstitudeDemo
    {
        string GetData();
    }

    public  class NsubstitudeDemo : INsubstitudeDemo
    {
        public virtual string GetData()
        {
            var data = "asd";
            return data;
        }

     
    }

    public class nested
    {
       public  NsubstitudeDemo obj=new NsubstitudeDemo();
        public virtual string callGetData()
        {
            string s= obj.GetData();
            return s;
        }
    }
}