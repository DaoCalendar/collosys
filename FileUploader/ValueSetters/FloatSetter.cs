using System;
using System.Globalization;

namespace FileUploader.ValueSetters
{
    public class FloatSetter :NumberSetter<float>
    {
       

        protected override float SetValue(string s, string format = "")
        {


            return Single.Parse(s, NumberStyles.Any);
        }
    }
}
