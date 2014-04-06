namespace FileUploader.ValueSetters
{
    public class StringSetter : ValueSetter<string>
    {
        protected override string GetValue(string s,string format="")
        {
            return (s == null ? string.Empty : s.Trim());
        }
    }
}